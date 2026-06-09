using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Clase utilitaria para validaciones reutilizables.
    /// </summary>
    public static class Validacion
    {
        /// <summary>
        /// Valida que un campo no este vacio.
        /// </summary>
        public static bool Requerido(string valor, string nombreCampo = "Campo")
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                MessageBox.Show($"{nombreCampo} es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valida que los campos requeridos esten llenos.
        /// </summary>
        public static bool CamposRequeridos(params (TextBox textBox, string nombreCampo)[] campos)
        {
            foreach (var campo in campos)
            {
                if (!Requerido(campo.textBox.Text, campo.nombreCampo))
                {
                    campo.textBox.Focus();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Valida formato de correo electronico.
        /// </summary>
        public static bool Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; // Opcional

            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, patron))
            {
                MessageBox.Show("El formato del correo electronico es invalido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valida formato de telefono (10 digitos).
        /// </summary>
        public static bool Telefono(string telefono)
        {
            if (string.IsNullOrWhiteSpace(telefono)) return true; // Opcional

            string patron = @"^\d{10}$";
            if (!Regex.IsMatch(telefono, patron))
            {
                MessageBox.Show("El telefono debe tener 10 digitos.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valida formato de RFC.
        /// </summary>
        public static bool RFC(string rfc)
        {
            if (string.IsNullOrWhiteSpace(rfc)) return true; // Opcional

            string patron = @"^[A-Z]{4}\d{6}[A-Z0-9]{3}$";
            if (!Regex.IsMatch(rfc.ToUpper(), patron))
            {
                MessageBox.Show("El formato del RFC es invalido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valida formato de codigo postal (5 digitos).
        /// </summary>
        public static bool CodigoPostal(string cp)
        {
            if (string.IsNullOrWhiteSpace(cp)) return true; // Opcional

            string patron = @"^\d{5}$";
            if (!Regex.IsMatch(cp, patron))
            {
                MessageBox.Show("El codigo postal debe tener 5 digitos.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Valida formato de numero de control (8 digitos).
        /// </summary>
        public static bool NumeroControl(string noControl)
        {
            if (string.IsNullOrWhiteSpace(noControl)) return true; // Opcional

            string patron = @"^\d{8}$";
            if (!Regex.IsMatch(noControl, patron))
            {
                MessageBox.Show("El numero de control debe tener 8 digitos.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Intenta convertir una fecha desde texto.
        /// </summary>
        public static bool TryParseFecha(string texto, out DateTime fecha)
        {
            return DateTime.TryParse(texto, out fecha);
        }

        /// <summary>
        /// Valida que una fecha no sea futura.
        /// </summary>
        public static bool FechaNoFutura(string texto, string nombreCampo = "Fecha")
        {
            if (string.IsNullOrWhiteSpace(texto)) return true; // Opcional

            if (DateTime.TryParse(texto, out DateTime fecha))
            {
                if (fecha > DateTime.Now)
                {
                    MessageBox.Show($"{nombreCampo} no puede ser futura.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Valida que una fecha no sea muy antigua (mas de 100 anos).
        /// </summary>
        public static bool FechaValida(string texto, string nombreCampo = "Fecha")
        {
            if (string.IsNullOrWhiteSpace(texto)) return true; // Opcional

            if (DateTime.TryParse(texto, out DateTime fecha))
            {
                if (fecha < DateTime.Now.AddYears(-100))
                {
                    MessageBox.Show($"{nombreCampo} parece ser muy antigua.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (fecha > DateTime.Now)
                {
                    MessageBox.Show($"{nombreCampo} no puede ser futura.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }
    }
}