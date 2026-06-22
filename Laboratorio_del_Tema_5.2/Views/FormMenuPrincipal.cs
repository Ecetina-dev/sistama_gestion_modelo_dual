using System;
using System.Drawing;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;
using System.Data.SqlClient;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormMenuPrincipal : Form
    {
        private readonly AuthController _authController;
        private readonly System.Windows.Forms.Timer _idleTimer;
        private readonly System.Windows.Forms.Timer _pollTimer;
        private const int INTERVALO_POLL_STATS_MINUTOS = 5;
        private DateTime _ultimaActividad;
        private DateTime _ultimaCargaStats = DateTime.MinValue;
        private bool _primeraActivacion = true;
        private bool _volviendoDeModuloHijo = false;
        private const int MINUTOS_INACTIVIDAD_MAX = 30;
        private const int SEGUNDOS_CACHE_STATS = 30;

        public FormMenuPrincipal()
        {
            InitializeComponent();
            _authController = new AuthController();
            _ultimaActividad = DateTime.Now;
            _idleTimer = new System.Windows.Forms.Timer
            {
                Interval = 60_000 // 1 minuto
            };
            _idleTimer.Tick += IdleTimer_Tick;
            _idleTimer.Start();

            // Issue #9: polling automático cada N minutos para mantener stats frescos
            // aunque el usuario no interactúe con ningún módulo
            _pollTimer = new System.Windows.Forms.Timer
            {
                Interval = INTERVALO_POLL_STATS_MINUTOS * 60_000
            };
            _pollTimer.Tick += PollTimer_Tick;
            _pollTimer.Start();

            // Issue #10: throttle del MouseMove para no ejecutar delegate 1000+ veces por seg
            this.MouseMove += FormMenuPrincipal_MouseMove;
            this.KeyDown += FormMenuPrincipal_KeyDown;
            this.MouseDown += FormMenuPrincipal_MouseDown;

            ConfigurarInterfaz();
            ConfigurarPermisos();
            InicializarCards();

            // Refrescar stats al volver de un módulo hijo (Bug #3 + Issue #6)
            this.Activated += FormMenuPrincipal_Activated;

            this.FormClosing += FormMenuPrincipal_FormClosing;
        }

        private void FormMenuPrincipal_Activated(object sender, EventArgs e)
        {
            _ultimaActividad = DateTime.Now;
            if (_primeraActivacion)
            {
                _primeraActivacion = false;
                return;
            }
            // Issue #6: solo forzar recarga si realmente volvimos de un módulo hijo
            // (no cuando el form se restaura desde minimize o recupera foco)
            if (_volviendoDeModuloHijo)
            {
                _volviendoDeModuloHijo = false;
                _ultimaCargaStats = DateTime.MinValue; // forzar recarga
                CargarEstadisticas();
            }
            // Si no, dejar que el cache de 30s funcione normalmente
        }

        private void FormMenuPrincipal_MouseMove(object sender, MouseEventArgs e) => ThrottleActividad();
        private void FormMenuPrincipal_KeyDown(object sender, KeyEventArgs e) => ThrottleActividad();
        private void FormMenuPrincipal_MouseDown(object sender, MouseEventArgs e) => ThrottleActividad();

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            // Solo refrescar si el usuario está activo (no idle)
            if ((DateTime.Now - _ultimaActividad).TotalMinutes < MINUTOS_INACTIVIDAD_MAX)
            {
                _ultimaCargaStats = DateTime.MinValue;
                CargarEstadisticas();
            }
        }

        private void InicializarCards()
        {
            // Propagar clicks a labels internos + accesibilidad con teclado
            HacerCardAccesible(cardAlumnos, btnAlumnos_Click);
            HacerCardAccesible(cardEmpresas, btnEmpresas_Click);
            HacerCardAccesible(cardProyectos, btnProyectos_Click);
            HacerCardAccesible(cardProfesores, btnProfesores_Click);
            HacerCardAccesible(cardMaterias, btnMaterias_Click);
            HacerCardAccesible(cardTemas, btnTemas_Click);
            HacerCardAccesible(cardGestionUsuarios, btnGestionUsuarios_Click);
            HacerCardAccesible(cardMigracionBD, btnMigracionBD_Click);
        }

        private void HacerCardAccesible(Panel card, EventHandler clickHandler)
        {
            card.TabStop = true;
            card.Cursor = Cursors.Hand;
            // Issue #11: guardar handler en Tag para que ProcessCmdKey lo invoque
            card.Tag = clickHandler;
            // Propagar click a todos los hijos (icono, título, descripción, permiso)
            foreach (Control child in card.Controls)
            {
                child.Cursor = Cursors.Hand;
                child.Click += clickHandler;
            }
            // Accesibilidad: Enter y Space disparan el click
            card.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
                {
                    e.SuppressKeyPress = true;
                    clickHandler?.Invoke(s, e);
                }
            };
        }

        private DateTime _ultimaActividadCheck = DateTime.MinValue;
        private const int THROTTLE_ACTIVIDAD_MS = 1000; // máx 1 update/seg

        private void ThrottleActividad()
        {
            // Solo actualizar _ultimaActividad una vez por segundo para no saturar
            if ((DateTime.Now - _ultimaActividadCheck).TotalMilliseconds < THROTTLE_ACTIVIDAD_MS)
                return;
            _ultimaActividadCheck = DateTime.Now;
            _ultimaActividad = DateTime.Now;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Issue #11: teclas globales incluso cuando el foco está en un child control
            if (keyData == Keys.Enter || keyData == Keys.Space)
            {
                var focused = this.ActiveControl as Panel;
                if (focused != null && focused.Tag is EventHandler handler)
                {
                    handler(focused, EventArgs.Empty);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void IdleTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan inactivo = DateTime.Now - _ultimaActividad;
            if (inactivo.TotalMinutes >= MINUTOS_INACTIVIDAD_MAX)
            {
                _idleTimer.Stop();
                MessageBox.Show(
                    $"Tu sesión se ha cerrado por inactividad ({MINUTOS_INACTIVIDAD_MAX} minutos sin actividad).",
                    "Sesión Cerrada por Inactividad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                _authController.CerrarSesion();
                this.Close();
                return;
            }
            if (SesionActiva.Instance != null &&
                SesionActiva.Instance.IsSesionExpirada(Seguridad.DuracionSesionHoras))
            {
                _idleTimer.Stop();
                _authController.CerrarSesion();
                this.Close();
            }
        }

        private void ConfigurarInterfaz()
        {
            if (SesionActiva.Instance == null || SesionActiva.Instance.Id_Usuario == 0)
                return;

            string username = SesionActiva.Instance.Username;
            string rol = CapitalizarPrimeraLetra(SesionActiva.Instance.Nombre_Rol);

            lblUsuarioNombre.Text = username;
            lblUsuarioRol.Text = rol;

            if (!string.IsNullOrEmpty(username))
            {
                // Para emails, tomar la parte antes del @ y la inicial del nombre local
                string display = username;
                int atIdx = username.IndexOf('@');
                if (atIdx > 0 && username.Length > 0) display = username.Substring(0, atIdx);
                lblAvatar.Text = display.Length > 0
                    ? display.Substring(0, 1).ToUpper()
                    : "?";
            }

            // Saludo personalizado
            lblPageTitulo.Text = $"¡Bienvenido, {CapitalizarPrimeraLetra(username)}!";

            if (SesionActiva.Instance.EsAdmin)
            {
                lblPageSubtitulo.Text = "Tienes acceso completo al sistema como administrador.";
            }
            else if (SesionActiva.Instance.EsProfesor)
            {
                lblPageSubtitulo.Text = "Gestiona tus alumnos, materias y proyectos asignados.";
            }
            else if (SesionActiva.Instance.EsEmpresa)
            {
                lblPageSubtitulo.Text = "Visualiza tus proyectos y alumnos asignados.";
            }
            else
            {
                lblPageSubtitulo.Text = "Bienvenido al sistema. Selecciona un modulo para continuar.";
            }

            // Último acceso
            if (SesionActiva.Instance.Fecha_Login != default)
            {
                lblUltimoAcceso.Text = $"Último acceso: {SesionActiva.Instance.Fecha_Login:dd MMM yyyy — HH:mm}";
            }

            // Cargar estadísticas del dashboard
            CargarEstadisticas();

            string logoPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "logo_modelo_dual.png");
            try
            {
                if (System.IO.File.Exists(logoPath))
                {
                    lblLogoIcono.Image = Image.FromFile(logoPath);
                }
                else
                {
                    Logger.Warning("Logo no encontrado: " + logoPath);
                }
            }
            catch (Exception logoEx)
            {
                Logger.Warning("Error al cargar logo: " + logoEx.Message);
            }
        }

        private string CapitalizarPrimeraLetra(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;
            return char.ToUpper(texto[0]) + texto.Substring(1).ToLower();
        }

        private void ConfigurarPermisos()
        {
            if (SesionActiva.Instance == null || SesionActiva.Instance.Id_Usuario == 0)
                return;

            bool esAdmin = SesionActiva.Instance.EsAdmin;
            bool esProfesor = SesionActiva.Instance.EsProfesor;
            bool esAlumno = SesionActiva.Instance.EsAlumno;
            bool esEmpresa = SesionActiva.Instance.EsEmpresa;

            bool puedeAlumnos = esAdmin || esProfesor;
            bool puedeEmpresas = esAdmin || esEmpresa;
            bool puedeProyectos = esAdmin || esProfesor || esAlumno || esEmpresa;
            bool puedeProfesores = esAdmin;
            bool puedeMaterias = esAdmin || esProfesor;
            bool puedeTemas = esAdmin || esProfesor;
            bool puedeGestionUsuarios = esAdmin;
            bool puedeMigracionBD = esAdmin;

            // Visibilidad de cards
            cardAlumnos.Visible = puedeAlumnos;
            cardEmpresas.Visible = puedeEmpresas;
            cardProyectos.Visible = puedeProyectos;
            cardProfesores.Visible = puedeProfesores;
            cardMaterias.Visible = puedeMaterias;
            cardTemas.Visible = puedeTemas;
            cardGestionUsuarios.Visible = puedeGestionUsuarios;
            cardMigracionBD.Visible = puedeMigracionBD;

            // Visibilidad de nav buttons
            btnNavAlumnos.Visible = puedeAlumnos;
            btnNavEmpresas.Visible = puedeEmpresas;
            btnNavProyectos.Visible = puedeProyectos;
            btnNavProfesores.Visible = puedeProfesores;
            btnNavMaterias.Visible = puedeMaterias;
            btnNavTemas.Visible = puedeTemas;
            btnNavGestionUsuarios.Visible = puedeGestionUsuarios;
        }

        // Handlers de clicks - uno por cada modulo
        private void btnAlumnos_Click(object sender, EventArgs e)
        {
            if (!SesionActiva.Instance.TienePrivilegio("profesor.crud_alumno") &&
                !SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (SesionActiva.Instance == null) return;
            AbrirFormulario<FormAlumnos>();
        }

        private void btnEmpresas_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.EsAdmin && !SesionActiva.Instance.EsEmpresa)
            {
                MessageBox.Show("No tienes permiso para acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormEmpresas>();
        }

        private void btnProyectos_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.EsAdmin &&
                !SesionActiva.Instance.EsProfesor &&
                !SesionActiva.Instance.EsAlumno &&
                !SesionActiva.Instance.EsEmpresa)
            {
                Logger.Warning($"Intento de acceso no autorizado a Proyectos: usuario {SesionActiva.Instance.Id_Usuario}");
                MessageBox.Show("No tienes permiso para acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormProyectos>();
        }

        private void btnProfesores_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormProfesores>();
        }

        private void btnMaterias_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.TienePrivilegio("profesor.crud_materia") &&
                !SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormMaterias>();
        }

        private void btnTemas_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.TienePrivilegio("profesor.crud_tema") &&
                !SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormTemas>();
        }

        private void btnGestionUsuarios_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("Solo el administrador puede acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormGestionUsuarios>();
        }

        private void AbrirFormulario<T>() where T : Form, new()
        {
            try
            {
                // Highlight active nav button
                string modulo = typeof(T).Name switch
                {
                    "FormAlumnos" => "Alumnos",
                    "FormEmpresas" => "Empresas",
                    "FormProyectos" => "Proyectos",
                    "FormProfesores" => "Profesores",
                    "FormMaterias" => "Materias",
                    "FormTemas" => "Temas",
                    "FormGestionUsuarios" => "Usuarios",
                    _ => null
                };
                if (modulo != null)
                    ResaltarNav(modulo);

                // Issue #6: marcar que vamos a abrir un módulo hijo
                _volviendoDeModuloHijo = true;

                T form = new T();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al abrir formulario {typeof(T).Name}", ex);
                _volviendoDeModuloHijo = false;
                MessageBox.Show(
                    $"No se pudo abrir la ventana: {ex.Message}\n\nVerifica que tengas permisos suficientes.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarEstadisticas()
        {
            try
            {
                // Caché 30s: si cargamos hace poco, no volver a consultar
                if ((DateTime.Now - _ultimaCargaStats).TotalSeconds < SEGUNDOS_CACHE_STATS)
                    return;

                using (var conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string sql = @"
                        SELECT
                            (SELECT COUNT(*) FROM v_alumnos_activos) AS total_alumnos,
                            (SELECT COUNT(*) FROM v_empresas_activas) AS total_empresas,
                            (SELECT COUNT(*) FROM v_profesores_activos) AS total_profesores,
                            (SELECT COUNT(*) FROM Usuario WHERE debe_cambiar_password = 1 AND is_deleted = 0) AS pendientes_activacion,
                            (SELECT COUNT(*) FROM v_proyectos_activos) AS total_proyectos,
                            (SELECT COUNT(*) FROM v_materias_activas) AS total_materias,
                            (SELECT COUNT(*) FROM tema WHERE is_deleted = 0) AS total_temas";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Issue #7: actualizar cache siempre, incluso si no hay filas
                        _ultimaCargaStats = DateTime.Now;
                        // Issue #5: marcar que la BD responde
                        MarcarEstadoConexion(true);

                        if (reader.Read())
                        {
                            int alumnos = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                            int empresas = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                            int profesores = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                            int pendientes = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                            int proyectos = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                            int materias = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                            int temas = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);

                            lblStatAlumnosNum.Text = alumnos.ToString();
                            lblStatEmpresasNum.Text = empresas.ToString();
                            lblStatProfesoresNum.Text = profesores.ToString();
                            lblStatPendientesNum.Text = pendientes.ToString();

                            // Issue #12: dashboard extendido con más stats
                            descAlumnos.Text = $"{alumnos} activos · {proyectos} proyectos";
                            descEmpresas.Text = $"{empresas} activas · {materias} materias";
                            descProfesores.Text = $"{profesores} activos · {temas} temas";

                            // Pendientes solo para admin
                            statPendientes.Visible = SesionActiva.Instance.EsAdmin;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al cargar estadísticas", ex);
                // Issue #5: mostrar al usuario que la BD no responde
                MarcarEstadoConexion(false);
            }
        }

        private void MarcarEstadoConexion(bool conectado)
        {
            try
            {
                lblEstadoConexion.Text = conectado ? "🟢 Conectado" : "🔴 Sin conexión";
                lblEstadoConexion.ForeColor = conectado
                    ? Color.FromArgb(40, 167, 69)
                    : Color.FromArgb(220, 53, 69);
            }
            catch { /* silencioso si el label no existe */ }
        }

        private void ResaltarNav(string moduloActivo)
        {
            Color normal = Color.FromArgb(28, 35, 51);
            Color activo = Color.FromArgb(0, 71, 160);
            Color hoverColor = Color.FromArgb(35, 45, 65);

            foreach (var btn in new[] { btnNavAlumnos, btnNavEmpresas, btnNavProyectos,
                btnNavProfesores, btnNavMaterias, btnNavTemas, btnNavGestionUsuarios })
            {
                btn.BackColor = normal;
                btn.FlatAppearance.MouseOverBackColor = hoverColor;
            }

            Button activoBtn = moduloActivo switch
            {
                "Alumnos" => btnNavAlumnos,
                "Empresas" => btnNavEmpresas,
                "Proyectos" => btnNavProyectos,
                "Profesores" => btnNavProfesores,
                "Materias" => btnNavMaterias,
                "Temas" => btnNavTemas,
                "Usuarios" => btnNavGestionUsuarios,
                _ => null
            };

            if (activoBtn != null)
                activoBtn.BackColor = activo;
        }

        private void btnMigracionBD_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null) return;
            if (!SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("Solo el administrador puede acceder a esta sección.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Issue #6: marcar que vamos a abrir un módulo hijo
            _volviendoDeModuloHijo = true;
            using (var form = new FormMigracionBD())
            {
                form.ShowDialog();
            }
        }

        private void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            if (SesionActiva.Instance == null || SesionActiva.Instance.Id_Usuario == 0)
            {
                MessageBox.Show("Debes iniciar sesión para cambiar tu contraseña.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var form = new FormCambiarPassword())
            {
                form.ShowDialog();
            }
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Deseas cerrar sesión?",
                "Cerrar Sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Deseas salir del sistema?",
                "Confirmar salida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (SesionActiva.Instance != null && SesionActiva.Instance.Id_Usuario > 0)
                {
                    _authController.CerrarSesion();
                }
                Program.SalirDelSistema = true;
                this.Close();
            }
        }

        private void FormMenuPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Issue #13, #14, #15: desuscribir event handlers para evitar fugas
            this.MouseMove -= FormMenuPrincipal_MouseMove;
            this.KeyDown -= FormMenuPrincipal_KeyDown;
            this.MouseDown -= FormMenuPrincipal_MouseDown;
            this.Activated -= FormMenuPrincipal_Activated;
            this.FormClosing -= FormMenuPrincipal_FormClosing;
            _idleTimer.Tick -= IdleTimer_Tick;
            _pollTimer.Tick -= PollTimer_Tick;
            _idleTimer?.Stop();
            _idleTimer?.Dispose();
            _pollTimer?.Stop();
            _pollTimer?.Dispose();
            // Si el usuario cierra con la X y aún hay sesión activa, cerrarla
            if (SesionActiva.Instance != null && SesionActiva.Instance.Id_Usuario > 0)
            {
                _authController.CerrarSesion();
            }
            LimpiarEstadoUI();
        }

        private void LimpiarEstadoUI()
        {
            // Resetear labels para que el próximo usuario no vea datos del anterior
            lblUsuarioNombre.Text = string.Empty;
            lblUsuarioRol.Text = string.Empty;
            lblPageTitulo.Text = string.Empty;
            lblPageSubtitulo.Text = string.Empty;
            lblUltimoAcceso.Text = string.Empty;
            lblAvatar.Text = string.Empty;
            lblStatAlumnosNum.Text = "0";
            lblStatEmpresasNum.Text = "0";
            lblStatProfesoresNum.Text = "0";
            lblStatPendientesNum.Text = "0";
            descAlumnos.Text = string.Empty;
            descEmpresas.Text = string.Empty;
            descProfesores.Text = string.Empty;
            // Resetear visibilidad de stats y cards de admin (Bug #2: ventana insegura post-logout)
            statPendientes.Visible = false;
            cardMigracionBD.Visible = false;
            cardGestionUsuarios.Visible = false;
            cardProfesores.Visible = false;
            cardMaterias.Visible = false;
            cardTemas.Visible = false;
            cardAlumnos.Visible = false;
            cardEmpresas.Visible = false;
            cardProyectos.Visible = false;
            // Resetear resaltado de nav
            Color normal = Color.FromArgb(28, 35, 51);
            foreach (var btn in new[] { btnNavAlumnos, btnNavEmpresas, btnNavProyectos,
                btnNavProfesores, btnNavMaterias, btnNavTemas, btnNavGestionUsuarios })
            {
                btn.BackColor = normal;
                btn.Visible = false;
            }
        }
    }
}

