using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormTemas : Form
    {
        private TemaController controller = new TemaController();
        private MateriaController materiaController = new MateriaController();
        private bool isEditing = false;
        private bool isNewRecord = false;

        public FormTemas()
        {
            InitializeComponent();
            CargarTemas();
            CargarMaterias();
        }

        private void CargarMaterias()
        {
            try
            {
                var materias = materiaController.Read();
                cmbMateria.DataSource = materias;
                cmbMateria.DisplayMember = "Nombre";
                cmbMateria.ValueMember = "Id_Materia";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materias: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarTemas()
        {
            try
            {
                var temas = controller.Read();
                dgvTemas.DataSource = temas;
                if (dgvTemas.Columns.Count > 0)
                {
                    if (dgvTemas.Columns.Contains("Id_Tema")) dgvTemas.Columns["Id_Tema"].Visible = false;
                    if (dgvTemas.Columns.Contains("Created_At")) dgvTemas.Columns["Created_At"].Visible = false;
                    if (dgvTemas.Columns.Contains("Updated_At")) dgvTemas.Columns["Updated_At"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar temas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            grpDatos.Visible = true;
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvTemas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un tema para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            grpDatos.Visible = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTemas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un tema para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show("¿Está seguro de eliminar este tema?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvTemas.SelectedRows[0].Cells["Id_Tema"].Value);
                if (controller.Delete(id))
                {
                    MessageBox.Show("Tema eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarTemas();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el tema.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            isEditing = false;
            grpDatos.Visible = false;
            CargarTemas();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtNumeroTema.Text, out int numeroTema))
            {
                MessageBox.Show("El campo Número de Tema debe ser un número entero.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtHorasEstimadas.Text, out decimal horasEstimadas))
            {
                MessageBox.Show("El campo Horas Estimadas debe ser un número válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Tema tema = new Tema
            {
                Nombre = txtNombre.Text.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                Id_Materia = cmbMateria.SelectedValue != null ? Convert.ToInt32(cmbMateria.SelectedValue) : 0,
                Numero_Tema = numeroTema,
                Horas_Estimadas = horasEstimadas,
                Status_Tema = "activo"
            };

            bool success = isNewRecord ? controller.Create(tema) : controller.Update(tema);

            if (success)
            {
                MessageBox.Show(isNewRecord ? "Tema creado correctamente." : "Tema actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isEditing = false;
                isNewRecord = false;
                grpDatos.Visible = false;
                CargarTemas();
            }
            else
            {
                MessageBox.Show("Error al guardar el tema.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            isEditing = false;
            isNewRecord = false;
            grpDatos.Visible = false;
            LimpiarFormulario();
        }

        private void CargarDatosFormulario()
        {
            if (dgvTemas.SelectedRows.Count > 0)
            {
                var row = dgvTemas.SelectedRows[0];
                var cells = row.Cells;
                txtIdTema.Text = cells["Id_Tema"]?.Value?.ToString() ?? cells["id_tema"]?.Value?.ToString() ?? "";
                txtNumeroTema.Text = cells["Numero_Tema"]?.Value?.ToString() ?? cells["numero_tema"]?.Value?.ToString() ?? "";
                txtNombre.Text = cells["Nombre"]?.Value?.ToString() ?? cells["nombre"]?.Value?.ToString() ?? "";
                txtDescripcion.Text = cells["Descripcion"]?.Value?.ToString() ?? cells["descripcion"]?.Value?.ToString() ?? "";
                txtHorasEstimadas.Text = cells["Horas_Estimadas"]?.Value?.ToString() ?? cells["horas_estimadas"]?.Value?.ToString() ?? "";

                var idMateriaCell = cells["Id_Materia"] ?? cells["id_materia"];
                if (idMateriaCell?.Value != null && idMateriaCell.Value != DBNull.Value)
                {
                    cmbMateria.SelectedValue = Convert.ToInt32(idMateriaCell.Value);
                }
                else
                {
                    cmbMateria.SelectedIndex = -1;
                }
            }
        }

        private void LimpiarFormulario()
        {
            txtIdTema.Text = "";
            txtNumeroTema.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtHorasEstimadas.Text = "";
            cmbMateria.SelectedIndex = -1;
        }
    }
}