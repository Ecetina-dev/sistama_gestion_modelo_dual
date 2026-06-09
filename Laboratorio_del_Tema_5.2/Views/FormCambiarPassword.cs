using System;
using System.Windows.Forms;
using System.Drawing;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormCambiarPassword : Form
    {
        private readonly AuthController _authController;

        public FormCambiarPassword()
        {
            InitializeComponent();
            _authController = new AuthController();
        }

        private void chkMostrarPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool mostrar = chkMostrarPassword.Checked;
            txtPasswordActual.PasswordChar = mostrar ? '\0' : '*';
            txtPasswordNuevo.PasswordChar = mostrar ? '\0' : '*';
            txtConfirmPassword.PasswordChar = mostrar ? '\0' : '*';
        }

        private void btnCambiar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            string passwordActual = txtPasswordActual.Text;
            string passwordNuevo = txtPasswordNuevo.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(passwordActual))
            {
                lblError.Text = "Ingresa tu contrasena actual";
                txtPasswordActual.Focus();
                return;
            }

            if (string.IsNullOrEmpty(passwordNuevo))
            {
                lblError.Text = "Ingresa la nueva contrasena";
                txtPasswordNuevo.Focus();
                return;
            }

            if (passwordNuevo.Length < 6)
            {
                lblError.Text = "La nueva contrasena debe tener al menos 6 caracteres";
                txtPasswordNuevo.Focus();
                return;
            }

            if (passwordNuevo != confirmPassword)
            {
                lblError.Text = "Las contrasenas no coinciden";
                txtConfirmPassword.Clear();
                txtConfirmPassword.Focus();
                return;
            }

            bool exito = _authController.CambiarPassword(
                SesionActiva.Instance.Id_Usuario,
                passwordActual,
                passwordNuevo);

            if (exito)
            {
                MessageBox.Show(
                    "Contrasena cambiada exitosamente",
                    "Exito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                lblError.Text = "Contrasena actual incorrecta";
                txtPasswordActual.Clear();
                txtPasswordActual.Focus();
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}