using System;
using System.Security.Cryptography;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers.Services
{
    /// <summary>
    /// Servicio para generación y validación segura de contraseñas.
    /// Separa las responsabilidades criptográficas del AuthController.
    /// </summary>
    public static class PasswordService
    {
        private const string CharsMinuscula = "abcdefghjkmnpqrstuvwxyz";
        private const string CharsMayuscula = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string CharsDigitos = "23456789";
        private const string CharsEspeciales = "!@#$%&*()_+-=[]{}|;:,.<>?";

        private static readonly char[] TodosLosCaracteres =
            (CharsMinuscula + CharsMayuscula + CharsDigitos + CharsEspeciales).ToCharArray();

        /// <summary>
        /// Genera una contraseña temporal criptográficamente segura.
        /// Usa RNGCryptoServiceProvider en vez de Random (que es predecible).
        /// </summary>
        public static string GenerarPasswordTemporal(int longitud = 10)
        {
            if (longitud < 8) longitud = 8;

            var pwd = new char[longitud];

            // Garantizar al menos uno de cada tipo
            pwd[0] = ElegirCaracterSeguro(CharsMayuscula.ToCharArray());
            pwd[1] = ElegirCaracterSeguro(CharsMinuscula.ToCharArray());
            pwd[2] = ElegirCaracterSeguro(CharsDigitos.ToCharArray());
            pwd[3] = ElegirCaracterSeguro(CharsEspeciales.ToCharArray());

            // Rellenar el resto con caracteres aleatorios
            for (int i = 4; i < longitud; i++)
            {
                pwd[i] = ElegirCaracterSeguro(TodosLosCaracteres);
            }

            // Mezclar para que los primeros 4 no estén en posiciones fijas
            MezclarArray(pwd);

            return new string(pwd);
        }

        private static char ElegirCaracterSeguro(char[] caracteres)
        {
            byte[] randomBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            int index = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % caracteres.Length;
            return caracteres[index];
        }

        private static void MezclarArray(char[] array)
        {
            byte[] randomBytes = new byte[4];
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    rng.GetBytes(randomBytes);
                    int j = Math.Abs(BitConverter.ToInt32(randomBytes, 0)) % (i + 1);
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
        }
    }
}
