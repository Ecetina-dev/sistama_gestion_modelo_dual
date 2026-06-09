using System;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Formulario base con metodos comunes y verificacion de permisos.
    /// Todos los Forms que requieren autorizacion deben heredar de este.
    /// </summary>
    public abstract class FormBase : Form
    {
        /// <summary>
        /// Privilegio requerido para acceder al Form.
        /// Si es null, solo requiere estar logueado.
        /// </summary>
        protected virtual string PrivilegioRequerido => null;

        /// <summary>
        /// Indica si solo el admin puede acceder.
        /// </summary>
        protected virtual bool SoloAdmin => false;

        public FormBase()
        {
            // Verificar sesion y permisos al construir el Form
            this.Load += FormBase_Load;
        }

        private void FormBase_Load(object sender, EventArgs e)
        {
            // Verificar que hay sesion activa
            if (SesionActiva.Instance == null || SesionActiva.Instance.Id_Usuario == 0)
            {
                MostrarAdvertencia("No hay sesion activa. Inicia sesion primero.");
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }

            // Verificar permisos
            if (!TieneAcceso())
            {
                MostrarAdvertencia("No tienes permisos para acceder a esta seccion.\n\n" +
                    "Si crees que es un error, contacta al administrador.");
                this.DialogResult = DialogResult.Abort;
                this.Close();
                return;
            }
        }

        /// <summary>
        /// Verifica si el usuario actual tiene acceso a este Form.
        /// </summary>
        private bool TieneAcceso()
        {
            // Admin siempre tiene acceso
            if (SesionActiva.Instance.EsAdmin)
                return true;

            // Si requiere admin solamente
            if (SoloAdmin)
                return false;

            // Verificar privilegio especifico
            if (!string.IsNullOrEmpty(PrivilegioRequerido))
            {
                return SesionActiva.Instance.TienePrivilegio(PrivilegioRequerido);
            }

            return true;
        }

        /// <summary>
        /// Muestra un mensaje de error.
        /// </summary>
        protected void MostrarError(string mensaje, string titulo = "Error")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Muestra un mensaje de exito.
        /// </summary>
        protected void MostrarExito(string mensaje, string titulo = "Exito")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Muestra una advertencia.
        /// </summary>
        protected void MostrarAdvertencia(string mensaje, string titulo = "Advertencia")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Pide confirmacion al usuario.
        /// </summary>
        protected bool Confirmar(string mensaje, string titulo = "Confirmar")
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        /// <summary>
        /// Pide confirmacion con valor por defecto No.
        /// </summary>
        protected bool ConfirmarAccion(string mensaje)
        {
            return MessageBox.Show(mensaje, "Confirmar accion",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        /// <summary>
        /// Metodo abstracto para inicializar datos - implementar en clases derivadas.
        /// </summary>
        protected abstract void InicializarDatos();

        /// <summary>
        /// Metodo abstracto para enlazar controles con datos - implementar en clases derivadas.
        /// </summary>
        protected abstract void EnlazarControles();

        /// <summary>
        /// Limpia los controles de un contenedor.
        /// </summary>
        protected void LimpiarContenedor(Control contenedor)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Clear();
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.SelectedIndex = -1;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.Value = DateTime.Now;
                }
                else if (control.HasChildren)
                {
                    LimpiarContenedor(control);
                }
            }
        }

        /// <summary>
        /// Habilita o deshabilita los controles de un contenedor.
        /// </summary>
        protected void SetControlesEnabled(Control contenedor, bool enabled)
        {
            foreach (Control control in contenedor.Controls)
            {
                if (control is Button) continue;
                control.Enabled = enabled;
            }
        }

        /// <summary>
        /// Verifica si el usuario tiene un privilegio especifico.
        /// </summary>
        protected bool TienePrivilegio(string privilegio)
        {
            return SesionActiva.Instance != null &&
                   SesionActiva.Instance.TienePrivilegio(privilegio);
        }
    }
}
