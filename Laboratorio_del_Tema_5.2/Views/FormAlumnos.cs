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
    public partial class FormAlumnos : Form
    {
        private AlumnoController controller = new AlumnoController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Alumno> _alumnosCache = new List<Alumno>();

        // Limites EXACTOS del modelo y BD
        private const int MAX_NO_CONTROL = 8;
        private const int EXT_MIN_NO_CONTROL = 8;
        private const int MAX_NOMBRE = 100;
        private const int MAX_APELLIDO = 100;
        private const int MAX_EMAIL = 150;
        private const int MAX_TELEFONO = 10;
        private const int EXT_MIN_TELEFONO = 10;

        public FormAlumnos()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarAlumnos();
        }

        private void ConfigurarFormulario()
        {
            // Limitar longitud de campos segun modelo
            txtNoControl.MaxLength = MAX_NO_CONTROL;
            txtNombre.MaxLength = MAX_NOMBRE;
            txtApellidoPaterno.MaxLength = MAX_APELLIDO;
            txtApellidoMaterno.MaxLength = MAX_APELLIDO;
            txtEmail.MaxLength = MAX_EMAIL;
            txtTelefono.MaxLength = MAX_TELEFONO;
            txtFechaNacimiento.MaxLength = 10;

            // Enter y Escape a nivel form
            this.KeyPreview = true;
            this.KeyDown += FormAlumnos_KeyDown;

            // Doble click en grid para editar
            dgvAlumnos.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Restaurar color al escribir
            EventHandler restaurar = (s, e) => {
                if (s is TextBox txt && txt.BackColor == Color.FromArgb(255, 235, 235))
                    txt.BackColor = Color.FromArgb(248, 250, 252);
            };
            txtNoControl.TextChanged += restaurar;
            txtNombre.TextChanged += restaurar;
            txtApellidoPaterno.TextChanged += restaurar;
            txtApellidoMaterno.TextChanged += restaurar;
            txtEmail.TextChanged += restaurar;
            txtTelefono.TextChanged += restaurar;
            txtFechaNacimiento.TextChanged += restaurar;

            // Enter navega al siguiente campo
            ConfigurarEnterSgteCampo(txtNoControl, txtNombre);
            ConfigurarEnterSgteCampo(txtNombre, txtApellidoPaterno);
            ConfigurarEnterSgteCampo(txtApellidoPaterno, txtApellidoMaterno);
            ConfigurarEnterSgteCampo(txtApellidoMaterno, txtEmail);
            ConfigurarEnterSgteCampo(txtEmail, txtTelefono);
            ConfigurarEnterSgteCampo(txtTelefono, txtFechaNacimiento);
            ConfigurarEnterSgteCampo(txtFechaNacimiento, btnGuardar);
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
            txtNoControl.TabIndex = 0;
            txtNombre.TabIndex = 1;
            txtApellidoPaterno.TabIndex = 2;
            txtApellidoMaterno.TabIndex = 3;
            txtEmail.TabIndex = 4;
            txtTelefono.TabIndex = 5;
            txtFechaNacimiento.TabIndex = 6;
            btnGuardar.TabIndex = 7;
            btnCancelar.TabIndex = 8;
        }

        // Enter sgte campo, Escape = cancelar
        private void FormAlumnos_KeyDown(object sender, KeyEventArgs e)
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

        private void CargarAlumnos()
        {
            try
            {
                _alumnosCache = controller.Read() ?? new List<Alumno>();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void CargarAlumnosConEmpresa()
        {
            try
            {
                dgvAlumnos.DataSource = controller.GetAlumnosConEmpresa();
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
                ? _alumnosCache
                : _alumnosCache.Where(a =>
                    (a.No_Control?.ToLower().Contains(filtro) ?? false) ||
                    (a.Nombre?.ToLower().Contains(filtro) ?? false) ||
                    (a.Apellido_Paterno?.ToLower().Contains(filtro) ?? false) ||
                    (a.Apellido_Materno?.ToLower().Contains(filtro) ?? false) ||
                    (a.Email?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            dgvAlumnos.DataSource = null;
            dgvAlumnos.DataSource = fuente;
            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            if (dgvAlumnos.Columns.Count == 0) return;

            // Ocultar columnas tecnicas
            var ocultar = new[] { "Id_Alumno", "Created_At", "Updated_At", "Status_Alumno",
                                  "Fecha_Nacimiento", "Password", "PasswordHash", "Privilegios" };
            foreach (var col in ocultar)
                if (dgvAlumnos.Columns.Contains(col))
                    dgvAlumnos.Columns[col].Visible = false;

            // Headers legibles
            var headers = new Dictionary<string, string>
            {
                ["No_Control"] = "No. Control",
                ["Nombre"] = "Nombre",
                ["Apellido_Paterno"] = "Ap. Paterno",
                ["Apellido_Materno"] = "Ap. Materno",
                ["Email"] = "Email",
                ["Telefono"] = "Telefono"
            };
            foreach (var kv in headers)
                if (dgvAlumnos.Columns.Contains(kv.Key))
                    dgvAlumnos.Columns[kv.Key].HeaderText = kv.Value;

            // Reordenar
            string[] orden = { "No_Control", "Nombre", "Apellido_Paterno", "Apellido_Materno", "Email", "Telefono" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvAlumnos.Columns.Contains(col))
                    dgvAlumnos.Columns[col].DisplayIndex = idx++;
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
            HabilitarNoControl(true);
            panelCardDatos.Visible = true;
            txtNoControl.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un alumno de la lista.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isNewRecord = false;
            isEditing = true;
            HabilitarNoControl(false); // No cambiar No.Control al editar
            CargarDatosFormulario();
            panelCardDatos.Visible = true;
            txtNombre.Focus();
        }

        private void HabilitarNoControl(bool habilitado)
        {
            txtNoControl.Enabled = habilitado;
            txtNoControl.ReadOnly = !habilitado;
            txtNoControl.BackColor = habilitado
                ? Color.FromArgb(248, 250, 252)
                : Color.FromArgb(235, 235, 235); // gris si readonly
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un alumno.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombre = $"{dgvAlumnos.SelectedRows[0].Cells["Nombre"]?.Value} " +
                         $"{dgvAlumnos.SelectedRows[0].Cells["Apellido_Paterno"]?.Value}";

            if (MessageBox.Show($"Eliminar a '{nombre.Trim()}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvAlumnos.SelectedRows[0].Cells["Id_Alumno"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarExito("Alumno eliminado.");
                    CargarAlumnos();
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
            CargarAlumnos();
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
        }

        private void btnVerEmpresas_Click(object sender, EventArgs e)
        {
            if (btnVerEmpresas.Text.Contains("Empresas"))
            {
                CargarAlumnosConEmpresa();
                btnVerEmpresas.Text = "\U0001F441\uFE0F  Solo Alumnos";
            }
            else
            {
                CargarAlumnos();
                btnVerEmpresas.Text = "\U0001F441\uFE0F  Ver Empresas";
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

            // 2. Duplicado
            int? idExcluir = isNewRecord ? null : (int?)Convert.ToInt32(txtIdAlumno.Text);
            if (controller.ExisteNoControl(txtNoControl.Text.Trim(), idExcluir))
            {
                MarcarError(txtNoControl);
                MostrarAdvertencia("El No. Control '" + txtNoControl.Text.Trim() + "' ya existe.");
                txtNoControl.Focus();
                return;
            }

            // 3. Construir objeto
            var alumno = new Alumno
            {
                No_Control = txtNoControl.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido_Paterno = txtApellidoPaterno.Text.Trim(),
                Apellido_Materno = NullIfEmpty(txtApellidoMaterno.Text),
                Email = NullIfEmpty(txtEmail.Text),
                Telefono = NullIfEmpty(txtTelefono.Text),
                Fecha_Nacimiento = ParsearFecha(txtFechaNacimiento.Text),
                Status_Alumno = Estatus.AlumnoActivo,
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
                    success = await System.Threading.Tasks.Task.Run(() => controller.Create(alumno));
                else
                {
                    alumno.Id_Alumno = Convert.ToInt32(txtIdAlumno.Text);
                    success = await System.Threading.Tasks.Task.Run(() => controller.Update(alumno));
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
                MostrarExito("Alumno guardado.");
                isEditing = false;
                isNewRecord = false;
                panelCardDatos.Visible = false;
                CargarAlumnos();
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

            // No. Control
            string nc = txtNoControl.Text.Trim();
            if (string.IsNullOrEmpty(nc))
            {
                MarcarError(txtNoControl);
                return "El numero de control es obligatorio.";
            }
            if (nc.Length < EXT_MIN_NO_CONTROL)
            {
                MarcarError(txtNoControl);
                return $"El No. Control debe tener {EXT_MIN_NO_CONTROL} caracteres.";
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(nc, @"^\d{8}$"))
            {
                MarcarError(txtNoControl);
                return "El No. Control debe tener 8 digitos (ej: 20001234).";
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

            // Fecha
            if (!string.IsNullOrWhiteSpace(txtFechaNacimiento.Text))
            {
                DateTime f;
                if (!DateTime.TryParse(txtFechaNacimiento.Text, out f))
                {
                    MarcarError(txtFechaNacimiento);
                    return "Fecha invalida (formato: aaaa-mm-dd).";
                }
                if (f > DateTime.Now)
                {
                    MarcarError(txtFechaNacimiento);
                    return "La fecha de nacimiento no puede ser futura.";
                }
                if (f < DateTime.Now.AddYears(-120))
                {
                    MarcarError(txtFechaNacimiento);
                    return "La fecha de nacimiento parece demasiado antigua.";
                }
            }

            return null;
        }

        private bool HayCambiosSinGuardar()
        {
            return !string.IsNullOrWhiteSpace(txtNoControl.Text) ||
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
            foreach (var txt in new[] { txtNoControl, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono, txtFechaNacimiento })
                txt.BackColor = Color.FromArgb(248, 250, 252);
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvAlumnos.SelectedRows.Count == 0) return;

            int idAlumno = (int)dgvAlumnos.SelectedRows[0].Cells["Id_Alumno"].Value;

            // Buscar en cache (tiene TODOS los datos, incluyendo Fecha_Nacimiento)
            var a = _alumnosCache.FirstOrDefault(x => x.Id_Alumno == idAlumno);
            if (a == null)
            {
                MostrarAdvertencia("No se pudieron cargar los datos completos del alumno.");
                return;
            }

            txtIdAlumno.Text = a.Id_Alumno.ToString();
            txtNoControl.Text = a.No_Control ?? "";
            txtNombre.Text = a.Nombre ?? "";
            txtApellidoPaterno.Text = a.Apellido_Paterno ?? "";
            txtApellidoMaterno.Text = a.Apellido_Materno ?? "";
            txtEmail.Text = a.Email ?? "";
            txtTelefono.Text = a.Telefono ?? "";
            txtFechaNacimiento.Text = a.Fecha_Nacimiento?.ToString("yyyy-MM-dd") ?? "";
        }

        private void LimpiarFormulario()
        {
            foreach (var txt in new[] { txtIdAlumno, txtNoControl, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono, txtFechaNacimiento })
                txt.Clear();
            RestaurarColores();
        }

        // ==================== HELPERS ====================

        private string NullIfEmpty(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private DateTime? ParsearFecha(string texto) =>
            DateTime.TryParse(texto, out DateTime f) ? f : (DateTime?)null;

        private void MostrarError(string msg) =>
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void MostrarAdvertencia(string msg) =>
            MessageBox.Show(msg, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void MostrarExito(string msg) =>
            MessageBox.Show(msg, "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
