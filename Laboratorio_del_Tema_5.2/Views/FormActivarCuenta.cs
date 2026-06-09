using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormActivarCuenta : Form
    {
        private readonly AuthController _authController;
        private bool _isLoading = false;

        public FormActivarCuenta()
        {
            InitializeComponent();
            _authController = new AuthController();
            ConfigurarControles();
        }

        private void ConfigurarControles()
        {
            txtUsuario.MaxLength = Seguridad.UsernameMaxLength;
            txtPasswordTemp.MaxLength = Seguridad.PasswordMaxLength;
            txtPasswordNuevo.MaxLength = Seguridad.PasswordMaxLength;
            txtConfirmar.MaxLength = Seguridad.PasswordMaxLength;

            this.Shown += (s, e) => txtUsuario.Focus();

            txtUsuario.TextChanged += (s, e) => { LimpiarError(); MarcarNormal(txtUsuario); };
            txtPasswordTemp.TextChanged += (s, e) => LimpiarError();
            txtPasswordNuevo.TextChanged += (s, e) =>
            {
                LimpiarError();
                MarcarNormal(txtPasswordNuevo);
                MarcarNormal(txtConfirmar);
                ActualizarFuerza();
            };
            txtConfirmar.TextChanged += (s, e) => { LimpiarError(); };

            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && !_isLoading)
                {
                    e.SuppressKeyPress = true;
                    this.Close();
                }
            };
        }

        private void ActualizarFuerza()
        {
            string pwd = txtPasswordNuevo.Text;
            if (string.IsNullOrEmpty(pwd))
            {
                lblFuerza.Visible = false;
                return;
            }

            lblFuerza.Visible = true;
            int score = CalcularFuerzaPassword(pwd);

            if (score < 3)
            {
                lblFuerza.Text = "Contrasena DEBIL - Agrega mas variedad";
                lblFuerza.ForeColor = Color.FromArgb(220, 53, 69);
            }
            else if (score < 5)
            {
                lblFuerza.Text = "Contrasena ACEPTABLE";
                lblFuerza.ForeColor = Color.FromArgb(255, 193, 7);
            }
            else
            {
                lblFuerza.Text = "Contrasena FUERTE";
                lblFuerza.ForeColor = Color.FromArgb(40, 167, 69);
            }
        }

        private int CalcularFuerzaPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return 0;
            int score = 0;
            if (password.Length >= 6) score++;
            if (password.Length >= 8) score++;
            if (password.Length >= 12) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]")) score++;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score++;
            return score;
        }

        private bool ValidarCampos()
        {
            string usuario = txtUsuario.Text.Trim();

            if (string.IsNullOrEmpty(usuario))
            {
                MostrarError("Ingresa tu matricula o usuario", txtUsuario);
                return false;
            }

            if (string.IsNullOrEmpty(txtPasswordTemp.Text))
            {
                MostrarError("Ingresa el password temporal", txtPasswordTemp);
                return false;
            }

            if (string.IsNullOrEmpty(txtPasswordNuevo.Text))
            {
                MostrarError("Ingresa tu nueva contrasena", txtPasswordNuevo);
                return false;
            }

            if (txtPasswordNuevo.Text.Length < Seguridad.PasswordMinLength)
            {
                MostrarError("Minimo 6 caracteres para la contrasena", txtPasswordNuevo);
                return false;
            }

            if (txtPasswordNuevo.Text != txtConfirmar.Text)
            {
                MostrarError("Las contrasenas no coinciden", txtConfirmar);
                return false;
            }

            if (txtPasswordTemp.Text == txtPasswordNuevo.Text)
            {
                MostrarError("Tu nueva contrasena debe ser diferente al password temporal", txtPasswordNuevo);
                return false;
            }

            return true;
        }

        private void MostrarError(string mensaje, Control controlFoco = null)
        {
            lblError.Text = "! " + mensaje;
            lblError.Visible = true;
            if (controlFoco != null)
            {
                if (controlFoco is TextBox txt)
                    txt.BackColor = Color.FromArgb(255, 235, 235);
                controlFoco.Focus();
            }
        }

        private void LimpiarError()
        {
            if (lblError.Visible)
            {
                lblError.Visible = false;
                lblError.Text = "";
            }
        }

        private void MarcarNormal(TextBox txt)
        {
            if (txt.BackColor == Color.FromArgb(255, 235, 235))
                txt.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void SetLoading(bool loading)
        {
            _isLoading = loading;
            btnActivar.Enabled = !loading;
            btnActivar.BackColor = loading
                ? Color.FromArgb(100, 140, 200)
                : Color.FromArgb(0, 71, 160);

            txtUsuario.Enabled = !loading;
            txtPasswordTemp.Enabled = !loading;
            txtPasswordNuevo.Enabled = !loading;
            txtConfirmar.Enabled = !loading;
            chkMostrarTemp.Enabled = !loading;
            chkMostrarNuevo.Enabled = !loading;

            btnActivar.Text = loading ? "Activando..." : "Activar mi Cuenta";
        }

        private async void btnActivar_Click(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (!ValidarCampos()) return;

            SetLoading(true);
            LimpiarError();

            string usuario = txtUsuario.Text.Trim();
            string passwordTemp = txtPasswordTemp.Text;
            string passwordNuevo = txtPasswordNuevo.Text;

            try
            {
                var resultado = await Task.Run(() =>
                    _authController.ActivarCuenta(usuario, passwordTemp, passwordNuevo));

                if (resultado.Success)
                {
                    MessageBox.Show(
                        "Tu cuenta ha sido activada exitosamente.\n\nYa puedes usar el sistema.",
                        "Cuenta Activada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MostrarError(resultado.Message, txtPasswordTemp);
                }
            }
            catch
            {
                MostrarError("Error de conexion. Verifica tu red.", txtUsuario);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkMostrarTemp_CheckedChanged(object sender, EventArgs e)
        {
            txtPasswordTemp.PasswordChar = chkMostrarTemp.Checked ? '\0' : '*';
        }

        private void chkMostrarNuevo_CheckedChanged(object sender, EventArgs e)
        {
            char c = chkMostrarNuevo.Checked ? '\0' : '*';
            txtPasswordNuevo.PasswordChar = c;
            txtConfirmar.PasswordChar = c;
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtPasswordTemp.Focus();
            }
        }

        private void txtPasswordTemp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtPasswordNuevo.Focus();
            }
        }

        private void txtPasswordNuevo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtConfirmar.Focus();
            }
        }

        private void txtConfirmar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !_isLoading)
            {
                e.SuppressKeyPress = true;
                btnActivar_Click(sender, e);
            }
        }
    }
}
