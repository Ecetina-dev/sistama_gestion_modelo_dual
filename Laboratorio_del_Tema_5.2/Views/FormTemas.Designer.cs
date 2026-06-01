namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormTemas
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.dgvTemas = new System.Windows.Forms.DataGridView();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.txtIdTema = new System.Windows.Forms.TextBox();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.cmbMateria = new System.Windows.Forms.ComboBox();
            this.lblMateria = new System.Windows.Forms.Label();
            this.txtNumeroTema = new System.Windows.Forms.TextBox();
            this.lblNumeroTema = new System.Windows.Forms.Label();
            this.txtHorasEstimadas = new System.Windows.Forms.TextBox();
            this.lblHorasEstimadas = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemas)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            this.dgvTemas.AllowUserToAddRows = false;
            this.dgvTemas.AllowUserToDeleteRows = false;
            this.dgvTemas.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTemas.Location = new System.Drawing.Point(12, 80);
            this.dgvTemas.Name = "dgvTemas";
            this.dgvTemas.ReadOnly = true;
            this.dgvTemas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTemas.Size = new System.Drawing.Size(776, 280);
            this.dgvTemas.TabIndex = 0;
            // 
            this.btnNuevo.Location = new System.Drawing.Point(12, 12);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(100, 30);
            this.btnNuevo.TabIndex = 1;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            this.btnEditar.Location = new System.Drawing.Point(118, 12);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(100, 30);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            this.btnEliminar.Location = new System.Drawing.Point(224, 12);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(100, 30);
            this.btnEliminar.TabIndex = 3;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            this.btnActualizar.Location = new System.Drawing.Point(330, 12);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.TabIndex = 4;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(500, 14);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(155, 24);
            this.lblTitulo.TabIndex = 6;
            this.lblTitulo.Text = "Gestión de Temas";
            // 
            this.grpDatos.Controls.Add(this.txtHorasEstimadas);
            this.grpDatos.Controls.Add(this.lblHorasEstimadas);
            this.grpDatos.Controls.Add(this.txtNumeroTema);
            this.grpDatos.Controls.Add(this.lblNumeroTema);
            this.grpDatos.Controls.Add(this.cmbMateria);
            this.grpDatos.Controls.Add(this.lblMateria);
            this.grpDatos.Controls.Add(this.txtIdTema);
            this.grpDatos.Controls.Add(this.btnCancelar);
            this.grpDatos.Controls.Add(this.btnGuardar);
            this.grpDatos.Controls.Add(this.txtDescripcion);
            this.grpDatos.Controls.Add(this.lblDescripcion);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Location = new System.Drawing.Point(12, 370);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(776, 180);
            this.grpDatos.TabIndex = 7;
            this.grpDatos.Text = "Datos del Tema";
            this.grpDatos.Visible = false;
            // 
            this.txtNombre.Location = new System.Drawing.Point(120, 25);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(250, 20);
            this.txtNombre.TabIndex = 0;
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 28);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(51, 13);
            this.lblNombre.TabIndex = 1;
            this.lblNombre.Text = "Nombre:";
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(120, 60);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(250, 50);
            this.txtDescripcion.TabIndex = 2;
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(20, 63);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(70, 13);
            this.lblDescripcion.TabIndex = 3;
            this.lblDescripcion.Text = "Descripción:";
            // 
            this.btnGuardar.Location = new System.Drawing.Point(550, 130);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 30);
            this.btnGuardar.TabIndex = 4;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            this.btnCancelar.Location = new System.Drawing.Point(660, 130);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 30);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            this.txtIdTema.Location = new System.Drawing.Point(120, 130);
            this.txtIdTema.Name = "txtIdTema";
            this.txtIdTema.ReadOnly = true;
            this.txtIdTema.Size = new System.Drawing.Size(80, 20);
            this.txtIdTema.TabIndex = 6;
            this.txtIdTema.Visible = false;
            // 
            this.cmbMateria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMateria.FormattingEnabled = true;
            this.cmbMateria.Location = new System.Drawing.Point(400, 25);
            this.cmbMateria.Name = "cmbMateria";
            this.cmbMateria.Size = new System.Drawing.Size(200, 21);
            this.cmbMateria.TabIndex = 7;
            // 
            this.lblMateria.AutoSize = true;
            this.lblMateria.Location = new System.Drawing.Point(400, 28);
            this.lblMateria.Name = "lblMateria";
            this.lblMateria.Size = new System.Drawing.Size(48, 13);
            this.lblMateria.TabIndex = 8;
            this.lblMateria.Text = "Materia:";
            // 
            this.txtNumeroTema.Location = new System.Drawing.Point(400, 60);
            this.txtNumeroTema.Name = "txtNumeroTema";
            this.txtNumeroTema.Size = new System.Drawing.Size(100, 20);
            this.txtNumeroTema.TabIndex = 9;
            // 
            this.lblNumeroTema.AutoSize = true;
            this.lblNumeroTema.Location = new System.Drawing.Point(400, 63);
            this.lblNumeroTema.Name = "lblNumeroTema";
            this.lblNumeroTema.Size = new System.Drawing.Size(82, 13);
            this.lblNumeroTema.TabIndex = 10;
            this.lblNumeroTema.Text = "Número Tema:";
            // 
            this.txtHorasEstimadas.Location = new System.Drawing.Point(400, 95);
            this.txtHorasEstimadas.Name = "txtHorasEstimadas";
            this.txtHorasEstimadas.Size = new System.Drawing.Size(100, 20);
            this.txtHorasEstimadas.TabIndex = 11;
            // 
            this.lblHorasEstimadas.AutoSize = true;
            this.lblHorasEstimadas.Location = new System.Drawing.Point(400, 98);
            this.lblHorasEstimadas.Name = "lblHorasEstimadas";
            this.lblHorasEstimadas.Size = new System.Drawing.Size(97, 13);
            this.lblHorasEstimadas.TabIndex = 12;
            this.lblHorasEstimadas.Text = "Horas Estimadas:";
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 560);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.dgvTemas);
            this.Name = "FormTemas";
            this.Text = "Gestión de Temas - Modelo Dual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.dgvTemas)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.DataGridView dgvTemas;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.TextBox txtIdTema;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.ComboBox cmbMateria;
        private System.Windows.Forms.Label lblMateria;
        private System.Windows.Forms.TextBox txtNumeroTema;
        private System.Windows.Forms.Label lblNumeroTema;
        private System.Windows.Forms.TextBox txtHorasEstimadas;
        private System.Windows.Forms.Label lblHorasEstimadas;
    }
}