using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Database;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Views
{
    /// <summary>
    /// Asistente interactivo de migración MySQL → SQL Server.
    /// 7 pasos: Bienvenida → MySQL → SQL Server → Selección → Migración → Verificación → Resumen
    /// </summary>
    public partial class FormMigracionBD : Form
    {
        private const int TOTAL_STEPS = 7;
        private int _currentStep = 0;

        private List<TablaInfo> _tablasMySQL;
        private MigracionHelper _helper;
        private string _sqlConnectionString;
        private bool _sqlServerConectado = false;
        private bool _sqlServerSkipped = false;
        private bool _mysqlScanneado = false;
        private bool _migracionCompletada = false;
        private bool _migracionEnCurso = false;
        private CancellationTokenSource _cancelTokenSource;
        private List<ResultadoMigracionTabla> _resultados;
        private List<string> _tablasMigradasExitosamente = new List<string>();
        private string _logFilePath;

        public FormMigracionBD()
        {
            InitializeComponent();

            _tablasMySQL = new List<TablaInfo>();
            _resultados = new List<ResultadoMigracionTabla>();
            _cancelTokenSource = new CancellationTokenSource();

            // Ocultar tabs del TabControl solo en runtime (en el Designer se ven para editar)
            tcSteps.ItemSize = new System.Drawing.Size(0, 1);
            tcSteps.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;

            // Inicializar log a archivo
            string logDir = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "MigracionBD_Logs");
            System.IO.Directory.CreateDirectory(logDir);
            _logFilePath = System.IO.Path.Combine(logDir,
                $"migracion_{DateTime.Now:yyyyMMdd_HHmmss}.log");

            MostrarPaso(0);
        }

        // =============================================
        // NAVEGACIÓN
        // =============================================
        private void MostrarPaso(int paso)
        {
            // Si estamos en migración y hay curso, no cambiar
            if (_migracionEnCurso && paso == 4) return;

            tcSteps.SelectedIndex = paso;
            _currentStep = paso;

            btnAnterior.Visible = paso > 0 && paso < 6;
            btnSiguiente.Visible = paso < 6 && paso != 4 && paso != 5;
            btnIniciarMigracion.Visible = (paso == 4);
            btnCancelar.Text = (paso == 6 || _migracionCompletada) ? "Cerrar" : "Cancelar";

            if (paso == 5)
            {
                btnSiguiente.Visible = true;
                btnSiguiente.Text = "Ver Resumen >>";
            }
            if (paso == 6)
            {
                btnSiguiente.Visible = false;
                btnAnterior.Visible = false;
            }

            string[] titulos = {
                "Bienvenida", "MySQL", "SQL Server",
                "Selección", "Migración", "Verificación", "Resumen"
            };
            lblStepIndicator.Text = $"Paso {paso + 1} de {TOTAL_STEPS} — {titulos[paso]}";
        }

        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            // Validar paso actual antes de avanzar
            if (_currentStep == 1 && !_mysqlScanneado)
            {
                MessageBox.Show("Escanéá las tablas de MySQL primero con el botón 'Escanear'.",
                    "Pendiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_currentStep == 2 && !_sqlServerConectado && !_sqlServerSkipped)
            {
                var r = MessageBox.Show(
                    "Todavía no probaste la conexión a SQL Server.\n\n" +
                    "• Hacé clic en 'Probar conexión' si ya tenés SQL Server instalado.\n" +
                    "• O hacé clic en 'Omitir' si querés solo ver los scripts generados.\n\n" +
                    "¿Querés continuar sin conexión a SQL Server?",
                    "Conexión no probada", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
                _sqlServerSkipped = true;
            }
            if (_currentStep == 3 && clbTablas.CheckedItems.Count == 0)
            {
                MessageBox.Show("Seleccioná al menos una tabla para migrar.",
                    "Pendiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_currentStep == 5)
            {
                // Ir a resumen
                if (!_migracionCompletada)
                {
                    MessageBox.Show("Completá la migración primero.",
                        "Pendiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MostrarPaso(6);
                GenerarResumen();
                return;
            }

            MostrarPaso(_currentStep + 1);

            // Acciones al entrar al nuevo paso
            if (_currentStep == 1) EscanearMySQL();
            if (_currentStep == 3) CargarChecklistSeleccion();
            if (_currentStep == 5) CargarVerificacion();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            if (_currentStep > 0)
                MostrarPaso(_currentStep - 1);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (_migracionEnCurso)
            {
                var r = MessageBox.Show(
                    "¿Cancelar la migración en curso?\n\n" +
                    "Las tablas ya creadas en SQL Server no se eliminarán.",
                    "Confirmar cancelación",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r == DialogResult.Yes)
                {
                    _cancelTokenSource.Cancel();
                    _migracionEnCurso = false;
                    btnIniciarMigracion.Enabled = true;
                    btnIniciarMigracion.Text = "🚀  Reintentar Migración";
                    AgregarLog("⚠️  Migración cancelada por el usuario.", Color.Yellow);
                }
                return;
            }

            if (_migracionCompletada || _currentStep == 0)
            {
                this.Close();
                return;
            }

            var resp = MessageBox.Show(
                "¿Cancelar el proceso de migración? Los datos ingresados se perderán.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
                this.Close();
        }

        // =============================================
        // PASO 2: MySQL
        // =============================================
        private void FormMigracionBD_Load(object sender, EventArgs e)
        {
            EscanearMySQL();
        }

        private void btnRefreshMySQL_Click(object sender, EventArgs e)
        {
            EscanearMySQL();
        }

        private void EscanearMySQL()
        {
            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    lblMySQLStatus.Text = "●  MySQL conectado";
                    lblMySQLStatus.ForeColor = Color.FromArgb(40, 167, 69);
                    lblMySQLVersion.Text = $"Versión: {conn.ServerVersion} | BD: {conn.Database}";
                }
            }
            catch (Exception ex)
            {
                lblMySQLStatus.Text = "●  MySQL no disponible";
                lblMySQLStatus.ForeColor = Color.FromArgb(220, 53, 69);
                lblMySQLVersion.Text = "Verificá que MySQL esté corriendo en localhost:3307";
                _mysqlScanneado = false;
                MessageBox.Show($"No se pudo conectar a MySQL.\n\n{ex.Message}\n\nVerificá que MySQL esté corriendo en localhost:3307 y que las credenciales en App.config sean correctas.",
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var helper = new MigracionHelper("");
                _tablasMySQL = helper.ObtenerTablasMySQL();

                var displayList = _tablasMySQL.Select(t => new {
                    t.Nombre,
                    t.RowCount,
                    ColumnasCount = t.Columnas.Count,
                    t.Comentario
                }).ToList();

                dgvTablasMySQL.DataSource = null;
                dgvTablasMySQL.DataSource = displayList;
                lblMySQLInstrucciones.Text = $"Se encontraron {_tablasMySQL.Count} tablas en la base de datos.";
                _mysqlScanneado = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al escanear MySQL: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error("Error scanning MySQL schema", ex);
                _mysqlScanneado = false;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // =============================================
        // PASO 3: SQL Server
        // =============================================
        private void rbtnSQLAuth_CheckedChanged(object sender, EventArgs e)
        {
            bool sqlAuth = rbtnSQLAuth.Checked;
            txtSQLUser.Enabled = sqlAuth;
            txtSQLPassword.Enabled = sqlAuth;
            lblSQLUser.Enabled = sqlAuth;
            lblSQLPassword.Enabled = sqlAuth;
        }

        private void btnTestSQL_Click(object sender, EventArgs e)
        {
            ProbarConexionSQL();
        }

        private void btnSkipSQL_Click(object sender, EventArgs e)
        {
            ActivarModoSoloScripts();
        }

        private void ActivarModoSoloScripts()
        {
            _sqlServerSkipped = true;
            _sqlServerConectado = false;
            lblSQLStatus.Text = "⏭  Modo solo scripts — sin SQL Server";
            lblSQLStatus.ForeColor = Color.FromArgb(255, 140, 0);
            btnTestSQL.Enabled = false;
            btnSkipSQL.Enabled = false;
            _sqlConnectionString = "";
            _helper = new MigracionHelper("");

            MessageBox.Show(
                "⏭  MODO 'SOLO SCRIPTS' ACTIVADO\n\n" +
                "No se detectó SQL Server en este equipo, pero igual podés:\n\n" +
                "✅  Ver los scripts T-SQL generados automáticamente\n" +
                "✅  Ver el resumen con la connection string y pasos a seguir\n\n" +
                "📌  Cuando instales SQL Server, volvé a este asistente y ejecutá la migración completa.",
                "Modo solo scripts", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProbarConexionSQL()
        {
            string server = txtSQLServer.Text.Trim();
            string db = txtSQLDatabase.Text.Trim();
            int timeout = 15;

            if (string.IsNullOrEmpty(server))
            {
                MessageBox.Show("Ingresá el nombre del servidor SQL Server.\n\n" +
                    "Ejemplos:\n" +
                    "• localhost              (instancia por defecto)\n" +
                    "• localhost\\SQLEXPRESS  (instancia Express)\n" +
                    "• .\\SQLEXPRESS          (forma corta)\n" +
                    "• localhost,1433         (puerto específico)\n" +
                    "• (local)\\SQLEXPRESS\n" +
                    "• 192.168.1.100\\MSSQLSERVER\n\n" +
                    "💡 Si no tenés SQL Server, usá 'Omitir (ver scripts)'.",
                    "Servidor requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(db))
            {
                MessageBox.Show("Ingresá el nombre de la base de datos destino.\n\n" +
                    "Creala primero en SQL Server Management Studio (SSMS):\n" +
                    "  CREATE DATABASE [ModeloDualDB_SQL];",
                    "BD requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder
                {
                    DataSource = server,
                    InitialCatalog = db,
                    ConnectTimeout = timeout
                };

                if (rbtnSQLAuth.Checked)
                {
                    builder.IntegratedSecurity = false;
                    builder.UserID = txtSQLUser.Text.Trim();
                    builder.Password = txtSQLPassword.Text;
                }
                else
                {
                    builder.IntegratedSecurity = true;
                }

                _sqlConnectionString = builder.ConnectionString;
                _helper = new MigracionHelper(_sqlConnectionString);

                Cursor = Cursors.WaitCursor;

                try
                {
                    using (var conn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                    {
                        conn.Open();
                        string version = conn.ServerVersion;
                        _sqlServerConectado = true;
                        _sqlServerSkipped = false;

                        lblSQLStatus.Text = $"✅  Conectado a SQL Server ({version})";
                        lblSQLStatus.ForeColor = Color.FromArgb(40, 167, 69);
                        MessageBox.Show(
                            $"Conexión exitosa a SQL Server!\n\n" +
                            $"Versión: {version}\n" +
                            $"Servidor: {server}\n" +
                            $"Base de datos: {db}",
                            "Conexión OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (System.Data.SqlClient.SqlException sqlEx)
                {
                    _sqlServerConectado = false;

                    // Si la BD no existe (error 4060), ofrecer crearla
                    if (sqlEx.Number == 4060)
                    {
                        lblSQLStatus.Text = "⚠️  La base de datos no existe";
                        lblSQLStatus.ForeColor = Color.FromArgb(255, 140, 0);

                        var r = MessageBox.Show(
                            $"⚠️  La base de datos '{db}' no existe en el servidor '{server}'.\n\n" +
                            "¿Querés que la cree automáticamente?\n\n" +
                            "   Sí  → Se ejecutará: CREATE DATABASE [{db}]\n" +
                            "   No  → Podés omitir y ver solo los scripts",
                            "Base de datos no encontrada",
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                        if (r == DialogResult.Yes)
                        {
                            try
                            {
                                Cursor = Cursors.WaitCursor;
                                bool creada = _helper.CrearBaseDeDatos(
                                    server, db, rbtnWindowsAuth.Checked,
                                    txtSQLUser.Text.Trim(), txtSQLPassword.Text);

                                if (creada)
                                {
                                    // Reintentar conexión con la BD recién creada
                                    using (var conn2 = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                                    {
                                        conn2.Open();
                                        string version = conn2.ServerVersion;
                                        _sqlServerConectado = true;
                                        _sqlServerSkipped = false;
                                        lblSQLStatus.Text = $"✅  BD '{db}' creada — Conectado ({version})";
                                        lblSQLStatus.ForeColor = Color.FromArgb(40, 167, 69);
                                        MessageBox.Show(
                                            $"✅  Base de datos '{db}' creada exitosamente!\n\n" +
                                            $"Servidor: {server}\nVersión: {version}",
                                            "BD Creada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                    return;
                                }
                            }
                            catch (Exception createEx)
                            {
                                MessageBox.Show(
                                    $"Error al crear la base de datos:\n\n{createEx.Message}\n\n" +
                                    "Creala manualmente desde SSMS con:\n" +
                                    $"  CREATE DATABASE [{db}];",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            finally { Cursor = Cursors.Default; }
                        }

                        if (r == DialogResult.No)
                        {
                            ActivarModoSoloScripts();
                            return;
                        }
                        return;
                    }

                    // Otros errores SQL
                    string ayuda = DiagnosticarErrorSQL(sqlEx, server, db, timeout);
                    lblSQLStatus.Text = $"❌  {sqlEx.Number}: {sqlEx.Message.Split('\r', '\n')[0]}";
                    lblSQLStatus.ForeColor = Color.FromArgb(220, 53, 69);
                    bool quiereOmitir = MessageBox.Show(
                        $"Error de SQL Server #{sqlEx.Number}:\n\n{sqlEx.Message}\n\n{ayuda}\n\n" +
                        "❓  ¿Querés omitir la conexión y ver solo los scripts generados?",
                        "Error de conexión", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;

                    if (quiereOmitir)
                    {
                        ActivarModoSoloScripts();
                        return;
                    }
                }
                catch (InvalidOperationException ioEx)
                {
                    _sqlServerConectado = false;
                    lblSQLStatus.Text = "❌  Error de configuración";
                    lblSQLStatus.ForeColor = Color.FromArgb(220, 53, 69);
                    MessageBox.Show(
                        $"Error de configuración:\n\n{ioEx.Message}\n\n" +
                        "Verificá que el nombre del servidor sea correcto y que SQL Server esté instalado.\n" +
                        "Si no tenés SQL Server, descargalo desde: https://go.microsoft.com/fwlink/?linkid=866662",
                        "Error de configuración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                _sqlServerConectado = false;
                lblSQLStatus.Text = "❌  Error inesperado";
                lblSQLStatus.ForeColor = Color.FromArgb(220, 53, 69);
                MessageBox.Show(
                    $"Error inesperado:\n\n{ex.Message}\n\n" +
                    "Esto puede ocurrir si System.Data.SqlClient no está disponible en tu sistema.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Diagnostica errores comunes de SQL Server y da ayuda específica.
        /// </summary>
        private string DiagnosticarErrorSQL(System.Data.SqlClient.SqlException ex, string server, string db, int timeout)
        {
            switch (ex.Number)
            {
                case -1:  // Provider error (SNI) - no server found
                case 2:   // Network-related or instance-specific error
                case 53:  // Named Pipes provider
                    return $@"🔧  POSIBLES CAUSAS:
1. SQL Server NO está instalado → Descargalo de https://go.microsoft.com/fwlink/?linkid=866662
2. El servicio 'SQL Server (MSSQLSERVER)' no está iniciado
   → Abrí 'services.msc' y verificá que esté en 'Running'
3. El nombre del servidor/instancia es incorrecto
   → Probá estas opciones:
      • localhost          (instancia por defecto)
      • (local)\SQLEXPRESS (instancia Express)
      • .\SQLEXPRESS       (instancia Express, forma corta)
      • localhost,1433      (puerto específico)
4. El protocolo TCP/IP está deshabilitado
   → Abrí 'SQL Server Configuration Manager' → Protocolos → Habilitar TCP/IP
5. El servicio 'SQL Server Browser' no está iniciado
   → Abrí 'services.msc' y buscá 'SQL Server Browser' → iniciarlo

📌  Si estás probando el asistente sin tener SQL Server,
    usá el botón 'Omitir (ver scripts)' para ver los scripts generados.";
                case 4060: // Cannot open database
                    return $@"🔧  La base de datos '{db}' no existe en el servidor '{server}'.
Creala desde SSMS con:
   CREATE DATABASE [{db}];

O desde cmd con:
   sqlcmd -S {server} -Q ""CREATE DATABASE [{db}]""";
                case 18456: // Login failed
                    return @"🔧  Error de autenticación. Verificá:
• Si usás Windows Auth: asegurate de estar logueado en Windows con una cuenta con acceso
• Si usás SQL Auth: verificá el usuario y password
• El usuario puede no tener permiso de acceso a SQL Server";
                case 121: // Timeout
                case 258:
                    return $@"🔧  Tiempo de espera agotado ({timeout}s).
• SQL Server puede estar en un servidor remoto no accesible
• Firewall bloqueando el puerto 1433
• La instancia no está escuchando en TCP/IP";
                default:
                    return @"🔧  VERIFICÁ:
1. ¿Tenés SQL Server instalado? (https://go.microsoft.com/fwlink/?linkid=866662)
2. ¿El servicio está corriendo? (services.msc → SQL Server)
3. ¿El nombre del servidor/instancia es correcto?
4. ¿Usás Windows o SQL Auth y las credenciales son correctas?

📌  Si no tenés SQL Server todavía, usá 'Omitir (ver scripts)'.";
            }
        }

        // =============================================
        // PASO 4: SELECCIÓN DE TABLAS
        // =============================================
        private void CargarChecklistSeleccion()
        {
            if (_tablasMySQL == null || _tablasMySQL.Count == 0)
            {
                MessageBox.Show("No hay tablas disponibles. Escaneá MySQL primero.",
                    "Sin datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clbTablas.Items.Clear();
            foreach (var t in _tablasMySQL)
                clbTablas.Items.Add(t.Nombre, true);

            lblSeleccionInfo.Text = $"Seleccioná las tablas que querés migrar ({_tablasMySQL.Count} disponibles):";
        }

        private void btnSeleccionarTodo_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTablas.Items.Count; i++)
                clbTablas.SetItemChecked(i, true);
        }

        private void btnDeseleccionarTodo_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbTablas.Items.Count; i++)
                clbTablas.SetItemChecked(i, false);
        }

        // =============================================
        // PASO 5: MIGRACIÓN
        // =============================================
        private async void btnIniciarMigracion_Click(object sender, EventArgs e)
        {
            if (_sqlServerSkipped)
            {
                // No hay SQL Server, solo mostramos scripts
                MostrarSoloScripts();
                return;
            }

            if (_helper == null || string.IsNullOrEmpty(_sqlConnectionString))
            {
                MessageBox.Show("Configurá y probá la conexión a SQL Server primero.",
                    "Sin conexión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnIniciarMigracion.Enabled = false;
            btnIniciarMigracion.Text = "Migrando... (Click Cancelar para detener)";
            txtLog.Clear();
            _resultados.Clear();
            _cancelTokenSource = new CancellationTokenSource();
            _migracionEnCurso = true;
            _migracionCompletada = false;

            var seleccionadas = new List<string>();
            for (int i = 0; i < clbTablas.Items.Count; i++)
            {
                if (clbTablas.GetItemChecked(i))
                    seleccionadas.Add(clbTablas.Items[i].ToString());
            }

            if (seleccionadas.Count == 0)
            {
                MessageBox.Show("No hay tablas seleccionadas.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIniciarMigracion.Enabled = true;
                btnIniciarMigracion.Text = "🚀  Iniciar Migración";
                _migracionEnCurso = false;
                return;
            }

            // Ordenar tablas respetando dependencias (FK): las tablas sin PK primero
            var tablasMigrar = _tablasMySQL
                .Where(t => seleccionadas.Contains(t.Nombre, StringComparer.OrdinalIgnoreCase))
                .OrderBy(t => t.PrimaryKey.Count == 0 ? 0 : 1)
                .ThenBy(t => t.Nombre)
                .ToList();

            var token = _cancelTokenSource.Token;

            AgregarLog("=== INICIANDO MIGRACIÓN ===", Color.Cyan);
            AgregarLog($"Origen: MySQL ({MySQLConnection.GetConnection().DataSource}/ModeloDualDB)", Color.White);
            AgregarLog($"Destino: SQL Server ({txtSQLServer.Text}/{txtSQLDatabase.Text})", Color.White);
            AgregarLog($"Modo: {(rbtnWindowsAuth.Checked ? "Windows Auth" : "SQL Auth")}", Color.White);
            AgregarLog($"Tablas a migrar: {tablasMigrar.Count} (ordenadas por dependencia)", Color.White);
            AgregarLog("");

            progressBarGeneral.Maximum = tablasMigrar.Count * 100;
            progressBarGeneral.Value = 0;
            lblProgresoTexto.Text = "Iniciando...";

            var bindingList = new List<MigracionRow>();
            foreach (var t in tablasMigrar)
            {
                bindingList.Add(new MigracionRow
                {
                    NombreTabla = t.Nombre,
                    StatusStr = "⏳  Pendiente",
                    FilasStr = "—",
                    DuracionStr = "—"
                });
            }
            dgvMigracion.DataSource = null;
            dgvMigracion.DataSource = bindingList;

            int totalExitosas = 0;
            int totalFallidas = 0;
            long totalFilasMigradas = 0;
            var inicioGeneral = DateTime.Now;

            for (int i = 0; i < tablasMigrar.Count; i++)
            {
                if (token.IsCancellationRequested)
                {
                    AgregarLog("⚠️  Migración cancelada por el usuario.", Color.Yellow);
                    break;
                }

                var tabla = tablasMigrar[i];
                int idx = i;

                ActualizarEstadoFila(idx, "⏳  Migrando...", "—", "—");

                var resultado = await Task.Run(() =>
                    _helper.MigrarTabla(tabla, (msg, progreso) =>
                    {
                        if (progreso >= 0)
                        {
                            this.Invoke((MethodInvoker)(() =>
                            {
                                if (progressBarGeneral.Value < progressBarGeneral.Maximum)
                                    progressBarGeneral.Value = Math.Min((idx * 100) + progreso, progressBarGeneral.Maximum);
                                lblProgresoTexto.Text = $"[{tabla.Nombre}] {msg}";
                            }));
                        }
                    })
                , token);

                _resultados.Add(resultado);

                if (resultado.Exitoso)
                {
                    totalExitosas++;
                    totalFilasMigradas += resultado.FilasDestino;
                    _tablasMigradasExitosamente.Add(tabla.Nombre);
                    ActualizarEstadoFila(idx, "✅  Completado",
                        resultado.FilasDestino.ToString("N0"),
                        resultado.Duracion.ToString(@"mm\:ss"));
                    AgregarLog($"✅ {tabla.Nombre}: {resultado.FilasDestino:N0} filas migradas en {resultado.Duracion.TotalSeconds:F1}s", Color.LightGreen);
                }
                else
                {
                    totalFallidas++;
                    ActualizarEstadoFila(idx, $"❌  {TruncarError(resultado.Error, 50)}",
                        "0", resultado.Duracion.ToString(@"mm\:ss"));
                    AgregarLog($"❌ {tabla.Nombre}: {resultado.Error}", Color.Orange);
                }

                progressBarGeneral.Value = Math.Min((idx + 1) * 100, progressBarGeneral.Maximum);
            }

            // Paso final: crear índices y FKs para TODAS las tablas (después de todos los datos)
            if (!_sqlServerSkipped && tablasMigrar.Count > 0)
            {
                AgregarLog("", Color.Cyan);
                AgregarLog("=== CREANDO ÍNDICES Y FK POST-MIGRACIÓN ===", Color.Cyan);
                try
                {
                    _helper.EjecutarPostMigracionGlobal(tablasMigrar, (msg, prog) =>
                    {
                        this.Invoke((MethodInvoker)(() =>
                        {
                            lblProgresoTexto.Text = msg;
                            progressBarGeneral.Value = Math.Min(prog, 100);
                        }));
                    });
                    AgregarLog("✅ Índices y FKs creados correctamente", Color.LightGreen);
                }
                catch (Exception postEx)
                {
                    AgregarLog($"⚠️  Error en post-migración (índices/FKs): {postEx.Message}", Color.Orange);
                }
            }

            var duracionTotal = DateTime.Now - inicioGeneral;

            // Guardar log a archivo
            GuardarLogAArchivo();
            AgregarLog($"📁  Log guardado en: {_logFilePath}", Color.Cyan);

            _migracionCompletada = true;
            _migracionEnCurso = false;

            AgregarLog("");
            AgregarLog("=== MIGRACIÓN FINALIZADA ===", Color.Cyan);
            AgregarLog($"Completadas: {totalExitosas}  |  Fallidas: {totalFallidas}",
                totalFallidas == 0 ? Color.LightGreen : Color.Orange);
            AgregarLog($"Total filas migradas: {totalFilasMigradas:N0}", Color.White);
            AgregarLog($"Duración total: {duracionTotal.Minutes}min {duracionTotal.Seconds}s", Color.White);
            AgregarLog($"📁  Log guardado en: {_logFilePath}", Color.Cyan);

            // Rollback automático si hubo errores
            if (totalFallidas > 0 && _tablasMigradasExitosamente.Count > 0)
            {
                var resp = MessageBox.Show(
                    $"⚠️  {totalFallidas} tabla(s) fallaron, pero {totalExitosas} se migraron correctamente.\n\n" +
                    "¿Querés ELIMINAR las tablas que se migraron correctamente\n" +
                    "para poder reintentar la migración desde cero?",
                    "Rollback",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (resp == DialogResult.Yes)
                {
                    EjecutarRollback();
                }
            }

            lblProgresoTexto.Text = $"Migración completada: {totalExitosas} tablas, {totalFilasMigradas:N0} filas";

            btnIniciarMigracion.Enabled = false;
            btnIniciarMigracion.Text = totalFallidas == 0 ? "✅  Migración exitosa" : "⚠️  Migración con errores";
            btnSiguiente.Visible = true;
            btnSiguiente.Text = "Verificar >>";
        }

        /// <summary>
        /// Elimina las tablas que se migraron exitosamente (rollback manual).
        /// </summary>
        private void EjecutarRollback()
        {
            Cursor = Cursors.WaitCursor;
            AgregarLog("", Color.Yellow);
            AgregarLog("=== ROLLBACK: ELIMINANDO TABLAS MIGRADAS ===", Color.Yellow);

            int eliminadas = 0;
            foreach (var nombreTabla in _tablasMigradasExitosamente)
            {
                try
                {
                    string dropSql = $"DROP TABLE IF EXISTS [dbo].[{nombreTabla}]";
                    using (var sqlConn = new System.Data.SqlClient.SqlConnection(_sqlConnectionString))
                    {
                        sqlConn.Open();
                        using (var cmd = new System.Data.SqlClient.SqlCommand(dropSql, sqlConn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    AgregarLog($"   🗑️  Tabla [{nombreTabla}] eliminada", Color.Yellow);
                    eliminadas++;
                }
                catch (Exception exRoll)
                {
                    AgregarLog($"   ❌  Error al eliminar [{nombreTabla}]: {exRoll.Message}", Color.Red);
                }
            }

            _tablasMigradasExitosamente.Clear();
            _resultados.Clear();
            _migracionCompletada = false;

            AgregarLog($"✅ Rollback completado: {eliminadas} tablas eliminadas", Color.LightGreen);
            Cursor = Cursors.Default;

            // Restaurar botón de migración
            btnIniciarMigracion.Enabled = true;
            btnIniciarMigracion.Text = "🚀  Reintentar Migración";
            btnSiguiente.Visible = false;
        }

        /// <summary>
        /// Modo solo scripts: cuando no hay SQL Server, muestra los scripts generados.
        /// </summary>
        private void MostrarSoloScripts()
        {
            txtLog.Clear();
            AgregarLog("=== MODO SOLO SCRIPTS (sin SQL Server) ===", Color.Cyan);
            AgregarLog("No se detectó conexión a SQL Server. Estos son los scripts", Color.White);
            AgregarLog("que se ejecutarán cuando tengas SQL Server disponible.", Color.White);
            AgregarLog("");

            foreach (var t in _tablasMySQL)
            {
                AgregarLog(t.CreateTableScriptTSQL, Color.LightBlue);
                AgregarLog("", Color.White);
            }

            AgregarLog("=== FIN DE SCRIPTS ===", Color.Cyan);
            AgregarLog("", Color.White);

            _migracionCompletada = true;
            btnIniciarMigracion.Enabled = false;
            btnIniciarMigracion.Text = "📜  Scripts generados";
            btnSiguiente.Visible = true;
            btnSiguiente.Text = "Ver Resumen >>";

            // Generar resumen parcial (sin datos migrados)
            GenerarResumenSoloScripts();
        }

        private void ActualizarEstadoFila(int idx, string status, string filas, string duracion)
        {
            var source = dgvMigracion.DataSource as List<MigracionRow>;
            if (source != null && idx < source.Count)
            {
                source[idx].StatusStr = status;
                source[idx].FilasStr = filas;
                source[idx].DuracionStr = duracion;
                dgvMigracion.Refresh();
            }
        }

        private void AgregarLog(string mensaje, Color? color = null)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke((MethodInvoker)(() => AgregarLog(mensaje, color)));
                return;
            }

            if (color.HasValue)
            {
                txtLog.SelectionColor = color.Value;
            }

            txtLog.AppendText(mensaje + "\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            txtLog.Refresh();

            // Escribir al archivo de log
            try
            {
                System.IO.File.AppendAllText(_logFilePath,
                    $"[{DateTime.Now:HH:mm:ss}] {mensaje}\n");
            }
            catch { }
        }

        /// <summary>
        /// Guarda el log de migración a un archivo.
        /// </summary>
        private void GuardarLogAArchivo()
        {
            try
            {
                System.IO.File.WriteAllText(_logFilePath,
                    $"╔════════════════════════════════════════════╗\n" +
                    $"║  MIGRACIÓN MySQL → SQL SERVER             ║\n" +
                    $"║  {DateTime.Now:yyyy-MM-dd HH:mm:ss}                ║\n" +
                    $"╚════════════════════════════════════════════╝\n\n" +
                    txtLog.Text);
            }
            catch { }
        }

        private string TruncarError(string error, int maxLen = 60)
        {
            if (string.IsNullOrEmpty(error) || error.Length <= maxLen)
                return error ?? "Error";
            return error.Substring(0, maxLen) + "...";
        }

        // =============================================
        // PASO 6: VERIFICACIÓN
        // =============================================
        private void CargarVerificacion()
        {
            if (!_migracionCompletada || _resultados.Count == 0)
            {
                lblVerificacionResultado.Text = "No hay datos de migración para verificar.";
                lblVerificacionResultado.ForeColor = Color.FromArgb(108, 117, 125);
                btnSiguiente.Enabled = false;
                return;
            }

            if (_sqlServerSkipped)
            {
                lblVerificacionResultado.Text = "⏭  Modo solo scripts — no hay datos que verificar";
                lblVerificacionResultado.ForeColor = Color.FromArgb(255, 140, 0);
                btnVerificar.Enabled = false;
                btnSiguiente.Enabled = true;
                return;
            }

            btnVerificar.Enabled = true;
            var verifList = new List<VerificacionRow>();
            int ok = 0, fail = 0;

            foreach (var r in _resultados)
            {
                bool coincide = r.Exitoso;
                if (coincide) ok++; else fail++;

                verifList.Add(new VerificacionRow
                {
                    NombreTabla = r.NombreTabla,
                    FilasOrigen = r.FilasOrigen,
                    FilasDestino = r.FilasDestino,
                    StatusVerifStr = coincide ? "✅  OK" : "❌  Falló"
                });
            }

            dgvVerificacion.DataSource = null;
            dgvVerificacion.DataSource = verifList;

            if (fail == 0)
            {
                lblVerificacionResultado.Text = $"✅  Todas las {ok} tablas se migraron correctamente";
                lblVerificacionResultado.ForeColor = Color.FromArgb(40, 167, 69);
            }
            else
            {
                lblVerificacionResultado.Text = $"⚠️  {ok} tablas OK, {fail} tablas con errores";
                lblVerificacionResultado.ForeColor = Color.FromArgb(220, 53, 69);
            }
        }

        private void btnVerificar_Click(object sender, EventArgs e)
        {
            if (_sqlServerSkipped)
            {
                MessageBox.Show("No hay conexión a SQL Server para verificar.\n\n" +
                    "Usá el botón 'Ver Resumen' para ver los scripts generados.",
                    "Modo solo scripts", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Cursor = Cursors.WaitCursor;

            try
            {
                foreach (var r in _resultados)
                {
                    if (r.Exitoso)
                    {
                        long actual = _helper.ObtenerConteoSQLServer(r.NombreTabla);
                        if (actual >= 0)
                            r.FilasDestino = actual;
                    }
                }

                CargarVerificacion();

                int totalOK = _resultados.Count(r => r.FilasOrigen == r.FilasDestino);
                string detalle = string.Join("\n", _resultados
                    .Where(r => r.FilasOrigen != r.FilasDestino)
                    .Select(r => $"  • {r.NombreTabla}: MySQL={r.FilasOrigen} vs SQL Server={r.FilasDestino}"));

                string msg = $"Verificación completada:\n\n{totalOK} de {_resultados.Count} tablas OK.\n" +
                    (string.IsNullOrEmpty(detalle) ? "" : $"\nIncidencias:\n{detalle}");

                MessageBox.Show(msg, "Verificación", MessageBoxButtons.OK,
                    totalOK == _resultados.Count ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en verificación:\n\n{ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error("Error en verificación de migración", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        // =============================================
        // PASO 7: RESUMEN
        // =============================================
        private void GenerarResumenSoloScripts()
        {
            _resultados.Clear();
            // Simular resultados para mostrar en resumen
            int i = 0;
            foreach (var t in _tablasMySQL)
            {
                _resultados.Add(new ResultadoMigracionTabla
                {
                    NombreTabla = t.Nombre,
                    FilasOrigen = t.RowCount,
                    FilasDestino = 0,
                    SchemaCreado = false,
                    DatosMigrados = false,
                    Error = "SQL Server no disponible"
                });
                i++;
            }
            GenerarResumen();
        }

        private void GenerarResumen()
        {
            if (_resultados == null || _resultados.Count == 0)
            {
                txtResumen.Text = "No hay datos de migración para mostrar.";
                return;
            }

            int totalOK = _resultados.Count(r => r.FilasOrigen == r.FilasDestino);
            long totalFilas = _resultados.Sum(r => r.FilasDestino);
            double duracionSeg = _resultados.Where(r => r.Exitoso).Sum(r => r.Duracion.TotalSeconds);
            int duracionMin = (int)(duracionSeg / 60);
            int duracionSegResto = (int)(duracionSeg % 60);

            string resumen = $@"╔═══════════════════════════════════════════════════╗
║        MIGRACIÓN MySQL → SQL SERVER          ║
╚═══════════════════════════════════════════════════╝

📋  RESUMEN EJECUTIVO
───────────────────────────────
• Total tablas procesadas: {_resultados.Count}
• Migradas correctamente: {totalOK}
• Con errores: {_resultados.Count - totalOK}
• Total filas migradas: {totalFilas:N0}
• Duración total: {duracionMin}min {duracionSegResto}s

🔌  ORIGEN (MySQL)
───────────────────────────────
• Servidor: localhost:3307
• Base de datos: ModeloDualDB
• Estado: {(MySQLConnection.TestConnection() ? "✅ Accesible" : "❌ No disponible")}

⚙️  DESTINO (SQL Server)
───────────────────────────────
• Servidor: {txtSQLServer.Text}
• Base de datos: {txtSQLDatabase.Text}
• Estado: {(_sqlServerConectado ? "✅ Conectado" : _sqlServerSkipped ? "⏭ Omitido" : "❌ No conectado")}
{(string.IsNullOrEmpty(_sqlConnectionString) ? "" : $@"
• Cadena de conexión:
  {_sqlConnectionString}")}

{(totalOK > 0 || _sqlServerSkipped ? "" : @"
⚠️  No se migraron datos. Instalá SQL Server y volvé a ejecutar el asistente.
")}
📋  PRÓXIMOS PASOS EN LA APP C#
───────────────────────────────
1. Agregar connection string de SQL Server en App.config:
   <connectionStrings>
     <add name=""SqlServer"" connectionString=""{(_sqlServerConectado ? _sqlConnectionString : "Server=localhost;Database=ModeloDualDB_SQL;Integrated Security=True;")}"" />
   </connectionStrings>

2. Crear Data/SqlServerConnection.cs (helper con Microsoft.Data.SqlClient)

3. Adaptar cada controlador para soportar ambos motores:
   - Crear una interfaz IDatabaseConnection
   - Implementar MySQLConnection y SqlServerConnection
   - Usar Factory Pattern para elegir el motor activo

4. Diferencias SQL a considerar en los controladores:
   • NOW()        → GETDATE()
   • LIMIT n      → SELECT TOP n
   • LIMIT n OFFSET m → OFFSET m ROWS FETCH NEXT n ROWS ONLY
   • IFNULL(x,y)  → ISNULL(x,y)
   • CONCAT(a,b)  → a + b
   • AUTO_INCREMENT → IDENTITY(1,1)
   • `backticks`  → [corchetes] o sin escapado

✦  Generado el {DateTime.Now:yyyy-MM-dd HH:mm}";

            txtResumen.Text = resumen;
        }
    }

    // =============================================
    // CLASES AUXILIARES
    // =============================================
    public class MigracionRow
    {
        public string NombreTabla { get; set; }
        public string StatusStr { get; set; }
        public string FilasStr { get; set; }
        public string DuracionStr { get; set; }
    }

    public class VerificacionRow
    {
        public string NombreTabla { get; set; }
        public long FilasOrigen { get; set; }
        public long FilasDestino { get; set; }
        public string StatusVerifStr { get; set; }
    }
}
