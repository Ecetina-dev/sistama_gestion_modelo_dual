using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Views
{
    /// <summary>
    /// Formulario de gestion de usuarios (solo admin).
    /// Permite cargar usuarios al sistema con password temporal.
    /// </summary>
    public partial class FormGestionUsuarios : Form
    {
        private readonly AuthController _authController;
        private List<Alumno> _alumnos = new List<Alumno>();
        private List<Profesor> _profesores = new List<Profesor>();
        private List<Empresa> _empresas = new List<Empresa>();
        private List<Usuario> _usuarios;

        // Constantes
        private const string TIPO_ALUMNO = "alumno";
        private const string TIPO_PROFESOR = "profesor";
        private const string TIPO_EMPRESA = "empresa";
        private const string TIPO_ADMIN = "none";

        public FormGestionUsuarios()
        {
            InitializeComponent();
            _authController = new AuthController();
            Inicializar();
        }

        private void Inicializar()
        {
            CargarDatos();
            CargarUsuarios();
            ConfigurarEventos();
        }

        private void CargarDatos()
        {
            try
            {
                var controllerAlumno = new AlumnoController();
                var controllerProfesor = new ProfesorController();
                var controllerEmpresa = new EmpresaController();

                _alumnos = controllerAlumno.Read() ?? new List<Alumno>();
                _profesores = controllerProfesor.Read() ?? new List<Profesor>();
                _empresas = controllerEmpresa.Read() ?? new List<Empresa>();
            }
            catch (Exception ex)
            {
                Logger.Error("Error al cargar entidades", ex);
            }
        }

        private void ConfigurarEventos()
        {
            cmbTipoUsuario.SelectedIndexChanged += (s, e) => ActualizarCombos();
        }

        private void CargarUsuarios()
        {
            try
            {
                _usuarios = _authController.ListarUsuarios();
                dgvUsuarios.DataSource = null;
                dgvUsuarios.DataSource = _usuarios;

                if (dgvUsuarios.Columns.Count > 0)
                {
                    if (dgvUsuarios.Columns.Contains("PasswordHash")) dgvUsuarios.Columns["PasswordHash"].Visible = false;
                    if (dgvUsuarios.Columns.Contains("Password")) dgvUsuarios.Columns["Password"].Visible = false;
                    if (dgvUsuarios.Columns.Contains("Password_Temporal_Hash")) dgvUsuarios.Columns["Password_Temporal_Hash"].Visible = false;
                    if (dgvUsuarios.Columns.Contains("Privilegios")) dgvUsuarios.Columns["Privilegios"].Visible = false;
                    if (dgvUsuarios.Columns.Contains("Created_At")) dgvUsuarios.Columns["Created_At"].HeaderText = "Creado";
                    if (dgvUsuarios.Columns.Contains("Debe_Cambiar_Password")) dgvUsuarios.Columns["Debe_Cambiar_Password"].HeaderText = "Pendiente Activar";
                    if (dgvUsuarios.Columns.Contains("Fecha_Activacion")) dgvUsuarios.Columns["Fecha_Activacion"].HeaderText = "Activado";
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al cargar usuarios", ex);
                MessageBox.Show("Error al cargar lista de usuarios: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarCombosCreacion()
        {
            cmbTipoUsuario.Items.Clear();
            cmbTipoUsuario.Items.Add(new ComboBoxItem { Text = "Alumno", Value = TIPO_ALUMNO });
            cmbTipoUsuario.Items.Add(new ComboBoxItem { Text = "Profesor", Value = TIPO_PROFESOR });
            cmbTipoUsuario.Items.Add(new ComboBoxItem { Text = "Empresa", Value = TIPO_EMPRESA });
            cmbTipoUsuario.Items.Add(new ComboBoxItem { Text = "Administrador (sin entidad)", Value = TIPO_ADMIN });
            cmbTipoUsuario.SelectedIndex = 0;
        }

        private void ActualizarCombos()
        {
            cmbEntidad.Items.Clear();
            cmbRol.Items.Clear();
            cmbEntidad.Enabled = false;
            txtUsername.Clear();
            txtEmail.Clear();

            if (cmbTipoUsuario.SelectedItem == null) return;

            var tipoItem = cmbTipoUsuario.SelectedItem as ComboBoxItem;
            string tipo = tipoItem.Value.ToString();
            string rolNombreEsperado = tipo;

            // Llenar combo de entidades
            if (tipo == TIPO_ALUMNO)
            {
                cmbEntidad.Enabled = true;
                foreach (var a in _alumnos)
                    cmbEntidad.Items.Add(new ComboBoxItem
                    {
                        Text = $"{a.No_Control} - {a.Nombre} {a.Apellido_Paterno}",
                        Value = a.Id_Alumno
                    });
            }
            else if (tipo == TIPO_PROFESOR)
            {
                cmbEntidad.Enabled = true;
                foreach (var p in _profesores)
                    cmbEntidad.Items.Add(new ComboBoxItem
                    {
                        Text = $"{p.No_Empleado} - {p.Nombre} {p.Apellido_Paterno}",
                        Value = p.Id_Profesor
                    });
            }
            else if (tipo == TIPO_EMPRESA)
            {
                cmbEntidad.Enabled = true;
                foreach (var e in _empresas)
                    cmbEntidad.Items.Add(new ComboBoxItem
                    {
                        Text = e.Nombre_Comercial,
                        Value = e.Id_Empresa
                    });
            }
            else if (tipo == TIPO_ADMIN)
            {
                cmbEntidad.Enabled = false;
            }

            // Llenar combo de roles
            var roles = _authController.ObtenerRoles();
            var rolesFiltrados = tipo == TIPO_ADMIN
                ? roles.Where(r => r.Nombre == "admin").ToList()
                : roles.Where(r => r.Nombre == rolNombreEsperado).ToList();

            foreach (var rol in rolesFiltrados)
                cmbRol.Items.Add(new ComboBoxItem
                {
                    Text = CapitalizeFirst(rol.Nombre),
                    Value = rol.Id_Rol
                });

            if (cmbRol.Items.Count > 0) cmbRol.SelectedIndex = 0;

            // Auto-completar username/email si hay entidad seleccionada
            if (cmbEntidad.SelectedItem is ComboBoxItem entidad)
            {
                txtUsername.Text = GenerarUsernameSugerido(tipo, entidad);
                txtEmail.Text = GenerarEmailSugerido(tipo, entidad);
            }
        }

        private string GenerarUsernameSugerido(string tipo, ComboBoxItem entidad)
        {
            // Username se genera a partir del numero/identificador
            return $"{tipo}_{entidad.Text.Substring(0, Math.Min(10, entidad.Text.Length))}"
                .Replace(" ", "_").Replace("-", "").ToLower();
        }

        private string GenerarEmailSugerido(string tipo, ComboBoxItem entidad)
        {
            return $"{tipo}_{DateTime.Now.Ticks}@modelodual.edu";
        }

        private string CapitalizeFirst(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }

        private void btnGenerarPassword_Click(object sender, EventArgs e)
        {
            string pwd = _authController.GenerarPasswordTemporal();
            txtPasswordTemporal.Text = pwd;
            lblPasswordGenerado.Text = $"Password generado: {pwd}";
            lblPasswordGenerado.Visible = true;
            btnCopiarPassword.Visible = true;
        }

        private void btnCopiarPassword_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPasswordTemporal.Text))
            {
                Clipboard.SetText(txtPasswordTemporal.Text);
                MessageBox.Show("Password copiado al portapapeles", "Copiado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void btnCargarUsuario_Click(object sender, EventArgs e)
        {
            if (!ValidarDatosCarga()) return;

            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim().ToLower();
            string passwordTemporal = string.IsNullOrEmpty(txtPasswordTemporal.Text)
                ? _authController.GenerarPasswordTemporal()
                : txtPasswordTemporal.Text;

            var tipoItem = cmbTipoUsuario.SelectedItem as ComboBoxItem;
            var rolItem = cmbRol.SelectedItem as ComboBoxItem;
            string tipo = tipoItem.Value.ToString();
            int idRol = Convert.ToInt32(rolItem.Value);

            string tipoEntidad = null;
            int? idEntidad = null;

            if (tipo != TIPO_ADMIN && cmbEntidad.SelectedItem != null)
            {
                var entidadItem = cmbEntidad.SelectedItem as ComboBoxItem;
                tipoEntidad = tipo;
                idEntidad = Convert.ToInt32(entidadItem.Value);
            }

            btnCargarUsuario.Enabled = false;
            btnCargarUsuario.Text = "Cargando...";

            try
            {
                var resultado = await Task.Run(() =>
                    _authController.CargarUsuario(username, email, idRol, tipoEntidad, idEntidad,
                        SesionActiva.Instance.Id_Usuario));

                if (resultado.Success)
                {
                    string msg = $"Usuario cargado Éxitosamente!\n\n" +
                                 $"Username: {resultado.Username}\n" +
                                 $"Password temporal: {resultado.PasswordTemporal}\n\n" +
                                 $"COMPARTIR ESTE PASSWORD CON EL USUARIO\n" +
                                 $"Para que active su cuenta.";

                    MessageBox.Show(msg, "Usuario Cargado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Clipboard.SetText(resultado.PasswordTemporal);

                    LimpiarFormulario();
                    CargarUsuarios();
                }
                else
                {
                    MessageBox.Show(resultado.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al cargar usuario", ex);
                MessageBox.Show("Error al cargar el usuario", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCargarUsuario.Enabled = true;
                btnCargarUsuario.Text = "Cargar Usuario al Sistema";
            }
        }

        private bool ValidarDatosCarga()
        {
            if (cmbTipoUsuario.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un tipo de usuario", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var tipoItem = cmbTipoUsuario.SelectedItem as ComboBoxItem;
            string tipo = tipoItem.Value.ToString();

            if (tipo != TIPO_ADMIN && cmbEntidad.SelectedItem == null)
            {
                MessageBox.Show("Selecciona la entidad a vincular", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbRol.SelectedItem == null)
            {
                MessageBox.Show("Selecciona un rol", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Ingresa un nombre de usuario", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Ingresa un email", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void LimpiarFormulario()
        {
            txtUsername.Clear();
            txtEmail.Clear();
            txtPasswordTemporal.Clear();
            lblPasswordGenerado.Visible = false;
            btnCopiarPassword.Visible = false;
            cmbEntidad.Items.Clear();
            cmbRol.Items.Clear();
        }

        private void btnResetearPassword_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un usuario de la lista", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuario = dgvUsuarios.SelectedRows[0].DataBoundItem as Usuario;
            if (usuario == null) return;

            var result = MessageBox.Show(
                $"Resetear el password de '{usuario.Username}'?\n\n" +
                "Se generara un nuevo password temporal que el usuario debera usar para reactivar su cuenta.",
                "Confirmar reset",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                var res = _authController.ResetearPassword(usuario.Id_Usuario, SesionActiva.Instance.Id_Usuario);
                if (res.Success)
                {
                    MessageBox.Show(
                        $"Password reseteado!\n\nUsername: {usuario.Username}\nNuevo password temporal: {res.PasswordTemporal}\n\n" +
                        "Comparte este password con el usuario para que reactive su cuenta.",
                        "Password Reseteado",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clipboard.SetText(res.PasswordTemporal);
                    CargarUsuarios();
                }
                else
                {
                    MessageBox.Show(res.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al resetear password", ex);
                MessageBox.Show("Error al resetear password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCambiarStatus_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un usuario de la lista", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var usuario = dgvUsuarios.SelectedRows[0].DataBoundItem as Usuario;
            if (usuario == null) return;

            // No permitir desactivar al admin principal
            if (usuario.Username == "admin" && usuario.Status == "activo")
            {
                MessageBox.Show("No puedes desactivar al usuario admin principal",
                    "Accion no permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nuevoStatus = usuario.Status == "activo" ? "inactivo" : "activo";
            var result = MessageBox.Show(
                $"Cambiar status de '{usuario.Username}' a '{nuevoStatus}'?",
                "Confirmar cambio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            if (_authController.CambiarStatusUsuario(usuario.Id_Usuario, nuevoStatus, SesionActiva.Instance.Id_Usuario))
            {
                MessageBox.Show("Status actualizado", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarUsuarios();
            }
            else
            {
                MessageBox.Show("Error al cambiar status", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarDatos();
            CargarUsuarios();
        }

        private void FormGestionUsuarios_Load(object sender, EventArgs e)
        {
            CargarCombosCreacion();
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public override string ToString() => Text;
        }
    }
}

