#pragma warning disable CS0414
namespace Laboratorio_del_Tema_5_2.Views
{
    partial class FormCambiarPassword
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.lblTitulo = new System.Windows.Forms.Label();
            this.txtPasswordActual = new System.Windows.Forms.TextBox();
            this.txtPasswordNuevo = new System.Windows.Forms.TextBox();
            this.txtConfirmPassword = new System.Windows.Forms.TextBox();
            this.lblPasswordActual = new System.Windows.Forms.Label();
            this.lblPasswordNuevo = new System.Windows.Forms.Label();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.btnCambiar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.chkMostrarPassword = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitulo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.lblTitulo.Location = new System.Drawing.Point(30, 30);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(220, 30);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Cambiar Contrasena";
            // 
            // lblPasswordActual
            // 
            this.lblPasswordActual.AutoSize = true;
            this.lblPasswordActual.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPasswordActual.Location = new System.Drawing.Point(30, 90);
            this.lblPasswordActual.Name = "lblPasswordActual";
            this.lblPasswordActual.Size = new System.Drawing.Size(117, 19);
            this.lblPasswordActual.TabIndex = 1;
            this.lblPasswordActual.Text = "Contrasena Actual";
            // 
            // txtPasswordActual
            // 
            this.txtPasswordActual.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPasswordActual.Location = new System.Drawing.Point(30, 115);
            this.txtPasswordActual.Name = "txtPasswordActual";
            this.txtPasswordActual.PasswordChar = '*';
            this.txtPasswordActual.Size = new System.Drawing.Size(340, 25);
            this.txtPasswordActual.TabIndex = 2;
            // 
            // lblPasswordNuevo
            // 
            this.lblPasswordNuevo.AutoSize = true;
            this.lblPasswordNuevo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPasswordNuevo.Location = new System.Drawing.Point(30, 155);
            this.lblPasswordNuevo.Name = "lblPasswordNuevo";
            this.lblPasswordNuevo.Size = new System.Drawing.Size(118, 19);
            this.lblPasswordNuevo.TabIndex = 3;
            this.lblPasswordNuevo.Text = "Nueva Contrasena";
            // 
            // txtPasswordNuevo
            // 
            this.txtPasswordNuevo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPasswordNuevo.Location = new System.Drawing.Point(30, 180);
            this.txtPasswordNuevo.Name = "txtPasswordNuevo";
            this.txtPasswordNuevo.PasswordChar = '*';
            this.txtPasswordNuevo.Size = new System.Drawing.Size(340, 25);
            this.txtPasswordNuevo.TabIndex = 3;
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblConfirmPassword.Location = new System.Drawing.Point(30, 220);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(162, 19);
            this.lblConfirmPassword.TabIndex = 4;
            this.lblConfirmPassword.Text = "Confirmar Nueva Contrasena";
            // 
            // txtConfirmPassword
            // 
            this.txtConfirmPassword.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtConfirmPassword.Location = new System.Drawing.Point(30, 245);
            this.txtConfirmPassword.Name = "txtConfirmPassword";
            this.txtConfirmPassword.PasswordChar = '*';
            this.txtConfirmPassword.Size = new System.Drawing.Size(340, 25);
            this.txtConfirmPassword.TabIndex = 4;
            // 
            // chkMostrarPassword
            // 
            this.chkMostrarPassword.AutoSize = true;
            this.chkMostrarPassword.Location = new System.Drawing.Point(30, 275);
            this.chkMostrarPassword.Name = "chkMostrarPassword";
            this.chkMostrarPassword.Size = new System.Drawing.Size(131, 19);
            this.chkMostrarPassword.TabIndex = 5;
            this.chkMostrarPassword.Text = "Mostrar contrasenas";
            this.chkMostrarPassword.UseVisualStyleBackColor = true;
            this.chkMostrarPassword.CheckedChanged += new System.EventHandler(this.chkMostrarPassword_CheckedChanged);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(30, 300);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(0, 19);
            this.lblError.TabIndex = 6;
            // 
            // btnCambiar
            // 
            this.btnCambiar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(71)))), ((int)(((byte)(160)))));
            this.btnCambiar.FlatAppearance.BorderSize = 0;
            this.btnCambiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCambiar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnCambiar.ForeColor = System.Drawing.Color.White;
            this.btnCambiar.Location = new System.Drawing.Point(30, 340);
            this.btnCambiar.Name = "btnCambiar";
            this.btnCambiar.Size = new System.Drawing.Size(340, 40);
            this.btnCambiar.TabIndex = 7;
            this.btnCambiar.Text = "Cambiar Contrasena";
            this.btnCambiar.UseVisualStyleBackColor = false;
            this.btnCambiar.Click += new System.EventHandler(this.btnCambiar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnCancelar.Location = new System.Drawing.Point(30, 390);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(340, 35);
            this.btnCancelar.TabIndex = 8;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // FormCambiarPassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 450);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.lblPasswordActual);
            this.Controls.Add(this.txtPasswordActual);
            this.Controls.Add(this.lblPasswordNuevo);
            this.Controls.Add(this.txtPasswordNuevo);
            this.Controls.Add(this.lblConfirmPassword);
            this.Controls.Add(this.txtConfirmPassword);
            this.Controls.Add(this.chkMostrarPassword);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnCambiar);
            this.Controls.Add(this.btnCancelar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormCambiarPassword";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cambiar Contrasena";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.TextBox txtPasswordActual;
        private System.Windows.Forms.TextBox txtPasswordNuevo;
        private System.Windows.Forms.TextBox txtConfirmPassword;
        private System.Windows.Forms.Label lblPasswordActual;
        private System.Windows.Forms.Label lblPasswordNuevo;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.CheckBox chkMostrarPassword;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Button btnCambiar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
