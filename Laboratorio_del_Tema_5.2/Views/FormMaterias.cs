using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormMaterias : Form
    {
        private MateriaController controller = new MateriaController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool showJoin = false;

        public FormMaterias()
        {
            InitializeComponent();
            CargarMaterias();
        }

        private void CargarMaterias()
        {
            try
            {
                if (showJoin)
                {
                    dgvMaterias.DataSource = controller.GetMateriasConProyectos();
                }
                else
                {
                    var materias = controller.Read();
                    dgvMaterias.DataSource = materias;
                    if (dgvMaterias.Columns.Count > 0)
                    {
                        if (dgvMaterias.Columns.Contains("Id_Materia")) dgvMaterias.Columns["Id_Materia"].Visible = false;
                        if (dgvMaterias.Columns.Contains("Created_At")) dgvMaterias.Columns["Created_At"].Visible = false;
                        if (dgvMaterias.Columns.Contains("Updated_At")) dgvMaterias.Columns["Updated_At"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar materias: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            grpDatos.Visible = true;
            txtClave.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvMaterias.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una materia para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            grpDatos.Visible = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvMaterias.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una materia para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show("¿Está seguro de eliminar esta materia?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvMaterias.SelectedRows[0].Cells["Id_Materia"].Value);
                if (controller.Delete(id))
                {
                    MessageBox.Show("Materia eliminada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarMaterias();
                }
                else
                {
                    MessageBox.Show("Error al eliminar la materia.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            showJoin = false;
            btnVerProyectos.Text = "Ver con Proyectos";
            isEditing = false;
            grpDatos.Visible = false;
            CargarMaterias();
        }

        private void btnVerProyectos_Click(object sender, EventArgs e)
        {
            showJoin = !showJoin;
            btnVerProyectos.Text = showJoin ? "Ver Solo Materias" : "Ver con Proyectos";
            CargarMaterias();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClave.Text) || string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Los campos Clave y Nombre son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int.TryParse(txtCreditos.Text, out int creditos);
            int.TryParse(txtSemestre.Text, out int semestre);
            int.TryParse(txtHorasTeoria.Text, out int horasTeoria);
            int.TryParse(txtHorasPractica.Text, out int horasPractica);

            Materia materia = new Materia
            {
                Clave_Materia = txtClave.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                Creditos = creditos,
                Semestre = semestre,
                Horas_Teoria = horasTeoria,
                Horas_Practica = horasPractica
            };

            bool success = isNewRecord ? controller.Create(materia) : controller.Update(materia);

            if (success)
            {
                MessageBox.Show(isNewRecord ? "Materia creada correctamente." : "Materia actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isEditing = false;
                isNewRecord = false;
                grpDatos.Visible = false;
                showJoin = false;
                btnVerProyectos.Text = "Ver con Proyectos";
                CargarMaterias();
            }
            else
            {
                MessageBox.Show("Error al guardar la materia.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (dgvMaterias.SelectedRows.Count > 0)
            {
                var row = dgvMaterias.SelectedRows[0];
                txtIdMateria.Text = row.Cells["Id_Materia"].Value?.ToString() ?? "";
                txtClave.Text = row.Cells["Clave_Materia"].Value?.ToString() ?? "";
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
                txtDescripcion.Text = row.Cells["Descripcion"].Value?.ToString() ?? "";
                txtCreditos.Text = row.Cells["Creditos"].Value?.ToString() ?? "";
                txtSemestre.Text = row.Cells["Semestre"].Value?.ToString() ?? "";
                txtHorasTeoria.Text = row.Cells["Horas_Teoria"].Value?.ToString() ?? "";
                txtHorasPractica.Text = row.Cells["Horas_Practica"].Value?.ToString() ?? "";
            }
        }

        private void LimpiarFormulario()
        {
            txtIdMateria.Text = "";
            txtClave.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtCreditos.Text = "";
            txtSemestre.Text = "";
            txtHorasTeoria.Text = "";
            txtHorasPractica.Text = "";
        }
    }
}