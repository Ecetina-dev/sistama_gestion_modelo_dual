using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Reusable validation helper for student identity, contact, academic,
    /// and lifecycle fields. The controller is responsible for required-field
    /// and uniqueness checks; this class focuses on format and business rules.
    /// </summary>
    public static class AlumnoValidator
    {
        private static readonly HashSet<string> EstadosTerminales = new HashSet<string>
        {
            Estatus.AlumnoBaja,
            Estatus.AlumnoEgresado
        };

        #region CURP

        public static bool ValidarCurp(string curp, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(curp))
            {
                error = MensajesAlumno.CurpInvalido;
                return false;
            }

            if (curp.Length != AlumnoConfig.CurpLength)
            {
                error = MensajesAlumno.CurpLongitudInvalida;
                return false;
            }

            // CURP mexicano: 18 caracteres alfanumericos
            // Posiciones 0-3: letras (nombre/apellido)
            // Posiciones 4-9: digitos fecha (AAMMDD) - validada
            // Posiciones 10-15: letras/digitos (homoclave)
            // Posicion 16-17: digitos (consulta + verificador)
            //
            // NOTA: El digito verificador (pos 17) NO es verificable client-side.
            // RENAPO/Gobierno de Mexico no ha publicado el algoritmo oficial.
            // Las implementaciones en internet son aproximaciones incorrectas.
            if (!Regex.IsMatch(curp, AlumnoConfig.CurpPattern))
            {
                error = MensajesAlumno.CurpFormatoInvalido;
                return false;
            }

            // Validar fecha integrada en posiciones 4-9 (AAMMDD)
            if (!ValidarFechaCurp(curp.Substring(4, 6)))
            {
                error = "La fecha de nacimiento en el CURP no es valida.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Valida los 6 digitos de fecha del CURP (AAMMDD).
        /// Solo valida dia y mes validos. El ano asume 1900-2099.
        /// </summary>
        private static bool ValidarFechaCurp(string fecha)
        {
            // fecha = AAMMDD (6 digitos)
            if (fecha.Length != 6 || !fecha.All(char.IsDigit))
                return false;

            int.TryParse(fecha.Substring(0, 2), out int aa);
            int.TryParse(fecha.Substring(2, 2), out int mm);
            int.TryParse(fecha.Substring(4, 2), out int dd);

            // Ano: asume 1900-2099 segun el dgito verificador (pos 16)
            // Solo validamos mes y dia
            if (mm < 1 || mm > 12) return false;

            int[] diasPorMes = { 0, 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (dd < 1 || dd > diasPorMes[mm]) return false;

            return true;
        }

        public static bool ValidarCurp(string curp)
            => ValidarCurp(curp, out _);

        /// <summary>
        /// Valida que los 6 digitos de fecha integrados en el CURP (posiciones 4-9)
        /// representen una fecha valida (AAMMDD).
        /// No verifica el ao completo - solo rango dia/mes valido.
        /// </summary>

        #endregion

        #region RFC

        public static bool ValidarRfc(string rfc, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(rfc))
                return true;

            if (!Regex.IsMatch(rfc, AlumnoConfig.RfcPattern))
            {
                error = MensajesAlumno.RfcInvalido;
                return false;
            }

            return true;
        }

        public static bool ValidarRfc(string rfc)
            => ValidarRfc(rfc, out _);

        #endregion

        #region Email

        public static bool ValidarEmail(string email, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(email))
            {
                error = MensajesAlumno.EmailInvalido;
                return false;
            }

            if (!Regex.IsMatch(email, AlumnoConfig.EmailPattern))
            {
                error = MensajesAlumno.EmailInvalido;
                return false;
            }

            return true;
        }

        public static bool ValidarEmail(string email)
            => ValidarEmail(email, out _);

        #endregion

        #region Phone

        public static bool ValidarTelefono(string telefono, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(telefono))
                return true;

            if (!Regex.IsMatch(telefono, AlumnoConfig.TelefonoPattern))
            {
                error = MensajesAlumno.TelefonoInvalido;
                return false;
            }

            return true;
        }

        public static bool ValidarTelefono(string telefono)
            => ValidarTelefono(telefono, out _);

        #endregion

        #region Age

        public static bool ValidarEdad(DateTime fechaNacimiento, int minEdad, int maxEdad, out string error)
        {
            error = string.Empty;
            int edad = CalcularEdad(fechaNacimiento);

            if (edad < minEdad || edad > maxEdad)
            {
                error = $"La edad debe estar entre {minEdad} y {maxEdad} años.";
                return false;
            }

            return true;
        }

        public static bool ValidarEdad(DateTime? fechaNacimiento, int minAnos, int maxAnos)
        {
            if (!fechaNacimiento.HasValue)
                return false;

            return ValidarEdad(fechaNacimiento.Value, minAnos, maxAnos, out _);
        }

        private static int CalcularEdad(DateTime fechaNacimiento)
        {
            DateTime hoy = DateTime.Today;
            int edad = hoy.Year - fechaNacimiento.Year;

            if (fechaNacimiento.Date > hoy.AddYears(-edad))
                edad--;

            return edad;
        }

        #endregion

        #region Status Transition

        public static bool ValidarTransicionStatus(string statusActual, string statusNuevo, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(statusActual) || string.IsNullOrWhiteSpace(statusNuevo))
            {
                error = MensajesAlumno.TransicionStatusNoPermitida;
                return false;
            }

            if (EstadosTerminales.Contains(statusActual) &&
                !string.Equals(statusActual, statusNuevo, StringComparison.OrdinalIgnoreCase))
            {
                error = MensajesAlumno.TransicionStatusNoPermitida;
                return false;
            }

            return true;
        }

        #endregion

        #region No. Control

        public static bool ValidarNoControl(string noControl, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(noControl))
            {
                error = MensajesAlumno.NoControlInvalido;
                return false;
            }

            if (!Regex.IsMatch(noControl, AlumnoConfig.NoControlPattern))
            {
                error = MensajesAlumno.NoControlInvalido;
                return false;
            }

            return true;
        }

        public static bool ValidarNoControl(string noControl)
            => ValidarNoControl(noControl, out _);

        #endregion

        #region Semester

        public static bool ValidarSemestre(int semestre, int duracionCarrera, out string error)
        {
            error = string.Empty;

            if (semestre < 1 || semestre > duracionCarrera)
            {
                error = MensajesAlumno.SemestreInvalido;
                return false;
            }

            return true;
        }

        public static bool ValidarSemestre(int semestre, int duracionCarrera)
            => ValidarSemestre(semestre, duracionCarrera, out _);

        #endregion

        #region Postal Code

        public static bool ValidarCodigoPostal(string cp, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(cp))
                return true;

            if (!Regex.IsMatch(cp, AlumnoConfig.CodigoPostalPattern))
            {
                error = "El código postal debe contener 5 dígitos.";
                return false;
            }

            return true;
        }

        public static bool ValidarCodigoPostal(string cp)
            => ValidarCodigoPostal(cp, out _);

        #endregion
    }
}
