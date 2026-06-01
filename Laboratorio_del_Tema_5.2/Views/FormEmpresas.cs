using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    /// <summary>
    /// Formulario para la gestion de empresas.
    /// </summary>
    public partial class FormEmpresas : Form
    {
        private EmpresaController controller = new EmpresaController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool showJoin = false;

        public FormEmpresas()
        {
            InitializeComponent();
            CargarEmpresas();
        }

        /// <summary>
        /// Carga la lista de empresas en el DataGridView.
        /// </summary>
        private void CargarEmpresas()
        {
            try
            {
                if (showJoin)
                {
                    var empresas = controller.GetEmpresasConAlumnos();
                    dgvEmpresas.DataSource = empresas;
                }
                else
                {
                    var empresas = controller.Read();
                    dgvEmpresas.DataSource = empresas;
                    
                    if (dgvEmpresas.Columns.Count > 0)
                    {
                        if (dgvEmpresas.Columns.Contains("Id_Empresa")) dgvEmpresas.Columns["Id_Empresa"].Visible = false;
                        if (dgvEmpresas.Columns.Contains("Created_At")) dgvEmpresas.Columns["Created_At"].Visible = false;
                        if (dgvEmpresas.Columns.Contains("Updated_At")) dgvEmpresas.Columns["Updated_At"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empresas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            grpDatos.Visible = true;
            txtNombreComercial.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvEmpresas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una empresa para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            grpDatos.Visible = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvEmpresas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una empresa para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("¿Está seguro de eliminar esta empresa?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvEmpresas.SelectedRows[0].Cells["id_empresa"].Value);
                if (controller.Delete(id))
                {
                    MessageBox.Show("Empresa eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarEmpresas();
                }
                else
                {
                    MessageBox.Show("Error al eliminar la empresa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            showJoin = false;
            btnVerAlumnos.Text = "Ver con Alumnos";
            isEditing = false;
            isNewRecord = false;
            grpDatos.Visible = false;
            CargarEmpresas();
        }

        private void btnVerAlumnos_Click(object sender, EventArgs e)
        {
            if (btnVerAlumnos.Text == "Ver con Alumnos")
            {
                showJoin = true;
                btnVerAlumnos.Text = "Ver Solo Empresas";
            }
            else
            {
                showJoin = false;
                btnVerAlumnos.Text = "Ver con Alumnos";
            }
            CargarEmpresas();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombreComercial.Text))
            {
                MessageBox.Show("El nombre comercial es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Empresa empresa = new Empresa
            {
                Id_Empresa = isNewRecord ? 0 : Convert.ToInt32(txtIdEmpresa.Text),
                Nombre_Comercial = txtNombreComercial.Text.Trim(),
                Razon_Social = string.IsNullOrWhiteSpace(txtRazonSocial.Text) ? null : txtRazonSocial.Text.Trim(),
                RFC = string.IsNullOrWhiteSpace(txtRFC.Text) ? null : txtRFC.Text.Trim(),
                Direccion = string.IsNullOrWhiteSpace(txtDireccion.Text) ? null : txtDireccion.Text.Trim(),
                Ciudad = string.IsNullOrWhiteSpace(txtCiudad.Text) ? null : txtCiudad.Text.Trim(),
                Estado = string.IsNullOrWhiteSpace(txtEstado.Text) ? null : txtEstado.Text.Trim(),
                CP = string.IsNullOrWhiteSpace(txtCP.Text) ? null : txtCP.Text.Trim(),
                Telefono_Empresa = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Email_Empresa = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Nombre_Contacto = string.IsNullOrWhiteSpace(txtContacto.Text) ? null : txtContacto.Text.Trim(),
                Puesto_Contacto = string.IsNullOrWhiteSpace(txtPuesto.Text) ? null : txtPuesto.Text.Trim()
            };

            bool success = isNewRecord ? controller.Create(empresa) : controller.Update(empresa);

            if (success)
            {
                MessageBox.Show(isNewRecord ? "Empresa creada correctamente." : "Empresa actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isEditing = false;
                isNewRecord = false;
                grpDatos.Visible = false;
                showJoin = false;
                btnVerAlumnos.Text = "Ver con Alumnos";
                CargarEmpresas();
            }
            else
            {
                MessageBox.Show("Error al guardar la empresa.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            isEditing = false;
            isNewRecord = false;
            grpDatos.Visible = false;
            LimpiarFormulario();
        }

        /// <summary>
        /// Carga los datos de la empresa seleccionada en el formulario.
        /// </summary>
        private void CargarDatosFormulario()
        {
            if (dgvEmpresas.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvEmpresas.SelectedRows[0];
                var cells = row.Cells;

                txtIdEmpresa.Text = cells["id_empresa"]?.Value?.ToString() ?? "";
                txtNombreComercial.Text = cells["nombre_comercial"]?.Value?.ToString() ?? "";
                txtRazonSocial.Text = cells["razon_social"]?.Value?.ToString() ?? "";
                txtRFC.Text = cells["rfc"]?.Value?.ToString() ?? "";
                txtDireccion.Text = cells["direccion"]?.Value?.ToString() ?? "";
                txtCiudad.Text = cells["ciudad"]?.Value?.ToString() ?? "";
                txtEstado.Text = cells["estado"]?.Value?.ToString() ?? "";
                txtCP.Text = cells["cp"]?.Value?.ToString() ?? "";
                txtTelefono.Text = cells["telefono_empresa"]?.Value?.ToString() ?? "";
                txtEmail.Text = cells["email_empresa"]?.Value?.ToString() ?? "";
                txtContacto.Text = cells["nombre_contacto"]?.Value?.ToString() ?? "";
                txtPuesto.Text = cells["puesto_contacto"]?.Value?.ToString() ?? "";
            }
        }

        /// <summary>
        /// Limpia todos los campos del formulario.
        /// </summary>
        private void LimpiarFormulario()
        {
            txtIdEmpresa.Text = "";
            txtNombreComercial.Text = "";
            txtRazonSocial.Text = "";
            txtRFC.Text = "";
            txtDireccion.Text = "";
            txtCiudad.Text = "";
            txtEstado.Text = "";
            txtCP.Text = "";
            txtTelefono.Text = "";
            txtEmail.Text = "";
            txtContacto.Text = "";
            txtPuesto.Text = "";
        }
    }
}