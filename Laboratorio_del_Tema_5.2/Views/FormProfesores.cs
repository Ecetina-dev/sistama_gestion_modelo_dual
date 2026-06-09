#pragma warning disable CS0414
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormProfesores : Form
    {
        private ProfesorController controller = new ProfesorController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Profesor> _cache = new List<Profesor>();

        // Limites EXACTOS del modelo y BD
        private const int MAX_NO_EMPLEADO = 20;
        private const int EXT_MIN_NO_EMPLEADO = 10;
        private const int MAX_NOMBRE = 100;
        private const int MAX_APELLIDO = 100;
        private const int MAX_EMAIL = 150;
        private const int MAX_TELEFONO = 10;
        private const int EXT_MIN_TELEFONO = 10;
        private const int MAX_DEPARTAMENTO = 100;
        private const int MAX_PUESTO = 100;

        public FormProfesores()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarProfesores();
        }

        private void ConfigurarFormulario()
        {
            // Limitar longitud de campos segun modelo
            txtNoEmpleado.MaxLength = MAX_NO_EMPLEADO;
            txtNombre.MaxLength = MAX_NOMBRE;
            txtApellidoPaterno.MaxLength = MAX_APELLIDO;
            txtApellidoMaterno.MaxLength = MAX_APELLIDO;
            txtEmail.MaxLength = MAX_EMAIL;
            txtTelefono.MaxLength = MAX_TELEFONO;
            txtDepartamento.MaxLength = MAX_DEPARTAMENTO;
            txtPuesto.MaxLength = MAX_PUESTO;

            // Enter y Escape a nivel form
            this.KeyPreview = true;
            this.KeyDown += FormProfesores_KeyDown;

            // Doble click en grid para editar
            dgvProfesores.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Restaurar color al escribir
            EventHandler restaurar = (s, e) => {
                if (s is TextBox txt && txt.BackColor == Color.FromArgb(255, 235, 235))
                    txt.BackColor = Color.FromArgb(248, 250, 252);
            };
            txtNoEmpleado.TextChanged += restaurar;
            txtNombre.TextChanged += restaurar;
            txtApellidoPaterno.TextChanged += restaurar;
            txtApellidoMaterno.TextChanged += restaurar;
            txtEmail.TextChanged += restaurar;
            txtTelefono.TextChanged += restaurar;
            txtDepartamento.TextChanged += restaurar;
            txtPuesto.TextChanged += restaurar;

            // Enter navega al siguiente campo
            ConfigurarEnterSgteCampo(txtNoEmpleado, txtNombre);
            ConfigurarEnterSgteCampo(txtNombre, txtApellidoPaterno);
            ConfigurarEnterSgteCampo(txtApellidoPaterno, txtApellidoMaterno);
            ConfigurarEnterSgteCampo(txtApellidoMaterno, txtEmail);
            ConfigurarEnterSgteCampo(txtEmail, txtTelefono);
            ConfigurarEnterSgteCampo(txtTelefono, txtDepartamento);
            ConfigurarEnterSgteCampo(txtDepartamento, txtPuesto);
            ConfigurarEnterSgteCampo(txtPuesto, btnGuardar);
            // Desde btnGuardar con Enter tambien guarda
            btnGuardar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !isLoading)
                {
                    e.SuppressKeyPress = true;
                    btnGuardar_Click(s, e);
                }
            };

            // Tab index
            txtNoEmpleado.TabIndex = 0;
            txtNombre.TabIndex = 1;
            txtApellidoPaterno.TabIndex = 2;
            txtApellidoMaterno.TabIndex = 3;
            txtEmail.TabIndex = 4;
            txtTelefono.TabIndex = 5;
            txtDepartamento.TabIndex = 6;
            txtPuesto.TabIndex = 7;
            btnGuardar.TabIndex = 8;
            btnCancelar.TabIndex = 9;
        }

        // Enter sgte campo, Escape = cancelar
        private void FormProfesores_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && panelCardDatos.Visible)
            {
                e.SuppressKeyPress = true;
                btnCancelar_Click(sender, e);
            }
        }

        private void ConfigurarEnterSgteCampo(TextBox actual, Control siguiente)
        {
            actual.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    siguiente.Focus();
                }
            };
        }

        // ==================== CARGA DE DATOS ====================

        private void CargarProfesores()
        {
            try
            {
                _cache = controller.Read() ?? new List<Profesor>();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void CargarProfesoresConProyectos()
        {
            try
            {
                dgvProfesores.DataSource = controller.GetProfesoresConProyectos();
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            var fuente = string.IsNullOrEmpty(filtro)
                ? _cache
                : _cache.Where(p =>
                    (p.No_Empleado?.ToLower().Contains(filtro) ?? false) ||
                    (p.Nombre?.ToLower().Contains(filtro) ?? false) ||
                    (p.Apellido_Paterno?.ToLower().Contains(filtro) ?? false) ||
                    (p.Apellido_Materno?.ToLower().Contains(filtro) ?? false) ||
                    (p.Email?.ToLower().Contains(filtro) ?? false) ||
                    (p.Departamento?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            dgvProfesores.DataSource = null;
            dgvProfesores.DataSource = fuente;
            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            if (dgvProfesores.Columns.Count == 0) return;

            // Ocultar columnas tecnicas
            var ocultar = new[] { "Id_Profesor", "Created_At", "Updated_At", "Status_Profesor" };
            foreach (var col in ocultar)
                if (dgvProfesores.Columns.Contains(col))
                    dgvProfesores.Columns[col].Visible = false;

            // Headers legibles
            var headers = new Dictionary<string, string>
            {
                ["No_Empleado"] = "No. Empleado",
                ["Nombre"] = "Nombre",
                ["Apellido_Paterno"] = "Ap. Paterno",
                ["Apellido_Materno"] = "Ap. Materno",
                ["Email"] = "Email",
                ["Telefono"] = "Telefono",
                ["Departamento"] = "Departamento",
                ["Puesto"] = "Puesto"
            };
            foreach (var kv in headers)
                if (dgvProfesores.Columns.Contains(kv.Key))
                    dgvProfesores.Columns[kv.Key].HeaderText = kv.Value;

            // Reordenar
            string[] orden = { "No_Empleado", "Nombre", "Apellido_Paterno", "Apellido_Materno",
                               "Email", "Telefono", "Departamento", "Puesto" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvProfesores.Columns.Contains(col))
                    dgvProfesores.Columns[col].DisplayIndex = idx++;
        }

        // ==================== NAVEGACION ====================

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltroBusqueda();
        }

        // ==================== CRUD ====================

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            panelCardDatos.Visible = true;
            txtNoEmpleado.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProfesores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un profesor de la lista.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            panelCardDatos.Visible = true;
            txtNombre.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvProfesores.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un profesor.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombre = $"{dgvProfesores.SelectedRows[0].Cells["Nombre"]?.Value} " +
                         $"{dgvProfesores.SelectedRows[0].Cells["Apellido_Paterno"]?.Value}";

            if (MessageBox.Show($"Eliminar a '{nombre.Trim()}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvProfesores.SelectedRows[0].Cells["Id_Profesor"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarExito("Profesor eliminado.");
                    CargarProfesores();
                }
                else
                    MostrarError("No se pudo eliminar. Puede tener registros asociados.");
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarProfesores();
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
        }

        private void btnVerProyectos_Click(object sender, EventArgs e)
        {
            if (btnVerProyectos.Text.Contains("Proyectos"))
            {
                CargarProfesoresConProyectos();
                btnVerProyectos.Text = "\U0001F441\uFE0F  Solo Profesores";
            }
            else
            {
                CargarProfesores();
                btnVerProyectos.Text = "\U0001F441\uFE0F  Ver Proyectos";
            }
        }

        // ==================== GUARDAR ====================

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            // 1. Validar
            string error = ValidarFormulario();
            if (error != null)
            {
                MostrarAdvertencia(error);
                return;
            }

            // 2. Duplicado No_Empleado
            int? idExcluir = isNewRecord ? null : (int?)Convert.ToInt32(txtIdProfesor.Text);
            if (controller.ExisteNoEmpleado(txtNoEmpleado.Text.Trim(), idExcluir))
            {
                MarcarError(txtNoEmpleado);
                MostrarAdvertencia("El No. Empleado '" + txtNoEmpleado.Text.Trim() + "' ya existe.");
                txtNoEmpleado.Focus();
                return;
            }

            // 3. Construir objeto
            var profesor = new Profesor
            {
                No_Empleado = txtNoEmpleado.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido_Paterno = txtApellidoPaterno.Text.Trim(),
                Apellido_Materno = NullIfEmpty(txtApellidoMaterno.Text),
                Email = NullIfEmpty(txtEmail.Text),
                Telefono = NullIfEmpty(txtTelefono.Text),
                Departamento = NullIfEmpty(txtDepartamento.Text),
                Puesto = NullIfEmpty(txtPuesto.Text),
                Status_Profesor = Estatus.ProfesorActivo,
            };

            // 4. Loading state
            isLoading = true;
            btnGuardar.Enabled = false;
            btnGuardar.Text = "Guardando...";
            Cursor = Cursors.WaitCursor;

            // 5. Guardar
            bool success;
            try
            {
                if (isNewRecord)
                    success = await System.Threading.Tasks.Task.Run(() => controller.Create(profesor));
                else
                {
                    profesor.Id_Profesor = Convert.ToInt32(txtIdProfesor.Text);
                    success = await System.Threading.Tasks.Task.Run(() => controller.Update(profesor));
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
                return;
            }
            finally
            {
                isLoading = false;
                btnGuardar.Enabled = true;
                btnGuardar.Text = "\u2714\uFE0F  Guardar";
                Cursor = Cursors.Default;
            }

            if (success)
            {
                MostrarExito("Profesor guardado.");
                isEditing = false;
                isNewRecord = false;
                panelCardDatos.Visible = false;
                CargarProfesores();
            }
            else
                MostrarError("Error al guardar. Verifica los datos.");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (HayCambiosSinGuardar())
            {
                var r = MessageBox.Show("Hay cambios sin guardar.\n\n\u00bfDescartarlos?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
            LimpiarFormulario();
            RestaurarColores();
        }

        // ==================== VALIDACION ====================

        private string ValidarFormulario()
        {
            RestaurarColores();

            // No. Empleado
            string ne = txtNoEmpleado.Text.Trim();
            if (string.IsNullOrEmpty(ne))
            {
                MarcarError(txtNoEmpleado);
                return "El numero de empleado es obligatorio.";
            }
            if (ne.Length < EXT_MIN_NO_EMPLEADO)
            {
                MarcarError(txtNoEmpleado);
                return $"El No. Empleado debe tener {EXT_MIN_NO_EMPLEADO} digitos.";
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(ne, @"^\d{10}$"))
            {
                MarcarError(txtNoEmpleado);
                return "El No. Empleado debe tener 10 digitos (ej: 1234567890).";
            }

            // Nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MarcarError(txtNombre);
                return "El nombre es obligatorio.";
            }
            if (txtNombre.Text.Trim().Length < 2)
            {
                MarcarError(txtNombre);
                return "El nombre debe tener al menos 2 caracteres.";
            }

            // Ap. Paterno
            if (string.IsNullOrWhiteSpace(txtApellidoPaterno.Text))
            {
                MarcarError(txtApellidoPaterno);
                return "El apellido paterno es obligatorio.";
            }

            // Email
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MarcarError(txtEmail);
                    return "Formato de email invalido (ej: usuario@dominio.com).";
                }
            }

            // Telefono
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                string tel = txtTelefono.Text.Trim();
                string soloDigitos = new string(tel.Where(char.IsDigit).ToArray());
                if (soloDigitos.Length != EXT_MIN_TELEFONO)
                {
                    MarcarError(txtTelefono);
                    return $"El telefono debe tener {EXT_MIN_TELEFONO} digitos (ej: 5551234567).";
                }
            }

            return null;
        }

        private bool HayCambiosSinGuardar()
        {
            return !string.IsNullOrWhiteSpace(txtNoEmpleado.Text) ||
                   !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                   !string.IsNullOrWhiteSpace(txtApellidoPaterno.Text);
        }

        private void MarcarError(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(255, 235, 235);
            txt.Focus();
        }

        private void RestaurarColores()
        {
            foreach (var txt in new[] { txtNoEmpleado, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono, txtDepartamento, txtPuesto })
                txt.BackColor = Color.FromArgb(248, 250, 252);
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvProfesores.SelectedRows.Count == 0) return;

            int idProfesor = (int)dgvProfesores.SelectedRows[0].Cells["Id_Profesor"].Value;

            // Buscar en cache (tiene TODOS los datos)
            var p = _cache.FirstOrDefault(x => x.Id_Profesor == idProfesor);
            if (p == null)
            {
                MostrarAdvertencia("No se pudieron cargar los datos completos del profesor.");
                return;
            }

            txtIdProfesor.Text = p.Id_Profesor.ToString();
            txtNoEmpleado.Text = p.No_Empleado ?? "";
            txtNombre.Text = p.Nombre ?? "";
            txtApellidoPaterno.Text = p.Apellido_Paterno ?? "";
            txtApellidoMaterno.Text = p.Apellido_Materno ?? "";
            txtEmail.Text = p.Email ?? "";
            txtTelefono.Text = p.Telefono ?? "";
            txtDepartamento.Text = p.Departamento ?? "";
            txtPuesto.Text = p.Puesto ?? "";
        }

        private void LimpiarFormulario()
        {
            foreach (var txt in new[] { txtIdProfesor, txtNoEmpleado, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono, txtDepartamento, txtPuesto })
                txt.Clear();
            RestaurarColores();
        }

        // ==================== HELPERS ====================

        private string NullIfEmpty(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void MostrarError(string msg) =>
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void MostrarAdvertencia(string msg) =>
            MessageBox.Show(msg, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void MostrarExito(string msg) =>
            MessageBox.Show(msg, "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
