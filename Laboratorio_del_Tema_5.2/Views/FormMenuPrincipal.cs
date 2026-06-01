using System;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Data;

namespace Laboratorio_del_Tema_5_2.Views
{
    /// <summary>
    /// Menu principal de la aplicacion.
    /// </summary>
    public partial class FormMenuPrincipal : Form
    {
        public FormMenuPrincipal()
        {
            InitializeComponent();
        }

        private void btnAlumnos_Click(object sender, EventArgs e)
        {
            FormAlumnos formAlumnos = new FormAlumnos();
            formAlumnos.ShowDialog();
        }

        private void btnEmpresas_Click(object sender, EventArgs e)
        {
            FormEmpresas formEmpresas = new FormEmpresas();
            formEmpresas.ShowDialog();
        }

        private void btnProyectos_Click(object sender, EventArgs e)
        {
            FormProyectos formProyectos = new FormProyectos();
            formProyectos.ShowDialog();
        }

        private void btnProfesores_Click(object sender, EventArgs e)
        {
            FormProfesores formProfesores = new FormProfesores();
            formProfesores.ShowDialog();
        }

        private void btnMaterias_Click(object sender, EventArgs e)
        {
            FormMaterias formMaterias = new FormMaterias();
            formMaterias.ShowDialog();
        }

        private void btnTemas_Click(object sender, EventArgs e)
        {
            FormTemas formTemas = new FormTemas();
            formTemas.ShowDialog();
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            if (MySQLConnection.TestConnection())
            {
                MessageBox.Show(
                    "Conexion a MySQL exitosa!\n\nVersion: " + MySQLConnection.GetMySQLVersion(),
                    "Conexion OK",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "No se pudo conectar a MySQL.\n\nVerifica que:\n1. MySQL este ejecutandose\n2. El password en MySQLConnection.cs sea correcto",
                    "Error de Conexion",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "¿Deseas salir del sistema?",
                "Confirmar salida",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}