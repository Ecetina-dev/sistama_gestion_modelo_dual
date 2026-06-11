#pragma warning disable CS0414
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading.Tasks;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;
using Laboratorio_del_Tema_5_2.Data;

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
            // Limites realistas segun el tipo de campo y la BD
            txtUsuario.MaxLength = 25;       // Matrícula o usuario
            txtPasswordTemp.MaxLength = 50;  // Password temporal generado
            txtPasswordNuevo.MaxLength = 50; // Contraseña nueva
            txtConfirmar.MaxLength = 50;     // Confirmación

            this.Shown += (s, e) => txtUsuario.Focus();

            // Restaurar color al escribir
            txtUsuario.TextChanged += (s, e) => { LimpiarError(); RestaurarColor(txtUsuario); };
            txtPasswordTemp.TextChanged += (s, e) => LimpiarError();
            txtPasswordNuevo.TextChanged += (s, e) =>
            {
                LimpiarError();
                RestaurarColor(txtPasswordNuevo);
                RestaurarColor(txtConfirmar);
                ActualizarFuerza();
            };
            txtConfirmar.TextChanged += (s, e) => LimpiarError();

            // Enter navega, Escape con confirmación
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape && !_isLoading)
                {
                    e.SuppressKeyPress = true;
                    btnCancelar_Click(s, e);
                }
            };
        }

        // ==================== INDICADOR DE FUERZA ====================

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
                lblFuerza.Text = "DÉBIL - Agrega mayúsculas, números y símbolos";
                lblFuerza.ForeColor = Color.FromArgb(220, 53, 69);
            }
            else if (score < 5)
            {
                lblFuerza.Text = "ACEPTABLE";
                lblFuerza.ForeColor = Color.FromArgb(255, 193, 7);
            }
            else
            {
                lblFuerza.Text = "FUERTE";
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
            if (Regex.IsMatch(password, @"[a-z]")) score++;
            if (Regex.IsMatch(password, @"[A-Z]")) score++;
            if (Regex.IsMatch(password, @"[0-9]")) score++;
            if (Regex.IsMatch(password, @"[^a-zA-Z0-9]")) score++;
            return score;
        }

        // ==================== VALIDACIÓN ====================

        private bool ValidarCampos()
        {
            RestaurarColores();
            string usuario = txtUsuario.Text.Trim();

            if (string.IsNullOrEmpty(usuario))
            {
                MarcarError(txtUsuario);
                MostrarError("Ingresa tu matrícula o usuario.");
                return false;
            }

            if (string.IsNullOrEmpty(txtPasswordTemp.Text))
            {
                MarcarError(txtPasswordTemp);
                MostrarError("Ingresa el password temporal que te entregaron.");
                return false;
            }

            string pwdNuevo = txtPasswordNuevo.Text;

            if (string.IsNullOrEmpty(pwdNuevo))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Ingresa tu nueva contraseña.");
                return false;
            }

            if (pwdNuevo.Length < Seguridad.PasswordMinLength)
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Mínimo " + Seguridad.PasswordMinLength + " caracteres.");
                return false;
            }

            // Requisitos enterprise para la contraseña
            if (!Regex.IsMatch(pwdNuevo, @"[A-Z]"))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Debe contener al menos una mayúscula.");
                return false;
            }
            if (!Regex.IsMatch(pwdNuevo, @"[a-z]"))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Debe contener al menos una minúscula.");
                return false;
            }
            if (!Regex.IsMatch(pwdNuevo, @"[0-9]"))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Debe contener al menos un número.");
                return false;
            }

            if (pwdNuevo != txtConfirmar.Text)
            {
                MarcarError(txtConfirmar);
                MostrarError("Las contraseñas no coinciden.");
                return false;
            }

            if (txtPasswordTemp.Text == pwdNuevo)
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Debe ser diferente al password temporal.");
                return false;
            }

            return true;
        }

        private void MarcarError(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(255, 235, 235);
            txt.Focus();
        }

        private void RestaurarColor(TextBox txt)
        {
            if (txt.BackColor == Color.FromArgb(255, 235, 235))
                txt.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void RestaurarColores()
        {
            txtUsuario.BackColor = Color.FromArgb(245, 245, 245);
            txtPasswordTemp.BackColor = Color.FromArgb(245, 245, 245);
            txtPasswordNuevo.BackColor = Color.FromArgb(245, 245, 245);
            txtConfirmar.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = "! " + mensaje;
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
            btnActivar.Enabled = !loading;
            txtUsuario.Enabled = !loading;
            txtPasswordTemp.Enabled = !loading;
            txtPasswordNuevo.Enabled = !loading;
            txtConfirmar.Enabled = !loading;
            chkMostrarTemp.Enabled = !loading;
            chkMostrarNuevo.Enabled = !loading;

            if (loading)
            {
                btnActivar.Text = "Activando...";
                btnActivar.BackColor = Color.FromArgb(100, 140, 200);
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                btnActivar.Text = "Activar mi Cuenta";
                btnActivar.BackColor = Color.FromArgb(0, 71, 160);
                Cursor = Cursors.Default;
            }
        }

        // ==================== ACTIVAR ====================

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
                // Test de conexión rápido
                bool conexionOk = await Task.Run(() =>
                {
                    try { using var conn = MySQLConnection.GetConnection(); conn.Open(); return true; }
                    catch { return false; }
                });

                if (!conexionOk)
                {
                    MostrarError("Error de conexión con el servidor.");
                    return;
                }

                var resultado = await Task.Run(() =>
                    _authController.ActivarCuenta(usuario, passwordTemp, passwordNuevo));

                if (resultado.Success)
                {
                    MessageBox.Show("¡Cuenta activada! Ya puedes iniciar sesión con tu nueva contraseña.",
                        "Activación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MarcarError(txtPasswordTemp);
                    MostrarError(resultado.Message);
                }
            }
            catch
            {
                MostrarError("Error de conexión. Verifica tu red.");
            }
            finally
            {
                SetLoading(false);
            }
        }

        // ==================== EVENTOS ====================

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (HayDatosIngresados())
            {
                var r = MessageBox.Show("¿Cancelar la activación? Los datos ingresados se perderán.",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }
            this.Close();
        }

        private bool HayDatosIngresados()
        {
            return !string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                   !string.IsNullOrWhiteSpace(txtPasswordTemp.Text) ||
                   !string.IsNullOrWhiteSpace(txtPasswordNuevo.Text) ||
                   !string.IsNullOrWhiteSpace(txtConfirmar.Text);
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
