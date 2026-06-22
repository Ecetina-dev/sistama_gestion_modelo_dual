#pragma warning disable CS0414
using System;
using System.Data.SqlClient;
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

        public string UsuarioPreLlenado { get; set; }

        public FormActivarCuenta()
        {
            InitializeComponent();
            _authController = new AuthController();
            ConfigurarControles();
        }

        private void ConfigurarControles()
        {
            // Limites realistas segun el tipo de campo y la BD
            txtUsuario.MaxLength = Seguridad.UsernameMaxLength;       // Usuario (50)
            txtPasswordTemp.MaxLength = Seguridad.PasswordMaxLength;  // Password temporal (128)
            txtPasswordNuevo.MaxLength = Seguridad.PasswordMaxLength; // Contraseña nueva (100)
            txtConfirmar.MaxLength = Seguridad.PasswordMaxLength;     // Confirmación (100)

            this.Shown += (s, e) =>
            {
                if (!string.IsNullOrEmpty(UsuarioPreLlenado))
                {
                    txtUsuario.Text = UsuarioPreLlenado;
                    txtUsuario.ReadOnly = true;
                    txtUsuario.BackColor = Color.FromArgb(235, 235, 235);
                    txtPasswordTemp.Focus();
                }
                else
                {
                    txtUsuario.Focus();
                }
                _ = VerificarEstadoConexion();
            };

            // Restaurar color al escribir
            txtUsuario.TextChanged += (s, e) => { LimpiarError(); RestaurarColor(txtUsuario); };
            txtPasswordTemp.TextChanged += (s, e) => LimpiarError();
            txtPasswordNuevo.TextChanged += (s, e) =>
            {
                LimpiarError();
                RestaurarColor(txtPasswordNuevo);
                RestaurarColor(txtConfirmar);
                ActualizarFuerza();
                ActualizarChecklist();
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

        private void ActualizarChecklist()
        {
            string pwd = txtPasswordNuevo.Text;
            int minChars = ParametroSistemaService.Instance.GetInt(Claves.MIN_CARACTERES_PASSWORD, 8);
            bool exigirMayus = ParametroSistemaService.Instance.GetBool(Claves.EXIGIR_MAYUSCULAS_PASSWORD, true);
            bool exigirNums = ParametroSistemaService.Instance.GetBool(Claves.EXIGIR_NUMEROS_PASSWORD, true);
            bool exigirEspecial = ParametroSistemaService.Instance.GetBool(Claves.EXIGIR_CARACTER_ESPECIAL_PASSWORD, true);

            ActualizarItemChecklist(lblCheckMinChars, pwd.Length >= minChars, $"Mínimo {minChars} caracteres");
            ActualizarItemChecklist(lblCheckMayus, Regex.IsMatch(pwd, @"[A-Z]"), "Una mayúscula");
            ActualizarItemChecklist(lblCheckMinus, Regex.IsMatch(pwd, @"[a-z]"), "Una minúscula");
            if (exigirNums) ActualizarItemChecklist(lblCheckNumero, Regex.IsMatch(pwd, @"[0-9]"), "Un número");
            if (exigirEspecial) ActualizarItemChecklist(lblCheckEspecial, Regex.IsMatch(pwd, @"[^a-zA-Z0-9]"), "Un carácter especial");

            lblCheckNumero.Visible = exigirNums;
            lblCheckEspecial.Visible = exigirEspecial;
        }

        private void ActualizarItemChecklist(Label lbl, bool cumple, string texto)
        {
            lbl.Text = $"{(cumple ? "✅" : "❌")} {texto}";
            lbl.ForeColor = cumple ? Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69);
        }

        private async Task VerificarEstadoConexion()
        {
            try
            {
                bool conectado = await Task.Run(() =>
                {
                    try { using var conn = SqlServerConnection.GetConnection(); conn.Open(); return true; }
                    catch { return false; }
                });
                lblEstadoConexion.Text = conectado ? "🟢  Servidor conectado" : "🔴  Sin conexión";
                lblEstadoConexion.ForeColor = conectado ? Color.FromArgb(40, 167, 69) : Color.FromArgb(220, 53, 69);
            }
            catch { lblEstadoConexion.Text = "🔴  Sin conexión"; lblEstadoConexion.ForeColor = Color.FromArgb(220, 53, 69); }
        }

        // ==================== INDICADOR DE FUERZA (LEGACY) ====================

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
            int minChars = ParametroSistemaService.Instance.GetInt(
                Claves.MIN_CARACTERES_PASSWORD, Seguridad.PasswordMinLength);
            if (password.Length >= minChars) score++;
            if (password.Length >= Math.Max(minChars + 2, 8)) score++;
            if (password.Length >= Math.Max(minChars + 4, 12)) score++;
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

            int minPasswordLength = ParametroSistemaService.Instance.GetInt(
                Claves.MIN_CARACTERES_PASSWORD, Seguridad.PasswordMinLength);

            if (pwdNuevo.Length < minPasswordLength)
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Mínimo " + minPasswordLength + " caracteres.");
                return false;
            }

            // Requisitos enterprise para la contraseña
            bool exigirMayus = ParametroSistemaService.Instance.GetBool(
                Claves.EXIGIR_MAYUSCULAS_PASSWORD, true);
            bool exigirNums = ParametroSistemaService.Instance.GetBool(
                Claves.EXIGIR_NUMEROS_PASSWORD, true);
            bool exigirEspecial = ParametroSistemaService.Instance.GetBool(
                Claves.EXIGIR_CARACTER_ESPECIAL_PASSWORD, Seguridad.ExigirCaracterEspecialPassword);

            if (exigirMayus && !Regex.IsMatch(pwdNuevo, @"[A-Z]"))
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
            if (exigirNums && !Regex.IsMatch(pwdNuevo, @"[0-9]"))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Debe contener al menos un número.");
                return false;
            }
            if (exigirEspecial && !Regex.IsMatch(pwdNuevo, @"[^a-zA-Z0-9]"))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Debe contener al menos un carácter especial (!@#$%...).");
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

        private void MostrarError(string mensaje, int intentosRestantes = -1)
        {
            lblError.Text = "! " + mensaje;
            lblError.Visible = true;

            // Aviso progresivo de intentos restantes (mismo patrón que login)
            if (mensaje.Contains("bloquead") || mensaje.Contains("Bloquead"))
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);
            }
            else if (intentosRestantes == 1)
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);
            }
            else if (intentosRestantes == 2)
            {
                lblError.ForeColor = Color.FromArgb(255, 140, 0);
            }
            else if (intentosRestantes >= 3)
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);
            }
            else
            {
                lblError.ForeColor = Color.FromArgb(220, 53, 69);
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
                    try { using var conn = SqlServerConnection.GetConnection(); conn.Open(); return true; }
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
                    MostrarError(resultado.Message, resultado.IntentosRestantes);
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

        private void lblVolverLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (HayDatosIngresados())
            {
                var r = MessageBox.Show("¿Volver al inicio de sesión? Los datos se perderán.",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }
            this.Close();
        }
    }
}
