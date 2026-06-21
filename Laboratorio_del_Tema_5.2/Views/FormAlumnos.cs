#pragma warning disable CS0414
#pragma warning disable IDE1006, IDE0090, IDE0028, IDE0305, IDE0300, IDE0017, IDE0039 // Sugerencias de estilo
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private List<Carrera> _carreras = new();

        // Paginacion
        private const int REGISTROS_POR_PAGINA = 50;
        private int _paginaActual = 1;
        private int _totalPaginas = 1;
        private List<Alumno> _alumnosFiltrados = new();

        // Ordenamiento de columnas
        private string _ultimaColumnaOrden = "";
        private bool _ordenAscendente = true;

        // Sincronizado con BD: alumno.no_control VARCHAR(15), constraint chk_alumno_no_control
        // exige exactamente 10 caracteres alfanumericos mayusculas/numeros.
        private const int MIN_NO_CONTROL = 10;
        private const int MAX_NO_CONTROL = 10;
        // Sincronizado con BD: alumno.nombre VARCHAR(100)
        private const int MAX_NOMBRE = 100;
        // Sincronizado con BD: alumno.apellido_paterno/apellido_materno VARCHAR(80)
        private const int MAX_APELLIDO = 80;
        // Sincronizado con BD: alumno.email VARCHAR(254) — RFC 5321
        private const int MAX_EMAIL = 254;
        // Sincronizado con BD: alumno.telefono VARCHAR(15)
        private const int MAX_TELEFONO = 15;
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
            txtNoControl.KeyPress += (s, e) => SoloAlfanumericoMayusculas(e);
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
                toolTip.SetToolTip(txtNoControl, $"{txtNoControl.Text.Length}/{MAX_NO_CONTROL} - letras mayúsculas y números");
            };

            this.KeyPreview = true;
            this.KeyDown += FormAlumnos_KeyDown;

            dgvAlumnos.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Tooltips en campos
            toolTip.SetToolTip(txtNoControl, "10 caracteres alfanuméricos mayúsculas/números (ej: 2021101001)");
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

            // Pin the grid to legacy columns only — Phase 1 enterprise fields
            // (CURP, Carrera, Semestre, Genero, etc.) are NOT visible; Phase 2 adds them.
            dgvAlumnos.AutoGenerateColumns = false;
            dgvAlumnos.DataSource = null;
            dgvAlumnos.DataSource = _alumnosCache;

            // Color de filas segun status
            dgvAlumnos.CellFormatting += DgvAlumnos_CellFormatting;

            // Ordenamiento por clic en columna
            dgvAlumnos.ColumnHeaderMouseClick += (s, e) =>
            {
                if (_alumnosFiltrados.Count == 0) return;
                string colName = dgvAlumnos.Columns[e.ColumnIndex].DataPropertyName;

                // Toggle sort direction
                bool asc = true;
                if (_ultimaColumnaOrden == colName)
                    asc = !_ordenAscendente;

                _ultimaColumnaOrden = colName;
                _ordenAscendente = asc;

                var propiedad = typeof(Alumno).GetProperty(colName);
                if (propiedad == null) return;

                _alumnosFiltrados = asc
                    ? _alumnosFiltrados.OrderBy(a => propiedad.GetValue(a, null) ?? "").ToList()
                    : _alumnosFiltrados.OrderByDescending(a => propiedad.GetValue(a, null) ?? "").ToList();

                _paginaActual = 1;
                AplicarPagina();

                // Visual indicator on header
                foreach (DataGridViewColumn col in dgvAlumnos.Columns)
                    col.HeaderText = col.HeaderText.Replace(" ▲", "").Replace(" ▼", "");
                dgvAlumnos.Columns[e.ColumnIndex].HeaderText += asc ? " ▲" : " ▼";
            };

            // Filtro por status
            cmbFiltroStatus.SelectedIndex = 0;
            cmbFiltroStatus.SelectedIndexChanged += (s, e) => AplicarFiltros();

            // Limpiar busqueda
            btnLimpiarBusqueda.Click += (s, e) =>
            {
                txtBuscar.Clear();
                txtBuscar.Focus();
            };

            // Exportar CSV
            btnExportarCSV.Click += (s, e) => ExportarCSV();

            // W-5: Tab order — left-to-right, top-to-bottom (legacy + enterprise)
            txtNoControl.TabIndex = 0;
            txtNombre.TabIndex = 1;
            txtApellidoPaterno.TabIndex = 2;
            txtApellidoMaterno.TabIndex = 3;
            txtCURP.TabIndex = 4;
            cmbGenero.TabIndex = 5;
            cmbCarrera.TabIndex = 6;
            nudSemestre.TabIndex = 7;
            cmbTurno.TabIndex = 8;
            dtpFechaIngreso.TabIndex = 9;
            txtGrupo.TabIndex = 10;
            txtEmail.TabIndex = 11;
            txtTelefono.TabIndex = 12;
            dtpFechaNacimiento.TabIndex = 13;
            btnGuardar.TabIndex = 14;
            btnCancelar.TabIndex = 15;

            // Cargar carreras y configurar controles enterprise
            CargarCarreras();
            ConfigurarControlesEnterprise();

            // Paginacion
            CrearPaginacion();

            // Undock toolbar para poder redimensionarlo manualmente
            panelToolbar.Dock = DockStyle.None;

            // Redimensionar controles al cambiar tamaño
            this.Resize += (s, e) => ReorganizarLayout();
        }

        private void CargarCarreras()
        {
            try
            {
                _carreras = new CarreraController().ReadActivas() ?? new List<Carrera>();
                // Insert "Sin carrera" placeholder at index 0
                _carreras.Insert(0, new Carrera { Id_Carrera = 0, Nombre = "(Sin carrera)" });

                // Reset combo to avoid stale SelectedIndex issues
                cmbCarrera.DataSource = null;
                cmbCarrera.DisplayMember = "Nombre";
                cmbCarrera.ValueMember = "Id_Carrera";
                cmbCarrera.DataSource = _carreras;

                // Seleccionar placeholder solo si hay items
                if (cmbCarrera.Items.Count > 0)
                    cmbCarrera.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Logger.Error("Error cargando carreras", ex);
                MessageBox.Show("No se pudieron cargar las carreras: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _carreras = new List<Carrera> { new Carrera { Id_Carrera = 0, Nombre = "(Sin carrera)" } };
                cmbCarrera.DataSource = null;
                cmbCarrera.DisplayMember = "Nombre";
                cmbCarrera.ValueMember = "Id_Carrera";
                cmbCarrera.DataSource = _carreras;
                if (cmbCarrera.Items.Count > 0)
                    cmbCarrera.SelectedIndex = 0;
            }
        }

        private void ConfigurarControlesEnterprise()
        {
            // CURP: uppercase, max 18 chars
            txtCURP.CharacterCasing = CharacterCasing.Upper;
            txtCURP.MaxLength = 18;

            // Semestre: range 1-20
            nudSemestre.Minimum = 1;
            nudSemestre.Maximum = 20;
            nudSemestre.Value = 1;
            nudSemestre.Width = 80;

            // Genero combo — W-2: "(Sin especificar)" as placeholder at index 0
            cmbGenero.Items.Clear();
            cmbGenero.Items.AddRange(new[] { "(Sin especificar)", Genero.Masculino, Genero.Femenino, Genero.NoBinario, Genero.PrefieroNoDecir });
            cmbGenero.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGenero.SelectedIndex = 0;

            // Turno combo: mostrar legible, guardar en minusculas (constraint BD)
            cmbTurno.Items.Clear();
            cmbTurno.Items.AddRange(new[] { "Matutino", "Vespertino", "Nocturno", "Mixto" });
            cmbTurno.DropDownStyle = ComboBoxStyle.DropDownList;

            // Fecha_Ingreso DateTimePicker (always has value - no checkbox)
            dtpFechaIngreso.Format = DateTimePickerFormat.Custom;
            dtpFechaIngreso.CustomFormat = "dd/MM/yyyy";
            dtpFechaIngreso.ShowCheckBox = false;
            dtpFechaIngreso.Value = DateTime.Today;
            dtpFechaIngreso.MinDate = new DateTime(1990, 1, 1);
            dtpFechaIngreso.MaxDate = DateTime.Today;

            // Grupo
            txtGrupo.MaxLength = 10;
        }

        private string ValidarCURP(string curp)
        {
            if (AlumnoValidator.ValidarCurp(curp, out string error))
                return null;
            return error;
        }

        private string ObtenerNombreCarrera(int? idCarrera)
        {
            if (!idCarrera.HasValue || idCarrera.Value == 0) return "";
            var c = _carreras.FirstOrDefault(x => x.Id_Carrera == idCarrera.Value);
            return c?.Nombre ?? "";
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

            // Filtro de status a la izquierda
            cmbFiltroStatus.Location = new Point(12, 12);

            // Buscador centrado en el toolbar
            int searchW = 200;
            int searchX = Math.Max(140, (cw - searchW) / 2);
            lblBuscarIcono.Location = new Point(searchX - 28, 14);
            txtBuscar.Location = new Point(searchX, 12);
            txtBuscar.Width = searchW;

            // Boton limpiar busqueda a la derecha del search
            btnLimpiarBusqueda.Location = new Point(searchX + searchW + 6, 14);

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
                AplicarFiltros();
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

        private void AplicarFiltros()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            string statusFiltro = cmbFiltroStatus.SelectedItem?.ToString() ?? "Todos";

            var query = _alumnosCache.AsEnumerable();

            // Filtro texto
            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(a =>
                    (a.No_Control?.ToLower().Contains(filtro) ?? false) ||
                    (a.Nombre?.ToLower().Contains(filtro) ?? false) ||
                    (a.Apellido_Paterno?.ToLower().Contains(filtro) ?? false) ||
                    (a.Apellido_Materno?.ToLower().Contains(filtro) ?? false) ||
                    (a.Email?.ToLower().Contains(filtro) ?? false)
                );
            }

            // Filtro status
            if (statusFiltro != "Todos")
            {
                string statusKey = statusFiltro.ToLower() switch
                {
                    "activos" => Estatus.AlumnoActivo,
                    "inactivos" => Estatus.AlumnoInactivo,
                    "egresados" => Estatus.AlumnoEgresado,
                    "suspendidos" => Estatus.AlumnoSuspendido,
                    "baja" => Estatus.AlumnoBaja,
                    _ => ""
                };
                query = query.Where(a => a.Status_Alumno?.ToLower() == statusKey);
            }

            _alumnosFiltrados = query.ToList();
            ActualizarContador();
        }

        private void ConfigurarGrid()
        {
            // Phase 1: explicitly bind only the legacy columns so new enterprise
            // properties (CURP, Carrera, Semestre, Genero, etc.) never appear in the grid.
            dgvAlumnos.AutoGenerateColumns = false;

            dgvAlumnos.Columns.Clear();

            var cols = new[]
            {
                new DataGridViewTextBoxColumn { DataPropertyName = "No_Control",       HeaderText = "No. Control",   Name = "No_Control" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Nombre",           HeaderText = "Nombre",        Name = "Nombre" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Apellido_Paterno", HeaderText = "Ap. Paterno",   Name = "Apellido_Paterno" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Apellido_Materno", HeaderText = "Ap. Materno",   Name = "Apellido_Materno" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Email",            HeaderText = "Email",         Name = "Email" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Telefono",         HeaderText = "Telefono",      Name = "Telefono" },
                new DataGridViewTextBoxColumn { DataPropertyName = "Status_Alumno",    HeaderText = "Status",        Name = "Status_Alumno" }
            };
            dgvAlumnos.Columns.AddRange(cols);

            // Hide all non-legacy columns explicitly so future enterprise additions
            // are invisible until Phase 2 adds them to the grid.
            var ocultar = new[] {
                "Id_Alumno", "Created_At", "Updated_At",
                "Fecha_Nacimiento", "Password", "PasswordHash", "Privilegios",
                "Curp", "Rfc", "Nss", "Genero", "Estado_Civil",
                "Nacionalidad", "Direccion_Calle", "Direccion_Numero",
                "Direccion_Colonia", "Direccion_Ciudad", "Direccion_Estado",
                "Direccion_Cp", "Telefono_Fijo",
                "Contacto_Emergencia_Nombre", "Contacto_Emergencia_Telefono",
                "Contacto_Emergencia_Parentesco", "Id_Carrera",
                "Semestre", "Grupo", "Turno", "Fecha_Ingreso",
                "Fecha_Egreso", "Fecha_Baja", "Motivo_Baja",
                "Promedio_General", "Created_By", "Updated_By",
                "Deleted_By", "Deleted_Reason", "Status_Change_Reason"
            };
            foreach (var col in ocultar)
                if (dgvAlumnos.Columns.Contains(col))
                    dgvAlumnos.Columns[col].Visible = false;

            string[] orden = { "No_Control", "Nombre", "Apellido_Paterno", "Apellido_Materno", "Email", "Telefono", "Status_Alumno" };
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
        private void txtBuscar_TextChanged(object sender, EventArgs e) => AplicarFiltros();

        // ==================== CRUD ====================

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            HabilitarNoControl(true);
            panelCardDatos.Visible = true;
            dtpFechaNacimiento.Checked = false;
            dtpFechaIngreso.Value = DateTime.Now;
            dtpFechaIngreso.Checked = true;
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

            // Collect deletion reason — required by AlumnoController.Delete(int, string)
            string razon = Microsoft.VisualBasic.Interaction.InputBox(
                "Motivo de eliminación (requerido):",
                "Eliminar Alumno",
                "");
            if (string.IsNullOrWhiteSpace(razon))
            {
                MostrarAdvertencia("La eliminación fue cancelada. El motivo es requerido.");
                return;
            }

            int id = (int)dgvAlumnos.SelectedRows[0].Cells["Id_Alumno"].Value;
            try
            {
                if (controller.Delete(id, razon))
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
                Curp = string.IsNullOrWhiteSpace(txtCURP.Text) ? null : txtCURP.Text.Trim().ToUpperInvariant(),
                Genero = cmbGenero.SelectedIndex > 0 ? cmbGenero.Items[cmbGenero.SelectedIndex].ToString() : null,  // W-2: skip placeholder at 0
                Id_Carrera = cmbCarrera.SelectedValue is int idC && idC > 0 ? idC : (int?)null,
                Semestre = (int)nudSemestre.Value,
                Turno = cmbTurno.SelectedIndex > 0 ? cmbTurno.SelectedItem?.ToString().ToLowerInvariant() : null,
                Fecha_Ingreso = dtpFechaIngreso.Value.Date,
                Grupo = string.IsNullOrWhiteSpace(txtGrupo.Text) ? null : txtGrupo.Text.Trim()
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
            if (nc.Length < MIN_NO_CONTROL || nc.Length > MAX_NO_CONTROL) { MarcarError(txtNoControl); return $"Debe tener exactamente {MIN_NO_CONTROL} caracteres."; }
            if (!System.Text.RegularExpressions.Regex.IsMatch(nc, AlumnoConfig.NoControlPattern)) { MarcarError(txtNoControl); return "Solo se permiten letras mayúsculas y números."; }

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
                if (soloDigitos.Length != MAX_TELEFONO)
                { MarcarError(txtTelefono); return $"El teléfono debe tener exactamente {MAX_TELEFONO} dígitos."; }
            }

            if (dtpFechaNacimiento.Checked)
            {
                if (dtpFechaNacimiento.Value > DateTime.Now)
                { MarcarError(txtFechaNacimiento); return "La fecha no puede ser futura."; }
            }

            // --- Enterprise field validations ---

            // CURP: required on new, optional on edit
            string curp = txtCURP.Text.Trim();
            if (isNewRecord && string.IsNullOrWhiteSpace(curp))
                return "El CURP es requerido para nuevos alumnos.";
            if (!string.IsNullOrWhiteSpace(curp))
            {
                string curpError = ValidarCURP(curp);
                if (curpError != null) return curpError;
            }

            // Genero requerido para nuevos alumnos
            if (isNewRecord && cmbGenero.SelectedIndex <= 0)
                return "El género es requerido.";

            // Carrera requerida para nuevos alumnos
            if (isNewRecord && (cmbCarrera.SelectedValue == null || (int)cmbCarrera.SelectedValue <= 0))
                return "La carrera es requerida.";

            // Turno requerido para nuevos alumnos
            if (isNewRecord && cmbTurno.SelectedIndex < 0)
                return "El turno es requerido.";

            // Fecha_Ingreso
            DateTime fechaIng = dtpFechaIngreso.Value.Date;
            if (fechaIng > DateTime.Today)
                return "La fecha de ingreso no puede ser futura.";
            if (fechaIng < new DateTime(1990, 1, 1))
                return "La fecha de ingreso no puede ser anterior a 1990.";

            // Semestre (1-20)
            int semestre = (int)nudSemestre.Value;
            if (semestre < 1 || semestre > 20)
                return "El semestre debe estar entre 1 y 20.";

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

            // Enterprise fields
            txtCURP.Text = a.Curp ?? "";

            // Genero: select by string match (W-2: items shifted by placeholder at 0)
            if (!string.IsNullOrEmpty(a.Genero))
            {
                int idx = cmbGenero.Items.IndexOf(a.Genero);
                // IndexOf returns position in items list (placeholder at 0 shifts everything)
                cmbGenero.SelectedIndex = idx > 0 ? idx : 0;  // default to placeholder if not found
            }
            else
            {
                cmbGenero.SelectedIndex = 0;  // placeholder
            }

            // Carrera
            if (a.Id_Carrera.HasValue && a.Id_Carrera.Value > 0)
                cmbCarrera.SelectedValue = a.Id_Carrera.Value;
            else
                cmbCarrera.SelectedIndex = 0;

            // Semestre
            nudSemestre.Value = a.Semestre.HasValue ? Math.Max(1, Math.Min(20, a.Semestre.Value)) : 1;

            // Turno: BD almacena minusculas, combo muestra TitleCase
            if (!string.IsNullOrEmpty(a.Turno))
            {
                string display = char.ToUpper(a.Turno[0]) + a.Turno.Substring(1);
                int idx = cmbTurno.Items.IndexOf(display);
                cmbTurno.SelectedIndex = idx >= 0 ? idx : 0;
            }
            else
            {
                cmbTurno.SelectedIndex = 0;
            }

            // Fecha_Ingreso
            if (a.Fecha_Ingreso.HasValue)
            {
                dtpFechaIngreso.Value = a.Fecha_Ingreso.Value;
                dtpFechaIngreso.Checked = true;
            }
            else
            {
                dtpFechaIngreso.Checked = false;
            }

            // Grupo
            txtGrupo.Text = a.Grupo ?? "";
        }

        private void LimpiarFormulario()
        {
            foreach (var txt in new[] { txtIdAlumno, txtNoControl, txtNombre, txtApellidoPaterno,
                txtApellidoMaterno, txtEmail, txtTelefono, txtCURP, txtGrupo })
                txt.Clear();
            cmbGenero.SelectedIndex = 0;  // W-2: placeholder "(Sin especificar)"
            cmbCarrera.SelectedIndex = 0;
            nudSemestre.Value = 1;
            cmbTurno.SelectedIndex = 0;  // W-1: default Matutino
            dtpFechaNacimiento.Checked = false;
            dtpFechaIngreso.Value = DateTime.Today;
            dtpFechaIngreso.Checked = true;  // W-3: always has value (no checkbox)
            RestaurarColores();
        }

        // ==================== HELPERS ====================

        private string NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void MostrarError(string msg) => MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        private void MostrarAdvertencia(string msg) => MessageBox.Show(msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private void MostrarÉxito(string msg) => MessageBox.Show(msg, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // ==================== EXPORTAR CSV ====================

        private string EscapeCSV(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "";
            // Proteger contra inyección de fórmulas (=, +, -, @)
            if (valor.StartsWith("=") || valor.StartsWith("+") || valor.StartsWith("-") || valor.StartsWith("@"))
                valor = "'" + valor;  // prefijo comilla simple neutraliza la fórmula
            // Escapar comillas dobles
            return "\"" + valor.Replace("\"", "\"\"") + "\"";
        }

        private void ExportarCSV()
        {
            if (_alumnosFiltrados.Count == 0)
            {
                MessageBox.Show("No hay datos para exportar.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV (*.csv)|*.csv";
                sfd.FileName = $"Alumnos_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using ( var sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        // Header
                        sw.WriteLine("No. Control,Nombre,Apellido Paterno,Apellido Materno,Email,Teléfono,Fecha Nacimiento,Status,CURP,Género,Carrera,Semestre,Turno,Fecha Ingreso,Grupo");

                        // Data
                        foreach (var a in _alumnosFiltrados)
                        {
                            string fecha = a.Fecha_Nacimiento?.ToString("yyyy-MM-dd") ?? "";
                            string fechaIng = a.Fecha_Ingreso?.ToString("dd/MM/yyyy") ?? "";
                            string carreraNombre = ObtenerNombreCarrera(a.Id_Carrera);
                            sw.WriteLine($"{EscapeCSV(a.No_Control)},{EscapeCSV(a.Nombre)},{EscapeCSV(a.Apellido_Paterno)},{EscapeCSV(a.Apellido_Materno ?? "")},{EscapeCSV(a.Email ?? "")},{EscapeCSV(a.Telefono ?? "")},{EscapeCSV(fecha)},{EscapeCSV(a.Status_Alumno)},{EscapeCSV(a.Curp ?? "")},{EscapeCSV(a.Genero ?? "")},{EscapeCSV(carreraNombre)},{EscapeCSV(a.Semestre?.ToString() ?? "")},{EscapeCSV(a.Turno ?? "")},{EscapeCSV(fechaIng)},{EscapeCSV(a.Grupo ?? "")}");
                        }
                    }
                    MessageBox.Show($"Exportados {_alumnosFiltrados.Count} registros a:\n{sfd.FileName}", "Exportación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MostrarError("Error al exportar: " + ex.Message);
                }
            }
        }

        // ==================== FILTROS DE ENTRADA ====================

        /// <summary>
        /// Solo permite letras mayusculas y digitos (para no_control).
        /// Convierte minusculas a mayusculas y bloquea todo lo demas.
        /// </summary>
        private void SoloAlfanumericoMayusculas(KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
                return;

            if (char.IsLower(e.KeyChar))
            {
                e.KeyChar = char.ToUpper(e.KeyChar);
                return;
            }

            if (!char.IsUpper(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        /// <summary>
        /// Solo permite digitos.
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

        private void dtpFechaIngreso_ValueChanged(object sender, EventArgs e)
        {

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
