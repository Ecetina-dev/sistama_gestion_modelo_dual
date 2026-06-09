namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormMaterias
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.panelSidebarFooter = new System.Windows.Forms.Panel();
            this.lblSideVersion = new System.Windows.Forms.Label();
            this.panelSidebarNav = new System.Windows.Forms.Panel();
            this.btnNavVolver = new System.Windows.Forms.Button();
            this.lblNavSeccion = new System.Windows.Forms.Label();
            this.lblSideTitulo = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelContent = new System.Windows.Forms.Panel();
            this.panelCardDatos = new System.Windows.Forms.Panel();
            this.panelCardActions = new System.Windows.Forms.Panel();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.panelCardBar = new System.Windows.Forms.Panel();
            this.panelCardBody = new System.Windows.Forms.Panel();
            this.lineaCard = new System.Windows.Forms.Label();
            this.lblCardTitulo = new System.Windows.Forms.Label();
            this.txtIdMateria = new System.Windows.Forms.TextBox();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.nudHorasPractica = new System.Windows.Forms.NumericUpDown();
            this.lblHorasPractica = new System.Windows.Forms.Label();
            this.nudHorasTeoria = new System.Windows.Forms.NumericUpDown();
            this.lblHorasTeoria = new System.Windows.Forms.Label();
            this.nudSemestre = new System.Windows.Forms.NumericUpDown();
            this.lblSemestre = new System.Windows.Forms.Label();
            this.nudCreditos = new System.Windows.Forms.NumericUpDown();
            this.lblCreditos = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblClave = new System.Windows.Forms.Label();
            this.dgvMaterias = new System.Windows.Forms.DataGridView();
            this.panelToolbar = new System.Windows.Forms.Panel();
            this.txtBuscar = new System.Windows.Forms.TextBox();
            this.lblBuscarIcono = new System.Windows.Forms.Label();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.panelTopBar = new System.Windows.Forms.Panel();
            this.lblSeparador = new System.Windows.Forms.Label();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelSidebar.SuspendLayout();
            this.panelSidebarFooter.SuspendLayout();
            this.panelSidebarNav.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.panelContent.SuspendLayout();
            this.panelCardDatos.SuspendLayout();
            this.panelCardActions.SuspendLayout();
            this.panelCardBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHorasPractica)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHorasTeoria)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSemestre)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCreditos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterias)).BeginInit();
            this.panelToolbar.SuspendLayout();
            this.panelTopBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(35)))), ((int)(((byte)(51)))));
            this.panelSidebar.Controls.Add(this.panelSidebarFooter);
            this.panelSidebar.Controls.Add(this.panelSidebarNav);
            this.panelSidebar.Controls.Add(this.lblSideTitulo);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(165, 569);
            this.panelSidebar.TabIndex = 0;
            // 
            // panelSidebarFooter
            // 
            this.panelSidebarFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(26)))), ((int)(((byte)(40)))));
            this.panelSidebarFooter.Controls.Add(this.lblSideVersion);
            this.panelSidebarFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelSidebarFooter.Location = new System.Drawing.Point(0, 504);
            this.panelSidebarFooter.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelSidebarFooter.Name = "panelSidebarFooter";
            this.panelSidebarFooter.Padding = new System.Windows.Forms.Padding(11, 8, 11, 8);
            this.panelSidebarFooter.Size = new System.Drawing.Size(165, 65);
            this.panelSidebarFooter.TabIndex = 2;
            // 
            // lblSideVersion
            // 
            this.lblSideVersion.AutoSize = true;
            this.lblSideVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSideVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(168)))), ((int)(((byte)(240)))));
            this.lblSideVersion.Location = new System.Drawing.Point(11, 10);
            this.lblSideVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSideVersion.Name = "lblSideVersion";
            this.lblSideVersion.Size = new System.Drawing.Size(91, 15);
            this.lblSideVersion.TabIndex = 0;
            this.lblSideVersion.Text = "v2.0 Enterprise";
            // 
            // panelSidebarNav
            // 
            this.panelSidebarNav.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(35)))), ((int)(((byte)(51)))));
            this.panelSidebarNav.Controls.Add(this.btnNavVolver);
            this.panelSidebarNav.Controls.Add(this.lblNavSeccion);
            this.panelSidebarNav.Location = new System.Drawing.Point(0, 57);
            this.panelSidebarNav.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelSidebarNav.Name = "panelSidebarNav";
            this.panelSidebarNav.Size = new System.Drawing.Size(165, 447);
            this.panelSidebarNav.TabIndex = 1;
            // 
            // btnNavVolver
            // 
            this.btnNavVolver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(35)))), ((int)(((byte)(51)))));
            this.btnNavVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNavVolver.FlatAppearance.BorderSize = 0;
            this.btnNavVolver.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.btnNavVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNavVolver.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnNavVolver.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
            this.btnNavVolver.Location = new System.Drawing.Point(8, 37);
            this.btnNavVolver.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnNavVolver.Name = "btnNavVolver";
            this.btnNavVolver.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.btnNavVolver.Size = new System.Drawing.Size(150, 34);
            this.btnNavVolver.TabIndex = 1;
            this.btnNavVolver.Text = "←  Volver al Menu";
            this.btnNavVolver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNavVolver.UseVisualStyleBackColor = false;
            this.btnNavVolver.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // lblNavSeccion
            // 
            this.lblNavSeccion.AutoSize = true;
            this.lblNavSeccion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNavSeccion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(130)))), ((int)(((byte)(150)))));
            this.lblNavSeccion.Location = new System.Drawing.Point(15, 12);
            this.lblNavSeccion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNavSeccion.Name = "lblNavSeccion";
            this.lblNavSeccion.Size = new System.Drawing.Size(64, 15);
            this.lblNavSeccion.TabIndex = 0;
            this.lblNavSeccion.Text = "ACCIONES";
            // 
            // lblSideTitulo
            // 
            this.lblSideTitulo.AutoSize = true;
            this.lblSideTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblSideTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(168)))), ((int)(((byte)(240)))));
            this.lblSideTitulo.Location = new System.Drawing.Point(15, 20);
            this.lblSideTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSideTitulo.Name = "lblSideTitulo";
            this.lblSideTitulo.Size = new System.Drawing.Size(126, 25);
            this.lblSideTitulo.TabIndex = 0;
            this.lblSideTitulo.Text = "📚  Materias";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.panelMain.Controls.Add(this.panelContent);
            this.panelMain.Controls.Add(this.panelTopBar);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(165, 0);
            this.panelMain.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(660, 569);
            this.panelMain.TabIndex = 1;
            // 
            // panelContent
            // 
            this.panelContent.AutoScroll = true;
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.panelContent.Controls.Add(this.panelCardDatos);
            this.panelContent.Controls.Add(this.dgvMaterias);
            this.panelContent.Controls.Add(this.panelToolbar);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 65);
            this.panelContent.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelContent.Name = "panelContent";
            this.panelContent.Padding = new System.Windows.Forms.Padding(19, 12, 19, 12);
            this.panelContent.Size = new System.Drawing.Size(660, 504);
            this.panelContent.TabIndex = 1;
            // 
            // panelCardDatos
            // 
            this.panelCardDatos.BackColor = System.Drawing.Color.White;
            this.panelCardDatos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCardDatos.Controls.Add(this.panelCardActions);
            this.panelCardDatos.Controls.Add(this.panelCardBar);
            this.panelCardDatos.Controls.Add(this.panelCardBody);
            this.panelCardDatos.Location = new System.Drawing.Point(19, 296);
            this.panelCardDatos.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelCardDatos.Name = "panelCardDatos";
            this.panelCardDatos.Size = new System.Drawing.Size(623, 268);
            this.panelCardDatos.TabIndex = 2;
            this.panelCardDatos.Visible = false;
            // 
            // panelCardActions
            // 
            this.panelCardActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.panelCardActions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCardActions.Controls.Add(this.btnCancelar);
            this.panelCardActions.Controls.Add(this.btnGuardar);
            this.panelCardActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCardActions.Location = new System.Drawing.Point(4, 220);
            this.panelCardActions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelCardActions.Name = "panelCardActions";
            this.panelCardActions.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelCardActions.Size = new System.Drawing.Size(617, 46);
            this.panelCardActions.TabIndex = 2;
            // 
            // btnCancelar
            // 
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnCancelar.Location = new System.Drawing.Point(458, 8);
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(75, 29);
            this.btnCancelar.TabIndex = 1;
            this.btnCancelar.Text = "Cancelar";
            this.toolTip.SetToolTip(this.btnCancelar, "Descartar cambios");
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Location = new System.Drawing.Point(540, 8);
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(68, 29);
            this.btnGuardar.TabIndex = 0;
            this.btnGuardar.Text = "✔️  Guardar";
            this.toolTip.SetToolTip(this.btnGuardar, "Guardar cambios");
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // panelCardBar
            // 
            this.panelCardBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.panelCardBar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelCardBar.Location = new System.Drawing.Point(0, 0);
            this.panelCardBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelCardBar.Name = "panelCardBar";
            this.panelCardBar.Size = new System.Drawing.Size(4, 266);
            this.panelCardBar.TabIndex = 0;
            // 
            // panelCardBody
            // 
            this.panelCardBody.BackColor = System.Drawing.Color.White;
            this.panelCardBody.Controls.Add(this.lineaCard);
            this.panelCardBody.Controls.Add(this.lblCardTitulo);
            this.panelCardBody.Controls.Add(this.txtIdMateria);
            this.panelCardBody.Controls.Add(this.txtDescripcion);
            this.panelCardBody.Controls.Add(this.lblDescripcion);
            this.panelCardBody.Controls.Add(this.nudHorasPractica);
            this.panelCardBody.Controls.Add(this.lblHorasPractica);
            this.panelCardBody.Controls.Add(this.nudHorasTeoria);
            this.panelCardBody.Controls.Add(this.lblHorasTeoria);
            this.panelCardBody.Controls.Add(this.nudSemestre);
            this.panelCardBody.Controls.Add(this.lblSemestre);
            this.panelCardBody.Controls.Add(this.nudCreditos);
            this.panelCardBody.Controls.Add(this.lblCreditos);
            this.panelCardBody.Controls.Add(this.txtNombre);
            this.panelCardBody.Controls.Add(this.lblNombre);
            this.panelCardBody.Controls.Add(this.txtClave);
            this.panelCardBody.Controls.Add(this.lblClave);
            this.panelCardBody.Location = new System.Drawing.Point(4, 0);
            this.panelCardBody.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelCardBody.Name = "panelCardBody";
            this.panelCardBody.Size = new System.Drawing.Size(617, 218);
            this.panelCardBody.TabIndex = 1;
            // 
            // lineaCard
            // 
            this.lineaCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lineaCard.Location = new System.Drawing.Point(14, 32);
            this.lineaCard.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lineaCard.Name = "lineaCard";
            this.lineaCard.Size = new System.Drawing.Size(30, 2);
            this.lineaCard.TabIndex = 1;
            // 
            // lblCardTitulo
            // 
            this.lblCardTitulo.AutoSize = true;
            this.lblCardTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.lblCardTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblCardTitulo.Location = new System.Drawing.Point(14, 10);
            this.lblCardTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCardTitulo.Name = "lblCardTitulo";
            this.lblCardTitulo.Size = new System.Drawing.Size(212, 25);
            this.lblCardTitulo.TabIndex = 0;
            this.lblCardTitulo.Text = "📋  Datos de la Materia";
            // 
            // txtIdMateria
            // 
            this.txtIdMateria.Location = new System.Drawing.Point(472, 109);
            this.txtIdMateria.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtIdMateria.Name = "txtIdMateria";
            this.txtIdMateria.ReadOnly = true;
            this.txtIdMateria.Size = new System.Drawing.Size(46, 20);
            this.txtIdMateria.TabIndex = 7;
            this.txtIdMateria.Visible = false;
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtDescripcion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtDescripcion.Location = new System.Drawing.Point(14, 163);
            this.txtDescripcion.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescripcion.Size = new System.Drawing.Size(590, 45);
            this.txtDescripcion.TabIndex = 6;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDescripcion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblDescripcion.Location = new System.Drawing.Point(14, 148);
            this.lblDescripcion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(75, 15);
            this.lblDescripcion.TabIndex = 8;
            this.lblDescripcion.Text = "Descripcion";
            // 
            // nudHorasPractica
            // 
            this.nudHorasPractica.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.nudHorasPractica.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudHorasPractica.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.nudHorasPractica.Location = new System.Drawing.Point(467, 109);
            this.nudHorasPractica.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudHorasPractica.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.nudHorasPractica.Name = "nudHorasPractica";
            this.nudHorasPractica.Size = new System.Drawing.Size(135, 29);
            this.nudHorasPractica.TabIndex = 5;
            this.nudHorasPractica.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblHorasPractica
            // 
            this.lblHorasPractica.AutoSize = true;
            this.lblHorasPractica.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHorasPractica.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblHorasPractica.Location = new System.Drawing.Point(467, 96);
            this.lblHorasPractica.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHorasPractica.Name = "lblHorasPractica";
            this.lblHorasPractica.Size = new System.Drawing.Size(84, 15);
            this.lblHorasPractica.TabIndex = 17;
            this.lblHorasPractica.Text = "H. Practica";
            // 
            // nudHorasTeoria
            // 
            this.nudHorasTeoria.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.nudHorasTeoria.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudHorasTeoria.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.nudHorasTeoria.Location = new System.Drawing.Point(316, 109);
            this.nudHorasTeoria.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudHorasTeoria.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.nudHorasTeoria.Name = "nudHorasTeoria";
            this.nudHorasTeoria.Size = new System.Drawing.Size(135, 29);
            this.nudHorasTeoria.TabIndex = 4;
            this.nudHorasTeoria.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblHorasTeoria
            // 
            this.lblHorasTeoria.AutoSize = true;
            this.lblHorasTeoria.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHorasTeoria.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblHorasTeoria.Location = new System.Drawing.Point(316, 96);
            this.lblHorasTeoria.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblHorasTeoria.Name = "lblHorasTeoria";
            this.lblHorasTeoria.Size = new System.Drawing.Size(75, 15);
            this.lblHorasTeoria.TabIndex = 15;
            this.lblHorasTeoria.Text = "H. Teoria";
            // 
            // nudSemestre
            // 
            this.nudSemestre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.nudSemestre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudSemestre.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.nudSemestre.Location = new System.Drawing.Point(165, 109);
            this.nudSemestre.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudSemestre.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            this.nudSemestre.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudSemestre.Name = "nudSemestre";
            this.nudSemestre.Size = new System.Drawing.Size(135, 29);
            this.nudSemestre.TabIndex = 3;
            this.nudSemestre.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudSemestre.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblSemestre
            // 
            this.lblSemestre.AutoSize = true;
            this.lblSemestre.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSemestre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblSemestre.Location = new System.Drawing.Point(165, 96);
            this.lblSemestre.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSemestre.Name = "lblSemestre";
            this.lblSemestre.Size = new System.Drawing.Size(59, 15);
            this.lblSemestre.TabIndex = 13;
            this.lblSemestre.Text = "Semestre";
            // 
            // nudCreditos
            // 
            this.nudCreditos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.nudCreditos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudCreditos.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.nudCreditos.Location = new System.Drawing.Point(14, 109);
            this.nudCreditos.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.nudCreditos.Maximum = new decimal(new int[] { 50, 0, 0, 0 });
            this.nudCreditos.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudCreditos.Name = "nudCreditos";
            this.nudCreditos.Size = new System.Drawing.Size(135, 29);
            this.nudCreditos.TabIndex = 2;
            this.nudCreditos.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudCreditos.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblCreditos
            // 
            this.lblCreditos.AutoSize = true;
            this.lblCreditos.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCreditos.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblCreditos.Location = new System.Drawing.Point(14, 96);
            this.lblCreditos.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCreditos.Name = "lblCreditos";
            this.lblCreditos.Size = new System.Drawing.Size(57, 15);
            this.lblCreditos.TabIndex = 11;
            this.lblCreditos.Text = "Creditos";
            // 
            // txtNombre
            // 
            this.txtNombre.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtNombre.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNombre.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtNombre.Location = new System.Drawing.Point(220, 60);
            this.txtNombre.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(382, 29);
            this.txtNombre.TabIndex = 1;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNombre.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblNombre.Location = new System.Drawing.Point(220, 47);
            this.lblNombre.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(61, 15);
            this.lblNombre.TabIndex = 9;
            this.lblNombre.Text = "Nombre *";
            // 
            // txtClave
            // 
            this.txtClave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.txtClave.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtClave.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.txtClave.Location = new System.Drawing.Point(14, 60);
            this.txtClave.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(190, 29);
            this.txtClave.TabIndex = 0;
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblClave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblClave.Location = new System.Drawing.Point(14, 47);
            this.lblClave.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(82, 15);
            this.lblClave.TabIndex = 7;
            this.lblClave.Text = "Clave_Materia *";
            // 
            // dgvMaterias
            // 
            this.dgvMaterias.AllowUserToAddRows = false;
            this.dgvMaterias.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvMaterias.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvMaterias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaterias.BackgroundColor = System.Drawing.Color.White;
            this.dgvMaterias.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(35)))), ((int)(((byte)(51)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.dgvMaterias.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvMaterias.ColumnHeadersHeight = 36;
            this.dgvMaterias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvMaterias.EnableHeadersVisualStyles = false;
            this.dgvMaterias.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(232)))), ((int)(((byte)(235)))));
            this.dgvMaterias.Location = new System.Drawing.Point(19, 65);
            this.dgvMaterias.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvMaterias.MultiSelect = false;
            this.dgvMaterias.Name = "dgvMaterias";
            this.dgvMaterias.ReadOnly = true;
            this.dgvMaterias.RowHeadersVisible = false;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            this.dgvMaterias.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvMaterias.RowTemplate.Height = 32;
            this.dgvMaterias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterias.Size = new System.Drawing.Size(622, 215);
            this.dgvMaterias.TabIndex = 1;
            // 
            // panelToolbar
            // 
            this.panelToolbar.BackColor = System.Drawing.Color.White;
            this.panelToolbar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelToolbar.Controls.Add(this.txtBuscar);
            this.panelToolbar.Controls.Add(this.lblBuscarIcono);
            this.panelToolbar.Controls.Add(this.btnActualizar);
            this.panelToolbar.Controls.Add(this.btnEliminar);
            this.panelToolbar.Controls.Add(this.btnEditar);
            this.panelToolbar.Controls.Add(this.btnNuevo);
            this.panelToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelToolbar.Location = new System.Drawing.Point(19, 12);
            this.panelToolbar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelToolbar.Name = "panelToolbar";
            this.panelToolbar.Padding = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panelToolbar.Size = new System.Drawing.Size(622, 45);
            this.panelToolbar.TabIndex = 0;
            // 
            // txtBuscar
            // 
            this.txtBuscar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtBuscar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBuscar.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtBuscar.Location = new System.Drawing.Point(463, 10);
            this.txtBuscar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtBuscar.Name = "txtBuscar";
            this.txtBuscar.Size = new System.Drawing.Size(148, 27);
            this.txtBuscar.TabIndex = 1;
            this.txtBuscar.TextChanged += new System.EventHandler(this.txtBuscar_TextChanged);
            // 
            // lblBuscarIcono
            // 
            this.lblBuscarIcono.AutoSize = true;
            this.lblBuscarIcono.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblBuscarIcono.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.lblBuscarIcono.Location = new System.Drawing.Point(435, 11);
            this.lblBuscarIcono.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBuscarIcono.Name = "lblBuscarIcono";
            this.lblBuscarIcono.Size = new System.Drawing.Size(33, 25);
            this.lblBuscarIcono.TabIndex = 0;
            this.lblBuscarIcono.Text = "🔍";
            // 
            // btnActualizar
            // 
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnActualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnActualizar.FlatAppearance.BorderSize = 0;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(255, 8);
            this.btnActualizar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(79, 26);
            this.btnActualizar.TabIndex = 4;
            this.btnActualizar.Text = "🔄  Refrescar";
            this.toolTip.SetToolTip(this.btnActualizar, "Recargar lista de materias");
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnEliminar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEliminar.FlatAppearance.BorderSize = 0;
            this.btnEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEliminar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEliminar.ForeColor = System.Drawing.Color.White;
            this.btnEliminar.Location = new System.Drawing.Point(172, 8);
            this.btnEliminar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(79, 26);
            this.btnEliminar.TabIndex = 3;
            this.btnEliminar.Text = "🗑️  Eliminar";
            this.toolTip.SetToolTip(this.btnEliminar, "Eliminar materia seleccionada");
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnEditar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEditar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnEditar.Location = new System.Drawing.Point(90, 8);
            this.btnEditar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(79, 26);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "✏️  Editar";
            this.toolTip.SetToolTip(this.btnEditar, "Editar materia seleccionada");
            this.btnEditar.UseVisualStyleBackColor = false;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnNuevo
            // 
            this.btnNuevo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnNuevo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNuevo.FlatAppearance.BorderSize = 0;
            this.btnNuevo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNuevo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNuevo.ForeColor = System.Drawing.Color.White;
            this.btnNuevo.Location = new System.Drawing.Point(8, 8);
            this.btnNuevo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(79, 26);
            this.btnNuevo.TabIndex = 0;
            this.btnNuevo.Text = "➕  Nuevo";
            this.toolTip.SetToolTip(this.btnNuevo, "Agregar nueva materia");
            this.btnNuevo.UseVisualStyleBackColor = false;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // panelTopBar
            // 
            this.panelTopBar.BackColor = System.Drawing.Color.White;
            this.panelTopBar.Controls.Add(this.lblSeparador);
            this.panelTopBar.Controls.Add(this.lblSubtitulo);
            this.panelTopBar.Controls.Add(this.lblTitulo);
            this.panelTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopBar.Location = new System.Drawing.Point(0, 0);
            this.panelTopBar.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelTopBar.Name = "panelTopBar";
            this.panelTopBar.Padding = new System.Windows.Forms.Padding(19, 15, 19, 8);
            this.panelTopBar.Size = new System.Drawing.Size(660, 65);
            this.panelTopBar.TabIndex = 0;
            // 
            // lblSeparador
            // 
            this.lblSeparador.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lblSeparador.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblSeparador.Location = new System.Drawing.Point(19, 55);
            this.lblSeparador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSeparador.Name = "lblSeparador";
            this.lblSeparador.Size = new System.Drawing.Size(622, 2);
            this.lblSeparador.TabIndex = 2;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblSubtitulo.Location = new System.Drawing.Point(170, 38);
            this.lblSubtitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(325, 19);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Administra las materias registradas en el sistema";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblTitulo.Location = new System.Drawing.Point(150, 7);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(338, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "📚  Gestion de Materias";
            // 
            // FormMaterias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(825, 569);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(754, 495);
            this.Name = "FormMaterias";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestion de Materias - Modelo Dual";
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.panelSidebarFooter.ResumeLayout(false);
            this.panelSidebarFooter.PerformLayout();
            this.panelSidebarNav.ResumeLayout(false);
            this.panelSidebarNav.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelContent.ResumeLayout(false);
            this.panelCardDatos.ResumeLayout(false);
            this.panelCardActions.ResumeLayout(false);
            this.panelCardBody.ResumeLayout(false);
            this.panelCardBody.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudHorasPractica)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHorasTeoria)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSemestre)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudCreditos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterias)).EndInit();
            this.panelToolbar.ResumeLayout(false);
            this.panelToolbar.PerformLayout();
            this.panelTopBar.ResumeLayout(false);
            this.panelTopBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        // ============= FIELDS =============
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelSidebarFooter;
        private System.Windows.Forms.Label lblSideVersion;
        private System.Windows.Forms.Label lblSideTitulo;
        private System.Windows.Forms.Panel panelSidebarNav;
        private System.Windows.Forms.Button btnNavVolver;
        private System.Windows.Forms.Label lblNavSeccion;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Panel panelTopBar;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.Label lblSeparador;
        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Panel panelToolbar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Label lblBuscarIcono;
        private System.Windows.Forms.DataGridView dgvMaterias;
        private System.Windows.Forms.Panel panelCardDatos;
        private System.Windows.Forms.Panel panelCardBar;
        private System.Windows.Forms.Panel panelCardBody;
        private System.Windows.Forms.Label lblCardTitulo;
        private System.Windows.Forms.Label lineaCard;
        private System.Windows.Forms.TextBox txtIdMateria;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.NumericUpDown nudHorasPractica;
        private System.Windows.Forms.Label lblHorasPractica;
        private System.Windows.Forms.NumericUpDown nudHorasTeoria;
        private System.Windows.Forms.Label lblHorasTeoria;
        private System.Windows.Forms.NumericUpDown nudSemestre;
        private System.Windows.Forms.Label lblSemestre;
        private System.Windows.Forms.NumericUpDown nudCreditos;
        private System.Windows.Forms.Label lblCreditos;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.Panel panelCardActions;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
