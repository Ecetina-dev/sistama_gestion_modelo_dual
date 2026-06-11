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
    public partial class FormMaterias : Form
    {
        private MateriaController controller = new MateriaController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Materia> _materiasCache = new List<Materia>();

        // Limites exactos del modelo y BD
        private const int MAX_CLAVE = 10;
        private const int MIN_NOMBRE = 3;
        private const int MAX_NOMBRE = 200;

        public FormMaterias()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarMaterias();
        }

        private void ConfigurarFormulario()
        {
            // Limitar longitud de campos segun modelo
            txtClave.MaxLength = MAX_CLAVE;
            txtNombre.MaxLength = MAX_NOMBRE;

            // Enter y Escape a nivel form
            this.KeyPreview = true;
            this.KeyDown += FormMaterias_KeyDown;

            // Doble click en grid para editar
            dgvMaterias.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Restaurar color al escribir
            EventHandler restaurar = (s, e) => {
                if (s is TextBox txt && txt.BackColor == Color.FromArgb(255, 235, 235))
                    txt.BackColor = Color.FromArgb(248, 250, 252);
            };
            txtClave.TextChanged += restaurar;
            txtNombre.TextChanged += restaurar;
            txtDescripcion.TextChanged += restaurar;

            // Enter navega al siguiente campo
            ConfigurarEnterSgteCampo(txtClave, txtNombre);
            ConfigurarEnterSgteCampo(txtNombre, nudCreditos);
            ConfigurarEnterSgteCampo(nudCreditos, nudSemestre);
            ConfigurarEnterSgteCampo(nudSemestre, nudHorasTeoria);
            ConfigurarEnterSgteCampo(nudHorasTeoria, nudHorasPractica);
            ConfigurarEnterSgteCampo(nudHorasPractica, txtDescripcion);
            ConfigurarEnterSgteCampo(txtDescripcion, btnGuardar);
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
            txtClave.TabIndex = 0;
            txtNombre.TabIndex = 1;
            nudCreditos.TabIndex = 2;
            nudSemestre.TabIndex = 3;
            nudHorasTeoria.TabIndex = 4;
            nudHorasPractica.TabIndex = 5;
            txtDescripcion.TabIndex = 6;
            btnGuardar.TabIndex = 7;
            btnCancelar.TabIndex = 8;
        }

        // Enter siguiente campo, Escape = cancelar
        private void FormMaterias_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && panelCardDatos.Visible)
            {
                e.SuppressKeyPress = true;
                btnCancelar_Click(sender, e);
            }
        }

        private void ConfigurarEnterSgteCampo(Control actual, Control siguiente)
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

        private void CargarMaterias()
        {
            try
            {
                _materiasCache = controller.Read() ?? new List<Materia>();
                AplicarFiltroBusqueda();
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
                ? _materiasCache
                : _materiasCache.Where(m =>
                    (m.Clave_Materia?.ToLower().Contains(filtro) ?? false) ||
                    (m.Nombre?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            dgvMaterias.DataSource = null;
            dgvMaterias.DataSource = fuente;
            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            if (dgvMaterias.Columns.Count == 0) return;

            // Ocultar columnas tecnicas
            var ocultar = new[] { "Id_Materia", "Created_At", "Updated_At", "Status_Materia" };
            foreach (var col in ocultar)
                if (dgvMaterias.Columns.Contains(col))
                    dgvMaterias.Columns[col].Visible = false;

            // Headers legibles
            var headers = new Dictionary<string, string>
            {
                ["Clave_Materia"] = "Clave",
                ["nombre"] = "nombre",
                ["Descripcion"] = "Descripcion",
                ["Creditos"] = "Creditos",
                ["Semestre"] = "Semestre",
                ["Horas_Teoria"] = "H. Teoria",
                ["Horas_Practica"] = "H. Practica"
            };
            foreach (var kv in headers)
                if (dgvMaterias.Columns.Contains(kv.Key))
                    dgvMaterias.Columns[kv.Key].HeaderText = kv.Value;

            // Reordenar
            string[] orden = { "Clave_Materia", "nombre", "Descripcion", "Creditos", "Semestre", "Horas_Teoria", "Horas_Practica" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvMaterias.Columns.Contains(col))
                    dgvMaterias.Columns[col].DisplayIndex = idx++;
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
            txtClave.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvMaterias.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una materia de la lista.", "Advertencia",
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
            if (dgvMaterias.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una materia.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombre = dgvMaterias.SelectedRows[0].Cells["nombre"]?.Value?.ToString() ?? "";

            if (MessageBox.Show($"Eliminar '{nombre}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvMaterias.SelectedRows[0].Cells["Id_Materia"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarÉxito("Materia eliminada.");
                    CargarMaterias();
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
            CargarMaterias();
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
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
            int? idExcluir = isNewRecord ? null : (int?)Convert.ToInt32(txtIdMateria.Text);
            if (controller.ExisteClave(txtClave.Text.Trim(), idExcluir))
            {
                MarcarError(txtClave);
                MostrarAdvertencia("La clave '" + txtClave.Text.Trim() + "' ya existe.");
                txtClave.Focus();
                return;
            }

            // 3. Construir objeto
            var materia = new Materia
            {
                Clave_Materia = txtClave.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Descripcion = NullIfEmpty(txtDescripcion.Text),
                Creditos = (int)nudCreditos.Value,
                Semestre = (int)nudSemestre.Value,
                Horas_Teoria = (int)nudHorasTeoria.Value,
                Horas_Practica = (int)nudHorasPractica.Value,
                Status_Materia = Estatus.MateriaActiva,
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
                    success = await System.Threading.Tasks.Task.Run(() => controller.Create(materia));
                else
                {
                    materia.Id_Materia = Convert.ToInt32(txtIdMateria.Text);
                    success = await System.Threading.Tasks.Task.Run(() => controller.Update(materia));
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
                MostrarÉxito("Materia guardada.");
                isEditing = false;
                isNewRecord = false;
                panelCardDatos.Visible = false;
                CargarMaterias();
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

            // Clave_Materia
            string clave = txtClave.Text.Trim();
            if (string.IsNullOrEmpty(clave))
            {
                MarcarError(txtClave);
                return "La clave de materia es obligatoria.";
            }
            if (clave.Length > MAX_CLAVE)
            {
                MarcarError(txtClave);
                return $"La clave debe tener maximo {MAX_CLAVE} caracteres.";
            }
            if (clave.Contains(" "))
            {
                MarcarError(txtClave);
                return "La clave no debe contener espacios.";
            }

            // Nombre
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MarcarError(txtNombre);
                return "El nombre es obligatorio.";
            }
            if (txtNombre.Text.Trim().Length < MIN_NOMBRE)
            {
                MarcarError(txtNombre);
                return $"El nombre debe tener al menos {MIN_NOMBRE} caracteres.";
            }

            return null;
        }

        private bool HayCambiosSinGuardar()
        {
            return !string.IsNullOrWhiteSpace(txtClave.Text) ||
                   !string.IsNullOrWhiteSpace(txtNombre.Text);
        }

        private void MarcarError(Control ctrl)
        {
            if (ctrl is TextBox txt)
                txt.BackColor = Color.FromArgb(255, 235, 235);
            else if (ctrl is NumericUpDown nud)
                nud.BackColor = Color.FromArgb(255, 235, 235);
            ctrl.Focus();
        }

        private void RestaurarColores()
        {
            foreach (var ctrl in new Control[] { txtClave, txtNombre, txtDescripcion,
                nudCreditos, nudSemestre, nudHorasTeoria, nudHorasPractica })
            {
                if (ctrl is TextBox txt)
                    txt.BackColor = Color.FromArgb(248, 250, 252);
                else if (ctrl is NumericUpDown nud)
                    nud.BackColor = Color.FromArgb(248, 250, 252);
            }
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvMaterias.SelectedRows.Count == 0) return;

            int idMateria = (int)dgvMaterias.SelectedRows[0].Cells["Id_Materia"].Value;

            // Buscar en cache (tiene TODOS los datos)
            var m = _materiasCache.FirstOrDefault(x => x.Id_Materia == idMateria);
            if (m == null)
            {
                MostrarAdvertencia("No se pudieron cargar los datos completos de la materia.");
                return;
            }

            txtIdMateria.Text = m.Id_Materia.ToString();
            txtClave.Text = m.Clave_Materia ?? "";
            txtNombre.Text = m.Nombre ?? "";
            txtDescripcion.Text = m.Descripcion ?? "";
            nudCreditos.Value = Math.Max(nudCreditos.Minimum, Math.Min(nudCreditos.Maximum, m.Creditos));
            nudSemestre.Value = Math.Max(nudSemestre.Minimum, Math.Min(nudSemestre.Maximum, m.Semestre > 0 ? m.Semestre : 1));
            nudHorasTeoria.Value = Math.Max(nudHorasTeoria.Minimum, Math.Min(nudHorasTeoria.Maximum, m.Horas_Teoria));
            nudHorasPractica.Value = Math.Max(nudHorasPractica.Minimum, Math.Min(nudHorasPractica.Maximum, m.Horas_Practica));
        }

        private void LimpiarFormulario()
        {
            txtIdMateria.Clear();
            txtClave.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            nudCreditos.Value = 1;
            nudSemestre.Value = 1;
            nudHorasTeoria.Value = 0;
            nudHorasPractica.Value = 0;
            RestaurarColores();
        }

        // ==================== HELPERS ====================

        private string NullIfEmpty(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void MostrarError(string msg) =>
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void MostrarAdvertencia(string msg) =>
            MessageBox.Show(msg, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void MostrarÉxito(string msg) =>
            MessageBox.Show(msg, "éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

