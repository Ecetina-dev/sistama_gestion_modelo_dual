using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormProyectos : Form
    {
        private ProyectoController controller = new ProyectoController();
        private bool isEditing = false;
        private bool isNewRecord = false;

        public FormProyectos()
        {
            InitializeComponent();
            CargarProyectos();
        }

        private void CargarProyectos()
        {
            try
            {
                var proyectos = controller.Read();
                dgvProyectos.DataSource = proyectos;
                if (dgvProyectos.Columns.Count > 0)
                {
                    if (dgvProyectos.Columns.Contains("Id_Proyecto")) dgvProyectos.Columns["Id_Proyecto"].Visible = false;
                    if (dgvProyectos.Columns.Contains("Created_At")) dgvProyectos.Columns["Created_At"].Visible = false;
                    if (dgvProyectos.Columns.Contains("Updated_At")) dgvProyectos.Columns["Updated_At"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar proyectos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (dgvProyectos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un proyecto para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            grpDatos.Visible = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProyectos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un proyecto para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show("¿Está seguro de eliminar este proyecto?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvProyectos.SelectedRows[0].Cells["Id_Proyecto"].Value);
                if (controller.Delete(id))
                {
                    MessageBox.Show("Proyecto eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarProyectos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el proyecto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            isEditing = false;
            grpDatos.Visible = false;
            CargarProyectos();
        }

        private void btnVerDetalle_Click(object sender, EventArgs e)
        {
            if (dgvProyectos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un proyecto para ver el detalle.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isNewRecord = false;
            isEditing = false;
            CargarDatosFormulario();
            grpDatos.Visible = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo Nombre es obligatorio.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime? fechaInicio = null;
            DateTime? fechaFin = null;
            int? horasTotales = null;

            if (!string.IsNullOrWhiteSpace(txtFechaInicio.Text))
            {
                DateTime.TryParse(txtFechaInicio.Text, out DateTime fi);
                fechaInicio = fi;
            }
            if (!string.IsNullOrWhiteSpace(txtFechaFin.Text))
            {
                DateTime.TryParse(txtFechaFin.Text, out DateTime ff);
                fechaFin = ff;
            }
            if (!string.IsNullOrWhiteSpace(txtHorasTotales.Text))
            {
                int.TryParse(txtHorasTotales.Text, out int ht);
                horasTotales = ht;
            }

            Proyecto proyecto = new Proyecto
            {
                Nombre = txtNombre.Text.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                Objetivos = string.IsNullOrWhiteSpace(txtObjetivos.Text) ? null : txtObjetivos.Text.Trim(),
                Fecha_Inicio = fechaInicio,
                Fecha_Fin = fechaFin,
                Horas_Totales = horasTotales,
                Status = cmbStatus.SelectedItem?.ToString()
            };

            bool success = isNewRecord ? controller.Create(proyecto) : controller.Update(proyecto);

            if (success)
            {
                MessageBox.Show(isNewRecord ? "Proyecto creado correctamente." : "Proyecto actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isEditing = false;
                isNewRecord = false;
                grpDatos.Visible = false;
                CargarProyectos();
            }
            else
            {
                MessageBox.Show("Error al guardar el proyecto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (dgvProyectos.SelectedRows.Count > 0)
            {
                var row = dgvProyectos.SelectedRows[0];
                var cells = row.Cells;
                txtIdProyecto.Text = cells["Id_Proyecto"]?.Value?.ToString() ?? cells["id_proyecto"]?.Value?.ToString() ?? "";
                txtNombre.Text = cells["Nombre"]?.Value?.ToString() ?? cells["nombre_proyecto"]?.Value?.ToString() ?? "";
                txtDescripcion.Text = cells["Descripcion"]?.Value?.ToString() ?? cells["descripcion"]?.Value?.ToString() ?? "";
                txtObjetivos.Text = cells["Objetivos"]?.Value?.ToString() ?? cells["objetivos"]?.Value?.ToString() ?? "";
                
                var fechaInicioCell = cells["Fecha_Inicio"] ?? cells["fecha_inicio"];
                txtFechaInicio.Text = fechaInicioCell?.Value != null ? Convert.ToDateTime(fechaInicioCell.Value).ToString("yyyy-MM-dd") : "";
                
                var fechaFinCell = cells["Fecha_Fin"] ?? cells["fecha_fin"];
                txtFechaFin.Text = fechaFinCell?.Value != null ? Convert.ToDateTime(fechaFinCell.Value).ToString("yyyy-MM-dd") : "";
                
                txtHorasTotales.Text = cells["Horas_Totales"]?.Value?.ToString() ?? cells["horas_totales"]?.Value?.ToString() ?? "";

                var statusCell = cells["Status"] ?? cells["status_proyecto"];
                string status = statusCell?.Value?.ToString();
                if (!string.IsNullOrEmpty(status))
                {
                    for (int i = 0; i < cmbStatus.Items.Count; i++)
                    {
                        if (cmbStatus.Items[i].ToString() == status)
                        {
                            cmbStatus.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    cmbStatus.SelectedIndex = -1;
                }
            }
        }

        private void LimpiarFormulario()
        {
            txtIdProyecto.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtObjetivos.Text = "";
            txtFechaInicio.Text = "";
            txtFechaFin.Text = "";
            txtHorasTotales.Text = "";
            cmbStatus.SelectedIndex = -1;
        }
    }
}