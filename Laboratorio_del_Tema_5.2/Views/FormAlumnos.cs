#pragma warning disable CS0414
#pragma warning disable IDE1006, IDE0090, IDE0028, IDE0305, IDE0300, IDE0017, IDE0039 // Sugerencias de estilo
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
        private readonly AlumnoController controller = new();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Alumno> _alumnosCache = new();

        // Paginacion
        private const int REGISTROS_POR_PAGINA = 50;
        private int _paginaActual = 1;
        private int _totalPaginas = 1;
        private List<Alumno> _alumnosFiltrados = new();

        private const int MAX_NO_CONTROL = 8;
        private const int EXT_MIN_NO_CONTROL = 8;
        private const int MAX_NOMBRE = 50;
        private const int MAX_APELLIDO = 25;
        private const int MAX_EMAIL = 80;
        private const int MAX_TELEFONO = 10;
        private const int EXT_MIN_TELEFONO = 10;

        // DateTimePicker creado programaticamente (no necesita Designer)
        private DateTimePicker dtpFechaNacimiento;

        public FormAlumnos()
        {
            InitializeComponent();
            CrearDateTimePicker();
            ConfigurarFormulario();
            CargarAlumnos();
        }

        private void CrearDateTimePicker()
        {
            dtpFechaNacimiento = new DateTimePicker();
            dtpFechaNacimiento.Format = DateTimePickerFormat.Custom;
            dtpFechaNacimiento.CustomFormat = "yyyy-MM-dd";
            dtpFechaNacimiento.ShowCheckBox = true;
            dtpFechaNacimiento.Checked = false;
            dtpFechaNacimiento.Font = new Font("Segoe UI", 12F);
            dtpFechaNacimiento.Location = txtFechaNacimiento.Location;
            dtpFechaNacimiento.Size = txtFechaNacimiento.Size;
            dtpFechaNacimiento.TabIndex = txtFechaNacimiento.TabIndex;
            dtpFechaNacimiento.MaxDate = DateTime.Now;
            dtpFechaNacimiento.MinDate = DateTime.Now.AddYears(-120);

            // Reemplazar textbox en el parent
            if (txtFechaNacimiento.Parent != null)
            {
                int idx = txtFechaNacimiento.Parent.Controls.GetChildIndex(txtFechaNacimiento);
                txtFechaNacimiento.Parent.Controls.Add(dtpFechaNacimiento);
                txtFechaNacimiento.Parent.Controls.SetChildIndex(dtpFechaNacimiento, idx);
                txtFechaNacimiento.Visible = false;
            }
        }

        private void ConfigurarFormulario()
        {
            txtNoControl.MaxLength = MAX_NO_CONTROL;
            txtNombre.MaxLength = MAX_NOMBRE;
            txtApellidoPaterno.MaxLength = MAX_APELLIDO;
            txtApellidoMaterno.MaxLength = MAX_APELLIDO;
            txtEmail.MaxLength = MAX_EMAIL;
            txtTelefono.MaxLength = MAX_TELEFONO;

            // Filtros de entrada por tipo de campo
            txtNoControl.KeyPress += (s, e) => SoloDigitos(e);
            txtNombre.KeyPress += (s, e) => SoloLetrasConAcentos(e);
            txtApellidoPaterno.KeyPress += (s, e) => SoloLetrasConAcentos(e);
            txtApellidoMaterno.KeyPress += (s, e) => SoloLetrasConAcentos(e);
            txtTelefono.KeyPress += (s, e) => SoloDigitos(e);

            // Indicador de progreso en No.Control
            txtNoControl.TextChanged += (s, e) =>
            {
                int len = txtNoControl.Text.Length;
                txtNoControl.Text = txtNoControl.Text; // fuerza el MaxLength
                if (len > 0)
                    txtNoControl.Tag = $"{len}/{MAX_NO_CONTROL}";
                else
                    txtNoControl.Tag = "";
            };
            // Tooltip dinamico para No.Control
            txtNoControl.TextChanged += (s, e) =>
            {
                toolTip.SetToolTip(txtNoControl, $"{txtNoControl.Text.Length}/{MAX_NO_CONTROL} - 8 dígitos");
            };

            this.KeyPreview = true;
            this.KeyDown += FormAlumnos_KeyDown;

            dgvAlumnos.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Tooltips en campos
            toolTip.SetToolTip(txtNoControl, "8 dígitos (ej: 20001234)");
            toolTip.SetToolTip(txtNombre, "Nombre del alumno");
            toolTip.SetToolTip(txtApellidoPaterno, "Apellido paterno");
            toolTip.SetToolTip(txtApellidoMaterno, "Apellido materno (opcional)");
            toolTip.SetToolTip(txtEmail, "Correo electrónico (ej: alumno@dominio.com)");
            toolTip.SetToolTip(txtTelefono, "10 dígitos (ej: 5551234567)");
            toolTip.SetToolTip(txtFechaNacimiento, "Formato: aaaa-mm-dd");
            toolTip.SetToolTip(dtpFechaNacimiento, "Selecciona la fecha de nacimiento");

            // Clic en icono de busqueda para limpiar
            lblBuscarIcono.Cursor = Cursors.Hand;
            lblBuscarIcono.Click += (s, e) =>
            {
                txtBuscar.Clear();
                txtBuscar.Focus();
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

            ConfigurarEnterSgteCampo(txtNoControl, txtNombre);
            ConfigurarEnterSgteCampo(txtNombre, txtApellidoPaterno);
            ConfigurarEnterSgteCampo(txtApellidoPaterno, txtApellidoMaterno);
            ConfigurarEnterSgteCampo(txtApellidoMaterno, txtEmail);
            ConfigurarEnterSgteCampo(txtEmail, txtTelefono);
            ConfigurarEnterSgteCampo(txtTelefono, dtpFechaNacimiento);
            ConfigurarEnterSgteCampo(txtFechaNacimiento, btnGuardar);
            dtpFechaNacimiento.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; btnGuardar.Focus(); }
            };

            btnGuardar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !isLoading) { e.SuppressKeyPress = true; btnGuardar_Click(s, e); }
            };

            // Color de filas segun status
            dgvAlumnos.CellFormatting += DgvAlumnos_CellFormatting;

            txtNoControl.TabIndex = 0;
            txtNombre.TabIndex = 1;
            txtApellidoPaterno.TabIndex = 2;
            txtApellidoMaterno.TabIndex = 3;
            txtEmail.TabIndex = 4;
            txtTelefono.TabIndex = 5;
            dtpFechaNacimiento.TabIndex = 6;
            btnGuardar.TabIndex = 7;
            btnCancelar.TabIndex = 8;

            // Paginacion
            CrearPaginacion();

            // Undock toolbar para poder redimensionarlo manualmente
            panelToolbar.Dock = DockStyle.None;

            // Redimensionar controles al cambiar tamaño
            this.Resize += (s, e) => ReorganizarLayout();
        }

        private Button btnPagAnterior, btnPagSiguiente;
        private Label lblPagInfo;

        private void CrearPaginacion()
        {
            btnPagAnterior = new Button();
            btnPagAnterior.Text = "◀  Anterior";
            btnPagAnterior.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPagAnterior.BackColor = Color.FromArgb(108, 117, 125);
            btnPagAnterior.ForeColor = Color.White;
            btnPagAnterior.FlatStyle = FlatStyle.Flat;
            btnPagAnterior.FlatAppearance.BorderSize = 0;
            btnPagAnterior.Cursor = Cursors.Hand;
            btnPagAnterior.Size = new Size(100, 32);
            btnPagAnterior.TabIndex = 20;
            btnPagAnterior.Click += (s, e) => { if (_paginaActual > 1) { _paginaActual--; AplicarPagina(); } };
            panelContent.Controls.Add(btnPagAnterior);

            lblPagInfo = new Label();
            lblPagInfo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPagInfo.ForeColor = Color.FromArgb(33, 37, 41);
            lblPagInfo.TextAlign = ContentAlignment.MiddleCenter;
            lblPagInfo.Size = new Size(220, 32);
            lblPagInfo.TabIndex = 21;
            panelContent.Controls.Add(lblPagInfo);

            btnPagSiguiente = new Button();
            btnPagSiguiente.Text = "Siguiente  ▶";
            btnPagSiguiente.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPagSiguiente.BackColor = Color.FromArgb(108, 117, 125);
            btnPagSiguiente.ForeColor = Color.White;
            btnPagSiguiente.FlatStyle = FlatStyle.Flat;
            btnPagSiguiente.FlatAppearance.BorderSize = 0;
            btnPagSiguiente.Cursor = Cursors.Hand;
            btnPagSiguiente.Size = new Size(110, 32);
            btnPagSiguiente.TabIndex = 22;
            btnPagSiguiente.Click += (s, e) => { if (_paginaActual < _totalPaginas) { _paginaActual++; AplicarPagina(); } };
            panelContent.Controls.Add(btnPagSiguiente);
        }

        private void ReorganizarLayout()
        {
            int cw = panelContent.ClientSize.Width;
            int ch = panelContent.ClientSize.Height;

            // Toolbar: solo con buscador, ocupa todo el ancho
            panelToolbar.Location = new Point(0, 0);
            panelToolbar.Width = cw;

            // Buscador SIEMPRE centrado en el toolbar
            int searchW = 200;
            int searchX = (cw - searchW) / 2;
            lblBuscarIcono.Location = new Point(searchX - 28, 14);
            txtBuscar.Location = new Point(searchX, 12);
            txtBuscar.Width = searchW;

            int toolbarBottom = panelToolbar.Height + 5;

            // Grid: mas pequeño cuando la card esta visible
            int gridH = panelCardDatos.Visible 
                ? (int)(ch * 0.45) - toolbarBottom  // 45% de alto
                : ch - toolbarBottom - 45;            // casi todo

            dgvAlumnos.Location = new Point(0, toolbarBottom);
            dgvAlumnos.Width = cw - 6;
            dgvAlumnos.Height = Math.Max(120, gridH);

            // Paginacion centrada debajo del grid
            int totalPagW = btnPagAnterior.Width + 10 + lblPagInfo.Width + 10 + btnPagSiguiente.Width;
            int pagX = (cw - totalPagW) / 2;
            int pagY = dgvAlumnos.Bottom + 5;
            btnPagAnterior.Location = new Point(Math.Max(0, pagX), pagY);
            lblPagInfo.Location = new Point(Math.Max(0, pagX + btnPagAnterior.Width + 10), pagY);
            btnPagSiguiente.Location = new Point(Math.Max(0, lblPagInfo.Right + 10), pagY);

            // Card datos: DEBAJO del grid (no encima)
            if (panelCardDatos.Visible)
            {
                int cardY = Math.Max(pagY + 45, dgvAlumnos.Bottom + 10);
                panelCardDatos.Location = new Point(0, cardY);
                panelCardDatos.Width = cw - 6;
                panelCardDatos.Height = Math.Max(180, ch - cardY - 5);
            }
        }

        private void AplicarPagina()
        {
            int total = _alumnosFiltrados.Count;
            _totalPaginas = Math.Max(1, (int)Math.Ceiling((double)total / REGISTROS_POR_PAGINA));
            _paginaActual = Math.Max(1, Math.Min(_paginaActual, _totalPaginas));

            var pagina = _alumnosFiltrados
                .Skip((_paginaActual - 1) * REGISTROS_POR_PAGINA)
                .Take(REGISTROS_POR_PAGINA)
                .ToList();

            dgvAlumnos.DataSource = null;
            dgvAlumnos.DataSource = pagina;
            ConfigurarGrid();

            lblPagInfo.Text = $"Pág {_paginaActual} de {_totalPaginas} ({total} registros)";
            btnPagAnterior.Enabled = _paginaActual > 1;
            btnPagSiguiente.Enabled = _paginaActual < _totalPaginas;

            ReorganizarLayout();
        }

        private void ActualizarContador()
        {
            _paginaActual = 1;
            AplicarPagina();
        }

        // Atajos de teclado: Ctrl+N nuevo, Ctrl+S guardar
        private void FormAlumnos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.N)
            {
                e.SuppressKeyPress = true;
                if (!panelCardDatos.Visible) btnNuevo_Click(sender, e);
            }
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;
                if (panelCardDatos.Visible && !isLoading) btnGuardar_Click(sender, e);
            }
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
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; siguiente.Focus(); }
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
                ActualizarContador();
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            _alumnosFiltrados = string.IsNullOrEmpty(filtro)
                ? _alumnosCache.ToList()
                : _alumnosCache.Where(a =>
                    (a.No_Control?.ToLower().Contains(filtro) ?? false) ||
                    (a.Nombre?.ToLower().Contains(filtro) ?? false) ||
                    (a.Apellido_Paterno?.ToLower().Contains(filtro) ?? false) ||
                    (a.Apellido_Materno?.ToLower().Contains(filtro) ?? false) ||
                    (a.Email?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            ActualizarContador();
        }

        private void ConfigurarGrid()
        {
            if (dgvAlumnos.Columns.Count == 0) return;

            var ocultar = new[] { "Id_Alumno", "Created_At", "Updated_At", "Status_Alumno",
                                  "Fecha_Nacimiento", "Password", "PasswordHash", "Privilegios" };
            foreach (var col in ocultar)
                if (dgvAlumnos.Columns.Contains(col))
                    dgvAlumnos.Columns[col].Visible = false;

            var headers = new Dictionary<string, string>
            {
                ["No_Control"] = "No. Control",
                ["Nombre"] = "Nombre",
                ["Apellido_Paterno"] = "Ap. Paterno",
                ["Apellido_Materno"] = "Ap. Materno",
                ["Email"] = "Email",
                ["Telefono"] = "Teléfono"
            };
            foreach (var kv in headers)
                if (dgvAlumnos.Columns.Contains(kv.Key))
                    dgvAlumnos.Columns[kv.Key].HeaderText = kv.Value;

            string[] orden = { "No_Control", "Nombre", "Apellido_Paterno", "Apellido_Materno", "Email", "Telefono" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvAlumnos.Columns.Contains(col))
                    dgvAlumnos.Columns[col].DisplayIndex = idx++;
        }

        // Color de fila segun status
        private void DgvAlumnos_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvAlumnos.Columns.Contains("Status_Alumno") && e.RowIndex >= 0)
            {
                var row = dgvAlumnos.Rows[e.RowIndex];
                var status = row.Cells["Status_Alumno"]?.Value?.ToString();
                if (status == "inactivo")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
                else if (status == "egresado")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(240, 255, 240);
            }
        }

        // ==================== NAVEGACION ====================

        private void btnCerrar_Click(object sender, EventArgs e) => this.Close();
        private void txtBuscar_TextChanged(object sender, EventArgs e) => AplicarFiltroBusqueda();

        // ==================== CRUD ====================

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            HabilitarNoControl(true);
            panelCardDatos.Visible = true;
            dtpFechaNacimiento.Checked = false;
            ReorganizarLayout();
            txtNoControl.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count == 0)
            {
                MostrarAdvertencia("Selecciona un alumno de la lista.");
                return;
            }

            isNewRecord = false;
            isEditing = true;
            HabilitarNoControl(false);
            CargarDatosFormulario();
            panelCardDatos.Visible = true;
            ReorganizarLayout();
            txtNombre.Focus();
        }

        private void HabilitarNoControl(bool habilitado)
        {
            txtNoControl.Enabled = habilitado;
            txtNoControl.ReadOnly = !habilitado;
            txtNoControl.BackColor = habilitado
                ? Color.FromArgb(248, 250, 252)
                : Color.FromArgb(235, 235, 235);
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvAlumnos.SelectedRows.Count == 0)
            {
                MostrarAdvertencia("Selecciona un alumno.");
                return;
            }

            var nombre = $"{dgvAlumnos.SelectedRows[0].Cells["Nombre"]?.Value} " +
                         $"{dgvAlumnos.SelectedRows[0].Cells["Apellido_Paterno"]?.Value}";

            if (MessageBox.Show($"Eliminar a '{nombre.Trim()}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvAlumnos.SelectedRows[0].Cells["Id_Alumno"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarÉxito("Alumno eliminado.");
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
            ReorganizarLayout();
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

            string error = ValidarFormulario();
            if (error != null) { MostrarAdvertencia(error); return; }

            int? idExcluir = isNewRecord ? null : (int?)Convert.ToInt32(txtIdAlumno.Text);
            if (controller.ExisteNoControl(txtNoControl.Text.Trim(), idExcluir))
            {
                MarcarError(txtNoControl);
                MostrarAdvertencia("El No. Control '" + txtNoControl.Text.Trim() + "' ya existe.");
                return;
            }

            var alumno = new Alumno
            {
                No_Control = txtNoControl.Text.Trim(),
                Nombre = txtNombre.Text.Trim(),
                Apellido_Paterno = txtApellidoPaterno.Text.Trim(),
                Apellido_Materno = NullIfEmpty(txtApellidoMaterno.Text),
                Email = NullIfEmpty(txtEmail.Text),
                Telefono = NullIfEmpty(txtTelefono.Text),
                Fecha_Nacimiento = dtpFechaNacimiento.Checked ? dtpFechaNacimiento.Value : (DateTime?)null,
                Status_Alumno = Estatus.AlumnoActivo,
            };

            isLoading = true;
            btnGuardar.Enabled = false;
            btnGuardar.Text = "Guardando...";
            Cursor = Cursors.WaitCursor;

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
                MostrarÉxito("Alumno guardado.");
            isEditing = false; isNewRecord = false;
            panelCardDatos.Visible = false;
            ReorganizarLayout();
            CargarAlumnos();
            }
            else
                MostrarError("Error al guardar. Verifica los datos.");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (HayCambiosSinGuardar())
            {
                var r = MessageBox.Show("Hay cambios sin guardar.\n\n¿Descartarlos?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }
            isEditing = false; isNewRecord = false;
            panelCardDatos.Visible = false;
            LimpiarFormulario();
            RestaurarColores();
        }

        // ==================== VALIDACION ====================

        private string ValidarFormulario()
        {
            RestaurarColores();

            string nc = txtNoControl.Text.Trim();
            if (string.IsNullOrEmpty(nc)) { MarcarError(txtNoControl); return "El número de control es obligatorio."; }
            if (nc.Length < EXT_MIN_NO_CONTROL) { MarcarError(txtNoControl); return $"Debe tener {EXT_MIN_NO_CONTROL} dígitos."; }
            if (!System.Text.RegularExpressions.Regex.IsMatch(nc, @"^\d{8}$")) { MarcarError(txtNoControl); return "Debe tener 8 dígitos (ej: 20001234)."; }

            if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MarcarError(txtNombre); return "El nombre es obligatorio."; }
            if (txtNombre.Text.Trim().Length < 2) { MarcarError(txtNombre); return "Mínimo 2 caracteres."; }
            if (string.IsNullOrWhiteSpace(txtApellidoPaterno.Text)) { MarcarError(txtApellidoPaterno); return "El apellido paterno es obligatorio."; }

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                { MarcarError(txtEmail); return "Email inválido (ej: usuario@dominio.com)."; }
            }

            if (!string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                string soloDigitos = new string(txtTelefono.Text.Where(char.IsDigit).ToArray());
                if (soloDigitos.Length != EXT_MIN_TELEFONO)
                { MarcarError(txtTelefono); return $"El teléfono debe tener {EXT_MIN_TELEFONO} dígitos."; }
            }

            if (dtpFechaNacimiento.Checked)
            {
                if (dtpFechaNacimiento.Value > DateTime.Now)
                { MarcarError(txtFechaNacimiento); return "La fecha no puede ser futura."; }
            }

            return null;
        }

        private bool HayCambiosSinGuardar() =>
            !string.IsNullOrWhiteSpace(txtNoControl.Text) ||
            !string.IsNullOrWhiteSpace(txtNombre.Text) ||
            !string.IsNullOrWhiteSpace(txtApellidoPaterno.Text);

        private void MarcarError(TextBox txt) { txt.BackColor = Color.FromArgb(255, 235, 235); txt.Focus(); }

        private void RestaurarColores()
        {
            foreach (var txt in new[] { txtNoControl, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono })
                txt.BackColor = Color.FromArgb(248, 250, 252);
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvAlumnos.SelectedRows.Count == 0) return;
            int idAlumno = (int)dgvAlumnos.SelectedRows[0].Cells["Id_Alumno"].Value;

            var a = _alumnosCache.FirstOrDefault(x => x.Id_Alumno == idAlumno);
            if (a == null) { MostrarAdvertencia("No se pudieron cargar los datos."); return; }

            txtIdAlumno.Text = a.Id_Alumno.ToString();
            txtNoControl.Text = a.No_Control ?? "";
            txtNombre.Text = a.Nombre ?? "";
            txtApellidoPaterno.Text = a.Apellido_Paterno ?? "";
            txtApellidoMaterno.Text = a.Apellido_Materno ?? "";
            txtEmail.Text = a.Email ?? "";
            txtTelefono.Text = a.Telefono ?? "";

            if (a.Fecha_Nacimiento.HasValue)
            {
                dtpFechaNacimiento.Value = a.Fecha_Nacimiento.Value;
                dtpFechaNacimiento.Checked = true;
            }
            else
            {
                dtpFechaNacimiento.Checked = false;
            }
        }

        private void LimpiarFormulario()
        {
            foreach (var txt in new[] { txtIdAlumno, txtNoControl, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono })
                txt.Clear();
            dtpFechaNacimiento.Checked = false;
            RestaurarColores();
        }

        // ==================== HELPERS ====================

        private string NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void MostrarError(string msg) => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void MostrarAdvertencia(string msg) => MessageBox.Show(msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void MostrarÉxito(string msg) => MessageBox.Show(msg, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // ==================== FILTROS DE ENTRADA ====================

        /// <summary>
        /// Solo permite dígitos (0-9) y teclas de control (Backspace, Delete, etc.)
        /// </summary>
        private void SoloDigitos(KeyPressEventArgs e)
        {
            // Permitir teclas de control (Backspace, Enter, etc.)
            if (char.IsControl(e.KeyChar))
                return;
            // Solo dígitos
            if (!char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        /// <summary>
        /// Solo permite letras (incluyendo acentos, ñ, ü) y espacios.
        /// Bloquea números y caracteres especiales.
        /// </summary>
        private void SoloLetrasConAcentos(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            // Caracteres permitidos en nombres
            string permitidos = "áéíóúüñÁÉÍÓÚÜÑ";
            if (char.IsLetter(e.KeyChar) || e.KeyChar == ' ' || e.KeyChar == '.' || permitidos.Contains(e.KeyChar))
                return;

            // TODO lo demás se bloquea
            e.Handled = true;
        }
    }
}
