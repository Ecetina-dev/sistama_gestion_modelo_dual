using System;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Views;

namespace Laboratorio_del_Tema_5._2
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMenuPrincipal());
        }
    }
}
