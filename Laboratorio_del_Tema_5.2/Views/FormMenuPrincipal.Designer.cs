namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormMenuPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.btnAlumnos = new System.Windows.Forms.Button();
            this.btnEmpresas = new System.Windows.Forms.Button();
            this.btnProyectos = new System.Windows.Forms.Button();
            this.btnProfesores = new System.Windows.Forms.Button();
            this.btnMaterias = new System.Windows.Forms.Button();
            this.btnTemas = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnTestConnection = new System.Windows.Forms.Button();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.panelMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.Location = new System.Drawing.Point(179, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(355, 37);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Modelo Dual - Gestión";
            // 
            // btnAlumnos
            // 
            this.btnAlumnos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnAlumnos.FlatAppearance.BorderSize = 0;
            this.btnAlumnos.ForeColor = System.Drawing.Color.White;
            this.btnAlumnos.Location = new System.Drawing.Point(48, 126);
            this.btnAlumnos.Name = "btnAlumnos";
            this.btnAlumnos.Size = new System.Drawing.Size(200, 60);
            this.btnAlumnos.TabIndex = 1;
            this.btnAlumnos.Text = "Alumnos";
            this.btnAlumnos.UseVisualStyleBackColor = false;
            this.btnAlumnos.Click += new System.EventHandler(this.btnAlumnos_Click);
            // 
            // btnEmpresas
            // 
            this.btnEmpresas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(150)))), ((int)(((byte)(136)))));
            this.btnEmpresas.FlatAppearance.BorderSize = 0;
            this.btnEmpresas.ForeColor = System.Drawing.Color.White;
            this.btnEmpresas.Location = new System.Drawing.Point(278, 126);
            this.btnEmpresas.Name = "btnEmpresas";
            this.btnEmpresas.Size = new System.Drawing.Size(200, 60);
            this.btnEmpresas.TabIndex = 2;
            this.btnEmpresas.Text = "Empresas";
            this.btnEmpresas.UseVisualStyleBackColor = false;
            this.btnEmpresas.Click += new System.EventHandler(this.btnEmpresas_Click);
            // 
            // btnProyectos
            // 
            this.btnProyectos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(180)))), ((int)(((byte)(100)))));
            this.btnProyectos.FlatAppearance.BorderSize = 0;
            this.btnProyectos.ForeColor = System.Drawing.Color.White;
            this.btnProyectos.Location = new System.Drawing.Point(508, 126);
            this.btnProyectos.Name = "btnProyectos";
            this.btnProyectos.Size = new System.Drawing.Size(200, 60);
            this.btnProyectos.TabIndex = 3;
            this.btnProyectos.Text = "Proyectos";
            this.btnProyectos.UseVisualStyleBackColor = false;
            this.btnProyectos.Click += new System.EventHandler(this.btnProyectos_Click);
            // 
            // btnProfesores
            // 
            this.btnProfesores.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(180)))));
            this.btnProfesores.FlatAppearance.BorderSize = 0;
            this.btnProfesores.ForeColor = System.Drawing.Color.White;
            this.btnProfesores.Location = new System.Drawing.Point(48, 216);
            this.btnProfesores.Name = "btnProfesores";
            this.btnProfesores.Size = new System.Drawing.Size(200, 60);
            this.btnProfesores.TabIndex = 4;
            this.btnProfesores.Text = "Profesores";
            this.btnProfesores.UseVisualStyleBackColor = false;
            this.btnProfesores.Click += new System.EventHandler(this.btnProfesores_Click);
            // 
            // btnMaterias
            // 
            this.btnMaterias.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(100)))), ((int)(((byte)(50)))));
            this.btnMaterias.FlatAppearance.BorderSize = 0;
            this.btnMaterias.ForeColor = System.Drawing.Color.White;
            this.btnMaterias.Location = new System.Drawing.Point(278, 216);
            this.btnMaterias.Name = "btnMaterias";
            this.btnMaterias.Size = new System.Drawing.Size(200, 60);
            this.btnMaterias.TabIndex = 5;
            this.btnMaterias.Text = "Materias";
            this.btnMaterias.UseVisualStyleBackColor = false;
            this.btnMaterias.Click += new System.EventHandler(this.btnMaterias_Click);
            // 
            // btnTemas
            // 
            this.btnTemas.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.btnTemas.FlatAppearance.BorderSize = 0;
            this.btnTemas.ForeColor = System.Drawing.Color.White;
            this.btnTemas.Location = new System.Drawing.Point(508, 216);
            this.btnTemas.Name = "btnTemas";
            this.btnTemas.Size = new System.Drawing.Size(200, 60);
            this.btnTemas.TabIndex = 6;
            this.btnTemas.Text = "Temas";
            this.btnTemas.UseVisualStyleBackColor = false;
            this.btnTemas.Click += new System.EventHandler(this.btnTemas_Click);
            // 
            // btnSalir
            // 
            this.btnSalir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.ForeColor = System.Drawing.Color.White;
            this.btnSalir.Location = new System.Drawing.Point(508, 306);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Size = new System.Drawing.Size(200, 45);
            this.btnSalir.TabIndex = 7;
            this.btnSalir.Text = "Salir";
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnTestConnection
            // 
            this.btnTestConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnTestConnection.FlatAppearance.BorderSize = 0;
            this.btnTestConnection.ForeColor = System.Drawing.Color.White;
            this.btnTestConnection.Location = new System.Drawing.Point(48, 306);
            this.btnTestConnection.Name = "btnTestConnection";
            this.btnTestConnection.Size = new System.Drawing.Size(200, 45);
            this.btnTestConnection.TabIndex = 8;
            this.btnTestConnection.Text = "Probar Conexión MySQL";
            this.btnTestConnection.UseVisualStyleBackColor = false;
            this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
            // 
            // panelMenu
            // 
            this.panelMenu.BackColor = System.Drawing.Color.White;
            this.panelMenu.Controls.Add(this.btnSalir);
            this.panelMenu.Controls.Add(this.btnTestConnection);
            this.panelMenu.Controls.Add(this.btnAlumnos);
            this.panelMenu.Controls.Add(this.btnEmpresas);
            this.panelMenu.Controls.Add(this.btnProyectos);
            this.panelMenu.Controls.Add(this.btnProfesores);
            this.panelMenu.Controls.Add(this.btnMaterias);
            this.panelMenu.Controls.Add(this.btnTemas);
            this.panelMenu.Controls.Add(this.lblSubtitulo);
            this.panelMenu.Controls.Add(this.lblTitulo);
            this.panelMenu.Location = new System.Drawing.Point(0, 0);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(733, 500);
            this.panelMenu.TabIndex = 9;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.Gray;
            this.lblSubtitulo.Location = new System.Drawing.Point(217, 80);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(241, 17);
            this.lblSubtitulo.TabIndex = 10;
            this.lblSubtitulo.Text = "Sistema de Gestión del Modelo Dual ";
            // 
            // FormMenuPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 500);
            this.Controls.Add(this.panelMenu);
            this.Name = "FormMenuPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modelo Dual - Menú Principal";
            this.panelMenu.ResumeLayout(false);
            this.panelMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Button btnAlumnos;
        private System.Windows.Forms.Button btnEmpresas;
        private System.Windows.Forms.Button btnProyectos;
        private System.Windows.Forms.Button btnProfesores;
        private System.Windows.Forms.Button btnMaterias;
        private System.Windows.Forms.Button btnTemas;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnTestConnection;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Label lblSubtitulo;
    }
}