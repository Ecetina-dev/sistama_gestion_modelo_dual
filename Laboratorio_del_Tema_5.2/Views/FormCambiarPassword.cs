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
    public partial class FormCambiarPassword : Form
    {
        private readonly AuthController _authController;
        private bool _isLoading = false;

        public FormCambiarPassword()
        {
            InitializeComponent();
            _authController = new AuthController();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            txtPasswordActual.MaxLength = Seguridad.PasswordMaxLength;
            txtPasswordNuevo.MaxLength = Seguridad.PasswordMaxLength;
            txtConfirmPassword.MaxLength = Seguridad.PasswordMaxLength;

            txtPasswordActual.Focus();

            // Restaurar color al escribir
            txtPasswordActual.TextChanged += (s, e) => RestaurarColor(txtPasswordActual);
            txtPasswordNuevo.TextChanged += (s, e) => { RestaurarColor(txtPasswordNuevo); LimpiarError(); };
            txtConfirmPassword.TextChanged += (s, e) => { RestaurarColor(txtConfirmPassword); LimpiarError(); };

            // Enter navega, Escape cierra
            this.KeyPreview = true;
            txtPasswordActual.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; txtPasswordNuevo.Focus(); } };
            txtPasswordNuevo.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; txtConfirmPassword.Focus(); } };
            txtConfirmPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && !_isLoading) { e.SuppressKeyPress = true; btnCambiar_Click(s, e); } };
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) { e.SuppressKeyPress = true; this.Close(); } };
        }

        private bool ValidarCampos()
        {
            RestaurarColores();

            if (string.IsNullOrEmpty(txtPasswordActual.Text))
            {
                MarcarError(txtPasswordActual);
                MostrarError("Ingresa tu contraseña actual.");
                return false;
            }

            string pwdNuevo = txtPasswordNuevo.Text;

            if (string.IsNullOrEmpty(pwdNuevo))
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Ingresa la nueva contraseña.");
                return false;
            }

            if (pwdNuevo.Length < Seguridad.PasswordMinLength)
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("Mínimo " + Seguridad.PasswordMinLength + " caracteres.");
                return false;
            }

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

            if (pwdNuevo != txtConfirmPassword.Text)
            {
                MarcarError(txtConfirmPassword);
                MostrarError("Las contraseñas no coinciden.");
                return false;
            }

            if (pwdNuevo == txtPasswordActual.Text)
            {
                MarcarError(txtPasswordNuevo);
                MostrarError("La nueva contraseña debe ser diferente a la actual.");
                return false;
            }

            return true;
        }

        private void MarcarError(TextBox txt) { txt.BackColor = Color.FromArgb(255, 235, 235); txt.Focus(); }
        private void RestaurarColor(TextBox txt) { if (txt.BackColor == Color.FromArgb(255, 235, 235)) txt.BackColor = Color.FromArgb(245, 245, 245); }
        private void RestaurarColores()
        {
            foreach (var t in new[] { txtPasswordActual, txtPasswordNuevo, txtConfirmPassword })
                t.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void MostrarError(string msg) { lblError.Text = msg; lblError.Visible = true; }
        private void LimpiarError() { lblError.Visible = false; lblError.Text = ""; }

        private async void btnCambiar_Click(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (!ValidarCampos()) return;

            _isLoading = true;
            btnCambiar.Enabled = false;
            btnCambiar.Text = "Cambiando...";
            Cursor = Cursors.WaitCursor;
            LimpiarError();

            try
            {
                bool conexionOk = await Task.Run(() =>
                { try { using var c = SqlServerConnection.GetConnection(); c.Open(); return true; } catch { return false; } });

                if (!conexionOk) { MostrarError("Error de conexión."); return; }

                bool exito = await Task.Run(() => _authController.CambiarPassword(
                    SesionActiva.Instance.Id_Usuario, txtPasswordActual.Text, txtPasswordNuevo.Text));

                if (exito)
                {
                    MessageBox.Show("Contraseña cambiada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MarcarError(txtPasswordActual);
                    MostrarError("Contraseña actual incorrecta.");
                    txtPasswordActual.Clear();
                    txtPasswordActual.Focus();
                }
            }
            catch
            {
                MostrarError("Error de conexión.");
            }
            finally
            {
                _isLoading = false;
                btnCambiar.Enabled = true;
                btnCambiar.Text = "Cambiar Contraseña";
                Cursor = Cursors.Default;
            }
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            char c = chkMostrarPassword.Checked ? '\0' : '*';
            txtPasswordActual.PasswordChar = c;
            txtPasswordNuevo.PasswordChar = c;
            txtConfirmPassword.PasswordChar = c;
        }

        private void btnCancelar_Click(object sender, EventArgs e) => this.Close();
    }
}
