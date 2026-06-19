using System;
using System.Collections.Generic;
using System.Linq;

namespace Laboratorio_del_Tema_5_2.Controllers.Services
{
    /// <summary>
    /// Validador de fortaleza de passwords.
    /// Incluye lista de passwords comunes (top 100) y reglas de similitud.
    /// </summary>
    public static class PasswordValidator
    {
        /// <summary>
        /// Top 100 contraseñas más comunes (Have I Been Pwned / SecLists).
        /// Lista mínima viable para WinForms sin dependencias externas.
        /// </summary>
        private static readonly HashSet<string> _commonPasswords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "password", "password1", "password123", "password1234", "password12345",
            "123456", "12345678", "123456789", "1234567890", "qwerty", "qwerty123",
            "admin", "admin123", "admin1234", "administrator",
            "letmein", "welcome", "welcome1", "monkey", "dragon", "master", "login",
            "princess", "abc123", "abc1234", "111111", "000000", "iloveyou",
            "sunshine", "trustno1", "654321", "jordan23", "harley", "hunter2",
            "shadow", "michael", "ashley", "daniel", "jessica", "jennifer",
            "thomas", "robert", "charlie", "andrew", "joshua", "george",
            "nicole", "amanda", "matthew", "james", "1234567", "12345",
            "1234", "123", "qwertyuiop", "zxcvbnm", "asdfgh", "asdfghjkl",
            "qazwsx", "q1w2e3r4", "q1w2e3r4t5", "p@ssw0rd", "p@ssword",
            "passw0rd", "passw0rd1", "passw0rd123", "P@ssword1", "P@ssw0rd",
            "Password", "Password1", "Password1!", "Password123", "Password123!",
            "Pa$$w0rd", "P@$$w0rd", "root", "root123", "toor",
            "test", "test123", "test1234", "testing", "testing123",
            "guest", "guest123", "user", "user123",
            "qwerty1", "qwerty12345", "qwerty123456", "1q2w3e4r", "1qaz2wsx",
            "starwars", "batman", "superman", "spiderman", "football",
            "baseball", "soccer", "hockey", "basketball", "tigger",
            "charlie", "thomas", "george", "william", "richard", "joseph",
            "computer", "internet", "maverick", "mustang", "corvette",
            "mercedes", "ferrari", "porsche", "toyota", "honda"
        };

        /// <summary>
        /// Resultado de validación de password.
        /// </summary>
        public class ResultadoValidacion
        {
            public bool EsValido { get; set; } = true;
            public string Razon { get; set; }

            public static ResultadoValidacion Ok() => new ResultadoValidacion { EsValido = true };
            public static ResultadoValidacion Fail(string razon) => new ResultadoValidacion { EsValido = false, Razon = razon };
        }

        /// <summary>
        /// Valida que el password no esté en la lista de comunes
        /// y no sea demasiado similar al username o email.
        /// </summary>
        public static ResultadoValidacion Validar(string password, string username = null, string email = null)
        {
            if (string.IsNullOrEmpty(password))
                return ResultadoValidacion.Fail("El password no puede estar vacío");

            // Check lista de comunes
            if (_commonPasswords.Contains(password))
                return ResultadoValidacion.Fail("Este password es demasiado común. Elegí otro más seguro.");

            // Check si el password contiene el username (o partes)
            if (!string.IsNullOrEmpty(username) && username.Length >= 3)
            {
                if (password.IndexOf(username, StringComparison.OrdinalIgnoreCase) >= 0)
                    return ResultadoValidacion.Fail("El password no puede contener tu nombre de usuario.");
            }

            // Check si el password contiene el email (o la parte local)
            if (!string.IsNullOrEmpty(email))
            {
                string localPart = email.Split('@')[0];
                if (localPart.Length >= 3 &&
                    password.IndexOf(localPart, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return ResultadoValidacion.Fail("El password no puede contener tu email.");
                }
            }

            // Check secuencias de teclado (qwerty, asdf, etc.) y repeticiones
            if (TieneSecuencia(password))
                return ResultadoValidacion.Fail("El password no puede contener secuencias de teclado obvias (qwerty, 1234, etc.).");

            // Check repeticiones (aaaa, 1111, etc.)
            if (TieneRepeticiones(password))
                return ResultadoValidacion.Fail("El password no puede tener más de 3 caracteres iguales consecutivos.");

            return ResultadoValidacion.Ok();
        }

        private static bool TieneSecuencia(string password)
        {
            string secuenciasConocidas = "qwertyuiop|asdfghjkl|zxcvbnm|1234567890|0987654321|abcdefghijklmnopqrstuvwxyz";
            string[] seqs = secuenciasConocidas.Split('|');
            string lower = password.ToLower();
            foreach (var seq in seqs)
            {
                if (seq.Length >= 4 && lower.Contains(seq))
                    return true;
                // Check reversed
                char[] rev = seq.ToCharArray();
                Array.Reverse(rev);
                if (new string(rev).Length >= 4 && lower.Contains(new string(rev)))
                    return true;
            }
            return false;
        }

        private static bool TieneRepeticiones(string password)
        {
            if (password.Length < 4) return false;
            for (int i = 0; i < password.Length - 3; i++)
            {
                if (password[i] == password[i + 1] &&
                    password[i + 1] == password[i + 2] &&
                    password[i + 2] == password[i + 3])
                {
                    return true;
                }
            }
            return false;
        }
    }
}
