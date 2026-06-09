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
    public partial class FormTemas : Form
    {
        private TemaController controller = new TemaController();
        private MateriaController materiaController = new MateriaController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Tema> _temasCache = new List<Tema>();

        // Limites exactos del modelo
        private const int MAX_NOMBRE = 200;
        private const int MAX_DESCRIPCION = 500;
        private const int MIN_NOMBRE = 3;
        private const int MAX_NUMERO_TEMA = 999;
        private const int MIN_NUMERO_TEMA = 1;

        public FormTemas()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarTemas();
            CargarMaterias();
        }

        private void ConfigurarFormulario()
        {
            // Limitar longitud de campos segun modelo
            txtNombre.MaxLength = MAX_NOMBRE;
            txtDescripcion.MaxLength = MAX_DESCRIPCION;

            // Configurar NumericUpDown
            nudNumeroTema.Minimum = MIN_NUMERO_TEMA;
            nudNumeroTema.Maximum = MAX_NUMERO_TEMA;
            nudNumeroTema.Value = MIN_NUMERO_TEMA;

            // Enter y Escape a nivel form
            this.KeyPreview = true;
            this.KeyDown += FormTemas_KeyDown;

            // Doble click en grid para editar
            dgvTemas.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Restaurar color al escribir
            EventHandler restaurar = (s, e) => {
                if (s is TextBox txt && txt.BackColor == Color.FromArgb(255, 235, 235))
                    txt.BackColor = Color.FromArgb(248, 250, 252);
            };
            txtNombre.TextChanged += restaurar;
            txtDescripcion.TextChanged += restaurar;
            cmbMateria.SelectedIndexChanged += (s, e) => {
                if (cmbMateria.BackColor == Color.FromArgb(255, 235, 235))
                    cmbMateria.BackColor = Color.FromArgb(248, 250, 252);
            };
            nudNumeroTema.ValueChanged += (s, e) => {
                if (nudNumeroTema.BackColor == Color.FromArgb(255, 235, 235))
                    nudNumeroTema.BackColor = Color.FromArgb(248, 250, 252);
            };

            // Enter navega al siguiente campo
            ConfigurarEnterSgteCampo(cmbMateria, nudNumeroTema);
            ConfigurarEnterSgteCampo(nudNumeroTema, txtNombre);
            ConfigurarEnterSgteCampo(txtNombre, txtDescripcion);
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
            cmbMateria.TabIndex = 0;
            nudNumeroTema.TabIndex = 1;
            txtNombre.TabIndex = 2;
            txtDescripcion.TabIndex = 3;
            btnGuardar.TabIndex = 4;
            btnCancelar.TabIndex = 5;
        }

        // Enter siguiente campo, Escape = cancelar
        private void FormTemas_KeyDown(object sender, KeyEventArgs e)
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

        private void CargarTemas()
        {
            try
            {
                _temasCache = controller.Read() ?? new List<Tema>();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void CargarMaterias()
        {
            try
            {
                var materias = materiaController.Read() ?? new List<Materia>();
                cmbMateria.DataSource = materias;
                cmbMateria.DisplayMember = "Nombre";
                cmbMateria.ValueMember = "Id_Materia";
                cmbMateria.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MostrarError("Error al cargar materias: " + ex.Message);
            }
        }

        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            var fuente = string.IsNullOrEmpty(filtro)
                ? _temasCache
                : _temasCache.Where(t =>
                    (t.Nombre?.ToLower().Contains(filtro) ?? false) ||
                    (t.Nombre_Materia?.ToLower().Contains(filtro) ?? false) ||
                    (t.Numero_Tema.ToString().Contains(filtro)) ||
                    (t.Descripcion?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            dgvTemas.DataSource = null;
            dgvTemas.DataSource = fuente;
            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            if (dgvTemas.Columns.Count == 0) return;

            // Ocultar columnas tecnicas
            var ocultar = new[] { "Id_Tema", "Id_Materia", "Created_At", "Updated_At", "Status_Tema" };
            foreach (var col in ocultar)
                if (dgvTemas.Columns.Contains(col))
                    dgvTemas.Columns[col].Visible = false;

            // Headers legibles
            var headers = new Dictionary<string, string>
            {
                ["Numero_Tema"] = "No. Tema",
                ["Nombre"] = "Nombre",
                ["Nombre_Materia"] = "Materia",
                ["Descripcion"] = "Descripcion"
            };
            foreach (var kv in headers)
                if (dgvTemas.Columns.Contains(kv.Key))
                    dgvTemas.Columns[kv.Key].HeaderText = kv.Value;

            // Reordenar
            string[] orden = { "Numero_Tema", "Nombre", "Nombre_Materia", "Descripcion" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvTemas.Columns.Contains(col))
                    dgvTemas.Columns[col].DisplayIndex = idx++;
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
            cmbMateria.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvTemas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un tema de la lista.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            panelCardDatos.Visible = true;
            cmbMateria.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvTemas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un tema.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombre = dgvTemas.SelectedRows[0].Cells["Nombre"]?.Value?.ToString() ?? "sin nombre";

            if (MessageBox.Show($"Eliminar tema '{nombre}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvTemas.SelectedRows[0].Cells["Id_Tema"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarExito("Tema eliminado.");
                    CargarTemas();
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
            CargarTemas();
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

            // 2. Construir objeto
            var tema = new Tema
            {
                Id_Materia = Convert.ToInt32(cmbMateria.SelectedValue),
                Numero_Tema = (int)nudNumeroTema.Value,
                Nombre = txtNombre.Text.Trim(),
                Descripcion = NullIfEmpty(txtDescripcion.Text),
                Status_Tema = Estatus.TemaActivo,
            };

            // 3. Loading state
            isLoading = true;
            btnGuardar.Enabled = false;
            btnGuardar.Text = "Guardando...";
            Cursor = Cursors.WaitCursor;

            // 4. Guardar
            bool success;
            try
            {
                if (isNewRecord)
                    success = await System.Threading.Tasks.Task.Run(() => controller.Create(tema));
                else
                {
                    tema.Id_Tema = Convert.ToInt32(txtIdTema.Text);
                    success = await System.Threading.Tasks.Task.Run(() => controller.Update(tema));
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
                MostrarExito("Tema guardado.");
                isEditing = false;
                isNewRecord = false;
                panelCardDatos.Visible = false;
                CargarTemas();
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

            // Materia
            if (cmbMateria.SelectedValue == null || Convert.ToInt32(cmbMateria.SelectedValue) <= 0)
            {
                MarcarErrorCombo(cmbMateria);
                return "Selecciona una materia.";
            }

            // Numero de Tema (1-999)
            if (nudNumeroTema.Value < MIN_NUMERO_TEMA || nudNumeroTema.Value > MAX_NUMERO_TEMA)
            {
                MarcarErrorNud(nudNumeroTema);
                return $"El numero de tema debe estar entre {MIN_NUMERO_TEMA} y {MAX_NUMERO_TEMA}.";
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
            return cmbMateria.SelectedIndex >= 0 ||
                   nudNumeroTema.Value > MIN_NUMERO_TEMA ||
                   !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                   !string.IsNullOrWhiteSpace(txtDescripcion.Text);
        }

        private void MarcarError(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(255, 235, 235);
            txt.Focus();
        }

        private void MarcarErrorCombo(ComboBox cmb)
        {
            cmb.BackColor = Color.FromArgb(255, 235, 235);
            cmb.Focus();
        }

        private void MarcarErrorNud(NumericUpDown nud)
        {
            nud.BackColor = Color.FromArgb(255, 235, 235);
            nud.Focus();
        }

        private void RestaurarColores()
        {
            txtNombre.BackColor = Color.FromArgb(248, 250, 252);
            txtDescripcion.BackColor = Color.FromArgb(248, 250, 252);
            cmbMateria.BackColor = Color.FromArgb(248, 250, 252);
            nudNumeroTema.BackColor = Color.FromArgb(248, 250, 252);
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvTemas.SelectedRows.Count == 0) return;

            int idTema = (int)dgvTemas.SelectedRows[0].Cells["Id_Tema"].Value;

            // Buscar en cache (tiene TODOS los datos)
            var t = _temasCache.FirstOrDefault(x => x.Id_Tema == idTema);
            if (t == null)
            {
                MostrarAdvertencia("No se pudieron cargar los datos completos del tema.");
                return;
            }

            txtIdTema.Text = t.Id_Tema.ToString();
            cmbMateria.SelectedValue = t.Id_Materia;
            nudNumeroTema.Value = t.Numero_Tema;
            txtNombre.Text = t.Nombre ?? "";
            txtDescripcion.Text = t.Descripcion ?? "";
        }

        private void LimpiarFormulario()
        {
            txtIdTema.Clear();
            cmbMateria.SelectedIndex = -1;
            nudNumeroTema.Value = MIN_NUMERO_TEMA;
            txtNombre.Clear();
            txtDescripcion.Clear();
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
