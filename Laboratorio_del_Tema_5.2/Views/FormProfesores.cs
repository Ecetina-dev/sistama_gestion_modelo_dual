using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormProfesores : Form
    {
        private ProfesorController controller = new ProfesorController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool showJoin = false;

        public FormProfesores()
        {
            InitializeComponent();
            CargarProfesores();
        }

        private void CargarProfesores()
        {
            try
            {
                if (showJoin)
                {
                    dgvProfesores.DataSource = controller.GetProfesoresConProyectos();
                }
                else
                {
                    var profesores = controller.Read();
                    dgvProfesores.DataSource = profesores;
                    if (dgvProfesores.Columns.Count > 0)
                    {
                        dgvProfesores.Columns["Id_Profesor"].Visible = false;
                        dgvProfesores.Columns["Created_At"].Visible = false;
                        dgvProfesores.Columns["Updated_At"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar profesores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            grpDatos.Visible = true;
            txtNoEmpleado.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProfesores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un profesor para editar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            grpDatos.Visible = true;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProfesores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un profesor para eliminar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show("¿Está seguro de eliminar este profesor?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                int id = Convert.ToInt32(dgvProfesores.SelectedRows[0].Cells["Id_Profesor"].Value);
                if (controller.Delete(id))
                {
                    MessageBox.Show("Profesor eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarProfesores();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el profesor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            showJoin = false;
            btnVerProyectos.Text = "Ver con Proyectos";
            isEditing = false;
            grpDatos.Visible = false;
            CargarProfesores();
        }

        private void btnVerProyectos_Click(object sender, EventArgs e)
        {
            showJoin = !showJoin;
            btnVerProyectos.Text = showJoin ? "Ver Solo Profesores" : "Ver con Proyectos";
            CargarProfesores();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNoEmpleado.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtApellidoPaterno.Text))
            {
                MessageBox.Show("Los campos No. Empleado, Nombre y Apellido Paterno son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Profesor profesor = new Profesor
            {
                No_Empleado = txtNoEmpleado.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido_Paterno = txtApellidoPaterno.Text.Trim(),
                Apellido_Materno = string.IsNullOrWhiteSpace(txtApellidoMaterno.Text) ? null : txtApellidoMaterno.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Departamento = string.IsNullOrWhiteSpace(txtDepartamento.Text) ? null : txtDepartamento.Text.Trim(),
                Puesto = string.IsNullOrWhiteSpace(txtPuesto.Text) ? null : txtPuesto.Text.Trim()
            };

            bool success = isNewRecord ? controller.Create(profesor) : controller.Update(profesor);

            if (success)
            {
                MessageBox.Show(isNewRecord ? "Profesor creado correctamente." : "Profesor actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                isEditing = false;
                isNewRecord = false;
                grpDatos.Visible = false;
                showJoin = false;
                btnVerProyectos.Text = "Ver con Proyectos";
                CargarProfesores();
            }
            else
            {
                MessageBox.Show("Error al guardar el profesor.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (dgvProfesores.SelectedRows.Count > 0)
            {
                var row = dgvProfesores.SelectedRows[0];
                txtIdProfesor.Text = row.Cells["Id_Profesor"].Value?.ToString() ?? "";
                txtNoEmpleado.Text = row.Cells["No_Empleado"].Value?.ToString() ?? "";
                txtNombre.Text = row.Cells["Nombre"].Value?.ToString() ?? "";
                txtApellidoPaterno.Text = row.Cells["Apellido_Paterno"].Value?.ToString() ?? "";
                txtApellidoMaterno.Text = row.Cells["Apellido_Materno"].Value?.ToString() ?? "";
                txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
                txtTelefono.Text = row.Cells["Telefono"].Value?.ToString() ?? "";
                txtDepartamento.Text = row.Cells["Departamento"].Value?.ToString() ?? "";
                txtPuesto.Text = row.Cells["Puesto"].Value?.ToString() ?? "";
            }
        }

        private void LimpiarFormulario()
        {
            txtIdProfesor.Text = "";
            txtNoEmpleado.Text = "";
            txtNombre.Text = "";
            txtApellidoPaterno.Text = "";
            txtApellidoMaterno.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtDepartamento.Text = "";
            txtPuesto.Text = "";
        }
    }
}