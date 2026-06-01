using System;
using MySqlConnector;

namespace Laboratorio_del_Tema_5_2.Data
{
    /// <summary>
    /// Clase estatica para gestionar la conexion a la base de datos MySQL.
    /// </summary>
    public static class MySQLConnection
    {
        private const string ConnectionString = 
            "Server=localhost;Database=ModeloDualDB;User=root;Password=Creeper77xd;Port=3307;";

        /// <summary>
        /// Obtiene una nueva conexion a MySQL.
        /// </summary>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        /// <summary>
        /// Verifica si la conexion a MySQL es exitosa.
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
    }
}