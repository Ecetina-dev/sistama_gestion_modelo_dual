#pragma warning disable CS0414
namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormGestionUsuarios
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGestionUsuarios));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabCargar = new System.Windows.Forms.TabPage();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblTipoUsuario = new System.Windows.Forms.Label();
            this.cmbTipoUsuario = new System.Windows.Forms.ComboBox();
            this.lblEntidad = new System.Windows.Forms.Label();
            this.cmbEntidad = new System.Windows.Forms.ComboBox();
            this.lblRol = new System.Windows.Forms.Label();
            this.cmbRol = new System.Windows.Forms.ComboBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblPasswordTemporal = new System.Windows.Forms.Label();
            this.txtPasswordTemporal = new System.Windows.Forms.TextBox();
            this.btnGenerarPassword = new System.Windows.Forms.Button();
            this.lblPasswordGenerado = new System.Windows.Forms.Label();
            this.btnCopiarPassword = new System.Windows.Forms.Button();
            this.btnCargarUsuario = new System.Windows.Forms.Button();
            this.tabListado = new System.Windows.Forms.TabPage();
            this.dgvUsuarios = new System.Windows.Forms.DataGridView();
            this.btnCambiarStatus = new System.Windows.Forms.Button();
            this.btnResetearPassword = new System.Windows.Forms.Button();
            this.btnActualizar = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabCargar.SuspendLayout();
            this.tabListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabCargar);
            this.tabControl.Controls.Add(this.tabListado);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(900, 600);
            this.tabControl.TabIndex = 0;
            // 
            // tabCargar
            // 
            this.tabCargar.BackColor = System.Drawing.Color.White;
            this.tabCargar.Controls.Add(this.lblInfo);
            this.tabCargar.Controls.Add(this.lblTitulo);
            this.tabCargar.Controls.Add(this.lblTipoUsuario);
            this.tabCargar.Controls.Add(this.cmbTipoUsuario);
            this.tabCargar.Controls.Add(this.lblEntidad);
            this.tabCargar.Controls.Add(this.cmbEntidad);
            this.tabCargar.Controls.Add(this.lblRol);
            this.tabCargar.Controls.Add(this.cmbRol);
            this.tabCargar.Controls.Add(this.lblUsername);
            this.tabCargar.Controls.Add(this.txtUsername);
            this.tabCargar.Controls.Add(this.lblEmail);
            this.tabCargar.Controls.Add(this.txtEmail);
            this.tabCargar.Controls.Add(this.lblPasswordTemporal);
            this.tabCargar.Controls.Add(this.txtPasswordTemporal);
            this.tabCargar.Controls.Add(this.btnGenerarPassword);
            this.tabCargar.Controls.Add(this.lblPasswordGenerado);
            this.tabCargar.Controls.Add(this.btnCopiarPassword);
            this.tabCargar.Controls.Add(this.btnCargarUsuario);
            this.tabCargar.Location = new System.Drawing.Point(4, 26);
            this.tabCargar.Name = "tabCargar";
            this.tabCargar.Padding = new System.Windows.Forms.Padding(20);
            this.tabCargar.Size = new System.Drawing.Size(892, 570);
            this.tabCargar.TabIndex = 0;
            this.tabCargar.Text = "Cargar Nuevo Usuario";
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblInfo.Location = new System.Drawing.Point(20, 45);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(626, 30);
            this.lblInfo.TabIndex = 1;
            this.lblInfo.Text = resources.GetString("lblInfo.Text");
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lblTitulo.Location = new System.Drawing.Point(20, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(239, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cargar Usuario al Sistema";
            // 
            // lblTipoUsuario
            // 
            this.lblTipoUsuario.AutoSize = true;
            this.lblTipoUsuario.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTipoUsuario.Location = new System.Drawing.Point(20, 95);
            this.lblTipoUsuario.Name = "lblTipoUsuario";
            this.lblTipoUsuario.Size = new System.Drawing.Size(105, 19);
            this.lblTipoUsuario.TabIndex = 2;
            this.lblTipoUsuario.Text = "Tipo de Usuario";
            // 
            // cmbTipoUsuario
            // 
            this.cmbTipoUsuario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipoUsuario.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbTipoUsuario.Location = new System.Drawing.Point(20, 117);
            this.cmbTipoUsuario.Name = "cmbTipoUsuario";
            this.cmbTipoUsuario.Size = new System.Drawing.Size(300, 28);
            this.cmbTipoUsuario.TabIndex = 3;
            // 
            // lblEntidad
            // 
            this.lblEntidad.AutoSize = true;
            this.lblEntidad.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEntidad.Location = new System.Drawing.Point(340, 95);
            this.lblEntidad.Name = "lblEntidad";
            this.lblEntidad.Size = new System.Drawing.Size(119, 19);
            this.lblEntidad.TabIndex = 4;
            this.lblEntidad.Text = "Entidad a Vincular";
            // 
            // cmbEntidad
            // 
            this.cmbEntidad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEntidad.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbEntidad.Location = new System.Drawing.Point(340, 117);
            this.cmbEntidad.Name = "cmbEntidad";
            this.cmbEntidad.Size = new System.Drawing.Size(500, 28);
            this.cmbEntidad.TabIndex = 5;
            // 
            // lblRol
            // 
            this.lblRol.AutoSize = true;
            this.lblRol.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblRol.Location = new System.Drawing.Point(20, 160);
            this.lblRol.Name = "lblRol";
            this.lblRol.Size = new System.Drawing.Size(28, 19);
            this.lblRol.TabIndex = 6;
            this.lblRol.Text = "Rol";
            // 
            // cmbRol
            // 
            this.cmbRol.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRol.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbRol.Location = new System.Drawing.Point(20, 182);
            this.cmbRol.Name = "cmbRol";
            this.cmbRol.Size = new System.Drawing.Size(300, 28);
            this.cmbRol.TabIndex = 7;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblUsername.Location = new System.Drawing.Point(20, 225);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(71, 19);
            this.lblUsername.TabIndex = 8;
            this.lblUsername.Text = "Username";
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtUsername.Location = new System.Drawing.Point(20, 247);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(300, 27);
            this.txtUsername.TabIndex = 9;
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblEmail.Location = new System.Drawing.Point(340, 225);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(41, 19);
            this.lblEmail.TabIndex = 10;
            this.lblEmail.Text = "Email";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtEmail.Location = new System.Drawing.Point(340, 247);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(500, 27);
            this.txtEmail.TabIndex = 11;
            // 
            // lblPasswordTemporal
            // 
            this.lblPasswordTemporal.AutoSize = true;
            this.lblPasswordTemporal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPasswordTemporal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblPasswordTemporal.Location = new System.Drawing.Point(20, 290);
            this.lblPasswordTemporal.Name = "lblPasswordTemporal";
            this.lblPasswordTemporal.Size = new System.Drawing.Size(165, 19);
            this.lblPasswordTemporal.TabIndex = 12;
            this.lblPasswordTemporal.Text = "PASSWORD TEMPORAL";
            // 
            // txtPasswordTemporal
            // 
            this.txtPasswordTemporal.Font = new System.Drawing.Font("Consolas", 13F);
            this.txtPasswordTemporal.Location = new System.Drawing.Point(20, 312);
            this.txtPasswordTemporal.Name = "txtPasswordTemporal";
            this.txtPasswordTemporal.ReadOnly = true;
            this.txtPasswordTemporal.Size = new System.Drawing.Size(400, 28);
            this.txtPasswordTemporal.TabIndex = 13;
            // 
            // btnGenerarPassword
            // 
            this.btnGenerarPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.btnGenerarPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGenerarPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerarPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnGenerarPassword.ForeColor = System.Drawing.Color.White;
            this.btnGenerarPassword.Location = new System.Drawing.Point(440, 311);
            this.btnGenerarPassword.Name = "btnGenerarPassword";
            this.btnGenerarPassword.Size = new System.Drawing.Size(180, 32);
            this.btnGenerarPassword.TabIndex = 14;
            this.btnGenerarPassword.Text = "Generar Password";
            this.btnGenerarPassword.UseVisualStyleBackColor = false;
            this.btnGenerarPassword.Click += new System.EventHandler(this.btnGenerarPassword_Click);
            // 
            // lblPasswordGenerado
            // 
            this.lblPasswordGenerado.AutoSize = true;
            this.lblPasswordGenerado.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblPasswordGenerado.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblPasswordGenerado.Location = new System.Drawing.Point(20, 345);
            this.lblPasswordGenerado.Name = "lblPasswordGenerado";
            this.lblPasswordGenerado.Size = new System.Drawing.Size(0, 15);
            this.lblPasswordGenerado.TabIndex = 15;
            this.lblPasswordGenerado.Visible = false;
            // 
            // btnCopiarPassword
            // 
            this.btnCopiarPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(162)))), ((int)(((byte)(184)))));
            this.btnCopiarPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopiarPassword.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCopiarPassword.ForeColor = System.Drawing.Color.White;
            this.btnCopiarPassword.Location = new System.Drawing.Point(630, 315);
            this.btnCopiarPassword.Name = "btnCopiarPassword";
            this.btnCopiarPassword.Size = new System.Drawing.Size(100, 24);
            this.btnCopiarPassword.TabIndex = 16;
            this.btnCopiarPassword.Text = "Copiar";
            this.btnCopiarPassword.UseVisualStyleBackColor = false;
            this.btnCopiarPassword.Visible = false;
            this.btnCopiarPassword.Click += new System.EventHandler(this.btnCopiarPassword_Click);
            // 
            // btnCargarUsuario
            // 
            this.btnCargarUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnCargarUsuario.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCargarUsuario.FlatAppearance.BorderSize = 0;
            this.btnCargarUsuario.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCargarUsuario.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnCargarUsuario.ForeColor = System.Drawing.Color.White;
            this.btnCargarUsuario.Location = new System.Drawing.Point(20, 400);
            this.btnCargarUsuario.Name = "btnCargarUsuario";
            this.btnCargarUsuario.Size = new System.Drawing.Size(820, 50);
            this.btnCargarUsuario.TabIndex = 17;
            this.btnCargarUsuario.Text = "Cargar Usuario al Sistema";
            this.btnCargarUsuario.UseVisualStyleBackColor = false;
            this.btnCargarUsuario.Click += new System.EventHandler(this.btnCargarUsuario_Click);
            // 
            // tabListado
            // 
            this.tabListado.BackColor = System.Drawing.Color.White;
            this.tabListado.Controls.Add(this.dgvUsuarios);
            this.tabListado.Controls.Add(this.btnCambiarStatus);
            this.tabListado.Controls.Add(this.btnResetearPassword);
            this.tabListado.Controls.Add(this.btnActualizar);
            this.tabListado.Location = new System.Drawing.Point(4, 26);
            this.tabListado.Name = "tabListado";
            this.tabListado.Padding = new System.Windows.Forms.Padding(20);
            this.tabListado.Size = new System.Drawing.Size(892, 570);
            this.tabListado.TabIndex = 1;
            this.tabListado.Text = "Listado y Gestion";
            // 
            // dgvUsuarios
            // 
            this.dgvUsuarios.AllowUserToAddRows = false;
            this.dgvUsuarios.AllowUserToDeleteRows = false;
            this.dgvUsuarios.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUsuarios.BackgroundColor = System.Drawing.Color.White;
            this.dgvUsuarios.Location = new System.Drawing.Point(20, 20);
            this.dgvUsuarios.MultiSelect = false;
            this.dgvUsuarios.Name = "dgvUsuarios";
            this.dgvUsuarios.ReadOnly = true;
            this.dgvUsuarios.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUsuarios.Size = new System.Drawing.Size(820, 450);
            this.dgvUsuarios.TabIndex = 0;
            // 
            // btnCambiarStatus
            // 
            this.btnCambiarStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnCambiarStatus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCambiarStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCambiarStatus.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCambiarStatus.ForeColor = System.Drawing.Color.White;
            this.btnCambiarStatus.Location = new System.Drawing.Point(520, 485);
            this.btnCambiarStatus.Name = "btnCambiarStatus";
            this.btnCambiarStatus.Size = new System.Drawing.Size(180, 35);
            this.btnCambiarStatus.TabIndex = 3;
            this.btnCambiarStatus.Text = "Activar/Desactivar";
            this.btnCambiarStatus.UseVisualStyleBackColor = false;
            this.btnCambiarStatus.Click += new System.EventHandler(this.btnCambiarStatus_Click);
            // 
            // btnResetearPassword
            // 
            this.btnResetearPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnResetearPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnResetearPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResetearPassword.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnResetearPassword.ForeColor = System.Drawing.Color.Black;
            this.btnResetearPassword.Location = new System.Drawing.Point(220, 485);
            this.btnResetearPassword.Name = "btnResetearPassword";
            this.btnResetearPassword.Size = new System.Drawing.Size(280, 35);
            this.btnResetearPassword.TabIndex = 2;
            this.btnResetearPassword.Text = "Resetear Password";
            this.btnResetearPassword.UseVisualStyleBackColor = false;
            this.btnResetearPassword.Click += new System.EventHandler(this.btnResetearPassword_Click);
            // 
            // btnActualizar
            // 
            this.btnActualizar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnActualizar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnActualizar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActualizar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnActualizar.ForeColor = System.Drawing.Color.White;
            this.btnActualizar.Location = new System.Drawing.Point(20, 485);
            this.btnActualizar.Name = "btnActualizar";
            this.btnActualizar.Size = new System.Drawing.Size(180, 35);
            this.btnActualizar.TabIndex = 1;
            this.btnActualizar.Text = "Actualizar Lista";
            this.btnActualizar.UseVisualStyleBackColor = false;
            this.btnActualizar.Click += new System.EventHandler(this.btnActualizar_Click);
            // 
            // FormGestionUsuarios
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.Name = "FormGestionUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gestión de Usuarios";
            this.Load += new System.EventHandler(this.FormGestionUsuarios_Load);
            this.tabControl.ResumeLayout(false);
            this.tabCargar.ResumeLayout(false);
            this.tabCargar.PerformLayout();
            this.tabListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvUsuarios)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabCargar;
        private System.Windows.Forms.TabPage tabListado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Label lblTipoUsuario;
        private System.Windows.Forms.ComboBox cmbTipoUsuario;
        private System.Windows.Forms.Label lblEntidad;
        private System.Windows.Forms.ComboBox cmbEntidad;
        private System.Windows.Forms.Label lblRol;
        private System.Windows.Forms.ComboBox cmbRol;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblPasswordTemporal;
        private System.Windows.Forms.TextBox txtPasswordTemporal;
        private System.Windows.Forms.Button btnGenerarPassword;
        private System.Windows.Forms.Label lblPasswordGenerado;
        private System.Windows.Forms.Button btnCopiarPassword;
        private System.Windows.Forms.Button btnCargarUsuario;
        private System.Windows.Forms.DataGridView dgvUsuarios;
        private System.Windows.Forms.Button btnActualizar;
        private System.Windows.Forms.Button btnResetearPassword;
        private System.Windows.Forms.Button btnCambiarStatus;
    }
}


