using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Database
{
    /// <summary>
    /// Información de una foreign key.
    /// </summary>
    public class ForeignKeyInfo
    {
        public string Nombre { get; set; }
        public string ColumnaOrigen { get; set; }
        public string TablaDestino { get; set; }
        public string ColumnaDestino { get; set; }
        public string DeleteRule { get; set; }
        public string UpdateRule { get; set; }
        public string CreateScriptTSQL { get; set; }
    }

    /// <summary>
    /// Información de un índice no-PK.
    /// </summary>
    public class IndexInfo
    {
        public string Nombre { get; set; }
        public string Columna { get; set; }
        public int Orden { get; set; }
        public bool EsUnico { get; set; }
        public string CreateScriptTSQL { get; set; }
    }

    /// <summary>
    /// Información de una columna de tabla extraída de MySQL.
    /// </summary>
    public class ColumnaInfo
    {
        public string Nombre { get; set; }
        public string TipoMySQL { get; set; }
        public string TipoTSQL { get; set; }
        public bool EsNullable { get; set; }
        public string DefaultValue { get; set; }
        public bool EsAutoIncrement { get; set; }
        public int? MaxLength { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public string Comentario { get; set; }
    }

    /// <summary>
    /// Información de una tabla extraída de MySQL.
    /// </summary>
    public class TablaInfo
    {
        public string Nombre { get; set; }
        public string Comentario { get; set; }
        public long RowCount { get; set; }
        public List<ColumnaInfo> Columnas { get; set; } = new List<ColumnaInfo>();
        public List<string> PrimaryKey { get; set; } = new List<string>();
        public List<ForeignKeyInfo> ForeignKeys { get; set; } = new List<ForeignKeyInfo>();
        public List<IndexInfo> Indices { get; set; } = new List<IndexInfo>();
        public string CreateTableScriptTSQL { get; set; }
        public string CreateForeignKeysScriptTSQL { get; set; }
        public string CreateIndexesScriptTSQL { get; set; }
    }

    /// <summary>
    /// Resultado de migración por tabla.
    /// </summary>
    public class ResultadoMigracionTabla
    {
        public string NombreTabla { get; set; }
        public long FilasOrigen { get; set; }
        public long FilasDestino { get; set; }
        public bool SchemaCreado { get; set; }
        public bool DatosMigrados { get; set; }
        public string Error { get; set; }
        public TimeSpan Duracion { get; set; }
        public bool Exitoso => string.IsNullOrEmpty(Error) && SchemaCreado && DatosMigrados;
    }

    /// <summary>
    /// Helper que orquesta la migración de MySQL a SQL Server.
    /// Lee el schema desde MySQL, genera T-SQL, migra datos y verifica.
    /// </summary>
    public class MigracionHelper
    {
        private readonly string _sqlConnectionString;

        public MigracionHelper(string sqlServerConnectionString)
        {
            _sqlConnectionString = sqlServerConnectionString;
        }

        #region Lectura de Schema desde MySQL

        /// <summary>
        /// Obtiene la lista de tablas con sus columnas desde MySQL.
        /// </summary>
        public List<TablaInfo> ObtenerTablasMySQL(List<string> tablasFiltro = null)
        {
            var tablas = new List<TablaInfo>();

            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    // Obtener tablas
                    string sqlTablas = @"
                        SELECT TABLE_NAME, TABLE_COMMENT, TABLE_ROWS
                        FROM information_schema.TABLES
                        WHERE TABLE_SCHEMA = DATABASE()
                        AND TABLE_TYPE = 'BASE TABLE'
                        ORDER BY TABLE_NAME";

                    using (var cmd = new MySqlCommand(sqlTablas, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string nombre = reader.GetString("TABLE_NAME");

                            if (tablasFiltro != null && !tablasFiltro.Contains(nombre, StringComparer.OrdinalIgnoreCase))
                                continue;

                            var tabla = new TablaInfo
                            {
                                Nombre = nombre,
                                Comentario = reader.IsDBNull(reader.GetOrdinal("TABLE_COMMENT"))
                                    ? "" : reader.GetString("TABLE_COMMENT"),
                                RowCount = reader.IsDBNull(reader.GetOrdinal("TABLE_ROWS"))
                                    ? 0 : reader.GetInt64("TABLE_ROWS")
                            };

                            tablas.Add(tabla);
                        }
                    }

                    // Obtener columnas, PKs, FKs e índices para cada tabla
                    foreach (var tabla in tablas)
                    {
                        tabla.Columnas = ObtenerColumnasMySQL(conn, tabla.Nombre);
                        tabla.PrimaryKey = ObtenerPrimaryKeyMySQL(conn, tabla.Nombre);
                        tabla.ForeignKeys = ObtenerForeignKeysMySQL(conn, tabla.Nombre);
                        tabla.Indices = ObtenerIndicesMySQL(conn, tabla.Nombre);
                        tabla.CreateTableScriptTSQL = GenerarCreateTableTSQL(tabla);
                        tabla.CreateForeignKeysScriptTSQL = GenerarForeignKeysTSQL(tabla);
                        tabla.CreateIndexesScriptTSQL = GenerarIndexesTSQL(tabla);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer schema de MySQL", ex);
                throw new Exception("Error al leer schema de MySQL: " + ex.Message);
            }

            return tablas;
        }

        private List<ColumnaInfo> ObtenerColumnasMySQL(MySqlConnection conn, string nombreTabla)
        {
            var columnas = new List<ColumnaInfo>();

            string sql = @"
                SELECT 
                    c.COLUMN_NAME,
                    c.COLUMN_TYPE,
                    c.DATA_TYPE,
                    c.IS_NULLABLE,
                    c.COLUMN_DEFAULT,
                    c.EXTRA,
                    c.CHARACTER_MAXIMUM_LENGTH,
                    c.NUMERIC_PRECISION,
                    c.NUMERIC_SCALE,
                    c.COLUMN_COMMENT
                FROM information_schema.COLUMNS c
                WHERE c.TABLE_SCHEMA = DATABASE()
                  AND c.TABLE_NAME = @tableName
                ORDER BY c.ORDINAL_POSITION";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", nombreTabla);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var col = new ColumnaInfo
                        {
                            Nombre = reader.GetString("COLUMN_NAME"),
                            TipoMySQL = reader.IsDBNull(reader.GetOrdinal("COLUMN_TYPE"))
                                ? "" : reader.GetString("COLUMN_TYPE"),
                            EsNullable = reader.GetString("IS_NULLABLE") == "YES",
                            MaxLength = reader.IsDBNull(reader.GetOrdinal("CHARACTER_MAXIMUM_LENGTH"))
                                ? (int?)null : Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]),
                            Precision = reader.IsDBNull(reader.GetOrdinal("NUMERIC_PRECISION"))
                                ? (int?)null : Convert.ToInt32(reader["NUMERIC_PRECISION"]),
                            Scale = reader.IsDBNull(reader.GetOrdinal("NUMERIC_SCALE"))
                                ? (int?)null : Convert.ToInt32(reader["NUMERIC_SCALE"]),
                            Comentario = reader.IsDBNull(reader.GetOrdinal("COLUMN_COMMENT"))
                                ? "" : reader.GetString("COLUMN_COMMENT"),
                            EsAutoIncrement = reader.GetString("EXTRA").Contains("auto_increment"),
                        };

                        // Default value como string
                        if (!reader.IsDBNull(reader.GetOrdinal("COLUMN_DEFAULT")))
                        {
                            string def = reader.GetString("COLUMN_DEFAULT");
                            // Convertir funciones MySQL a T-SQL
                            if (string.Equals(def, "CURRENT_TIMESTAMP", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(def, "current_timestamp()", StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(def, "now()", StringComparison.OrdinalIgnoreCase))
                            {
                                col.DefaultValue = "GETDATE()";
                            }
                            else if (string.Equals(def, "NULL", StringComparison.OrdinalIgnoreCase) ||
                                     string.IsNullOrEmpty(def))
                            {
                                col.DefaultValue = null;
                            }
                            else
                            {
                                // Detectar si es número o expresión
                                bool esNumero = long.TryParse(def, out _) ||
                                                double.TryParse(def, System.Globalization.NumberStyles.Any,
                                                    System.Globalization.CultureInfo.InvariantCulture, out _);
                                bool esFuncion = def.Contains("(") || def.Contains("`");

                                if (esNumero || esFuncion)
                                {
                                    col.DefaultValue = def;
                                }
                                else
                                {
                                    // Es un string literal → comillas simples T-SQL
                                    string valorLimpio = def.TrimStart('\'').TrimEnd('\'');
                                    col.DefaultValue = $"'{valorLimpio.Replace("'", "''")}'";
                                }
                            }
                        }

                        // Convertir tipo
                        col.TipoTSQL = ConvertirTipoMySQLtoTSQL(col);

                        columnas.Add(col);
                    }
                }
            }

            return columnas;
        }

        private List<string> ObtenerPrimaryKeyMySQL(MySqlConnection conn, string nombreTabla)
        {
            var pk = new List<string>();

            string sql = @"
                SELECT k.COLUMN_NAME
                FROM information_schema.TABLE_CONSTRAINTS tc
                INNER JOIN information_schema.KEY_COLUMN_USAGE k 
                    ON tc.TABLE_SCHEMA = k.TABLE_SCHEMA 
                    AND tc.TABLE_NAME = k.TABLE_NAME 
                    AND tc.CONSTRAINT_NAME = k.CONSTRAINT_NAME
                WHERE tc.TABLE_SCHEMA = DATABASE()
                  AND tc.TABLE_NAME = @tableName
                  AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                ORDER BY k.ORDINAL_POSITION";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", nombreTabla);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        pk.Add(reader.GetString("COLUMN_NAME"));
                }
            }

            return pk;
        }

        private List<ForeignKeyInfo> ObtenerForeignKeysMySQL(MySqlConnection conn, string nombreTabla)
        {
            var fks = new List<ForeignKeyInfo>();

            string sql = @"
                SELECT 
                    k.CONSTRAINT_NAME,
                    k.COLUMN_NAME,
                    k.REFERENCED_TABLE_NAME,
                    k.REFERENCED_COLUMN_NAME,
                    COALESCE(r.DELETE_RULE, 'NO ACTION') AS DELETE_RULE,
                    COALESCE(r.UPDATE_RULE, 'NO ACTION') AS UPDATE_RULE
                FROM information_schema.KEY_COLUMN_USAGE k
                LEFT JOIN information_schema.REFERENTIAL_CONSTRAINTS r
                    ON k.CONSTRAINT_NAME = r.CONSTRAINT_NAME
                    AND k.CONSTRAINT_SCHEMA = r.CONSTRAINT_SCHEMA
                WHERE k.TABLE_SCHEMA = DATABASE()
                  AND k.TABLE_NAME = @tableName
                  AND k.REFERENCED_TABLE_NAME IS NOT NULL";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", nombreTabla);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        fks.Add(new ForeignKeyInfo
                        {
                            Nombre = reader.GetString("CONSTRAINT_NAME"),
                            ColumnaOrigen = reader.GetString("COLUMN_NAME"),
                            TablaDestino = reader.GetString("REFERENCED_TABLE_NAME"),
                            ColumnaDestino = reader.GetString("REFERENCED_COLUMN_NAME"),
                            DeleteRule = reader.GetString("DELETE_RULE"),
                            UpdateRule = reader.GetString("UPDATE_RULE")
                        });
                    }
                }
            }

            return fks;
        }

        private List<IndexInfo> ObtenerIndicesMySQL(MySqlConnection conn, string nombreTabla)
        {
            var indices = new List<IndexInfo>();

            string sql = @"
                SELECT 
                    INDEX_NAME,
                    COLUMN_NAME,
                    NON_UNIQUE,
                    SEQ_IN_INDEX
                FROM information_schema.STATISTICS
                WHERE TABLE_SCHEMA = DATABASE()
                  AND TABLE_NAME = @tableName
                  AND INDEX_NAME != 'PRIMARY'
                ORDER BY INDEX_NAME, SEQ_IN_INDEX";

            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@tableName", nombreTabla);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        indices.Add(new IndexInfo
                        {
                            Nombre = reader.GetString("INDEX_NAME"),
                            Columna = reader.GetString("COLUMN_NAME"),
                            Orden = reader.GetInt32("SEQ_IN_INDEX"),
                            EsUnico = reader.GetInt64("NON_UNIQUE") == 0
                        });
                    }
                }
            }

            return indices;
        }

        #endregion

        #region Conversión de Tipos MySQL → T-SQL

        /// <summary>
        /// Convierte un tipo de dato MySQL a su equivalente en T-SQL.
        /// </summary>
        public static string ConvertirTipoMySQLtoTSQL(ColumnaInfo col)
        {
            string tipoCompleto = col.TipoMySQL.ToLower();
            string tipoBase = tipoCompleto.Split('(', ' ')[0];
            bool esUnsigned = tipoCompleto.Contains("unsigned");
            bool esTinyint1 = tipoBase == "tinyint" && col.TipoMySQL.Contains("(1)");

            // TINYINT(1) en MySQL es boolean → BIT en SQL Server
            if (esTinyint1) return "BIT";

            switch (tipoBase)
            {
                // Enteros
                case "tinyint": return "TINYINT";
                case "smallint": return "SMALLINT";
                case "mediumint": return esUnsigned ? "INT" : "INT";
                case "int": return "INT";
                case "integer": return "INT";
                case "bigint": return "BIGINT";
                case "bit": return "BIT";

                // Decimales
                case "decimal": return $"DECIMAL({col.Precision ?? 18},{col.Scale ?? 0})";
                case "numeric": return $"NUMERIC({col.Precision ?? 18},{col.Scale ?? 0})";
                case "float": return "FLOAT";
                case "double": return "FLOAT";
                case "real": return "REAL";

                // Cadenas
                case "char":
                    return col.MaxLength.HasValue ? $"CHAR({col.MaxLength})" : "CHAR(1)";
                case "varchar":
                    return col.MaxLength.HasValue && col.MaxLength <= 8000
                        ? $"VARCHAR({col.MaxLength})"
                        : "VARCHAR(MAX)";
                case "tinytext":
                case "text":
                case "mediumtext":
                case "longtext":
                    return "NVARCHAR(MAX)";

                // Binarios
                case "binary": return $"BINARY({col.MaxLength ?? 1})";
                case "varbinary": return col.MaxLength.HasValue && col.MaxLength <= 8000
                        ? $"VARBINARY({col.MaxLength})" : "VARBINARY(MAX)";
                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                    return "VARBINARY(MAX)";

                // Fecha/Hora
                case "date": return "DATE";
                case "datetime":
                case "timestamp": return "DATETIME2";
                case "time": return "TIME";
                case "year": return "SMALLINT";

                // Otros
                case "enum": return $"VARCHAR({Math.Max(col.MaxLength ?? 50, 50)})";
                case "set": return $"VARCHAR({Math.Max(col.MaxLength ?? 200, 200)})";
                case "json": return "NVARCHAR(MAX)";
                case "geometry":
                case "point":
                case "linestring":
                case "polygon":
                    return "NVARCHAR(MAX)";

                default:
                    Logger.Warning($"Tipo MySQL no reconocido: '{tipoBase}', usando NVARCHAR(255)");
                    return "NVARCHAR(255)";
            }
        }

        #endregion

        #region Generación de CREATE TABLE en T-SQL

        /// <summary>
        /// Genera el script CREATE TABLE en T-SQL para una tabla de MySQL.
        /// </summary>
        public string GenerarCreateTableTSQL(TablaInfo tabla)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"-- Tabla: {tabla.Nombre}");
            if (!string.IsNullOrEmpty(tabla.Comentario))
                sb.AppendLine($"-- {tabla.Comentario}");
            sb.AppendLine($"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{tabla.Nombre}]') AND type in (N'U'))");
            sb.AppendLine($"BEGIN");
            sb.AppendLine($"    CREATE TABLE [dbo].[{tabla.Nombre}] (");
            sb.AppendLine("");

            for (int i = 0; i < tabla.Columnas.Count; i++)
            {
                var col = tabla.Columnas[i];
                string nullable = col.EsNullable ? "NULL" : "NOT NULL";
                string identity = col.EsAutoIncrement ? " IDENTITY(1,1)" : "";
                string defValue = string.IsNullOrEmpty(col.DefaultValue) ? "" : $" DEFAULT {col.DefaultValue}";

                sb.Append($"        [{col.Nombre}] {col.TipoTSQL}{identity} {nullable}{defValue}");

                if (i < tabla.Columnas.Count - 1)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            // Primary Key
            if (tabla.PrimaryKey.Count > 0)
            {
                sb.AppendLine($"        ,CONSTRAINT [PK_{tabla.Nombre}] PRIMARY KEY CLUSTERED (");
                sb.AppendLine($"            {string.Join(",\n            ", tabla.PrimaryKey.Select(pk => $"[{pk}] ASC"))}");
                sb.AppendLine("        )");
            }

            sb.AppendLine("    );");
            sb.AppendLine("END;");

            return sb.ToString();
        }

        /// <summary>
        /// Genera los scripts ALTER TABLE para las FOREIGN KEYs.
        /// </summary>
        public string GenerarForeignKeysTSQL(TablaInfo tabla)
        {
            if (tabla.ForeignKeys.Count == 0) return "";

            var sb = new StringBuilder();
            foreach (var fk in tabla.ForeignKeys)
            {
                string deleteRule = fk.DeleteRule.ToUpper() switch
                {
                    "CASCADE" => "CASCADE",
                    "SET NULL" => "SET NULL",
                    "SET DEFAULT" => "SET DEFAULT",
                    _ => "NO ACTION"  // RESTRICT → NO ACTION
                };

                sb.AppendLine($"IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'{fk.Nombre}') BEGIN");
                sb.AppendLine($"    ALTER TABLE [dbo].[{tabla.Nombre}]");
                sb.AppendLine($"        ADD CONSTRAINT [{fk.Nombre}] FOREIGN KEY ([{fk.ColumnaOrigen}])");
                sb.AppendLine($"        REFERENCES [dbo].[{fk.TablaDestino}]([{fk.ColumnaDestino}])");
                sb.AppendLine($"        ON DELETE {deleteRule};");
                sb.AppendLine($"END;");
                sb.AppendLine();

                fk.CreateScriptTSQL = sb.ToString();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Genera los scripts CREATE INDEX para los índices no-PK.
        /// </summary>
        public string GenerarIndexesTSQL(TablaInfo tabla)
        {
            if (tabla.Indices.Count == 0) return "";

            var sb = new StringBuilder();
            // Agrupar por nombre de índice (un índice puede tener varias columnas)
            var grupos = tabla.Indices.GroupBy(i => i.Nombre);

            foreach (var grupo in grupos)
            {
                string unique = grupo.First().EsUnico ? "UNIQUE " : "";
                string columnas = string.Join(", ",
                    grupo.OrderBy(i => i.Orden).Select(i => $"[{i.Columna}] ASC"));

                sb.AppendLine($"IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'{grupo.Key}' AND object_id = OBJECT_ID(N'[dbo].[{tabla.Nombre}]')) BEGIN");
                sb.AppendLine($"    CREATE {unique}NONCLUSTERED INDEX [{grupo.Key}]");
                sb.AppendLine($"        ON [dbo].[{tabla.Nombre}]({columnas});");
                sb.AppendLine($"END;");
                sb.AppendLine();

                foreach (var idx in grupo)
                    idx.CreateScriptTSQL = sb.ToString();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Ejecuta los scripts de FK e índices para TODAS las tablas después de migrar los datos.
        /// Esto evita errores de dependencia cuando la tabla A tiene FK a la tabla B.
        /// </summary>
        public void EjecutarPostMigracionGlobal(List<TablaInfo> tablas, Action<string, int> reportarProgreso)
        {
            int total = tablas.Count;
            int i = 0;

            // Primero todos los índices (no tienen dependencias)
            foreach (var tabla in tablas.Where(t => !string.IsNullOrEmpty(t.CreateIndexesScriptTSQL)))
            {
                int progreso = (int)((double)i / total * 50);
                reportarProgreso?.Invoke($"Índice [{tabla.Nombre}]...", progreso);
                EjecutarScriptTSQL(tabla.CreateIndexesScriptTSQL);
                i++;
            }

            // Después todas las FKs (pueden depender unas de otras, orden crea/drop)
            i = 0;
            foreach (var tabla in tablas.Where(t => !string.IsNullOrEmpty(t.CreateForeignKeysScriptTSQL)))
            {
                int progreso = 50 + (int)((double)i / total * 50);
                reportarProgreso?.Invoke($"FK [{tabla.Nombre}]...", progreso);
                EjecutarScriptTSQL(tabla.CreateForeignKeysScriptTSQL);
                i++;
            }

            reportarProgreso?.Invoke($"✅ Post-migración completada: {tablas.Count} tablas procesadas", 100);
        }

        #endregion

        #region Ejecución en SQL Server

        /// <summary>
        /// Prueba la conexión a SQL Server.
        /// </summary>
        public bool ProbarConexionSQLServer(out string version)
        {
            version = "";
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                {
                    conn.Open();
                    version = conn.ServerVersion;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ejecuta un script T-SQL (para CREATE TABLE).
        /// </summary>
        public void EjecutarScriptTSQL(string script)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
            {
                conn.Open();
                using (var cmd = new System.Data.SqlClient.SqlCommand(script, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Obtiene el conteo de filas de una tabla en SQL Server.
        /// </summary>
        public long ObtenerConteoSQLServer(string nombreTabla)
        {
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand($"SELECT COUNT(*) FROM [{nombreTabla}]", conn))
                    {
                        return Convert.ToInt64(cmd.ExecuteScalar());
                    }
                }
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Migra datos de una tabla de MySQL a SQL Server usando SqlBulkCopy.
        /// </summary>
        public ResultadoMigracionTabla MigrarTabla(TablaInfo tabla, Action<string, int> reportarProgreso)
        {
            var resultado = new ResultadoMigracionTabla
            {
                NombreTabla = tabla.Nombre,
                FilasOrigen = tabla.RowCount
            };

            var inicio = DateTime.Now;

            try
            {
                // Paso 1: Crear la tabla en SQL Server (si no existe)
                reportarProgreso?.Invoke($"Creando tabla [{tabla.Nombre}]...", 0);
                EjecutarScriptTSQL(tabla.CreateTableScriptTSQL);
                resultado.SchemaCreado = true;

                // Si la tabla ya tenía datos (re-ejecución), truncar para evitar duplicados
                long filasActuales = ObtenerConteoSQLServer(tabla.Nombre);
                if (filasActuales > 0)
                {
                    reportarProgreso?.Invoke($"Truncando {filasActuales} filas existentes en [{tabla.Nombre}]...", 10);
                    using (var sqlConnTrunc = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                    {
                        sqlConnTrunc.Open();
                        using (var cmdTrunc = sqlConnTrunc.CreateCommand())
                        {
                            if (tabla.PrimaryKey.Count > 0 && tabla.Columnas.Any(c => c.EsAutoIncrement))
                            {
                                // Con IDENTITY, necesitamos DELETE (TRUNCATE requiere ALTER TABLE)
                                cmdTrunc.CommandText = $"DELETE FROM [{tabla.Nombre}]";
                            }
                            else
                            {
                                cmdTrunc.CommandText = $"TRUNCATE TABLE [{tabla.Nombre}]";
                            }
                            cmdTrunc.ExecuteNonQuery();
                        }
                    }
                }

                // Paso 2: Migrar datos
                reportarProgreso?.Invoke($"Migrando datos a [{tabla.Nombre}]...", 25);

                long totalMigradas = 0;

                using (var mysqlConn = MySQLConnection.GetConnection())
                {
                    mysqlConn.Open();

                    bool tieneIdentity = tabla.Columnas.Any(c => c.EsAutoIncrement);
                    string columnasSelect = string.Join(", ", tabla.Columnas.Select(c => $"`{c.Nombre}`"));
                    string sqlSelect = $"SELECT {columnasSelect} FROM `{tabla.Nombre}`";

                    using (var cmdMySQL = new MySqlCommand(sqlSelect, mysqlConn))
                    using (var reader = cmdMySQL.ExecuteReader())
                    {
                        // Crear DataTable completo
                        var dtCompleto = new DataTable();
                        foreach (var col in tabla.Columnas)
                            dtCompleto.Columns.Add(col.Nombre, typeof(object));

                        // Leer todas las filas primero para analizar identity
                        while (reader.Read())
                        {
                            var row = dtCompleto.NewRow();
                            foreach (var col in tabla.Columnas)
                            {
                                try
                                {
                                    object valor = reader[col.Nombre];
                                    row[col.Nombre] = valor == DBNull.Value ? DBNull.Value : valor;
                                }
                                catch { row[col.Nombre] = DBNull.Value; }
                            }
                            dtCompleto.Rows.Add(row);
                        }

                        // Determinar si realmente necesitamos IDENTITY_INSERT
                        bool identityConValores = false;
                        string colIdentity = null;
                        if (tieneIdentity)
                        {
                            var columnaIdentity = tabla.Columnas.FirstOrDefault(c => c.EsAutoIncrement);
                            if (columnaIdentity != null)
                            {
                                colIdentity = columnaIdentity.Nombre;
                                // Verificar si hay al menos un valor no-null en la columna identity
                                identityConValores = dtCompleto.AsEnumerable()
                                    .Any(row => row[colIdentity] != null && row[colIdentity] != DBNull.Value &&
                                        Convert.ToInt64(row[colIdentity]) > 0);
                            }
                        }

                        // Crear el DataTable final (con o sin identity según corresponda)
                        var dtFinal = new DataTable();
                        foreach (var col in tabla.Columnas)
                        {
                            if (identityConValores && col.EsAutoIncrement)
                                continue; // identity va incluida con IDENTITY_INSERT ON
                            dtFinal.Columns.Add(col.Nombre, typeof(object));
                        }

                        // Copiar datos al dtFinal
                        foreach (DataRow rowOrig in dtCompleto.Rows)
                        {
                            var rowNew = dtFinal.NewRow();
                            foreach (DataColumn col in dtFinal.Columns)
                                rowNew[col.ColumnName] = rowOrig[col.ColumnName];
                            dtFinal.Rows.Add(rowNew);
                        }

                        // Bulk insert con o sin identity según corresponda
                        int batchSize = 1000;

                        for (int i = 0; i < dtFinal.Rows.Count; i += batchSize)
                        {
                            var batch = dtFinal.Clone();
                            for (int j = i; j < Math.Min(i + batchSize, dtFinal.Rows.Count); j++)
                                batch.ImportRow(dtFinal.Rows[j]);

                            BulkInsertSQLServer(tabla.Nombre, batch, identityConValores);
                            totalMigradas += batch.Rows.Count;

                            if (dtFinal.Rows.Count > 0)
                            {
                                int progreso = 25 + (int)((double)totalMigradas / Math.Max(1, tabla.RowCount) * 70);
                                reportarProgreso?.Invoke(
                                    $"Migradas {totalMigradas} filas a [{tabla.Nombre}]...",
                                    Math.Min(progreso, 95));
                            }
                        }
                    }
                }

                resultado.DatosMigrados = true;
                resultado.FilasDestino = totalMigradas;
                resultado.Duracion = DateTime.Now - inicio;

                reportarProgreso?.Invoke($"✅ Tabla [{tabla.Nombre}] migrada: {totalMigradas} filas", 100);
            }
            catch (Exception ex)
            {
                resultado.Error = ex.Message;
                resultado.Duracion = DateTime.Now - inicio;
                Logger.Error($"Error migrando tabla {tabla.Nombre}", ex);
                reportarProgreso?.Invoke($"❌ Error en [{tabla.Nombre}]: {ex.Message}", -1);
            }

            return resultado;
        }

        private void BulkInsertSQLServer(string nombreTabla, DataTable dt, bool tieneIdentity)
        {
            using (var sqlConn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
            {
                sqlConn.Open();

                using (var bulkCopy = new System.Data.SqlClient.SqlBulkCopy(sqlConn))
                {
                    bulkCopy.DestinationTableName = $"[dbo].[{nombreTabla}]";
                    bulkCopy.BatchSize = dt.Rows.Count;
                    bulkCopy.BulkCopyTimeout = 120;

                    // Mapeo explícito de columnas por nombre (evita errores por orden/colación)
                    foreach (DataColumn col in dt.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    // Si tiene identity, activamos SET IDENTITY_INSERT
                    if (tieneIdentity)
                    {
                        bulkCopy.EnableStreaming = false;
                        using (var cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SET IDENTITY_INSERT [{nombreTabla}] ON;";
                            cmd.ExecuteNonQuery();
                        }
                    }

                    bulkCopy.WriteToServer(dt);

                    if (tieneIdentity)
                    {
                        using (var cmd = sqlConn.CreateCommand())
                        {
                            cmd.CommandText = $"SET IDENTITY_INSERT [{nombreTabla}] OFF;";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Verifica que los conteos de filas coincidan entre MySQL y SQL Server.
        /// </summary>
        public ResultadoMigracionTabla VerificarTabla(TablaInfo tabla, long filasMigradas)
        {
            var resultado = new ResultadoMigracionTabla
            {
                NombreTabla = tabla.Nombre,
                FilasOrigen = tabla.RowCount,
                SchemaCreado = true,
                DatosMigrados = true
            };

            try
            {
                long filasSQL = ObtenerConteoSQLServer(tabla.Nombre);
                resultado.FilasDestino = filasSQL;

                if (filasSQL != resultado.FilasOrigen)
                {
                    resultado.Error = $"Las filas no coinciden: MySQL={resultado.FilasOrigen}, SQL Server={filasSQL}";
                }
            }
            catch (Exception ex)
            {
                resultado.Error = "Error en verificación: " + ex.Message;
            }

            return resultado;
        }

        /// <summary>
        /// Obtiene las tablas existentes en la base de datos SQL Server destino.
        /// </summary>
        public List<string> ObtenerTablasSQLServer()
        {
            var tablas = new List<string>();
            try
            {
                using (var conn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                {
                    conn.Open();
                    DataTable schema = conn.GetSchema("Tables");
                    foreach (DataRow row in schema.Rows)
                    {
                        string tipo = row["TABLE_TYPE"].ToString();
                        if (tipo == "BASE TABLE")
                            tablas.Add(row["TABLE_NAME"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning($"No se pudieron obtener tablas de SQL Server: {ex.Message}");
            }
            return tablas;
        }

        /// <summary>
        /// Crea una base de datos en SQL Server si no existe.
        /// Se conecta a 'master' para ejecutar el CREATE DATABASE.
        /// </summary>
        public bool CrearBaseDeDatos(string server, string dbName, bool useWindowsAuth, string user, string password)
        {
            try
            {
                // Connection string apuntando a master
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder
                {
                    DataSource = server,
                    InitialCatalog = "master",
                    ConnectTimeout = 30
                };

                if (useWindowsAuth)
                    builder.IntegratedSecurity = true;
                else
                {
                    builder.IntegratedSecurity = false;
                    builder.UserID = user;
                    builder.Password = password;
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(builder.ConnectionString))
                {
                    conn.Open();

                    // Verificar si ya existe
                    string checkSql = $"SELECT COUNT(*) FROM sys.databases WHERE name = @db";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(checkSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@db", dbName);
                        int existe = (int)cmd.ExecuteScalar();
                        if (existe > 0)
                            return true; // ya existe
                    }

                    // Crear la base de datos
                    string createSql = $"CREATE DATABASE [{dbName}]";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(createSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al crear base de datos '{dbName}'", ex);
                throw new Exception($"No se pudo crear la base de datos '{dbName}': {ex.Message}");
            }
        }

        #endregion
    }
}
