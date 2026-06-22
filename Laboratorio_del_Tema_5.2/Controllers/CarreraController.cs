using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    /// <summary>
    /// Read-only career catalog controller for combo-box data binding.
    /// Full CRUD is deferred to a later slice; this controller exposes only
    /// the queries required by the student form and other consumers.
    /// </summary>
    public class CarreraController
    {
        /// <summary>
        /// Returns all careers (active and inactive) ordered by name.
        /// </summary>
        public List<Carrera> Read()
        {
            List<Carrera> carreras = new List<Carrera>();

            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Carrera ORDER BY nombre";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            carreras.Add(MapCarreraFromReader(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error reading carreras", ex);
                throw new CrudOperationException("Ocurrio un error al obtener las carreras.", "Read", null);
            }

            return carreras;
        }

        /// <summary>
        /// Returns only active careers ordered by name, suitable for combo boxes
        /// where inactive careers should not be selectable.
        /// </summary>
        public List<Carrera> ReadActivas()
        {
            List<Carrera> carreras = new List<Carrera>();

            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Carrera WHERE status = @status ORDER BY nombre";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", Estatus.CarreraActiva);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                carreras.Add(MapCarreraFromReader(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error reading active carreras", ex);
                throw new CrudOperationException("Ocurrio un error al obtener las carreras activas.", "Read", null);
            }

            return carreras;
        }

        /// <summary>
        /// Returns a single career by ID, or null if not found.
        /// </summary>
        public Carrera ReadById(int idCarrera)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Carrera WHERE id_carrera = @id_carrera";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_carrera", idCarrera);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                return MapCarreraFromReader(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error reading carrera ID: {idCarrera}", ex);
                throw new CrudOperationException("Ocurrio un error al buscar la carrera.", "ReadById", null);
            }

            return null;
        }

        private Carrera MapCarreraFromReader(SqlDataReader reader)
        {
            Carrera carrera = new Carrera();
            carrera.Id_Carrera = reader.GetInt32(reader.GetOrdinal("id_carrera"));
            carrera.Clave = reader.GetString(reader.GetOrdinal("clave"));
            carrera.Nombre = reader.GetString(reader.GetOrdinal("nombre"));

            // La columna descripcion no existe en algunas versiones del schema.
            // Se usa try/catch para mantener compatibilidad sin romper la lectura.
            try
            {
                int idxDescripcion = reader.GetOrdinal("descripcion");
                carrera.Descripcion = reader.IsDBNull(idxDescripcion) ? null : reader.GetString(idxDescripcion);
            }
            catch (IndexOutOfRangeException)
            {
                carrera.Descripcion = null;
            }

            carrera.Duracion_Semestres = reader.GetInt32(reader.GetOrdinal("duracion_semestres"));
            carrera.Status = reader.GetString(reader.GetOrdinal("status"));
            carrera.Created_At = reader.GetDateTime(reader.GetOrdinal("created_at"));
            carrera.Updated_At = reader.GetDateTime(reader.GetOrdinal("updated_at"));

            return carrera;
        }
    }
}
