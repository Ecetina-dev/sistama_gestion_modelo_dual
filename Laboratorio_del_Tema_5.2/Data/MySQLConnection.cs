using System;
using System.Configuration;
using System.Threading.Tasks;
using Laboratorio_del_Tema_5_2.Utils;
using MySqlConnector;

namespace Laboratorio_del_Tema_5_2.Data
{
    /// <summary>
    /// Proporciona metodos para gestionar la conexion a la base de datos MySQL.
    /// Soporta operaciones sincronas y asincronas para enterprise-grade performance.
    /// </summary>
    public static class MySQLConnection
    {
        private static string ConnectionString
        {
            get
            {
                try
                {
                    return ConfigurationManager.ConnectionStrings["MySQL"]?.ConnectionString
                        ?? throw new InvalidOperationException("No se encontro la cadena de conexion 'MySQL' en App.config.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al cargar connection string: " + ex.Message);
                    throw;
                }
            }
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Abre una conexión de forma asíncrona para operaciones no bloqueantes.
        /// </summary>
        public static async Task<MySqlConnection> GetOpenConnectionAsync()
        {
            var conn = new MySqlConnection(ConnectionString);
            await conn.OpenAsync().ConfigureAwait(false);
            return conn;
        }

        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Test de conexión fallido", ex);
                return false;
            }
        }

        public static string GetMySQLVersion()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return conn.ServerVersion;
                }
            }
            catch (Exception ex)
            {
                return "No conectado: " + ex.Message;
            }
        }

        public static int GetTimeout()
        {
            try
            {
                string timeoutStr = ConfigurationManager.AppSettings["TimeoutConexion"];
                return int.TryParse(timeoutStr, out int timeout) ? timeout : 30;
            }
            catch
            {
                return 30;
            }
        }
    }
}
