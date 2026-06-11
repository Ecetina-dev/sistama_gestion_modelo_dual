#pragma warning disable CS0414
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
            txtLogin.MaxLength = 25;        // Usuario/email
            txtPassword.MaxLength = 50;      // Contraseña

            this.Shown += (s, e) =>
            {
                if (string.IsNullOrEmpty(txtLogin.Text))
                    txtLogin.Focus();
                else
                    txtPassword.Focus();
                VerificarCapsLock();
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
                    string usuario = File.ReadAllText(_recordarPath).Trim();
                    if (!string.IsNullOrEmpty(usuario))
                    {
                        txtLogin.Text = usuario;
                        chkRecordar.Checked = true;
                    }
                }
            }
            catch { /* ignorar errores de archivo */ }
        }

        private void GuardarUsuarioRecordado()
        {
            try
            {
                string dir = Path.GetDirectoryName(_recordarPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                if (chkRecordar.Checked)
                    File.WriteAllText(_recordarPath, txtLogin.Text.Trim());
                else if (File.Exists(_recordarPath))
                    File.Delete(_recordarPath);
            }
            catch { /* ignorar errores de archivo */ }
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

            if (txtPassword.Text.Length < Seguridad.PasswordMinLength)
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

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            lblError.Visible = true;
        }

        private void LimpiarError()
        {
            if (lblError.Visible)
            {
                lblError.Visible = false;
                lblError.Text = "";
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
                    GuardarUsuarioRecordado();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MarcarError(txtPassword);
                    MostrarError(resultado.Message);
                    txtPassword.Clear();
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
            txtPassword.PasswordChar = chkMostrarPassword.Checked ? '\0' : '*';
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
            MessageBox.Show("Contacta al administrador del sistema para recuperar tu contraseña.",
                "Recuperar Contraseña", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
    }
}
