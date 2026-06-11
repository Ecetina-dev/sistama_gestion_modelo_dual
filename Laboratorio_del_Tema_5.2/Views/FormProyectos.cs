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
    public partial class FormProyectos : Form
    {
        private ProyectoController controller = new ProyectoController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Proyecto> _proyectosCache = new List<Proyecto>();

        private const int MIN_NOMBRE = 3;

        public FormProyectos()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarProyectos();
        }

        private void ConfigurarFormulario()
        {
            txtNombre.MaxLength = 200;
            txtDescripcion.MaxLength = 1000;
            txtObjetivos.MaxLength = 1000;

            this.KeyPreview = true;
            this.KeyDown += FormProyectos_KeyDown;

            dgvProyectos.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            EventHandler restaurar = (s, e) =>
            {
                if (s is TextBox txt && txt.BackColor == Color.FromArgb(255, 235, 235))
                    txt.BackColor = Color.FromArgb(248, 250, 252);
                else if (s is ComboBox cmb && cmb.BackColor == Color.FromArgb(255, 235, 235))
                    cmb.BackColor = Color.White;
                else if (s is DateTimePicker dtp && dtp.BackColor == Color.FromArgb(255, 235, 235))
                    dtp.BackColor = Color.White;
                else if (s is NumericUpDown nud && nud.BackColor == Color.FromArgb(255, 235, 235))
                    nud.BackColor = Color.White;
            };
            txtNombre.TextChanged += restaurar;
            txtDescripcion.TextChanged += restaurar;
            txtObjetivos.TextChanged += restaurar;
            dtpFechaInicio.ValueChanged += restaurar;
            dtpFechaFin.ValueChanged += restaurar;
            nudHorasTotales.ValueChanged += restaurar;
            cmbStatus.SelectedIndexChanged += restaurar;

            ConfigurarEnterSgteCampo(txtNombre, txtDescripcion);
            ConfigurarEnterSgteCampo(txtDescripcion, txtObjetivos);

            btnGuardar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !isLoading)
                {
                    e.SuppressKeyPress = true;
                    btnGuardar_Click(s, e);
                }
            };

            txtNombre.TabIndex = 0;
            txtDescripcion.TabIndex = 1;
            txtObjetivos.TabIndex = 2;
            dtpFechaInicio.TabIndex = 3;
            dtpFechaFin.TabIndex = 4;
            nudHorasTotales.TabIndex = 5;
            cmbStatus.TabIndex = 6;
            btnGuardar.TabIndex = 7;
            btnCancelar.TabIndex = 8;
        }

        private void FormProyectos_KeyDown(object sender, KeyEventArgs e)
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

        private void CargarProyectos()
        {
            try
            {
                _proyectosCache = controller.Read() ?? new List<Proyecto>();
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
                ? _proyectosCache
                : _proyectosCache.Where(p =>
                    (p.Nombre?.ToLower().Contains(filtro) ?? false) ||
                    (p.Descripcion?.ToLower().Contains(filtro) ?? false) ||
                    (p.Status?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            dgvProyectos.DataSource = null;
            dgvProyectos.DataSource = fuente;
            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            if (dgvProyectos.Columns.Count == 0) return;

            var ocultar = new[] { "Id_Proyecto", "Created_At", "Updated_At",
                                  "Descripcion", "Objetivos", "Horas_Totales" };
            foreach (var col in ocultar)
                if (dgvProyectos.Columns.Contains(col))
                    dgvProyectos.Columns[col].Visible = false;

            var headers = new Dictionary<string, string>
            {
                ["nombre"] = "nombre",
                ["Fecha_Inicio"] = "Fecha Inicio",
                ["Fecha_Fin"] = "Fecha Fin",
                ["Status"] = "Estado"
            };
            foreach (var kv in headers)
                if (dgvProyectos.Columns.Contains(kv.Key))
                    dgvProyectos.Columns[kv.Key].HeaderText = kv.Value;

            string[] orden = { "nombre", "Fecha_Inicio", "Fecha_Fin", "Status" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvProyectos.Columns.Contains(col))
                    dgvProyectos.Columns[col].DisplayIndex = idx++;
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
            txtNombre.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvProyectos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un proyecto de la lista.", "Advertencia",
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
            if (dgvProyectos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona un proyecto.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombre = dgvProyectos.SelectedRows[0].Cells["nombre"]?.Value?.ToString() ?? "";

            if (MessageBox.Show($"Eliminar '{nombre}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvProyectos.SelectedRows[0].Cells["Id_Proyecto"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarÉxito("Proyecto eliminado.");
                    CargarProyectos();
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
            CargarProyectos();
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
        }

        // ==================== GUARDAR ====================

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            string error = ValidarFormulario();
            if (error != null)
            {
                MostrarAdvertencia(error);
                return;
            }

            var proyecto = new Proyecto
            {
                Nombre = txtNombre.Text.Trim(),
                Descripcion = NullIfEmpty(txtDescripcion.Text),
                Objetivos = NullIfEmpty(txtObjetivos.Text),
                Fecha_Inicio = dtpFechaInicio.Checked ? dtpFechaInicio.Value : (DateTime?)null,
                Fecha_Fin = dtpFechaFin.Checked ? dtpFechaFin.Value : (DateTime?)null,
                Horas_Totales = nudHorasTotales.Value > 0 ? (int)nudHorasTotales.Value : (int?)null,
                Status = cmbStatus.SelectedItem?.ToString()
            };

            isLoading = true;
            btnGuardar.Enabled = false;
            btnGuardar.Text = "Guardando...";
            Cursor = Cursors.WaitCursor;

            bool success;
            try
            {
                if (isNewRecord)
                    success = await System.Threading.Tasks.Task.Run(() => controller.Create(proyecto));
                else
                {
                    proyecto.Id_Proyecto = Convert.ToInt32(txtIdProyecto.Text);
                    success = await System.Threading.Tasks.Task.Run(() => controller.Update(proyecto));
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
                MostrarÉxito("Proyecto guardado.");
                isEditing = false;
                isNewRecord = false;
                panelCardDatos.Visible = false;
                CargarProyectos();
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

            string nombre = txtNombre.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MarcarError(txtNombre);
                return "El nombre del proyecto es obligatorio.";
            }
            if (nombre.Length < MIN_NOMBRE)
            {
                MarcarError(txtNombre);
                return $"El nombre debe tener al menos {MIN_NOMBRE} caracteres.";
            }

            if (cmbStatus.SelectedIndex == -1)
            {
                MarcarError(cmbStatus);
                return "Selecciona un estado para el proyecto.";
            }

            if (dtpFechaInicio.Checked && dtpFechaFin.Checked)
            {
                if (dtpFechaFin.Value < dtpFechaInicio.Value)
                {
                    MarcarError(dtpFechaFin);
                    return "La fecha de fin no puede ser anterior a la fecha de inicio.";
                }
            }

            return null;
        }

        private bool HayCambiosSinGuardar()
        {
            return !string.IsNullOrWhiteSpace(txtNombre.Text) ||
                   !string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                   !string.IsNullOrWhiteSpace(txtObjetivos.Text) ||
                   dtpFechaInicio.Checked ||
                   dtpFechaFin.Checked ||
                   nudHorasTotales.Value > 0 ||
                   cmbStatus.SelectedIndex != -1;
        }

        private void MarcarError(Control ctrl)
        {
            ctrl.BackColor = Color.FromArgb(255, 235, 235);
            ctrl.Focus();
        }

        private void RestaurarColores()
        {
            var controls = new Control[] { txtNombre, txtDescripcion, txtObjetivos,
                                           dtpFechaInicio, dtpFechaFin, nudHorasTotales, cmbStatus };
            foreach (var ctrl in controls)
            {
                if (ctrl is TextBox)
                    ctrl.BackColor = Color.FromArgb(248, 250, 252);
                else
                    ctrl.BackColor = Color.White;
            }
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvProyectos.SelectedRows.Count == 0) return;

            int idProyecto = (int)dgvProyectos.SelectedRows[0].Cells["Id_Proyecto"].Value;

            var p = _proyectosCache.FirstOrDefault(x => x.Id_Proyecto == idProyecto);
            if (p == null)
            {
                MostrarAdvertencia("No se pudieron cargar los datos completos del proyecto.");
                return;
            }

            txtIdProyecto.Text = p.Id_Proyecto.ToString();
            txtNombre.Text = p.Nombre ?? "";
            txtDescripcion.Text = p.Descripcion ?? "";
            txtObjetivos.Text = p.Objetivos ?? "";

            if (p.Fecha_Inicio.HasValue)
            {
                dtpFechaInicio.Checked = true;
                dtpFechaInicio.Value = p.Fecha_Inicio.Value;
            }
            else
            {
                dtpFechaInicio.Checked = false;
                dtpFechaInicio.Value = DateTime.Now;
            }

            if (p.Fecha_Fin.HasValue)
            {
                dtpFechaFin.Checked = true;
                dtpFechaFin.Value = p.Fecha_Fin.Value;
            }
            else
            {
                dtpFechaFin.Checked = false;
                dtpFechaFin.Value = DateTime.Now;
            }

            nudHorasTotales.Value = p.Horas_Totales ?? 0;

            string status = p.Status ?? "";
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

        private void LimpiarFormulario()
        {
            txtIdProyecto.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtObjetivos.Clear();
            dtpFechaInicio.Checked = false;
            dtpFechaInicio.Value = DateTime.Now;
            dtpFechaFin.Checked = false;
            dtpFechaFin.Value = DateTime.Now;
            nudHorasTotales.Value = 0;
            cmbStatus.SelectedIndex = -1;
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

