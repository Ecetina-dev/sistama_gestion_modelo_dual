using System;
using System.Drawing;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormMenuPrincipal : Form
    {
        private readonly AuthController _authController;

        public FormMenuPrincipal()
        {
            InitializeComponent();
            _authController = new AuthController();
            ConfigurarInterfaz();
            ConfigurarPermisos();
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
                lblAvatar.Text = username.Substring(0, 1).ToUpper();
            }

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

            try
            {
                string logoPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "logo_modelo_dual.png");
                if (System.IO.File.Exists(logoPath))
                {
                    lblLogoIcono.Image = Image.FromFile(logoPath);
                }
            }
            catch { }
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
            bool puedeTestConnection = esAdmin;

            // Visibilidad de cards
            cardAlumnos.Visible = puedeAlumnos;
            cardEmpresas.Visible = puedeEmpresas;
            cardProyectos.Visible = puedeProyectos;
            cardProfesores.Visible = puedeProfesores;
            cardMaterias.Visible = puedeMaterias;
            cardTemas.Visible = puedeTemas;
            cardGestionUsuarios.Visible = puedeGestionUsuarios;
            cardTestConnection.Visible = puedeTestConnection;

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
                MessageBox.Show("No tienes permiso para acceder a esta seccion.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormAlumnos>();
        }

        private void btnEmpresas_Click(object sender, EventArgs e)
        {
            if (!SesionActiva.Instance.EsAdmin && !SesionActiva.Instance.EsEmpresa)
            {
                MessageBox.Show("No tienes permiso para acceder a esta seccion.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormEmpresas>();
        }

        private void btnProyectos_Click(object sender, EventArgs e)
        {
            AbrirFormulario<FormProyectos>();
        }

        private void btnProfesores_Click(object sender, EventArgs e)
        {
            if (!SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta seccion.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormProfesores>();
        }

        private void btnMaterias_Click(object sender, EventArgs e)
        {
            if (!SesionActiva.Instance.TienePrivilegio("profesor.crud_materia") &&
                !SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta seccion.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormMaterias>();
        }

        private void btnTemas_Click(object sender, EventArgs e)
        {
            if (!SesionActiva.Instance.TienePrivilegio("profesor.crud_tema") &&
                !SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("No tienes permiso para acceder a esta seccion.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormTemas>();
        }

        private void btnGestionUsuarios_Click(object sender, EventArgs e)
        {
            if (!SesionActiva.Instance.EsAdmin)
            {
                MessageBox.Show("Solo el administrador puede acceder a esta seccion.",
                    "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AbrirFormulario<FormGestionUsuarios>();
        }

        private void AbrirFormulario<T>() where T : Form, new()
        {
            try
            {
                T form = new T();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al abrir formulario {typeof(T).Name}", ex);
                MessageBox.Show(
                    $"No se pudo abrir la ventana: {ex.Message}\n\nVerifica que tengas permisos suficientes.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (MySQLConnection.TestConnection())
            {
                MessageBox.Show(
                    "Conexion a MySQL exitosa!\n\nVersion: " + MySQLConnection.GetMySQLVersion(),
                    "Conexion OK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "No se pudo conectar a MySQL.\n\nVerifica que:\n1. MySQL este ejecutandose\n2. El password en App.config sea correcto",
                    "Error de Conexion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            using (var form = new FormCambiarPassword())
            {
                form.ShowDialog();
            }
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Deseas cerrar sesion?",
                "Cerrar Sesion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _authController.CerrarSesion();
                this.Close();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Deseas salir del sistema?",
                "Confirmar salida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (SesionActiva.Instance != null && SesionActiva.Instance.Id_Usuario > 0)
                {
                    _authController.CerrarSesion();
                }
                Application.Exit();
            }
        }
    }
}
