using System;
using System.Drawing;
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

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        private const byte VK_CAPITAL = 0x14;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        private const uint KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        private static extern short GetKeyState(int keyCode);

        public FormLogin()
        {
            InitializeComponent();
            _authController = new AuthController();
            ConfigurarControles();
        }

        private void ConfigurarControles()
        {
            // Limites de longitud para prevenir inputs excesivos
            txtLogin.MaxLength = Seguridad.LoginMaxLength;
            txtPassword.MaxLength = Seguridad.PasswordMaxLength;

            // Auto-focus en el primer campo
            this.Shown += (s, e) =>
            {
                txtLogin.Focus();
                VerificarCapsLock();
            };

            // Limpiar errores al escribir
            txtLogin.TextChanged += (s, e) => { LimpiarError(); VerificarCapsLock(); };
            txtPassword.TextChanged += (s, e) => { LimpiarError(); VerificarCapsLock(); };

            // Enter envia el form
            txtLogin.KeyDown += new KeyEventHandler(this.TxtLogin_KeyDown);
            txtPassword.KeyDown += new KeyEventHandler(this.TxtPassword_KeyDown);

            // Escape limpia o cierra
            this.KeyPreview = true;
            this.KeyDown += FormLogin_KeyDown;
        }

        private void TxtLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtPassword.Focus();
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !_isLoading)
            {
                e.SuppressKeyPress = true;
                btnIniciarSesion_Click(sender, e);
            }
        }

        private void FormLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && !_isLoading)
            {
                e.SuppressKeyPress = true;
                this.Close();
            }
        }

        private void VerificarCapsLock()
        {
            try
            {
                // Verificar estado de Caps Lock
                bool capsLockOn = (GetKeyState(0x14) & 0xffff) != 0;

                if (capsLockOn != _capsLockOn)
                {
                    _capsLockOn = capsLockOn;
                    lblCapsLock.Visible = capsLockOn;
                }
            }
            catch
            {
                // Si falla, no hacer nada
            }
        }

        private void CargarLogo()
        {
            try
            {
                string logoPath = System.IO.Path.Combine(Application.StartupPath, "Resources", "logo_modelo_dual.png");
                if (System.IO.File.Exists(logoPath))
                {
                    pictureBoxLogo.Image = Image.FromFile(logoPath);
                }
                else
                {
                    pictureBoxLogo.BackColor = Color.FromArgb(0, 71, 160);
                    pictureBoxLogo.Image = null;
                }
            }
            catch
            {
                pictureBoxLogo.BackColor = Color.FromArgb(0, 71, 160);
            }
        }

        private bool ValidarCampos()
        {
            string login = txtLogin.Text.Trim();

            if (string.IsNullOrEmpty(login))
            {
                MostrarError("Ingresa tu usuario o email", txtLogin);
                return false;
            }

            if (login.Length < Seguridad.UsernameMinLength)
            {
                MostrarError($"El usuario debe tener al menos {Seguridad.UsernameMinLength} caracteres", txtLogin);
                return false;
            }

            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                MostrarError("Ingresa tu contrasena", txtPassword);
                return false;
            }

            if (txtPassword.Text.Length < Seguridad.PasswordMinLength)
            {
                MostrarError($"La contrasena debe tener al menos {Seguridad.PasswordMinLength} caracteres", txtPassword);
                return false;
            }

            return true;
        }

        private void MostrarError(string mensaje, Control controlFoco = null)
        {
            lblError.Text = mensaje;
            lblError.Visible = true;
            picError.Visible = true;

            if (controlFoco != null)
            {
                controlFoco.Focus();
            }
        }

        private void LimpiarError()
        {
            if (lblError.Visible)
            {
                lblError.Visible = false;
                picError.Visible = false;
            }
        }

        private void SetLoading(bool loading)
        {
            _isLoading = loading;
            btnIniciarSesion.Enabled = !loading;
            txtLogin.Enabled = !loading;
            txtPassword.Enabled = !loading;
            chkMostrarPassword.Enabled = !loading;
            lnkCrearCuenta.Enabled = !loading;
            lnkRecuperar.Enabled = !loading;

            if (loading)
            {
                btnIniciarSesion.Text = "Verificando...";
                btnIniciarSesion.BackColor = Color.FromArgb(0, 100, 180);
            }
            else
            {
                btnIniciarSesion.Text = "Iniciar Sesion";
                btnIniciarSesion.BackColor = Color.FromArgb(0, 71, 160);
            }
        }

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
                // Test de conexion rapido antes de validar
                bool conexionOk = await Task.Run(() =>
                {
                    try
                    {
                        using (var conn = MySQLConnection.GetConnection())
                        {
                            conn.Open();
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (!conexionOk)
                {
                    MostrarError(Seguridad.MsgErrorConexion, txtLogin);
                    return;
                }

                // Ejecutar validacion en task
                var resultado = await Task.Run(() => _authController.ValidarCredenciales(login, password));

                if (resultado.Success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MostrarError(resultado.Message, txtPassword);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error en login", ex);
                MostrarError(Seguridad.MsgErrorGenerico, txtLogin);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkMostrarPassword.Checked ? '\0' : '*';
        }

        private void lnkCrearCuenta_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var formActivar = new FormActivarCuenta())
            {
                if (formActivar.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show(
                        "Tu cuenta ha sido activada. Ya puedes iniciar sesion con tu nueva contrasena.",
                        "Cuenta Activada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    txtLogin.Clear();
                    txtPassword.Clear();
                    txtLogin.Focus();
                }
            }
        }

        private void lnkRecuperar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(
                "Contacta al administrador del sistema para recuperar tu contrasena.",
                "Recuperar Contrasena",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void FormLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK && e.CloseReason == CloseReason.UserClosing)
            {
                var result = MessageBox.Show(
                    "Deseas salir del sistema?",
                    "Confirmar salida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}