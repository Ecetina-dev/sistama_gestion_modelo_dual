using System;
using System.Configuration;
using MySqlConnector;

namespace Laboratorio_del_Tema_5_2.Data
{
    /// <summary>
    /// Proporciona metodos para gestionar la conexion a la base de datos MySQL.
    /// </summary>
    public static class MySQLConnection
    {
        /// <summary>
        /// Obtiene la cadena de conexion desde App.config.
        /// </summary>
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

        /// <summary>
        /// Obtiene una nueva conexion a la base de datos.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Verifica si la conexion a MySQL funciona correctamente.
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    Console.WriteLine("Conexion a MySQL exitosa!");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de conexion: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la version del servidor MySQL.
        /// </summary>
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

        /// <summary>
        /// Obtiene el timeout de conexion configurado.
        /// </summary>
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