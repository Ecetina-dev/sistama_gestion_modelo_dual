#pragma warning disable CS0414
namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormActivarCuenta
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.panelIzquierdo = new System.Windows.Forms.Panel();
            this.lblIcono = new System.Windows.Forms.Label();
            this.lblSubtituloIzq = new System.Windows.Forms.Label();
            this.lblTituloIzq = new System.Windows.Forms.Label();
            this.panelFormulario = new System.Windows.Forms.Panel();
            this.btnActivar = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.panelCampos = new System.Windows.Forms.Panel();
            this.lblFuerza = new System.Windows.Forms.Label();
            this.lblConfirmarTxt = new System.Windows.Forms.Label();
            this.lblNuevoPwdTxt = new System.Windows.Forms.Label();
            this.lblPasswordTempTxt = new System.Windows.Forms.Label();
            this.lblUsuarioTxt = new System.Windows.Forms.Label();
            this.chkMostrarNuevo = new System.Windows.Forms.CheckBox();
            this.txtConfirmar = new System.Windows.Forms.TextBox();
            this.txtPasswordNuevo = new System.Windows.Forms.TextBox();
            this.lblLineaDivisoria = new System.Windows.Forms.Label();
            this.chkMostrarTemp = new System.Windows.Forms.CheckBox();
            this.txtPasswordTemp = new System.Windows.Forms.TextBox();
            this.txtUsuario = new System.Windows.Forms.TextBox();
            this.lblSeparador = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelIzquierdo.SuspendLayout();
            this.panelFormulario.SuspendLayout();
            this.panelCampos.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelIzquierdo
            // 
            this.panelIzquierdo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.panelIzquierdo.Controls.Add(this.lblIcono);
            this.panelIzquierdo.Controls.Add(this.lblSubtituloIzq);
            this.panelIzquierdo.Controls.Add(this.lblTituloIzq);
            this.panelIzquierdo.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelIzquierdo.Location = new System.Drawing.Point(0, 0);
            this.panelIzquierdo.Margin = new System.Windows.Forms.Padding(2);
            this.panelIzquierdo.Name = "panelIzquierdo";
            this.panelIzquierdo.Size = new System.Drawing.Size(150, 455);
            this.panelIzquierdo.TabIndex = 0;
            // 
            // lblIcono
            // 
            this.lblIcono.AutoSize = true;
            this.lblIcono.Font = new System.Drawing.Font("Segoe UI Emoji", 40F, System.Drawing.FontStyle.Bold);
            this.lblIcono.ForeColor = System.Drawing.Color.White;
            this.lblIcono.Location = new System.Drawing.Point(35, 40);
            this.lblIcono.Name = "lblIcono";
            this.lblIcono.Size = new System.Drawing.Size(104, 72);
            this.lblIcono.TabIndex = 2;
            this.lblIcono.Text = "🔑";
            // 
            // lblSubtituloIzq
            // 
            this.lblSubtituloIzq.AutoSize = true;
            this.lblSubtituloIzq.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtituloIzq.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblSubtituloIzq.Location = new System.Drawing.Point(11, 214);
            this.lblSubtituloIzq.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtituloIzq.Name = "lblSubtituloIzq";
            this.lblSubtituloIzq.Size = new System.Drawing.Size(126, 38);
            this.lblSubtituloIzq.TabIndex = 1;
            this.lblSubtituloIzq.Text = "Sistema de Gestión\r\nModelo Dual";
            // 
            // lblTituloIzq
            // 
            this.lblTituloIzq.AutoSize = true;
            this.lblTituloIzq.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTituloIzq.ForeColor = System.Drawing.Color.White;
            this.lblTituloIzq.Location = new System.Drawing.Point(19, 131);
            this.lblTituloIzq.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTituloIzq.Name = "lblTituloIzq";
            this.lblTituloIzq.Size = new System.Drawing.Size(109, 74);
            this.lblTituloIzq.TabIndex = 0;
            this.lblTituloIzq.Text = "Activar\r\nCuenta";
            // 
            // panelFormulario
            // 
            this.panelFormulario.BackColor = System.Drawing.Color.White;
            this.panelFormulario.Controls.Add(this.btnActivar);
            this.panelFormulario.Controls.Add(this.lblError);
            this.panelFormulario.Controls.Add(this.panelCampos);
            this.panelFormulario.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFormulario.Location = new System.Drawing.Point(150, 0);
            this.panelFormulario.Margin = new System.Windows.Forms.Padding(2);
            this.panelFormulario.Name = "panelFormulario";
            this.panelFormulario.Size = new System.Drawing.Size(450, 455);
            this.panelFormulario.TabIndex = 1;
            // 
            // btnActivar
            // 
            this.btnActivar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnActivar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnActivar.FlatAppearance.BorderSize = 0;
            this.btnActivar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnActivar.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.btnActivar.ForeColor = System.Drawing.Color.White;
            this.btnActivar.Location = new System.Drawing.Point(15, 388);
            this.btnActivar.Margin = new System.Windows.Forms.Padding(2);
            this.btnActivar.Name = "btnActivar";
            this.btnActivar.Size = new System.Drawing.Size(420, 39);
            this.btnActivar.TabIndex = 2;
            this.btnActivar.Text = "Activar mi Cuenta";
            this.btnActivar.UseVisualStyleBackColor = false;
            this.btnActivar.Click += new System.EventHandler(this.btnActivar_Click);
            // 
            // lblError
            // 
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblError.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.lblError.Location = new System.Drawing.Point(15, 348);
            this.lblError.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(420, 29);
            this.lblError.TabIndex = 1;
            this.lblError.Visible = false;
            // 
            // panelCampos
            // 
            this.panelCampos.BackColor = System.Drawing.Color.White;
            this.panelCampos.Controls.Add(this.lblFuerza);
            this.panelCampos.Controls.Add(this.lblConfirmarTxt);
            this.panelCampos.Controls.Add(this.lblNuevoPwdTxt);
            this.panelCampos.Controls.Add(this.lblPasswordTempTxt);
            this.panelCampos.Controls.Add(this.lblUsuarioTxt);
            this.panelCampos.Controls.Add(this.chkMostrarNuevo);
            this.panelCampos.Controls.Add(this.txtConfirmar);
            this.panelCampos.Controls.Add(this.txtPasswordNuevo);
            this.panelCampos.Controls.Add(this.lblLineaDivisoria);
            this.panelCampos.Controls.Add(this.chkMostrarTemp);
            this.panelCampos.Controls.Add(this.txtPasswordTemp);
            this.panelCampos.Controls.Add(this.txtUsuario);
            this.panelCampos.Controls.Add(this.lblSeparador);
            this.panelCampos.Controls.Add(this.lblDescripcion);
            this.panelCampos.Controls.Add(this.lblTitulo);
            this.panelCampos.Location = new System.Drawing.Point(15, 24);
            this.panelCampos.Margin = new System.Windows.Forms.Padding(2);
            this.panelCampos.Name = "panelCampos";
            this.panelCampos.Size = new System.Drawing.Size(420, 315);
            this.panelCampos.TabIndex = 0;
            // 
            // lblFuerza
            // 
            this.lblFuerza.AutoSize = true;
            this.lblFuerza.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblFuerza.Location = new System.Drawing.Point(0, 223);
            this.lblFuerza.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblFuerza.Name = "lblFuerza";
            this.lblFuerza.Size = new System.Drawing.Size(0, 15);
            this.lblFuerza.TabIndex = 8;
            this.lblFuerza.Visible = false;
            // 
            // lblConfirmarTxt
            // 
            this.lblConfirmarTxt.AutoSize = true;
            this.lblConfirmarTxt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblConfirmarTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblConfirmarTxt.Location = new System.Drawing.Point(3, 231);
            this.lblConfirmarTxt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblConfirmarTxt.Name = "lblConfirmarTxt";
            this.lblConfirmarTxt.Size = new System.Drawing.Size(154, 19);
            this.lblConfirmarTxt.TabIndex = 8;
            this.lblConfirmarTxt.Text = "Confirmar contraseña";
            // 
            // lblNuevoPwdTxt
            // 
            this.lblNuevoPwdTxt.AutoSize = true;
            this.lblNuevoPwdTxt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblNuevoPwdTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblNuevoPwdTxt.Location = new System.Drawing.Point(1, 173);
            this.lblNuevoPwdTxt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblNuevoPwdTxt.Name = "lblNuevoPwdTxt";
            this.lblNuevoPwdTxt.Size = new System.Drawing.Size(129, 19);
            this.lblNuevoPwdTxt.TabIndex = 6;
            this.lblNuevoPwdTxt.Text = "Nueva contraseña";
            // 
            // lblPasswordTempTxt
            // 
            this.lblPasswordTempTxt.AutoSize = true;
            this.lblPasswordTempTxt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblPasswordTempTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblPasswordTempTxt.Location = new System.Drawing.Point(0, 107);
            this.lblPasswordTempTxt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPasswordTempTxt.Name = "lblPasswordTempTxt";
            this.lblPasswordTempTxt.Size = new System.Drawing.Size(139, 19);
            this.lblPasswordTempTxt.TabIndex = 3;
            this.lblPasswordTempTxt.Text = "Password temporal";
            // 
            // lblUsuarioTxt
            // 
            this.lblUsuarioTxt.AutoSize = true;
            this.lblUsuarioTxt.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblUsuarioTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblUsuarioTxt.Location = new System.Drawing.Point(0, 54);
            this.lblUsuarioTxt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblUsuarioTxt.Name = "lblUsuarioTxt";
            this.lblUsuarioTxt.Size = new System.Drawing.Size(137, 19);
            this.lblUsuarioTxt.TabIndex = 2;
            this.lblUsuarioTxt.Text = "Usuario / Matrícula";
            // 
            // chkMostrarNuevo
            // 
            this.chkMostrarNuevo.AutoSize = true;
            this.chkMostrarNuevo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkMostrarNuevo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkMostrarNuevo.Location = new System.Drawing.Point(6, 287);
            this.chkMostrarNuevo.Margin = new System.Windows.Forms.Padding(2);
            this.chkMostrarNuevo.Name = "chkMostrarNuevo";
            this.chkMostrarNuevo.Size = new System.Drawing.Size(198, 23);
            this.chkMostrarNuevo.TabIndex = 10;
            this.chkMostrarNuevo.Text = "Mostrar ambas contraseñas";
            this.chkMostrarNuevo.CheckedChanged += new System.EventHandler(this.chkMostrarNuevo_CheckedChanged);
            // 
            // txtConfirmar
            // 
            this.txtConfirmar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtConfirmar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConfirmar.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.txtConfirmar.Location = new System.Drawing.Point(0, 251);
            this.txtConfirmar.Margin = new System.Windows.Forms.Padding(2);
            this.txtConfirmar.Name = "txtConfirmar";
            this.txtConfirmar.PasswordChar = '*';
            this.txtConfirmar.Size = new System.Drawing.Size(420, 31);
            this.txtConfirmar.TabIndex = 9;
            this.txtConfirmar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConfirmar_KeyDown);
            // 
            // txtPasswordNuevo
            // 
            this.txtPasswordNuevo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtPasswordNuevo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPasswordNuevo.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.txtPasswordNuevo.Location = new System.Drawing.Point(0, 195);
            this.txtPasswordNuevo.Margin = new System.Windows.Forms.Padding(2);
            this.txtPasswordNuevo.Name = "txtPasswordNuevo";
            this.txtPasswordNuevo.PasswordChar = '*';
            this.txtPasswordNuevo.Size = new System.Drawing.Size(420, 31);
            this.txtPasswordNuevo.TabIndex = 7;
            this.txtPasswordNuevo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPasswordNuevo_KeyDown);
            // 
            // lblLineaDivisoria
            // 
            this.lblLineaDivisoria.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.lblLineaDivisoria.Location = new System.Drawing.Point(0, 179);
            this.lblLineaDivisoria.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblLineaDivisoria.Name = "lblLineaDivisoria";
            this.lblLineaDivisoria.Size = new System.Drawing.Size(420, 1);
            this.lblLineaDivisoria.TabIndex = 6;
            // 
            // chkMostrarTemp
            // 
            this.chkMostrarTemp.AutoSize = true;
            this.chkMostrarTemp.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkMostrarTemp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.chkMostrarTemp.Location = new System.Drawing.Point(3, 154);
            this.chkMostrarTemp.Margin = new System.Windows.Forms.Padding(2);
            this.chkMostrarTemp.Name = "chkMostrarTemp";
            this.chkMostrarTemp.Size = new System.Drawing.Size(198, 23);
            this.chkMostrarTemp.TabIndex = 5;
            this.chkMostrarTemp.Text = "Mostrar password temporal";
            this.chkMostrarTemp.CheckedChanged += new System.EventHandler(this.chkMostrarTemp_CheckedChanged);
            // 
            // txtPasswordTemp
            // 
            this.txtPasswordTemp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtPasswordTemp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPasswordTemp.Font = new System.Drawing.Font("Consolas", 13F);
            this.txtPasswordTemp.Location = new System.Drawing.Point(0, 126);
            this.txtPasswordTemp.Margin = new System.Windows.Forms.Padding(2);
            this.txtPasswordTemp.Name = "txtPasswordTemp";
            this.txtPasswordTemp.PasswordChar = '*';
            this.txtPasswordTemp.Size = new System.Drawing.Size(420, 28);
            this.txtPasswordTemp.TabIndex = 4;
            this.txtPasswordTemp.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPasswordTemp_KeyDown);
            // 
            // txtUsuario
            // 
            this.txtUsuario.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.txtUsuario.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsuario.Font = new System.Drawing.Font("Segoe UI", 13F);
            this.txtUsuario.Location = new System.Drawing.Point(0, 76);
            this.txtUsuario.Margin = new System.Windows.Forms.Padding(2);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new System.Drawing.Size(420, 31);
            this.txtUsuario.TabIndex = 3;
            this.txtUsuario.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUsuario_KeyDown);
            // 
            // lblSeparador
            // 
            this.lblSeparador.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lblSeparador.Location = new System.Drawing.Point(0, 53);
            this.lblSeparador.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSeparador.Name = "lblSeparador";
            this.lblSeparador.Size = new System.Drawing.Size(30, 2);
            this.lblSeparador.TabIndex = 2;
            // 
            // lblDescripcion
            // 
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblDescripcion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDescripcion.Location = new System.Drawing.Point(0, 29);
            this.lblDescripcion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(297, 19);
            this.lblDescripcion.TabIndex = 1;
            this.lblDescripcion.Text = "Ingresa tus datos y elige una nueva contraseña";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(218, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Activar mi Cuenta";
            // 
            // FormActivarCuenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(600, 455);
            this.Controls.Add(this.panelFormulario);
            this.Controls.Add(this.panelIzquierdo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormActivarCuenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Activar Cuenta - Modelo Dual";
            this.panelIzquierdo.ResumeLayout(false);
            this.panelIzquierdo.PerformLayout();
            this.panelFormulario.ResumeLayout(false);
            this.panelCampos.ResumeLayout(false);
            this.panelCampos.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel panelIzquierdo;
        private System.Windows.Forms.Label lblIcono;
        private System.Windows.Forms.Label lblTituloIzq;
        private System.Windows.Forms.Label lblSubtituloIzq;
        private System.Windows.Forms.Panel panelFormulario;
        private System.Windows.Forms.Panel panelCampos;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.Label lblSeparador;
        private System.Windows.Forms.Label lblUsuarioTxt;
        private System.Windows.Forms.TextBox txtUsuario;
        private System.Windows.Forms.Label lblPasswordTempTxt;
        private System.Windows.Forms.TextBox txtPasswordTemp;
        private System.Windows.Forms.CheckBox chkMostrarTemp;
        private System.Windows.Forms.Label lblLineaDivisoria;
        private System.Windows.Forms.Label lblNuevoPwdTxt;
        private System.Windows.Forms.TextBox txtPasswordNuevo;
        private System.Windows.Forms.Label lblConfirmarTxt;
        private System.Windows.Forms.TextBox txtConfirmar;
        private System.Windows.Forms.CheckBox chkMostrarNuevo;
        private System.Windows.Forms.Label lblFuerza;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnActivar;
    }
}


