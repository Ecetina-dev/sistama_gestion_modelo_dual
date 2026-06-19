using System;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;
using Laboratorio_del_Tema_5_2.Views;

namespace Laboratorio_del_Tema_5_2
{
    internal static class Program
    {
        /// <summary>Indica si el usuario eligió 'Salir del Sistema' desde el menú.</summary>
        public static bool SalirDelSistema = false;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            while (true)
            {
                using (var loginForm = new FormLogin())
                {
                    if (loginForm.ShowDialog() != DialogResult.OK)
                    {
                        break;
                    }

                    // Si el usuario debe cambiar password, forzar cambio antes de ir al menu
                    if (SesionActiva.Instance.Debe_Cambiar_Password)
                    {
                        MessageBox.Show(
                            "Detectamos que tu password actual es el temporal.\n" +
                            "Por seguridad, debes cambiarlo antes de continuar.",
                            "Cambio de Password Requerido",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        bool passwordCambiado = false;
                        while (!passwordCambiado)
                        {
                            using (var formCambiar = new FormCambiarPassword())
                            {
                                if (formCambiar.ShowDialog() != DialogResult.OK)
                                {
                                    // El usuario cancelo, cerrar sesion
                                    var auth = new Controllers.AuthController();
                                    auth.CerrarSesion();
                                    passwordCambiado = false;
                                    break;
                                }
                                // Verificar si ya no requiere cambio
                                if (!SesionActiva.Instance.Debe_Cambiar_Password)
                                {
                                    passwordCambiado = true;
                                }
                            }
                        }

                        if (!passwordCambiado) continue; // Volver al login
                    }

                    using (var mainForm = new FormMenuPrincipal())
                    {
                        mainForm.ShowDialog();
                    }

                    if (SalirDelSistema) break;
                }
            }
        }
    }
}
