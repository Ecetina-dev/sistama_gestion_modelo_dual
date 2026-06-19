namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormMigracionBD
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMigracionBD));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblStepIndicator = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlNav = new System.Windows.Forms.Panel();
            this.btnIniciarMigracion = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnSiguiente = new System.Windows.Forms.Button();
            this.btnAnterior = new System.Windows.Forms.Button();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.tcSteps = new System.Windows.Forms.TabControl();
            this.tpBienvenida = new System.Windows.Forms.TabPage();
            this.lblBienvenidaPasos = new System.Windows.Forms.Label();
            this.lblBienvenidaDesc = new System.Windows.Forms.Label();
            this.lblBienvenidaTitulo = new System.Windows.Forms.Label();
            this.tpMySQL = new System.Windows.Forms.TabPage();
            this.dgvTablasMySQL = new System.Windows.Forms.DataGridView();
            this.colTabla = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFilas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColumnas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colComentario = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnRefreshMySQL = new System.Windows.Forms.Button();
            this.pnlMySQLStatus = new System.Windows.Forms.Panel();
            this.lblMySQLStatus = new System.Windows.Forms.Label();
            this.lblMySQLVersion = new System.Windows.Forms.Label();
            this.lblMySQLInstrucciones = new System.Windows.Forms.Label();
            this.tpSQLServer = new System.Windows.Forms.TabPage();
            this.grpSQLAuth = new System.Windows.Forms.GroupBox();
            this.lblSQLPassword = new System.Windows.Forms.Label();
            this.txtSQLPassword = new System.Windows.Forms.TextBox();
            this.lblSQLUser = new System.Windows.Forms.Label();
            this.txtSQLUser = new System.Windows.Forms.TextBox();
            this.rbtnSQLAuth = new System.Windows.Forms.RadioButton();
            this.rbtnWindowsAuth = new System.Windows.Forms.RadioButton();
            this.lblSQLServer = new System.Windows.Forms.Label();
            this.txtSQLServer = new System.Windows.Forms.TextBox();
            this.lblSQLDatabase = new System.Windows.Forms.Label();
            this.txtSQLDatabase = new System.Windows.Forms.TextBox();
            this.lblSQLInstrucciones = new System.Windows.Forms.Label();
            this.btnTestSQL = new System.Windows.Forms.Button();
            this.btnSkipSQL = new System.Windows.Forms.Button();
            this.lblSQLStatus = new System.Windows.Forms.Label();
            this.tpSeleccion = new System.Windows.Forms.TabPage();
            this.clbTablas = new System.Windows.Forms.CheckedListBox();
            this.btnDeseleccionarTodo = new System.Windows.Forms.Button();
            this.btnSeleccionarTodo = new System.Windows.Forms.Button();
            this.lblSeleccionInfo = new System.Windows.Forms.Label();
            this.tpMigracion = new System.Windows.Forms.TabPage();
            this.progressBarGeneral = new System.Windows.Forms.ProgressBar();
            this.lblProgresoTexto = new System.Windows.Forms.Label();
            this.dgvMigracion = new System.Windows.Forms.DataGridView();
            this.colMTable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMFilas = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMDuracion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.tpVerificacion = new System.Windows.Forms.TabPage();
            this.dgvVerificacion = new System.Windows.Forms.DataGridView();
            this.colVTabla = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVMySQL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVSQL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblVerificacionResultado = new System.Windows.Forms.Label();
            this.btnVerificar = new System.Windows.Forms.Button();
            this.tpResumen = new System.Windows.Forms.TabPage();
            this.txtResumen = new System.Windows.Forms.RichTextBox();
            this.lblResumenTitulo = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlNav.SuspendLayout();
            this.pnlBody.SuspendLayout();
            this.tcSteps.SuspendLayout();
            this.tpBienvenida.SuspendLayout();
            this.tpMySQL.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTablasMySQL)).BeginInit();
            this.pnlMySQLStatus.SuspendLayout();
            this.tpSQLServer.SuspendLayout();
            this.grpSQLAuth.SuspendLayout();
            this.tpSeleccion.SuspendLayout();
            this.tpMigracion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMigracion)).BeginInit();
            this.tpVerificacion.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerificacion)).BeginInit();
            this.tpResumen.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.lblStepIndicator);
            this.pnlHeader.Controls.Add(this.lblSubtitulo);
            this.pnlHeader.Controls.Add(this.lblTitulo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(860, 80);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblStepIndicator
            // 
            this.lblStepIndicator.AutoSize = true;
            this.lblStepIndicator.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStepIndicator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(210)))), ((int)(((byte)(240)))));
            this.lblStepIndicator.Location = new System.Drawing.Point(22, 52);
            this.lblStepIndicator.Name = "lblStepIndicator";
            this.lblStepIndicator.Size = new System.Drawing.Size(69, 15);
            this.lblStepIndicator.TabIndex = 0;
            this.lblStepIndicator.Text = "Paso 1 de 7";
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(210)))), ((int)(((byte)(240)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(22, 28);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(321, 19);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Guía de migración: MySQL → Microsoft SQL Server";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Location = new System.Drawing.Point(20, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(333, 32);
            this.lblTitulo.TabIndex = 2;
            this.lblTitulo.Text = "Migración de Base de Datos";
            // 
            // pnlNav
            // 
            this.pnlNav.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.pnlNav.Controls.Add(this.btnIniciarMigracion);
            this.pnlNav.Controls.Add(this.btnCancelar);
            this.pnlNav.Controls.Add(this.btnSiguiente);
            this.pnlNav.Controls.Add(this.btnAnterior);
            this.pnlNav.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNav.Location = new System.Drawing.Point(0, 540);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(860, 60);
            this.pnlNav.TabIndex = 1;
            // 
            // btnIniciarMigracion
            // 
            this.btnIniciarMigracion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnIniciarMigracion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIniciarMigracion.FlatAppearance.BorderSize = 0;
            this.btnIniciarMigracion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciarMigracion.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnIniciarMigracion.ForeColor = System.Drawing.Color.White;
            this.btnIniciarMigracion.Location = new System.Drawing.Point(460, 12);
            this.btnIniciarMigracion.Name = "btnIniciarMigracion";
            this.btnIniciarMigracion.Size = new System.Drawing.Size(250, 36);
            this.btnIniciarMigracion.TabIndex = 0;
            this.btnIniciarMigracion.Text = "🚀  Iniciar Migración";
            this.btnIniciarMigracion.UseVisualStyleBackColor = false;
            this.btnIniciarMigracion.Visible = false;
            this.btnIniciarMigracion.Click += new System.EventHandler(this.btnIniciarMigracion_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderSize = 0;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancelar.ForeColor = System.Drawing.Color.White;
            this.btnCancelar.Location = new System.Drawing.Point(720, 12);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(120, 36);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnSiguiente
            // 
            this.btnSiguiente.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnSiguiente.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSiguiente.FlatAppearance.BorderSize = 0;
            this.btnSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSiguiente.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSiguiente.ForeColor = System.Drawing.Color.White;
            this.btnSiguiente.Location = new System.Drawing.Point(590, 12);
            this.btnSiguiente.Name = "btnSiguiente";
            this.btnSiguiente.Size = new System.Drawing.Size(120, 36);
            this.btnSiguiente.TabIndex = 2;
            this.btnSiguiente.Text = "Siguiente >";
            this.btnSiguiente.UseVisualStyleBackColor = false;
            this.btnSiguiente.Click += new System.EventHandler(this.btnSiguiente_Click);
            // 
            // btnAnterior
            // 
            this.btnAnterior.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(58)))), ((int)(((byte)(64)))));
            this.btnAnterior.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAnterior.FlatAppearance.BorderSize = 0;
            this.btnAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnterior.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAnterior.ForeColor = System.Drawing.Color.White;
            this.btnAnterior.Location = new System.Drawing.Point(340, 12);
            this.btnAnterior.Name = "btnAnterior";
            this.btnAnterior.Size = new System.Drawing.Size(110, 36);
            this.btnAnterior.TabIndex = 3;
            this.btnAnterior.Text = "< Anterior";
            this.btnAnterior.UseVisualStyleBackColor = false;
            this.btnAnterior.Click += new System.EventHandler(this.btnAnterior_Click);
            // 
            // pnlBody
            // 
            this.pnlBody.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.pnlBody.Controls.Add(this.tcSteps);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 80);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Padding = new System.Windows.Forms.Padding(15);
            this.pnlBody.Size = new System.Drawing.Size(860, 460);
            this.pnlBody.TabIndex = 2;
            // 
            // tcSteps
            // 
            this.tcSteps.Controls.Add(this.tpBienvenida);
            this.tcSteps.Controls.Add(this.tpMySQL);
            this.tcSteps.Controls.Add(this.tpSQLServer);
            this.tcSteps.Controls.Add(this.tpSeleccion);
            this.tcSteps.Controls.Add(this.tpMigracion);
            this.tcSteps.Controls.Add(this.tpVerificacion);
            this.tcSteps.Controls.Add(this.tpResumen);
            this.tcSteps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcSteps.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tcSteps.Location = new System.Drawing.Point(15, 15);
            this.tcSteps.Name = "tcSteps";
            this.tcSteps.SelectedIndex = 0;
            this.tcSteps.Size = new System.Drawing.Size(830, 430);
            this.tcSteps.TabIndex = 0;
            // 
            // tpBienvenida
            // 
            this.tpBienvenida.BackColor = System.Drawing.Color.White;
            this.tpBienvenida.Controls.Add(this.lblBienvenidaPasos);
            this.tpBienvenida.Controls.Add(this.lblBienvenidaDesc);
            this.tpBienvenida.Controls.Add(this.lblBienvenidaTitulo);
            this.tpBienvenida.Location = new System.Drawing.Point(4, 26);
            this.tpBienvenida.Name = "tpBienvenida";
            this.tpBienvenida.Padding = new System.Windows.Forms.Padding(15);
            this.tpBienvenida.Size = new System.Drawing.Size(822, 400);
            this.tpBienvenida.TabIndex = 0;
            this.tpBienvenida.Text = "Bienvenida";
            // 
            // lblBienvenidaPasos
            // 
            this.lblBienvenidaPasos.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBienvenidaPasos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblBienvenidaPasos.Location = new System.Drawing.Point(20, 140);
            this.lblBienvenidaPasos.Name = "lblBienvenidaPasos";
            this.lblBienvenidaPasos.Size = new System.Drawing.Size(780, 250);
            this.lblBienvenidaPasos.TabIndex = 0;
            this.lblBienvenidaPasos.Text = resources.GetString("lblBienvenidaPasos.Text");
            // 
            // lblBienvenidaDesc
            // 
            this.lblBienvenidaDesc.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblBienvenidaDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblBienvenidaDesc.Location = new System.Drawing.Point(20, 60);
            this.lblBienvenidaDesc.Name = "lblBienvenidaDesc";
            this.lblBienvenidaDesc.Size = new System.Drawing.Size(780, 60);
            this.lblBienvenidaDesc.TabIndex = 1;
            this.lblBienvenidaDesc.Text = resources.GetString("lblBienvenidaDesc.Text");
            // 
            // lblBienvenidaTitulo
            // 
            this.lblBienvenidaTitulo.AutoSize = true;
            this.lblBienvenidaTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblBienvenidaTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblBienvenidaTitulo.Location = new System.Drawing.Point(33, 33);
            this.lblBienvenidaTitulo.Name = "lblBienvenidaTitulo";
            this.lblBienvenidaTitulo.Size = new System.Drawing.Size(475, 30);
            this.lblBienvenidaTitulo.TabIndex = 2;
            this.lblBienvenidaTitulo.Text = "Asistente de Migración MySQL → SQL Server";
            // 
            // tpMySQL
            // 
            this.tpMySQL.BackColor = System.Drawing.Color.White;
            this.tpMySQL.Controls.Add(this.dgvTablasMySQL);
            this.tpMySQL.Controls.Add(this.btnRefreshMySQL);
            this.tpMySQL.Controls.Add(this.pnlMySQLStatus);
            this.tpMySQL.Controls.Add(this.lblMySQLInstrucciones);
            this.tpMySQL.Location = new System.Drawing.Point(4, 26);
            this.tpMySQL.Name = "tpMySQL";
            this.tpMySQL.Padding = new System.Windows.Forms.Padding(15);
            this.tpMySQL.Size = new System.Drawing.Size(822, 400);
            this.tpMySQL.TabIndex = 1;
            this.tpMySQL.Text = "MySQL";
            // 
            // dgvTablasMySQL
            // 
            this.dgvTablasMySQL.AllowUserToAddRows = false;
            this.dgvTablasMySQL.AllowUserToDeleteRows = false;
            this.dgvTablasMySQL.AllowUserToResizeRows = false;
            this.dgvTablasMySQL.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTablasMySQL.BackgroundColor = System.Drawing.Color.White;
            this.dgvTablasMySQL.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTablasMySQL.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTabla,
            this.colFilas,
            this.colColumnas,
            this.colComentario});
            this.dgvTablasMySQL.Location = new System.Drawing.Point(5, 90);
            this.dgvTablasMySQL.Name = "dgvTablasMySQL";
            this.dgvTablasMySQL.ReadOnly = true;
            this.dgvTablasMySQL.RowHeadersVisible = false;
            this.dgvTablasMySQL.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTablasMySQL.Size = new System.Drawing.Size(800, 310);
            this.dgvTablasMySQL.TabIndex = 0;
            // 
            // colTabla
            // 
            this.colTabla.DataPropertyName = "Nombre";
            this.colTabla.HeaderText = "Tabla";
            this.colTabla.Name = "colTabla";
            this.colTabla.ReadOnly = true;
            // 
            // colFilas
            // 
            this.colFilas.DataPropertyName = "RowCount";
            this.colFilas.HeaderText = "Filas (estimado)";
            this.colFilas.Name = "colFilas";
            this.colFilas.ReadOnly = true;
            // 
            // colColumnas
            // 
            this.colColumnas.DataPropertyName = "ColumnasCount";
            this.colColumnas.HeaderText = "Columnas";
            this.colColumnas.Name = "colColumnas";
            this.colColumnas.ReadOnly = true;
            // 
            // colComentario
            // 
            this.colComentario.DataPropertyName = "Comentario";
            this.colComentario.HeaderText = "Comentario";
            this.colComentario.Name = "colComentario";
            this.colComentario.ReadOnly = true;
            // 
            // btnRefreshMySQL
            // 
            this.btnRefreshMySQL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnRefreshMySQL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshMySQL.FlatAppearance.BorderSize = 0;
            this.btnRefreshMySQL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshMySQL.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefreshMySQL.ForeColor = System.Drawing.Color.White;
            this.btnRefreshMySQL.Location = new System.Drawing.Point(690, 58);
            this.btnRefreshMySQL.Name = "btnRefreshMySQL";
            this.btnRefreshMySQL.Size = new System.Drawing.Size(120, 28);
            this.btnRefreshMySQL.TabIndex = 1;
            this.btnRefreshMySQL.Text = "🔄  Escanear";
            this.btnRefreshMySQL.UseVisualStyleBackColor = false;
            this.btnRefreshMySQL.Click += new System.EventHandler(this.btnRefreshMySQL_Click);
            // 
            // pnlMySQLStatus
            // 
            this.pnlMySQLStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.pnlMySQLStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMySQLStatus.Controls.Add(this.lblMySQLStatus);
            this.pnlMySQLStatus.Controls.Add(this.lblMySQLVersion);
            this.pnlMySQLStatus.Location = new System.Drawing.Point(5, 5);
            this.pnlMySQLStatus.Name = "pnlMySQLStatus";
            this.pnlMySQLStatus.Size = new System.Drawing.Size(800, 50);
            this.pnlMySQLStatus.TabIndex = 2;
            // 
            // lblMySQLStatus
            // 
            this.lblMySQLStatus.AutoSize = true;
            this.lblMySQLStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMySQLStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblMySQLStatus.Location = new System.Drawing.Point(10, 6);
            this.lblMySQLStatus.Name = "lblMySQLStatus";
            this.lblMySQLStatus.Size = new System.Drawing.Size(146, 19);
            this.lblMySQLStatus.TabIndex = 0;
            this.lblMySQLStatus.Text = "●  MySQL conectado";
            // 
            // lblMySQLVersion
            // 
            this.lblMySQLVersion.AutoSize = true;
            this.lblMySQLVersion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMySQLVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.lblMySQLVersion.Location = new System.Drawing.Point(10, 28);
            this.lblMySQLVersion.Name = "lblMySQLVersion";
            this.lblMySQLVersion.Size = new System.Drawing.Size(48, 15);
            this.lblMySQLVersion.TabIndex = 1;
            this.lblMySQLVersion.Text = "Versión:";
            // 
            // lblMySQLInstrucciones
            // 
            this.lblMySQLInstrucciones.AutoSize = true;
            this.lblMySQLInstrucciones.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMySQLInstrucciones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblMySQLInstrucciones.Location = new System.Drawing.Point(20, 65);
            this.lblMySQLInstrucciones.Name = "lblMySQLInstrucciones";
            this.lblMySQLInstrucciones.Size = new System.Drawing.Size(214, 15);
            this.lblMySQLInstrucciones.TabIndex = 3;
            this.lblMySQLInstrucciones.Text = "Tablas encontradas en la base de datos:";
            // 
            // tpSQLServer
            // 
            this.tpSQLServer.BackColor = System.Drawing.Color.White;
            this.tpSQLServer.Controls.Add(this.grpSQLAuth);
            this.tpSQLServer.Controls.Add(this.lblSQLServer);
            this.tpSQLServer.Controls.Add(this.txtSQLServer);
            this.tpSQLServer.Controls.Add(this.lblSQLDatabase);
            this.tpSQLServer.Controls.Add(this.txtSQLDatabase);
            this.tpSQLServer.Controls.Add(this.lblSQLInstrucciones);
            this.tpSQLServer.Controls.Add(this.btnTestSQL);
            this.tpSQLServer.Controls.Add(this.btnSkipSQL);
            this.tpSQLServer.Controls.Add(this.lblSQLStatus);
            this.tpSQLServer.Location = new System.Drawing.Point(4, 26);
            this.tpSQLServer.Name = "tpSQLServer";
            this.tpSQLServer.Padding = new System.Windows.Forms.Padding(15);
            this.tpSQLServer.Size = new System.Drawing.Size(822, 400);
            this.tpSQLServer.TabIndex = 2;
            this.tpSQLServer.Text = "SQL Server";
            // 
            // grpSQLAuth
            // 
            this.grpSQLAuth.Controls.Add(this.lblSQLPassword);
            this.grpSQLAuth.Controls.Add(this.txtSQLPassword);
            this.grpSQLAuth.Controls.Add(this.lblSQLUser);
            this.grpSQLAuth.Controls.Add(this.txtSQLUser);
            this.grpSQLAuth.Controls.Add(this.rbtnSQLAuth);
            this.grpSQLAuth.Controls.Add(this.rbtnWindowsAuth);
            this.grpSQLAuth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpSQLAuth.Location = new System.Drawing.Point(8, 120);
            this.grpSQLAuth.Name = "grpSQLAuth";
            this.grpSQLAuth.Size = new System.Drawing.Size(585, 115);
            this.grpSQLAuth.TabIndex = 0;
            this.grpSQLAuth.TabStop = false;
            // 
            // lblSQLPassword
            // 
            this.lblSQLPassword.AutoSize = true;
            this.lblSQLPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSQLPassword.Location = new System.Drawing.Point(310, 58);
            this.lblSQLPassword.Name = "lblSQLPassword";
            this.lblSQLPassword.Size = new System.Drawing.Size(60, 15);
            this.lblSQLPassword.TabIndex = 0;
            this.lblSQLPassword.Text = "Password:";
            // 
            // txtSQLPassword
            // 
            this.txtSQLPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSQLPassword.Location = new System.Drawing.Point(310, 76);
            this.txtSQLPassword.Name = "txtSQLPassword";
            this.txtSQLPassword.PasswordChar = '●';
            this.txtSQLPassword.Size = new System.Drawing.Size(250, 25);
            this.txtSQLPassword.TabIndex = 1;
            // 
            // lblSQLUser
            // 
            this.lblSQLUser.AutoSize = true;
            this.lblSQLUser.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSQLUser.Location = new System.Drawing.Point(10, 58);
            this.lblSQLUser.Name = "lblSQLUser";
            this.lblSQLUser.Size = new System.Drawing.Size(50, 15);
            this.lblSQLUser.TabIndex = 2;
            this.lblSQLUser.Text = "Usuario:";
            // 
            // txtSQLUser
            // 
            this.txtSQLUser.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSQLUser.Location = new System.Drawing.Point(10, 76);
            this.txtSQLUser.Name = "txtSQLUser";
            this.txtSQLUser.Size = new System.Drawing.Size(250, 25);
            this.txtSQLUser.TabIndex = 3;
            this.txtSQLUser.Text = "sa";
            // 
            // rbtnSQLAuth
            // 
            this.rbtnSQLAuth.AutoSize = true;
            this.rbtnSQLAuth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbtnSQLAuth.Location = new System.Drawing.Point(190, 22);
            this.rbtnSQLAuth.Name = "rbtnSQLAuth";
            this.rbtnSQLAuth.Size = new System.Drawing.Size(158, 19);
            this.rbtnSQLAuth.TabIndex = 4;
            this.rbtnSQLAuth.Text = "Autenticación SQL Server";
            this.rbtnSQLAuth.UseVisualStyleBackColor = true;
            this.rbtnSQLAuth.CheckedChanged += new System.EventHandler(this.rbtnSQLAuth_CheckedChanged);
            // 
            // rbtnWindowsAuth
            // 
            this.rbtnWindowsAuth.AutoSize = true;
            this.rbtnWindowsAuth.Checked = true;
            this.rbtnWindowsAuth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rbtnWindowsAuth.Location = new System.Drawing.Point(10, 22);
            this.rbtnWindowsAuth.Name = "rbtnWindowsAuth";
            this.rbtnWindowsAuth.Size = new System.Drawing.Size(167, 19);
            this.rbtnWindowsAuth.TabIndex = 5;
            this.rbtnWindowsAuth.TabStop = true;
            this.rbtnWindowsAuth.Text = "Autenticación de Windows";
            this.rbtnWindowsAuth.UseVisualStyleBackColor = true;
            this.rbtnWindowsAuth.CheckedChanged += new System.EventHandler(this.rbtnSQLAuth_CheckedChanged);
            // 
            // lblSQLServer
            // 
            this.lblSQLServer.AutoSize = true;
            this.lblSQLServer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSQLServer.Location = new System.Drawing.Point(23, 58);
            this.lblSQLServer.Name = "lblSQLServer";
            this.lblSQLServer.Size = new System.Drawing.Size(53, 15);
            this.lblSQLServer.TabIndex = 1;
            this.lblSQLServer.Text = "Servidor:";
            // 
            // txtSQLServer
            // 
            this.txtSQLServer.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSQLServer.Location = new System.Drawing.Point(8, 80);
            this.txtSQLServer.Name = "txtSQLServer";
            this.txtSQLServer.Size = new System.Drawing.Size(350, 25);
            this.txtSQLServer.TabIndex = 2;
            this.txtSQLServer.Text = "localhost";
            // 
            // lblSQLDatabase
            // 
            this.lblSQLDatabase.AutoSize = true;
            this.lblSQLDatabase.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSQLDatabase.Location = new System.Drawing.Point(385, 54);
            this.lblSQLDatabase.Name = "lblSQLDatabase";
            this.lblSQLDatabase.Size = new System.Drawing.Size(68, 15);
            this.lblSQLDatabase.TabIndex = 3;
            this.lblSQLDatabase.Text = "BD Destino:";
            // 
            // txtSQLDatabase
            // 
            this.txtSQLDatabase.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSQLDatabase.Location = new System.Drawing.Point(370, 80);
            this.txtSQLDatabase.Name = "txtSQLDatabase";
            this.txtSQLDatabase.Size = new System.Drawing.Size(230, 25);
            this.txtSQLDatabase.TabIndex = 4;
            this.txtSQLDatabase.Text = "ModeloDualDB_SQL";
            // 
            // lblSQLInstrucciones
            // 
            this.lblSQLInstrucciones.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSQLInstrucciones.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblSQLInstrucciones.Location = new System.Drawing.Point(8, 8);
            this.lblSQLInstrucciones.Name = "lblSQLInstrucciones";
            this.lblSQLInstrucciones.Size = new System.Drawing.Size(800, 50);
            this.lblSQLInstrucciones.TabIndex = 5;
            this.lblSQLInstrucciones.Text = "Configurá la conexión al servidor SQL Server donde se va a migrar la base de dato" +
    "s.\r\nLa base de datos destino debe existir. Creala con: CREATE DATABASE [ModeloDu" +
    "alDB_SQL];";
            // 
            // btnTestSQL
            // 
            this.btnTestSQL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnTestSQL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestSQL.FlatAppearance.BorderSize = 0;
            this.btnTestSQL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestSQL.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnTestSQL.ForeColor = System.Drawing.Color.White;
            this.btnTestSQL.Location = new System.Drawing.Point(8, 250);
            this.btnTestSQL.Name = "btnTestSQL";
            this.btnTestSQL.Size = new System.Drawing.Size(200, 36);
            this.btnTestSQL.TabIndex = 6;
            this.btnTestSQL.Text = "🔌  Probar conexión";
            this.btnTestSQL.UseVisualStyleBackColor = false;
            this.btnTestSQL.Click += new System.EventHandler(this.btnTestSQL_Click);
            // 
            // btnSkipSQL
            // 
            this.btnSkipSQL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnSkipSQL.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSkipSQL.FlatAppearance.BorderSize = 0;
            this.btnSkipSQL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSkipSQL.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSkipSQL.ForeColor = System.Drawing.Color.White;
            this.btnSkipSQL.Location = new System.Drawing.Point(220, 250);
            this.btnSkipSQL.Name = "btnSkipSQL";
            this.btnSkipSQL.Size = new System.Drawing.Size(180, 36);
            this.btnSkipSQL.TabIndex = 7;
            this.btnSkipSQL.Text = "⏭  Omitir (ver scripts)";
            this.btnSkipSQL.UseVisualStyleBackColor = false;
            this.btnSkipSQL.Click += new System.EventHandler(this.btnSkipSQL_Click);
            // 
            // lblSQLStatus
            // 
            this.lblSQLStatus.AutoSize = true;
            this.lblSQLStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblSQLStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblSQLStatus.Location = new System.Drawing.Point(425, 271);
            this.lblSQLStatus.Name = "lblSQLStatus";
            this.lblSQLStatus.Size = new System.Drawing.Size(136, 19);
            this.lblSQLStatus.TabIndex = 8;
            this.lblSQLStatus.Text = "⏳  Sin probar aún";
            // 
            // tpSeleccion
            // 
            this.tpSeleccion.BackColor = System.Drawing.Color.White;
            this.tpSeleccion.Controls.Add(this.clbTablas);
            this.tpSeleccion.Controls.Add(this.btnDeseleccionarTodo);
            this.tpSeleccion.Controls.Add(this.btnSeleccionarTodo);
            this.tpSeleccion.Controls.Add(this.lblSeleccionInfo);
            this.tpSeleccion.Location = new System.Drawing.Point(4, 26);
            this.tpSeleccion.Name = "tpSeleccion";
            this.tpSeleccion.Padding = new System.Windows.Forms.Padding(15);
            this.tpSeleccion.Size = new System.Drawing.Size(822, 400);
            this.tpSeleccion.TabIndex = 3;
            this.tpSeleccion.Text = "Selección";
            // 
            // clbTablas
            // 
            this.clbTablas.CheckOnClick = true;
            this.clbTablas.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.clbTablas.FormattingEnabled = true;
            this.clbTablas.Location = new System.Drawing.Point(8, 40);
            this.clbTablas.Name = "clbTablas";
            this.clbTablas.Size = new System.Drawing.Size(800, 364);
            this.clbTablas.TabIndex = 0;
            // 
            // btnDeseleccionarTodo
            // 
            this.btnDeseleccionarTodo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnDeseleccionarTodo.Location = new System.Drawing.Point(108, 8);
            this.btnDeseleccionarTodo.Name = "btnDeseleccionarTodo";
            this.btnDeseleccionarTodo.Size = new System.Drawing.Size(90, 28);
            this.btnDeseleccionarTodo.TabIndex = 1;
            this.btnDeseleccionarTodo.Text = "Ninguna";
            this.btnDeseleccionarTodo.UseVisualStyleBackColor = true;
            this.btnDeseleccionarTodo.Click += new System.EventHandler(this.btnDeseleccionarTodo_Click);
            // 
            // btnSeleccionarTodo
            // 
            this.btnSeleccionarTodo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnSeleccionarTodo.Location = new System.Drawing.Point(8, 8);
            this.btnSeleccionarTodo.Name = "btnSeleccionarTodo";
            this.btnSeleccionarTodo.Size = new System.Drawing.Size(90, 28);
            this.btnSeleccionarTodo.TabIndex = 2;
            this.btnSeleccionarTodo.Text = "Todas";
            this.btnSeleccionarTodo.UseVisualStyleBackColor = true;
            this.btnSeleccionarTodo.Click += new System.EventHandler(this.btnSeleccionarTodo_Click);
            // 
            // lblSeleccionInfo
            // 
            this.lblSeleccionInfo.AutoSize = true;
            this.lblSeleccionInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSeleccionInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblSeleccionInfo.Location = new System.Drawing.Point(235, 29);
            this.lblSeleccionInfo.Name = "lblSeleccionInfo";
            this.lblSeleccionInfo.Size = new System.Drawing.Size(216, 15);
            this.lblSeleccionInfo.TabIndex = 3;
            this.lblSeleccionInfo.Text = "Seleccioná las tablas que querés migrar:";
            // 
            // tpMigracion
            // 
            this.tpMigracion.BackColor = System.Drawing.Color.White;
            this.tpMigracion.Controls.Add(this.progressBarGeneral);
            this.tpMigracion.Controls.Add(this.lblProgresoTexto);
            this.tpMigracion.Controls.Add(this.dgvMigracion);
            this.tpMigracion.Controls.Add(this.txtLog);
            this.tpMigracion.Location = new System.Drawing.Point(4, 26);
            this.tpMigracion.Name = "tpMigracion";
            this.tpMigracion.Padding = new System.Windows.Forms.Padding(15);
            this.tpMigracion.Size = new System.Drawing.Size(822, 400);
            this.tpMigracion.TabIndex = 4;
            this.tpMigracion.Text = "Migración";
            // 
            // progressBarGeneral
            // 
            this.progressBarGeneral.Location = new System.Drawing.Point(8, 8);
            this.progressBarGeneral.Name = "progressBarGeneral";
            this.progressBarGeneral.Size = new System.Drawing.Size(800, 22);
            this.progressBarGeneral.TabIndex = 0;
            // 
            // lblProgresoTexto
            // 
            this.lblProgresoTexto.AutoSize = true;
            this.lblProgresoTexto.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProgresoTexto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblProgresoTexto.Location = new System.Drawing.Point(23, 50);
            this.lblProgresoTexto.Name = "lblProgresoTexto";
            this.lblProgresoTexto.Size = new System.Drawing.Size(126, 15);
            this.lblProgresoTexto.TabIndex = 1;
            this.lblProgresoTexto.Text = "Listo para comenzar...";
            // 
            // dgvMigracion
            // 
            this.dgvMigracion.AllowUserToAddRows = false;
            this.dgvMigracion.AllowUserToDeleteRows = false;
            this.dgvMigracion.AllowUserToResizeRows = false;
            this.dgvMigracion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMigracion.BackgroundColor = System.Drawing.Color.White;
            this.dgvMigracion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMigracion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMTable,
            this.colMStatus,
            this.colMFilas,
            this.colMDuracion});
            this.dgvMigracion.Location = new System.Drawing.Point(8, 55);
            this.dgvMigracion.Name = "dgvMigracion";
            this.dgvMigracion.ReadOnly = true;
            this.dgvMigracion.RowHeadersVisible = false;
            this.dgvMigracion.Size = new System.Drawing.Size(800, 140);
            this.dgvMigracion.TabIndex = 2;
            // 
            // colMTable
            // 
            this.colMTable.DataPropertyName = "NombreTabla";
            this.colMTable.HeaderText = "Tabla";
            this.colMTable.Name = "colMTable";
            this.colMTable.ReadOnly = true;
            // 
            // colMStatus
            // 
            this.colMStatus.DataPropertyName = "StatusStr";
            this.colMStatus.HeaderText = "Estado";
            this.colMStatus.Name = "colMStatus";
            this.colMStatus.ReadOnly = true;
            // 
            // colMFilas
            // 
            this.colMFilas.DataPropertyName = "FilasStr";
            this.colMFilas.HeaderText = "Filas Migradas";
            this.colMFilas.Name = "colMFilas";
            this.colMFilas.ReadOnly = true;
            // 
            // colMDuracion
            // 
            this.colMDuracion.DataPropertyName = "DuracionStr";
            this.colMDuracion.HeaderText = "Duración";
            this.colMDuracion.Name = "colMDuracion";
            this.colMDuracion.ReadOnly = true;
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(83)))));
            this.txtLog.Location = new System.Drawing.Point(8, 200);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(800, 200);
            this.txtLog.TabIndex = 3;
            this.txtLog.Text = "";
            // 
            // tpVerificacion
            // 
            this.tpVerificacion.BackColor = System.Drawing.Color.White;
            this.tpVerificacion.Controls.Add(this.dgvVerificacion);
            this.tpVerificacion.Controls.Add(this.lblVerificacionResultado);
            this.tpVerificacion.Controls.Add(this.btnVerificar);
            this.tpVerificacion.Location = new System.Drawing.Point(4, 26);
            this.tpVerificacion.Name = "tpVerificacion";
            this.tpVerificacion.Padding = new System.Windows.Forms.Padding(15);
            this.tpVerificacion.Size = new System.Drawing.Size(822, 400);
            this.tpVerificacion.TabIndex = 5;
            this.tpVerificacion.Text = "Verificación";
            // 
            // dgvVerificacion
            // 
            this.dgvVerificacion.AllowUserToAddRows = false;
            this.dgvVerificacion.AllowUserToDeleteRows = false;
            this.dgvVerificacion.AllowUserToResizeRows = false;
            this.dgvVerificacion.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvVerificacion.BackgroundColor = System.Drawing.Color.White;
            this.dgvVerificacion.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVerificacion.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVTabla,
            this.colVMySQL,
            this.colVSQL,
            this.colVStatus});
            this.dgvVerificacion.Location = new System.Drawing.Point(8, 35);
            this.dgvVerificacion.Name = "dgvVerificacion";
            this.dgvVerificacion.ReadOnly = true;
            this.dgvVerificacion.RowHeadersVisible = false;
            this.dgvVerificacion.Size = new System.Drawing.Size(800, 290);
            this.dgvVerificacion.TabIndex = 0;
            // 
            // colVTabla
            // 
            this.colVTabla.DataPropertyName = "NombreTabla";
            this.colVTabla.HeaderText = "Tabla";
            this.colVTabla.Name = "colVTabla";
            this.colVTabla.ReadOnly = true;
            // 
            // colVMySQL
            // 
            this.colVMySQL.DataPropertyName = "FilasOrigen";
            this.colVMySQL.HeaderText = "MySQL (origen)";
            this.colVMySQL.Name = "colVMySQL";
            this.colVMySQL.ReadOnly = true;
            // 
            // colVSQL
            // 
            this.colVSQL.DataPropertyName = "FilasDestino";
            this.colVSQL.HeaderText = "SQL Server (destino)";
            this.colVSQL.Name = "colVSQL";
            this.colVSQL.ReadOnly = true;
            // 
            // colVStatus
            // 
            this.colVStatus.DataPropertyName = "StatusVerifStr";
            this.colVStatus.HeaderText = "Estado";
            this.colVStatus.Name = "colVStatus";
            this.colVStatus.ReadOnly = true;
            // 
            // lblVerificacionResultado
            // 
            this.lblVerificacionResultado.AutoSize = true;
            this.lblVerificacionResultado.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblVerificacionResultado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblVerificacionResultado.Location = new System.Drawing.Point(11, 10);
            this.lblVerificacionResultado.Name = "lblVerificacionResultado";
            this.lblVerificacionResultado.Size = new System.Drawing.Size(217, 19);
            this.lblVerificacionResultado.TabIndex = 1;
            this.lblVerificacionResultado.Text = "Verificación de datos migrados";
            // 
            // btnVerificar
            // 
            this.btnVerificar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnVerificar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerificar.FlatAppearance.BorderSize = 0;
            this.btnVerificar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerificar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnVerificar.ForeColor = System.Drawing.Color.White;
            this.btnVerificar.Location = new System.Drawing.Point(8, 340);
            this.btnVerificar.Name = "btnVerificar";
            this.btnVerificar.Size = new System.Drawing.Size(200, 36);
            this.btnVerificar.TabIndex = 2;
            this.btnVerificar.Text = "🔍  Verificar integridad";
            this.btnVerificar.UseVisualStyleBackColor = false;
            this.btnVerificar.Click += new System.EventHandler(this.btnVerificar_Click);
            // 
            // tpResumen
            // 
            this.tpResumen.BackColor = System.Drawing.Color.White;
            this.tpResumen.Controls.Add(this.txtResumen);
            this.tpResumen.Controls.Add(this.lblResumenTitulo);
            this.tpResumen.Location = new System.Drawing.Point(4, 26);
            this.tpResumen.Name = "tpResumen";
            this.tpResumen.Padding = new System.Windows.Forms.Padding(15);
            this.tpResumen.Size = new System.Drawing.Size(822, 400);
            this.tpResumen.TabIndex = 6;
            this.tpResumen.Text = "Resumen";
            // 
            // txtResumen
            // 
            this.txtResumen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtResumen.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtResumen.Location = new System.Drawing.Point(8, 40);
            this.txtResumen.Name = "txtResumen";
            this.txtResumen.ReadOnly = true;
            this.txtResumen.Size = new System.Drawing.Size(800, 365);
            this.txtResumen.TabIndex = 0;
            this.txtResumen.Text = "";
            // 
            // lblResumenTitulo
            // 
            this.lblResumenTitulo.AutoSize = true;
            this.lblResumenTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblResumenTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblResumenTitulo.Location = new System.Drawing.Point(23, 10);
            this.lblResumenTitulo.Name = "lblResumenTitulo";
            this.lblResumenTitulo.Size = new System.Drawing.Size(248, 25);
            this.lblResumenTitulo.TabIndex = 1;
            this.lblResumenTitulo.Text = "📄  Resumen de Migración";
            // 
            // FormMigracionBD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 600);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlNav);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMigracionBD";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Migración de Base de Datos - Asistente";
            this.Load += new System.EventHandler(this.FormMigracionBD_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlNav.ResumeLayout(false);
            this.pnlBody.ResumeLayout(false);
            this.tcSteps.ResumeLayout(false);
            this.tpBienvenida.ResumeLayout(false);
            this.tpBienvenida.PerformLayout();
            this.tpMySQL.ResumeLayout(false);
            this.tpMySQL.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTablasMySQL)).EndInit();
            this.pnlMySQLStatus.ResumeLayout(false);
            this.pnlMySQLStatus.PerformLayout();
            this.tpSQLServer.ResumeLayout(false);
            this.tpSQLServer.PerformLayout();
            this.grpSQLAuth.ResumeLayout(false);
            this.grpSQLAuth.PerformLayout();
            this.tpSeleccion.ResumeLayout(false);
            this.tpSeleccion.PerformLayout();
            this.tpMigracion.ResumeLayout(false);
            this.tpMigracion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMigracion)).EndInit();
            this.tpVerificacion.ResumeLayout(false);
            this.tpVerificacion.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVerificacion)).EndInit();
            this.tpResumen.ResumeLayout(false);
            this.tpResumen.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // ========== FIELD DECLARATIONS ==========

        // Header
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblStepIndicator;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblTitulo;

        // Navigation
        private System.Windows.Forms.Panel pnlNav;
        private System.Windows.Forms.Button btnIniciarMigracion;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnSiguiente;
        private System.Windows.Forms.Button btnAnterior;
        private System.Windows.Forms.Panel pnlBody;

        // TabControl
        private System.Windows.Forms.TabControl tcSteps;

        // Tab 0: Bienvenida
        private System.Windows.Forms.TabPage tpBienvenida;
        private System.Windows.Forms.Label lblBienvenidaTitulo;
        private System.Windows.Forms.Label lblBienvenidaDesc;
        private System.Windows.Forms.Label lblBienvenidaPasos;

        // Tab 1: MySQL
        private System.Windows.Forms.TabPage tpMySQL;
        private System.Windows.Forms.DataGridView dgvTablasMySQL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTabla;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFilas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColumnas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colComentario;
        private System.Windows.Forms.Button btnRefreshMySQL;
        private System.Windows.Forms.Panel pnlMySQLStatus;
        private System.Windows.Forms.Label lblMySQLVersion;
        private System.Windows.Forms.Label lblMySQLStatus;
        private System.Windows.Forms.Label lblMySQLInstrucciones;

        // Tab 2: SQL Server
        private System.Windows.Forms.TabPage tpSQLServer;
        private System.Windows.Forms.GroupBox grpSQLAuth;
        private System.Windows.Forms.Label lblSQLPassword;
        private System.Windows.Forms.TextBox txtSQLPassword;
        private System.Windows.Forms.Label lblSQLUser;
        private System.Windows.Forms.TextBox txtSQLUser;
        private System.Windows.Forms.RadioButton rbtnSQLAuth;
        private System.Windows.Forms.RadioButton rbtnWindowsAuth;
        private System.Windows.Forms.Label lblSQLServer;
        private System.Windows.Forms.TextBox txtSQLServer;
        private System.Windows.Forms.Label lblSQLDatabase;
        private System.Windows.Forms.TextBox txtSQLDatabase;
        private System.Windows.Forms.Button btnTestSQL;
        private System.Windows.Forms.Button btnSkipSQL;
        private System.Windows.Forms.Label lblSQLStatus;
        private System.Windows.Forms.Label lblSQLInstrucciones;

        // Tab 3: Selección
        private System.Windows.Forms.TabPage tpSeleccion;
        private System.Windows.Forms.CheckedListBox clbTablas;
        private System.Windows.Forms.Button btnDeseleccionarTodo;
        private System.Windows.Forms.Button btnSeleccionarTodo;
        private System.Windows.Forms.Label lblSeleccionInfo;

        // Tab 4: Migración
        private System.Windows.Forms.TabPage tpMigracion;
        private System.Windows.Forms.ProgressBar progressBarGeneral;
        private System.Windows.Forms.Label lblProgresoTexto;
        private System.Windows.Forms.DataGridView dgvMigracion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMFilas;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMDuracion;
        private System.Windows.Forms.RichTextBox txtLog;

        // Tab 5: Verificación
        private System.Windows.Forms.TabPage tpVerificacion;
        private System.Windows.Forms.DataGridView dgvVerificacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVTabla;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVMySQL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVSQL;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVStatus;
        private System.Windows.Forms.Label lblVerificacionResultado;
        private System.Windows.Forms.Button btnVerificar;

        // Tab 6: Resumen
        private System.Windows.Forms.TabPage tpResumen;
        private System.Windows.Forms.RichTextBox txtResumen;
        private System.Windows.Forms.Label lblResumenTitulo;
    }
}
