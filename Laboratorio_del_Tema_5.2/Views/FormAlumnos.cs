using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Views
{
    /// <summary>
    /// Formulario para la gestion de alumnos.
    /// </summary>
    public partial class FormAlumnos : Form
    {
        private AlumnoController controller = new AlumnoController();
        private bool isEditing = false;
        private bool isNewRecord = false;

        public FormAlumnos()
        {
            InitializeComponent();
            CargarAlumnos();
        }

        /// <summary>
        /// Carga la lista de alumnos en el DataGridView.
        /// </summary>
        private void CargarAlumnos()
        {
            try
            {
                List<Alumno> alumnos = controller.Read();
                dgvAlumnos.DataSource = alumnos;

                if (dgvAlumnos.Columns.Count > 0)
                {
                    if (dgvAlumnos.Columns.Contains("Id_Alumno")) dgvAlumnos.Columns["Id_Alumno"].Visible = false;
                    if (dgvAlumnos.Columns.Contains("Created_At")) dgvAlumnos.Columns["Created_At"].Visible = false;
                    if (dgvAlumnos.Columns.Contains("Updated_At")) dgvAlumnos.Columns["Updated_At"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar alumnos: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Carga la lista de alumnos con su empresa asignada.
        /// </summary>
        private void CargarAlumnosConEmpresa()
        {
            try
            {
                List<AlumnoConEmpresa> alumnos = controller.GetAlumnosConEmpresa();
                dgvAlumnos.DataSource = alumnos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar alumnos con empresa: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvAlumnos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count > 0 && !isEditing)
            {
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            grpDatos.Visible = true;
            txtNoControl.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un alumno para editar.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            grpDatos.Visible = true;
            txtNoControl.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un alumno para eliminar.",
                    "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "¿Está seguro de eliminar este alumno?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int idAlumno = Convert.ToInt32(dgvAlumnos.SelectedRows[0].Cells["Id_Alumno"].Value);
                if (controller.Delete(idAlumno))
                {
                    MessageBox.Show("Alumno eliminado correctamente.",
                        "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAlumnos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el alumno.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarAlumnos();
            isEditing = false;
            isNewRecord = false;
            grpDatos.Visible = false;
        }

        private void btnVerEmpresas_Click(object sender, EventArgs e)
        {
            if (btnVerEmpresas.Text == "Ver con Empresas")
            {
                CargarAlumnosConEmpresa();
                btnVerEmpresas.Text = "Ver Solo Alumnos";
            }
            else
            {
                CargarAlumnos();
                btnVerEmpresas.Text = "Ver con Empresas";
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNoControl.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellidoPaterno.Text))
            {
                MessageBox.Show("Los campos marcados con * son obligatorios.",
                    "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime? fechaNac = null;
            if (!string.IsNullOrWhiteSpace(txtFechaNacimiento.Text))
            {
                DateTime.TryParse(txtFechaNacimiento.Text, out DateTime fecha);
                fechaNac = fecha;
            }

            Alumno alumno = new Alumno
            {
                No_Control = txtNoControl.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido_Paterno = txtApellidoPaterno.Text.Trim(),
                Apellido_Materno = string.IsNullOrWhiteSpace(txtApellidoMaterno.Text) ? null : txtApellidoMaterno.Text.Trim(),
                Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(),
                Fecha_Nacimiento = fechaNac
            };

            bool success = false;

            if (isNewRecord)
            {
                success = controller.Create(alumno);
            }
            else
            {
                alumno.Id_Alumno = Convert.ToInt32(txtIdAlumno.Text);
                success = controller.Update(alumno);
            }

            if (success)
            {
                MessageBox.Show(
                    isNewRecord ? "Alumno creado correctamente." : "Alumno actualizado correctamente.",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                isEditing = false;
                isNewRecord = false;
                grpDatos.Visible = false;
                CargarAlumnos();
            }
            else
            {
                MessageBox.Show("Error al guardar el alumno.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// Carga los datos del alumno seleccionado en el formulario.
        /// </summary>
        private void CargarDatosFormulario()
        {
            if (dgvAlumnos.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvAlumnos.SelectedRows[0];
                var cells = row.Cells;

                txtIdAlumno.Text = cells["Id_Alumno"]?.Value?.ToString() ?? "";
                txtNoControl.Text = cells["No_Control"]?.Value?.ToString() ?? "";
                txtNombre.Text = cells["Nombre"]?.Value?.ToString() ?? "";
                txtApellidoPaterno.Text = cells["Apellido_Paterno"]?.Value?.ToString() ?? "";
                txtApellidoMaterno.Text = cells["Apellido_Materno"]?.Value?.ToString() ?? "";
                txtEmail.Text = cells["Email"]?.Value?.ToString() ?? "";
                txtTelefono.Text = cells["Telefono"]?.Value?.ToString() ?? "";

                var fechaNacCell = cells["Fecha_Nacimiento"] ?? cells["fecha_nacimiento"];
                if (fechaNacCell?.Value != null && fechaNacCell.Value != DBNull.Value)
                {
                    txtFechaNacimiento.Text = Convert.ToDateTime(fechaNacCell.Value).ToString("yyyy-MM-dd");
                }
                else
                {
                    txtFechaNacimiento.Text = "";
                }
            }
        }

        /// <summary>
        /// Limpia todos los campos del formulario.
        /// </summary>
        private void LimpiarFormulario()
        {
            txtIdAlumno.Text = "";
            txtNoControl.Text = "";
            txtNombre.Text = "";
            txtApellidoPaterno.Text = "";
            txtApellidoMaterno.Text = "";
            txtEmail.Text = "";
            txtTelefono.Text = "";
            txtFechaNacimiento.Text = "";
        }
    }
}