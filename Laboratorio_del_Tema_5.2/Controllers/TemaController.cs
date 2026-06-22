using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class TemaController
    {
        public bool Create(Tema tema)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Tema 
                                      (id_materia, numero_tema, nombre, descripcion, status_tema) 
                                      VALUES 
                                      (@id_materia, @numero_tema, @nombre, @descripcion, @status_tema)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", tema.Id_Materia);
                        cmd.Parameters.AddWithValue("@numero_tema", tema.Numero_Tema);
                        cmd.Parameters.AddWithValue("@nombre", tema.Nombre);
                        cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(tema.Descripcion) ? (object)DBNull.Value : tema.Descripcion);
                        cmd.Parameters.AddWithValue("@status_tema", string.IsNullOrEmpty(tema.Status_Tema) ? Estatus.TemaActivo : tema.Status_Tema);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear tema", ex);
                return false;
            }
        }

        public List<Tema> Read()
        {
            List<Tema> temas = new List<Tema>();

            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT t.*, m.nombre AS nombre_materia 
                                    FROM Tema t
                                    INNER JOIN Materia m ON t.id_materia = m.id_materia
                                    ORDER BY t.id_materia, t.numero_tema";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tema tema = new Tema();
                            tema.Id_Tema = reader.GetInt32(reader.GetOrdinal("id_tema"));
                            tema.Id_Materia = reader.GetInt32(reader.GetOrdinal("id_materia"));
                            tema.Numero_Tema = reader.GetInt32(reader.GetOrdinal("numero_tema"));
                            tema.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                            
                            int idx = reader.GetOrdinal("descripcion");
                            tema.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            tema.Status_Tema = reader.GetString(reader.GetOrdinal("status_tema"));
                            tema.Created_At = reader.GetDateTime(reader.GetOrdinal("created_at"));
                            tema.Updated_At = reader.GetDateTime(reader.GetOrdinal("updated_at"));
                            
                            idx = reader.GetOrdinal("nombre_materia");
                            tema.Nombre_Materia = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            temas.Add(tema);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer temas", ex);
            }
            return temas;
        }

        public Tema ReadById(int idTema)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT t.*, m.nombre AS nombre_materia 
                                    FROM Tema t
                                    INNER JOIN Materia m ON t.id_materia = m.id_materia
                                    WHERE t.id_tema = @id_tema";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_tema", idTema);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Tema tema = new Tema();
                                tema.Id_Tema = reader.GetInt32(reader.GetOrdinal("id_tema"));
                                tema.Id_Materia = reader.GetInt32(reader.GetOrdinal("id_materia"));
                                tema.Numero_Tema = reader.GetInt32(reader.GetOrdinal("numero_tema"));
                                tema.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                                
                                int idx = reader.GetOrdinal("descripcion");
                                tema.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                tema.Status_Tema = reader.GetString(reader.GetOrdinal("status_tema"));
                                tema.Created_At = reader.GetDateTime(reader.GetOrdinal("created_at"));
                                tema.Updated_At = reader.GetDateTime(reader.GetOrdinal("updated_at"));
                                
                                idx = reader.GetOrdinal("nombre_materia");
                                tema.Nombre_Materia = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                return tema;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al buscar tema ID: {idTema}", ex);
            }
            return null;
        }

        // READ by Materia - Obtener temas de una materia
        public List<Tema> ReadByMateria(int idMateria)
        {
            List<Tema> temas = new List<Tema>();

            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT t.*, m.nombre AS nombre_materia 
                                    FROM Tema t
                                    INNER JOIN Materia m ON t.id_materia = m.id_materia
                                    WHERE t.id_materia = @id_materia
                                    ORDER BY t.numero_tema";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", idMateria);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tema tema = new Tema();
                                tema.Id_Tema = reader.GetInt32(reader.GetOrdinal("id_tema"));
                                tema.Id_Materia = reader.GetInt32(reader.GetOrdinal("id_materia"));
                                tema.Numero_Tema = reader.GetInt32(reader.GetOrdinal("numero_tema"));
                                tema.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                                
                                int idx = reader.GetOrdinal("descripcion");
                                tema.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                tema.Status_Tema = reader.GetString(reader.GetOrdinal("status_tema"));
                                tema.Created_At = reader.GetDateTime(reader.GetOrdinal("created_at"));
                                tema.Updated_At = reader.GetDateTime(reader.GetOrdinal("updated_at"));
                                
                                idx = reader.GetOrdinal("nombre_materia");
                                tema.Nombre_Materia = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                temas.Add(tema);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al leer temas por materia ID: {idMateria}", ex);
            }
            return temas;
        }

        public bool Update(Tema tema)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Tema SET 
                                      id_materia = @id_materia,
                                      numero_tema = @numero_tema,
                                      nombre = @nombre,
                                      descripcion = @descripcion,
                                      status_tema = @status_tema
                                      WHERE id_tema = @id_tema";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_tema", tema.Id_Tema);
                        cmd.Parameters.AddWithValue("@id_materia", tema.Id_Materia);
                        cmd.Parameters.AddWithValue("@numero_tema", tema.Numero_Tema);
                        cmd.Parameters.AddWithValue("@nombre", tema.Nombre);
                        cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(tema.Descripcion) ? (object)DBNull.Value : tema.Descripcion);
                        cmd.Parameters.AddWithValue("@status_tema", string.IsNullOrEmpty(tema.Status_Tema) ? Estatus.TemaActivo : tema.Status_Tema);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al actualizar tema", ex);
                return false;
            }
        }

        public bool Delete(int idTema)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Tema WHERE id_tema = @id_tema";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_tema", idTema);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar tema ID: {idTema}", ex);
                return false;
            }
        }
    }
}