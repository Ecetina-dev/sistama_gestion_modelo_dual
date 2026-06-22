using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;
using Laboratorio_del_Tema_5_2.Data;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormLogin : Form
    {
        private readonly AuthController _authController;
        private bool _isLoading = false;
        private bool _capsLockOn = false;
        private bool _skipClearError = false;  // evita que TextChanged borre el error durante Clear()

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        private static readonly string _recordarPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "ModeloDual", "login_prefs.txt");

        public FormLogin()
        {
            InitializeComponent();
            _authController = new AuthController();
            ConfigurarControles();
            CargarUsuarioRecordado();
        }

        private void ConfigurarControles()
        {
            txtLogin.MaxLength = Seguridad.LoginMaxLength;     // Usuario/email (120)
            txtPassword.MaxLength = Seguridad.PasswordMaxLength;  // Contraseña (100)

            this.Shown += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtLogin.Text))
                    txtLogin.Focus();
                else
                    txtPassword.Focus();
                VerificarCapsLock();
                _ = VerificarEstadoConexion();
            };

            // Restaurar color al escribir
            txtLogin.TextChanged += (s, e) =>
            {
                LimpiarError();
                VerificarCapsLock();
                if (txtLogin.BackColor == Color.FromArgb(255, 235, 235))
                    txtLogin.BackColor = Color.FromArgb(245, 245, 245);
            };
            txtPassword.TextChanged += (s, e) =>
            {
                if (_skipClearError) return;
                LimpiarError();
                VerificarCapsLock();
                if (txtPassword.BackColor == Color.FromArgb(255, 235, 235))
                    txtPassword.BackColor = Color.FromArgb(245, 245, 245);
            };

            // Enter navega/Submit, Escape cierra
            txtLogin.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; txtPassword.Focus(); }
            };
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !_isLoading) { e.SuppressKeyPress = true; btnIniciarSesion_Click(s, e); }
            };
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && !_isLoading) { e.SuppressKeyPress = true; this.Close(); }
            };
        }

        // ==================== RECORDAR USUARIO ====================

        private void CargarUsuarioRecordado()
        {
            try
            {
                string dir = Path.GetDirectoryName(_recordarPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (File.Exists(_recordarPath))
                {
                    byte[] encryptedData = File.ReadAllBytes(_recordarPath);
                    byte[] decryptedBytes = ProtectedData.Unprotect(encryptedData, null,
                        DataProtectionScope.CurrentUser);
                    string usuario = Encoding.UTF8.GetString(decryptedBytes).Trim();
                    if (!string.IsNullOrEmpty(usuario))
                    {
                        txtLogin.Text = usuario;
                        chkRecordar.Checked = true;
                    }
                }
            }
            catch { }
        }

        private void GuardarUsuarioRecordado()
        {
            try
            {
                string dir = Path.GetDirectoryName(_recordarPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (chkRecordar.Checked)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(txtLogin.Text.Trim());
                    byte[] encrypted = ProtectedData.Protect(bytes, null,
                        DataProtectionScope.CurrentUser);
                    File.WriteAllBytes(_recordarPath, encrypted);
                }
                else if (File.Exists(_recordarPath))
                    File.Delete(_recordarPath);
            }
            catch { }
        }

        // ==================== CAPS LOCK ====================

        private void VerificarCapsLock()
        {
            try
            {
                bool capsLockOn = (GetKeyState(0x14) & 0xffff) != 0;
                if (capsLockOn != _capsLockOn)
                {
                    _capsLockOn = capsLockOn;
                    lblCapsLock.Visible = capsLockOn;
                }
            }
            catch { }
        }

        // ==================== ESTADO DE CONEXIÓN ====================

        private async Task VerificarEstadoConexion()
        {
            try
            {
                bool conectado = await Task.Run(() =>
                {
                    try { using var conn = MySQLConnection.GetConnection(); conn.Open(); return true; }
                    catch { return false; }
                });

                lblEstadoConexion.Text = conectado ? "🟢  Servidor conectado" : "🔴  Sin conexión";
                lblEstadoConexion.ForeColor = conectado
                    ? Color.FromArgb(40, 167, 69)
                    : Color.FromArgb(220, 53, 69);
            }
            catch
            {
                lblEstadoConexion.Text = "🔴  Sin conexión";
                lblEstadoConexion.ForeColor = Color.FromArgb(220, 53, 69);
            }
        }

        // ==================== LOGO ====================

        private void CargarLogo()
        {
            try
            {
                string logoPath = Path.Combine(Application.StartupPath, "Resources", "logo_modelo_dual.png");
                if (File.Exists(logoPath))
                    pictureBoxLogo.Image = Image.FromFile(logoPath);
                else
                    pictureBoxLogo.BackColor = Color.FromArgb(0, 71, 160);
            }
            catch
            {
                pictureBoxLogo.BackColor = Color.FromArgb(0, 71, 160);
            }
        }

        // ==================== VALIDACIÓN VISUAL ====================

        private bool ValidarCampos()
        {
            RestaurarColores();
            string login = txtLogin.Text.Trim();

            if (string.IsNullOrEmpty(login))
            {
                MarcarError(txtLogin);
                MostrarError("Ingresa tu usuario o email.");
                return false;
            }

            if (login.Length < Seguridad.UsernameMinLength)
            {
                MarcarError(txtLogin);
                MostrarError($"El usuario debe tener al menos {Seguridad.UsernameMinLength} caracteres.");
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MarcarError(txtPassword);
                MostrarError("Ingresa tu contraseña.");
                return false;
            }

            if (txtPassword.Text.Length < ParametroSistemaService.Instance.GetInt(
                Claves.MIN_CARACTERES_PASSWORD, Seguridad.PasswordMinLength))
            {
                MarcarError(txtPassword);
                MostrarError($"La contraseña debe tener al menos {Seguridad.PasswordMinLength} caracteres.");
                return false;
            }

            return true;
        }

        private void MarcarError(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(255, 235, 235);
            txt.Focus();
        }

        private void RestaurarColores()
        {
            txtLogin.BackColor = Color.FromArgb(245, 245, 245);
            txtPassword.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void MostrarError(string mensaje, int intentosRestantes = -1)
        {
            lblError.Text = mensaje;
            lblError.Visible = true;
            picError.Visible = true;

            // Prioridad: bloqueo > último intento > advertencia > error normal
            if (mensaje.Contains("bloquead") || mensaje.Contains("Bloquead"))
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);   // rojo intenso
                picError.Text = "🔒";
            }
            else if (intentosRestantes == 1)
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);   // rojo: ÚLTIMO intento
                picError.Text = "⚠";
            }
            else if (intentosRestantes == 2)
            {
                lblError.ForeColor = Color.FromArgb(255, 140, 0);   // naranja: advertencia
                picError.Text = "⚠";
            }
            else if (intentosRestantes >= 3)
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);   // rojo normal
                picError.Text = "!";
            }
            else
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);   // error normal
                picError.Text = "!";
            }
        }

        private void LimpiarError()
        {
            if (lblError.Visible)
            {
                lblError.Visible = false;
                lblError.Text = "";
                picError.Visible = false;
            }
        }

        // ==================== LOADING ====================

        private void SetLoading(bool loading)
        {
            _isLoading = loading;
            btnIniciarSesion.Enabled = !loading;
            txtLogin.Enabled = !loading;
            txtPassword.Enabled = !loading;
            chkMostrarPassword.Enabled = !loading;
            chkRecordar.Enabled = !loading;
            lnkCrearCuenta.Enabled = !loading;
            lnkRecuperar.Enabled = !loading;

            if (loading)
            {
                btnIniciarSesion.Text = "Verificando...";
                btnIniciarSesion.BackColor = Color.FromArgb(0, 100, 180);
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                btnIniciarSesion.Text = "Iniciar Sesión";
                btnIniciarSesion.BackColor = Color.FromArgb(0, 71, 160);
                Cursor = Cursors.Default;
            }
        }

        // ==================== LOGIN ====================

        private async void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (!ValidarCampos()) return;

            SetLoading(true);
            LimpiarError();

            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                bool conexionOk = await Task.Run(() =>
                {
                    try { using var conn = MySQLConnection.GetConnection(); conn.Open(); return true; }
                    catch { return false; }
                });

                if (!conexionOk)
                {
                    MostrarError(Seguridad.MsgErrorConexion);
                    return;
                }

                var resultado = await Task.Run(() => _authController.ValidarCredenciales(login, password));

                if (resultado.Success)
                {
                    if (resultado.PasswordExpirado)
                    {
                        SetLoading(false);
                        MessageBox.Show(
                            "Tu contraseña ha expirado (>90 días).\n" +
                            "Por seguridad, debés cambiarla antes de continuar.",
                            "Contraseña Expirada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        // Cargar sesión completa (rol, privilegios, entidad) para que
                        // la autorización del menú funcione durante el cambio de password
                        if (!_authController.IniciarSesionPorPasswordExpirado(resultado.Usuario))
                        {
                            MostrarError("No se pudo preparar la sesión para cambio de contraseña.");
                            return;
                        }
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }

                    if (resultado.RequiereActivacion)
                    {
                        SetLoading(false);
                        using var formActivar = new FormActivarCuenta
                        {
                            UsuarioPreLlenado = login
                        };
                        formActivar.ShowDialog();
                        txtLogin.Clear();
                        txtPassword.Clear();
                        txtLogin.Focus();
                        return;
                    }

                    GuardarUsuarioRecordado();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else if (resultado.CuentaEliminada)
                {
                    MarcarError(txtLogin);
                    MostrarError("Esta cuenta ha sido eliminada. Contacta al administrador.");
                }
                else
                {
                    // Primero mostrar el error, luego limpiar password SIN que TextChanged lo borre
                    MarcarError(txtPassword);
                    MostrarError(resultado.Message, resultado.IntentosRestantes);
                    _skipClearError = true;
                    txtPassword.Clear();
                    _skipClearError = false;
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error en login", ex);
                MostrarError(Seguridad.MsgErrorGenerico);
            }
            finally
            {
                SetLoading(false);
            }
        }

        // ==================== EVENTOS ====================

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkMostrarPassword.Checked ? '\0' : '\u25CF';
        }

        private void lnkCrearCuenta_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using var formActivar = new FormActivarCuenta();
            if (formActivar.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Tu cuenta ha sido activada. Ya puedes iniciar sesión.", "Cuenta Activada",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtLogin.Clear();
                txtPassword.Clear();
                txtLogin.Focus();
            }
        }

        private void lnkRecuperar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var result = MessageBox.Show(
                "Para recuperar tu contraseña, contacta al administrador del sistema.\n\n" +
                "¿Deseas abrir la ventana de activación por si tienes un código temporal?",
                "Recuperar Contraseña", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                lnkCrearCuenta_LinkClicked(sender, e);
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show("¿Deseas salir del sistema?", "Confirmar salida",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) e.Cancel = true;
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
