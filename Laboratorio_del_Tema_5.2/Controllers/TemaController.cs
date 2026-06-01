using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class TemaController
    {
        // ============================================
        // CREATE - Insertar nuevo tema
        // ============================================
        public bool Create(Tema tema)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Tema 
                                     (id_materia, numero_tema, nombre, descripcion, 
                                      horas_estimadas, status_tema) 
                                     VALUES 
                                     (@id_materia, @numero_tema, @nombre, @descripcion,
                                      @horas_estimadas, @status_tema)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", tema.Id_Materia);
                        cmd.Parameters.AddWithValue("@numero_tema", tema.Numero_Tema);
                        cmd.Parameters.AddWithValue("@nombre", tema.Nombre);
                        cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(tema.Descripcion) ? (object)DBNull.Value : tema.Descripcion);
                        cmd.Parameters.AddWithValue("@horas_estimadas", tema.Horas_Estimadas);
                        cmd.Parameters.AddWithValue("@status_tema", string.IsNullOrEmpty(tema.Status_Tema) ? "activo" : tema.Status_Tema);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al crear tema: " + ex.Message);
                return false;
            }
        }

        // ============================================
        // READ - Obtener todos los temas
        // ============================================
        public List<Tema> Read()
        {
            List<Tema> temas = new List<Tema>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT t.*, m.nombre AS nombre_materia 
                                    FROM Tema t
                                    INNER JOIN Materia m ON t.id_materia = m.id_materia
                                    ORDER BY t.id_materia, t.numero_tema";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Tema tema = new Tema();
                            tema.Id_Tema = reader.GetInt32("id_tema");
                            tema.Id_Materia = reader.GetInt32("id_materia");
                            tema.Numero_Tema = reader.GetInt32("numero_tema");
                            tema.Nombre = reader.GetString("nombre");
                            
                            int idx = reader.GetOrdinal("descripcion");
                            tema.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            tema.Horas_Estimadas = reader.GetDecimal("horas_estimadas");
                            tema.Status_Tema = reader.GetString("status_tema");
                            tema.Created_At = reader.GetDateTime("created_at");
                            tema.Updated_At = reader.GetDateTime("updated_at");
                            
                            idx = reader.GetOrdinal("nombre_materia");
                            tema.Nombre_Materia = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            temas.Add(tema);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer temas: " + ex.Message);
            }

            return temas;
        }

        // ============================================
        // READ by ID - Obtener tema específico
        // ============================================
        public Tema ReadById(int idTema)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT t.*, m.nombre AS nombre_materia 
                                    FROM Tema t
                                    INNER JOIN Materia m ON t.id_materia = m.id_materia
                                    WHERE t.id_tema = @id_tema";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_tema", idTema);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Tema tema = new Tema();
                                tema.Id_Tema = reader.GetInt32("id_tema");
                                tema.Id_Materia = reader.GetInt32("id_materia");
                                tema.Numero_Tema = reader.GetInt32("numero_tema");
                                tema.Nombre = reader.GetString("nombre");
                                
                                int idx = reader.GetOrdinal("descripcion");
                                tema.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                tema.Horas_Estimadas = reader.GetDecimal("horas_estimadas");
                                tema.Status_Tema = reader.GetString("status_tema");
                                tema.Created_At = reader.GetDateTime("created_at");
                                tema.Updated_At = reader.GetDateTime("updated_at");
                                
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
                Console.WriteLine("Error al buscar tema: " + ex.Message);
            }

            return null;
        }

        // ============================================
        // READ by Materia - Obtener temas de una materia
        // ============================================
        public List<Tema> ReadByMateria(int idMateria)
        {
            List<Tema> temas = new List<Tema>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT t.*, m.nombre AS nombre_materia 
                                    FROM Tema t
                                    INNER JOIN Materia m ON t.id_materia = m.id_materia
                                    WHERE t.id_materia = @id_materia
                                    ORDER BY t.numero_tema";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", idMateria);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Tema tema = new Tema();
                                tema.Id_Tema = reader.GetInt32("id_tema");
                                tema.Id_Materia = reader.GetInt32("id_materia");
                                tema.Numero_Tema = reader.GetInt32("numero_tema");
                                tema.Nombre = reader.GetString("nombre");
                                
                                int idx = reader.GetOrdinal("descripcion");
                                tema.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                tema.Horas_Estimadas = reader.GetDecimal("horas_estimadas");
                                tema.Status_Tema = reader.GetString("status_tema");
                                tema.Created_At = reader.GetDateTime("created_at");
                                tema.Updated_At = reader.GetDateTime("updated_at");
                                
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
                Console.WriteLine("Error al leer temas por materia: " + ex.Message);
            }

            return temas;
        }

        // ============================================
        // UPDATE - Actualizar tema
        // ============================================
        public bool Update(Tema tema)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Tema SET 
                                     id_materia = @id_materia,
                                     numero_tema = @numero_tema,
                                     nombre = @nombre,
                                     descripcion = @descripcion,
                                     horas_estimadas = @horas_estimadas,
                                     status_tema = @status_tema
                                     WHERE id_tema = @id_tema";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_tema", tema.Id_Tema);
                        cmd.Parameters.AddWithValue("@id_materia", tema.Id_Materia);
                        cmd.Parameters.AddWithValue("@numero_tema", tema.Numero_Tema);
                        cmd.Parameters.AddWithValue("@nombre", tema.Nombre);
                        cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(tema.Descripcion) ? (object)DBNull.Value : tema.Descripcion);
                        cmd.Parameters.AddWithValue("@horas_estimadas", tema.Horas_Estimadas);
                        cmd.Parameters.AddWithValue("@status_tema", string.IsNullOrEmpty(tema.Status_Tema) ? "activo" : tema.Status_Tema);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar tema: " + ex.Message);
                return false;
            }
        }

        // ============================================
        // DELETE - Eliminar tema
        // ============================================
        public bool Delete(int idTema)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Tema WHERE id_tema = @id_tema";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_tema", idTema);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al eliminar tema: " + ex.Message);
                return false;
            }
        }
    }
}