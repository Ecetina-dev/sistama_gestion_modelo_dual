using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Laboratorio_del_Tema_5_2.Data;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Claves conocidas de la tabla parametro_sistema.
    /// Centralizadas para evitar magic strings.
    /// </summary>
    public static class Claves
    {
        public const string MAX_INTENTOS_LOGIN = "MAX_INTENTOS_LOGIN";
        public const string TIEMPO_BLOQUEO_MINUTOS = "TIEMPO_BLOQUEO_MINUTOS";
        public const string MIN_CARACTERES_PASSWORD = "MIN_CARACTERES_PASSWORD";
        public const string EXIGIR_MAYUSCULAS_PASSWORD = "EXIGIR_MAYUSCULAS_PASSWORD";
        public const string EXIGIR_NUMEROS_PASSWORD = "EXIGIR_NUMEROS_PASSWORD";
        public const string EXIGIR_CARACTER_ESPECIAL_PASSWORD = "EXIGIR_CARACTER_ESPECIAL_PASSWORD";
        public const string DIAS_CADUCIDAD_PASSWORD = "DIAS_CADUCIDAD_PASSWORD";
    }

    /// <summary>
    /// Servicio singleton que carga los parametros del sistema desde la tabla
    /// parametro_sistema en MySQL y los cachea en memoria durante la vida de la aplicacion.
    /// 
    /// Si la base de datos no esta disponible, devuelve los valores por defecto.
    /// </summary>
    public class ParametroSistemaService
    {
        private static ParametroSistemaService _instance;
        private static readonly object _lock = new object();
        private readonly Dictionary<string, string> _cache;
        private bool _loaded;

        private ParametroSistemaService()
        {
            _cache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _loaded = false;
        }

        public static ParametroSistemaService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ParametroSistemaService();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Carga todos los parametros desde la base de datos.
        /// Solo se ejecuta una vez (lazy init en el primer acceso).
        /// </summary>
        private void EnsureLoaded()
        {
            if (_loaded) return;

            lock (_lock)
            {
                if (_loaded) return;

                try
                {
                    using (SqlConnection conn = SqlServerConnection.GetConnection())
                    {
                        conn.Open();

                        string query = "SELECT clave, valor FROM parametro_sistema";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string clave = reader.GetString(reader.GetOrdinal("clave"));
                                string valor = reader.IsDBNull(reader.GetOrdinal("valor"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("valor"));
                                _cache[clave] = valor;
                            }
                        }
                    }
                }
                catch
                {
                    // Si la BD no esta disponible, se usan los valores por defecto.
                    // El cache queda vacio, y los metodos Get* devuelven los defaults.
                }

                _loaded = true;
            }
        }

        /// <summary>
        /// Obtiene un parametro como entero.
        /// </summary>
        public int GetInt(string clave, int defaultValue)
        {
            EnsureLoaded();
            if (_cache.TryGetValue(clave, out string valor) &&
                int.TryParse(valor, out int resultado))
            {
                return resultado;
            }
            return defaultValue;
        }

        /// <summary>
        /// Obtiene un parametro como booleano (1 = true, 0 = false).
        /// </summary>
        public bool GetBool(string clave, bool defaultValue)
        {
            EnsureLoaded();
            if (_cache.TryGetValue(clave, out string valor) &&
                int.TryParse(valor, out int intVal))
            {
                return intVal == 1;
            }
            return defaultValue;
        }

        /// <summary>
        /// Obtiene un parametro como string.
        /// </summary>
        public string GetString(string clave, string defaultValue)
        {
            EnsureLoaded();
            if (_cache.TryGetValue(clave, out string valor) && valor != null)
            {
                return valor;
            }
            return defaultValue;
        }

        /// <summary>
        /// Fuerza la recarga desde la base de datos (util si se actualizan parametros en runtime).
        /// </summary>
        public void Reload()
        {
            lock (_lock)
            {
                _cache.Clear();
                _loaded = false;
                EnsureLoaded();
            }
        }
    }
}
