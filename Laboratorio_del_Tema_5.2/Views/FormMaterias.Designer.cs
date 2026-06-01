namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormMaterias
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.dgvMaterias = new System.Windows.Forms.DataGridView();
            this.btnNuevo = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.btnVerProyectos = new System.Windows.Forms.Button();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.grpDatos = new System.Windows.Forms.GroupBox();
            this.txtHorasPractica = new System.Windows.Forms.TextBox();
            this.lblHorasPractica = new System.Windows.Forms.Label();
            this.txtHorasTeoria = new System.Windows.Forms.TextBox();
            this.lblHorasTeoria = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtIdMateria = new System.Windows.Forms.TextBox();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.txtCreditos = new System.Windows.Forms.TextBox();
            this.lblCreditos = new System.Windows.Forms.Label();
            this.txtSemestre = new System.Windows.Forms.TextBox();
            this.lblSemestre = new System.Windows.Forms.Label();
            this.txtNombre = new System.Windows.Forms.TextBox();
            this.lblNombre = new System.Windows.Forms.Label();
            this.txtClave = new System.Windows.Forms.TextBox();
            this.lblClave = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterias)).BeginInit();
            this.grpDatos.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvMaterias
            // 
            this.dgvMaterias.AllowUserToAddRows = false;
            this.dgvMaterias.AllowUserToDeleteRows = false;
            this.dgvMaterias.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaterias.Location = new System.Drawing.Point(12, 80);
            this.dgvMaterias.Name = "dgvMaterias";
            this.dgvMaterias.ReadOnly = true;
            this.dgvMaterias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMaterias.Size = new System.Drawing.Size(784, 280);
            this.dgvMaterias.TabIndex = 0;
            // 
            // btnNuevo
            // 
            this.btnNuevo.Location = new System.Drawing.Point(12, 12);
            this.btnNuevo.Name = "btnNuevo";
            this.btnNuevo.Size = new System.Drawing.Size(100, 30);
            this.btnNuevo.TabIndex = 1;
            this.btnNuevo.Text = "Nuevo";
            this.btnNuevo.UseVisualStyleBackColor = true;
            this.btnNuevo.Click += new System.EventHandler(this.btnNuevo_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Location = new System.Drawing.Point(118, 12);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Size = new System.Drawing.Size(100, 30);
            this.btnEditar.TabIndex = 2;
            this.btnEditar.Text = "Editar";
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // btnEliminar
            // 
            this.btnEliminar.Location = new System.Drawing.Point(224, 12);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(100, 30);
            this.btnEliminar.TabIndex = 3;
            this.btnEliminar.Text = "Eliminar";
            this.btnEliminar.UseVisualStyleBackColor = true;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            // 
            // btnActualizar
            // 
            this.btnActualizar.Location = new System.Drawing.Point(330, 12);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(100, 30);
            this.btnActualizar.TabIndex = 4;
            this.btnActualizar.Text = "Actualizar";
            this.btnActualizar.UseVisualStyleBackColor = true;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // btnVerProyectos
            // 
            this.btnVerProyectos.Location = new System.Drawing.Point(436, 12);
            this.btnVerProyectos.Name = "btnVerProyectos";
            this.btnVerProyectos.Size = new System.Drawing.Size(170, 30);
            this.btnVerProyectos.TabIndex = 5;
            this.btnVerProyectos.Text = "Ver con Proyectos";
            this.btnVerProyectos.UseVisualStyleBackColor = true;
            this.btnVerProyectos.Click += new System.EventHandler(this.btnVerProyectos_Click);
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(612, 13);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(195, 24);
            this.lblTitulo.TabIndex = 6;
            this.lblTitulo.Text = "Gestión de Materias";
            // 
            // grpDatos
            // 
            this.grpDatos.Controls.Add(this.txtHorasPractica);
            this.grpDatos.Controls.Add(this.lblHorasPractica);
            this.grpDatos.Controls.Add(this.txtHorasTeoria);
            this.grpDatos.Controls.Add(this.lblHorasTeoria);
            this.grpDatos.Controls.Add(this.txtDescripcion);
            this.grpDatos.Controls.Add(this.lblDescripcion);
            this.grpDatos.Controls.Add(this.txtIdMateria);
            this.grpDatos.Controls.Add(this.btnCancelar);
            this.grpDatos.Controls.Add(this.btnGuardar);
            this.grpDatos.Controls.Add(this.txtCreditos);
            this.grpDatos.Controls.Add(this.lblCreditos);
            this.grpDatos.Controls.Add(this.txtSemestre);
            this.grpDatos.Controls.Add(this.lblSemestre);
            this.grpDatos.Controls.Add(this.txtNombre);
            this.grpDatos.Controls.Add(this.lblNombre);
            this.grpDatos.Controls.Add(this.txtClave);
            this.grpDatos.Controls.Add(this.lblClave);
            this.grpDatos.Location = new System.Drawing.Point(12, 370);
            this.grpDatos.Name = "grpDatos";
            this.grpDatos.Size = new System.Drawing.Size(776, 220);
            this.grpDatos.TabIndex = 7;
            this.grpDatos.TabStop = false;
            this.grpDatos.Text = "Datos de la Materia";
            this.grpDatos.Visible = false;
            // 
            // txtHorasPractica
            // 
            this.txtHorasPractica.Location = new System.Drawing.Point(264, 130);
            this.txtHorasPractica.Name = "txtHorasPractica";
            this.txtHorasPractica.Size = new System.Drawing.Size(60, 20);
            this.txtHorasPractica.TabIndex = 28;
            // 
            // lblHorasPractica
            // 
            this.lblHorasPractica.AutoSize = true;
            this.lblHorasPractica.Location = new System.Drawing.Point(214, 133);
            this.lblHorasPractica.Name = "lblHorasPractica";
            this.lblHorasPractica.Size = new System.Drawing.Size(43, 13);
            this.lblHorasPractica.TabIndex = 29;
            this.lblHorasPractica.Text = "H.Prac:";
            // 
            // txtHorasTeoria
            // 
            this.txtHorasTeoria.Location = new System.Drawing.Point(264, 95);
            this.txtHorasTeoria.Name = "txtHorasTeoria";
            this.txtHorasTeoria.Size = new System.Drawing.Size(60, 20);
            this.txtHorasTeoria.TabIndex = 26;
            // 
            // lblHorasTeoria
            // 
            this.lblHorasTeoria.AutoSize = true;
            this.lblHorasTeoria.Location = new System.Drawing.Point(214, 98);
            this.lblHorasTeoria.Name = "lblHorasTeoria";
            this.lblHorasTeoria.Size = new System.Drawing.Size(40, 13);
            this.lblHorasTeoria.TabIndex = 27;
            this.lblHorasTeoria.Text = "H.Teo:";
            // 
            // txtDescripcion
            // 
            this.txtDescripcion.Location = new System.Drawing.Point(500, 25);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.Size = new System.Drawing.Size(260, 80);
            this.txtDescripcion.TabIndex = 24;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new System.Drawing.Point(420, 28);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(66, 13);
            this.lblDescripcion.TabIndex = 25;
            this.lblDescripcion.Text = "Descripción:";
            // 
            // txtIdMateria
            // 
            this.txtIdMateria.Location = new System.Drawing.Point(120, 165);
            this.txtIdMateria.Name = "txtIdMateria";
            this.txtIdMateria.ReadOnly = true;
            this.txtIdMateria.Size = new System.Drawing.Size(80, 20);
            this.txtIdMateria.TabIndex = 7;
            this.txtIdMateria.Visible = false;
            // 
            // btnCancelar
            // 
            this.btnCancelar.Location = new System.Drawing.Point(660, 180);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(100, 30);
            this.btnCancelar.TabIndex = 23;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(550, 180);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(100, 30);
            this.btnGuardar.TabIndex = 22;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // txtCreditos
            // 
            this.txtCreditos.Location = new System.Drawing.Point(120, 130);
            this.txtCreditos.Name = "txtCreditos";
            this.txtCreditos.Size = new System.Drawing.Size(80, 20);
            this.txtCreditos.TabIndex = 8;
            // 
            // lblCreditos
            // 
            this.lblCreditos.AutoSize = true;
            this.lblCreditos.Location = new System.Drawing.Point(20, 133);
            this.lblCreditos.Name = "lblCreditos";
            this.lblCreditos.Size = new System.Drawing.Size(48, 13);
            this.lblCreditos.TabIndex = 9;
            this.lblCreditos.Text = "Créditos:";
            // 
            // txtSemestre
            // 
            this.txtSemestre.Location = new System.Drawing.Point(120, 95);
            this.txtSemestre.Name = "txtSemestre";
            this.txtSemestre.Size = new System.Drawing.Size(80, 20);
            this.txtSemestre.TabIndex = 10;
            // 
            // lblSemestre
            // 
            this.lblSemestre.AutoSize = true;
            this.lblSemestre.Location = new System.Drawing.Point(20, 98);
            this.lblSemestre.Name = "lblSemestre";
            this.lblSemestre.Size = new System.Drawing.Size(54, 13);
            this.lblSemestre.TabIndex = 11;
            this.lblSemestre.Text = "Semestre:";
            // 
            // txtNombre
            // 
            this.txtNombre.Location = new System.Drawing.Point(120, 60);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(250, 20);
            this.txtNombre.TabIndex = 12;
            // 
            // lblNombre
            // 
            this.lblNombre.AutoSize = true;
            this.lblNombre.Location = new System.Drawing.Point(20, 63);
            this.lblNombre.Name = "lblNombre";
            this.lblNombre.Size = new System.Drawing.Size(47, 13);
            this.lblNombre.TabIndex = 13;
            this.lblNombre.Text = "Nombre:";
            // 
            // txtClave
            // 
            this.txtClave.Location = new System.Drawing.Point(120, 25);
            this.txtClave.Name = "txtClave";
            this.txtClave.Size = new System.Drawing.Size(150, 20);
            this.txtClave.TabIndex = 14;
            // 
            // lblClave
            // 
            this.lblClave.AutoSize = true;
            this.lblClave.Location = new System.Drawing.Point(20, 28);
            this.lblClave.Name = "lblClave";
            this.lblClave.Size = new System.Drawing.Size(37, 13);
            this.lblClave.TabIndex = 15;
            this.lblClave.Text = "Clave:";
            // 
            // FormMaterias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 600);
            this.Controls.Add(this.grpDatos);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.btnVerProyectos);
            this.Controls.Add(this.btnActualizar);
            this.Controls.Add(this.btnEliminar);
            this.Controls.Add(this.btnEditar);
            this.Controls.Add(this.btnNuevo);
            this.Controls.Add(this.dgvMaterias);
            this.Name = "FormMaterias";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gestión de Materias - Modelo Dual";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaterias)).EndInit();
            this.grpDatos.ResumeLayout(false);
            this.grpDatos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.DataGridView dgvMaterias;
        private System.Windows.Forms.Button btnNuevo;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Button btnEliminar;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnVerProyectos;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.GroupBox grpDatos;
        private System.Windows.Forms.TextBox txtCreditos;
        private System.Windows.Forms.Label lblCreditos;
        private System.Windows.Forms.TextBox txtSemestre;
        private System.Windows.Forms.Label lblSemestre;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.TextBox txtClave;
        private System.Windows.Forms.Label lblClave;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.TextBox txtIdMateria;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtHorasTeoria;
        private System.Windows.Forms.Label lblHorasTeoria;
        private System.Windows.Forms.TextBox txtHorasPractica;
        private System.Windows.Forms.Label lblHorasPractica;
    }
}