#pragma warning disable CS0414
namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormLogin
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.panelContenedor = new System.Windows.Forms.Panel();
            this.panelForm = new System.Windows.Forms.Panel();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.lblError = new System.Windows.Forms.Label();
            this.picError = new System.Windows.Forms.Label();
            this.chkMostrarPassword = new System.Windows.Forms.CheckBox();
            this.lblCapsLock = new System.Windows.Forms.Label();
            this.lnkRecuperar = new System.Windows.Forms.LinkLabel();
            this.lnkCrearCuenta = new System.Windows.Forms.LinkLabel();
            this.btnIniciarSesion = new System.Windows.Forms.Button();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtLogin = new System.Windows.Forms.TextBox();
            this.lblSubtitulo = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.lblTituloLogo = new System.Windows.Forms.Label();
            this.lblSubtituloLogo = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panelForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            this.panelLogo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // panelContenedor
            // 
            this.panelContenedor.BackColor = System.Drawing.Color.White;
            this.panelContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContenedor.Location = new System.Drawing.Point(0, 0);
            this.panelContenedor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelContenedor.Name = "panelContenedor";
            this.panelContenedor.Size = new System.Drawing.Size(675, 447);
            this.panelContenedor.TabIndex = 0;
            // 
            // panelForm
            // 
            this.panelForm.BackColor = System.Drawing.Color.White;
            this.panelForm.Controls.Add(this.picLoading);
            this.panelForm.Controls.Add(this.lblError);
            this.panelForm.Controls.Add(this.picError);
            this.panelForm.Controls.Add(this.chkMostrarPassword);
            this.panelForm.Controls.Add(this.lblCapsLock);
            this.panelForm.Controls.Add(this.lnkRecuperar);
            this.panelForm.Controls.Add(this.lnkCrearCuenta);
            this.panelForm.Controls.Add(this.btnIniciarSesion);
            this.panelForm.Controls.Add(this.txtPassword);
            this.panelForm.Controls.Add(this.txtLogin);
            this.panelForm.Controls.Add(this.lblSubtitulo);
            this.panelForm.Controls.Add(this.lblTitulo);
            this.panelForm.Location = new System.Drawing.Point(300, 0);
            this.panelForm.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelForm.Name = "panelForm";
            this.panelForm.Size = new System.Drawing.Size(375, 447);
            this.panelForm.TabIndex = 1;
            // 
            // picLoading
            // 
            this.picLoading.Location = new System.Drawing.Point(345, 276);
            this.picLoading.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(22, 24);
            this.picLoading.TabIndex = 10;
            this.picLoading.TabStop = false;
            this.picLoading.Visible = false;
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblError.Location = new System.Drawing.Point(56, 134);
            this.lblError.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(281, 20);
            this.lblError.TabIndex = 2;
            this.lblError.Visible = false;
            // 
            // picError
            // 
            this.picError.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.picError.ForeColor = System.Drawing.Color.Red;
            this.picError.Location = new System.Drawing.Point(38, 134);
            this.picError.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.picError.Name = "picError";
            this.picError.Size = new System.Drawing.Size(15, 16);
            this.picError.TabIndex = 9;
            this.picError.Text = "!";
            this.picError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.picError.Visible = false;
            // 
            // chkMostrarPassword
            // 
            this.chkMostrarPassword.AutoSize = true;
            this.chkMostrarPassword.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.chkMostrarPassword.Location = new System.Drawing.Point(38, 240);
            this.chkMostrarPassword.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkMostrarPassword.Name = "chkMostrarPassword";
            this.chkMostrarPassword.Size = new System.Drawing.Size(155, 24);
            this.chkMostrarPassword.TabIndex = 5;
            this.chkMostrarPassword.Text = "Mostrar contrasena";
            this.chkMostrarPassword.UseVisualStyleBackColor = true;
            this.chkMostrarPassword.CheckedChanged += new System.EventHandler(this.chkMostrarPassword_CheckedChanged);
            // 
            // lblCapsLock
            // 
            this.lblCapsLock.AutoSize = true;
            this.lblCapsLock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCapsLock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.lblCapsLock.Location = new System.Drawing.Point(165, 240);
            this.lblCapsLock.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCapsLock.Name = "lblCapsLock";
            this.lblCapsLock.Size = new System.Drawing.Size(98, 15);
            this.lblCapsLock.TabIndex = 11;
            this.lblCapsLock.Text = "Caps Lock activo";
            this.lblCapsLock.Visible = false;
            // 
            // lnkRecuperar
            // 
            this.lnkRecuperar.AutoSize = true;
            this.lnkRecuperar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lnkRecuperar.LinkColor = System.Drawing.Color.Gray;
            this.lnkRecuperar.Location = new System.Drawing.Point(109, 353);
            this.lnkRecuperar.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkRecuperar.Name = "lnkRecuperar";
            this.lnkRecuperar.Size = new System.Drawing.Size(160, 19);
            this.lnkRecuperar.TabIndex = 8;
            this.lnkRecuperar.TabStop = true;
            this.lnkRecuperar.Text = "Olvidaste tu contrasena?";
            this.lnkRecuperar.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkRecuperar_LinkClicked);
            // 
            // lnkCrearCuenta
            // 
            this.lnkCrearCuenta.AutoSize = true;
            this.lnkCrearCuenta.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lnkCrearCuenta.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lnkCrearCuenta.Location = new System.Drawing.Point(127, 333);
            this.lnkCrearCuenta.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkCrearCuenta.Name = "lnkCrearCuenta";
            this.lnkCrearCuenta.Size = new System.Drawing.Size(124, 20);
            this.lnkCrearCuenta.TabIndex = 7;
            this.lnkCrearCuenta.TabStop = true;
            this.lnkCrearCuenta.Text = "Activar mi cuenta";
            this.lnkCrearCuenta.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCrearCuenta_LinkClicked);
            // 
            // btnIniciarSesion
            // 
            this.btnIniciarSesion.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnIniciarSesion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIniciarSesion.FlatAppearance.BorderSize = 0;
            this.btnIniciarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIniciarSesion.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.btnIniciarSesion.ForeColor = System.Drawing.Color.White;
            this.btnIniciarSesion.Location = new System.Drawing.Point(38, 276);
            this.btnIniciarSesion.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnIniciarSesion.Name = "btnIniciarSesion";
            this.btnIniciarSesion.Size = new System.Drawing.Size(300, 39);
            this.btnIniciarSesion.TabIndex = 6;
            this.btnIniciarSesion.Text = "Iniciar Sesion";
            this.btnIniciarSesion.UseVisualStyleBackColor = false;
            this.btnIniciarSesion.Click += new System.EventHandler(this.btnIniciarSesion_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.txtPassword.Location = new System.Drawing.Point(38, 207);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(300, 31);
            this.txtPassword.TabIndex = 4;
            // 
            // txtLogin
            // 
            this.txtLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtLogin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLogin.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.txtLogin.Location = new System.Drawing.Point(38, 162);
            this.txtLogin.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtLogin.Name = "txtLogin";
            this.txtLogin.Size = new System.Drawing.Size(300, 31);
            this.txtLogin.TabIndex = 3;
            // 
            // lblSubtitulo
            // 
            this.lblSubtitulo.AutoSize = true;
            this.lblSubtitulo.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.lblSubtitulo.ForeColor = System.Drawing.Color.Gray;
            this.lblSubtitulo.Location = new System.Drawing.Point(38, 93);
            this.lblSubtitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtitulo.Name = "lblSubtitulo";
            this.lblSubtitulo.Size = new System.Drawing.Size(115, 25);
            this.lblSubtitulo.TabIndex = 1;
            this.lblSubtitulo.Text = "Iniciar Sesion";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lblTitulo.Location = new System.Drawing.Point(38, 49);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(204, 47);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Bienvenido";
            // 
            // panelLogo
            // 
            this.panelLogo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.panelLogo.Controls.Add(this.lblTituloLogo);
            this.panelLogo.Controls.Add(this.lblSubtituloLogo);
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(300, 447);
            this.panelLogo.TabIndex = 0;
            // 
            // lblTituloLogo
            // 
            this.lblTituloLogo.AutoSize = true;
            this.lblTituloLogo.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTituloLogo.ForeColor = System.Drawing.Color.White;
            this.lblTituloLogo.Location = new System.Drawing.Point(52, 325);
            this.lblTituloLogo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTituloLogo.Name = "lblTituloLogo";
            this.lblTituloLogo.Size = new System.Drawing.Size(183, 37);
            this.lblTituloLogo.TabIndex = 1;
            this.lblTituloLogo.Text = "Modelo Dual";
            // 
            // lblSubtituloLogo
            // 
            this.lblSubtituloLogo.AutoSize = true;
            this.lblSubtituloLogo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblSubtituloLogo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblSubtituloLogo.Location = new System.Drawing.Point(71, 358);
            this.lblSubtituloLogo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtituloLogo.Name = "lblSubtituloLogo";
            this.lblSubtituloLogo.Size = new System.Drawing.Size(143, 21);
            this.lblSubtituloLogo.TabIndex = 2;
            this.lblSubtituloLogo.Text = "Sistema de Gestion";
            // 
            // pictureBoxLogo
            // 
            this.pictureBoxLogo.Location = new System.Drawing.Point(50, 175);
            this.pictureBoxLogo.Name = "pictureBoxLogo";
            this.pictureBoxLogo.Size = new System.Drawing.Size(300, 200);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.TabIndex = 0;
            this.pictureBoxLogo.TabStop = false;
            // 
            // FormLogin
            // 
            this.AcceptButton = this.btnIniciarSesion;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 447);
            this.Controls.Add(this.panelForm);
            this.Controls.Add(this.panelLogo);
            this.Controls.Add(this.panelContenedor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Modelo Dual - Iniciar Sesion";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLogin_FormClosing);
            this.panelForm.ResumeLayout(false);
            this.panelForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            this.panelLogo.ResumeLayout(false);
            this.panelLogo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelContenedor;
        private System.Windows.Forms.Panel panelForm;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblSubtitulo;
        private System.Windows.Forms.TextBox txtLogin;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.CheckBox chkMostrarPassword;
        private System.Windows.Forms.Button btnIniciarSesion;
        private System.Windows.Forms.LinkLabel lnkCrearCuenta;
        private System.Windows.Forms.LinkLabel lnkRecuperar;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Label picError;
        private System.Windows.Forms.PictureBox picLoading;
        private System.Windows.Forms.Label lblCapsLock;
        private System.Windows.Forms.Label lblTituloLogo;
        private System.Windows.Forms.Label lblSubtituloLogo;
    }
}
