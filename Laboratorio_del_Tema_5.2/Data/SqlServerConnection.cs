using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Laboratorio_del_Tema_5_2.Data
{
    /// <summary>
    /// Helper para conexión a SQL Server.
    /// Sigue el mismo patrón que MySQLConnection.
    /// 
    /// Para activarlo, agregar en App.config:
    /// 
    /// <connectionStrings>
    ///     <add name="SqlServer" 
    ///          connectionString="Server=localhost\SQLEXPRESS;Database=ModeloDualDB_SQL;Integrated Security=True;"
    ///          providerName="System.Data.SqlClient" />
    /// </connectionStrings>
    /// </summary>
    public static class SqlServerConnection
    {
        private static string ConnectionString
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["SqlServer"]?.ConnectionString
                        ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'SqlServer' en App.config.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error al cargar connection string SQL Server: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Obtiene una nueva conexion a SQL Server.
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Verifica si la conexion a SQL Server funciona.
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    Console.WriteLine("Conexion a SQL Server exitosa!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de conexion SQL Server: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la version del servidor SQL Server.
        /// </summary>
        public static string GetSqlServerVersion()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return conn.ServerVersion;
                }
            }
            catch (Exception ex)
            {
                return "No conectado: " + ex.Message;
            }
        }
    }
}
