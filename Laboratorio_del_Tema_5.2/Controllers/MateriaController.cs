using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class MateriaController
    {
        public bool Create(Materia materia)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Materia 
                                     (clave_materia, nombre, descripcion, creditos, semestre,
                                      horas_teoria, horas_practica, status_materia) 
                                     VALUES 
                                     (@clave_materia, @nombre, @descripcion, @creditos, @semestre,
                                      @horas_teoria, @horas_practica, @status_materia)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clave_materia", materia.Clave_Materia);
                        cmd.Parameters.AddWithValue("@nombre", materia.Nombre);
                        cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(materia.Descripcion) ? (object)DBNull.Value : materia.Descripcion);
                        cmd.Parameters.AddWithValue("@creditos", materia.Creditos);
                        cmd.Parameters.AddWithValue("@semestre", materia.Semestre > 0 ? materia.Semestre : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@horas_teoria", materia.Horas_Teoria);
                        cmd.Parameters.AddWithValue("@horas_practica", materia.Horas_Practica);
                        cmd.Parameters.AddWithValue("@status_materia", string.IsNullOrEmpty(materia.Status_Materia) ? Estatus.MateriaActiva : materia.Status_Materia);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear materia", ex);
                return false;
            }
        }

        public List<Materia> Read()
        {
            List<Materia> materias = new List<Materia>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Materia ORDER BY semestre, clave_materia";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Materia materia = new Materia();
                            materia.Id_Materia = reader.GetInt32("id_materia");
                            materia.Clave_Materia = reader.GetString("clave_materia");
                            materia.Nombre = reader.GetString("nombre");
                            
                            int idx = reader.GetOrdinal("descripcion");
                            materia.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            materia.Creditos = reader.GetInt32("creditos");
                            
                            idx = reader.GetOrdinal("semestre");
                            materia.Semestre = reader.IsDBNull(idx) ? 0 : reader.GetInt32(idx);
                            
                            materia.Horas_Teoria = reader.GetInt32("horas_teoria");
                            materia.Horas_Practica = reader.GetInt32("horas_practica");
                            materia.Status_Materia = reader.GetString("status_materia");
                            materia.Created_At = reader.GetDateTime("created_at");
                            materia.Updated_At = reader.GetDateTime("updated_at");
                            
                            materias.Add(materia);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer materias", ex);
            }
            return materias;
        }

        public Materia ReadById(int idMateria)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Materia WHERE id_materia = @id_materia";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", idMateria);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Materia materia = new Materia();
                                materia.Id_Materia = reader.GetInt32("id_materia");
                                materia.Clave_Materia = reader.GetString("clave_materia");
                                materia.Nombre = reader.GetString("nombre");
                                
                                int idx = reader.GetOrdinal("descripcion");
                                materia.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                materia.Creditos = reader.GetInt32("creditos");
                                
                                idx = reader.GetOrdinal("semestre");
                                materia.Semestre = reader.IsDBNull(idx) ? 0 : reader.GetInt32(idx);
                                
                                materia.Horas_Teoria = reader.GetInt32("horas_teoria");
                                materia.Horas_Practica = reader.GetInt32("horas_practica");
                                materia.Status_Materia = reader.GetString("status_materia");
                                materia.Created_At = reader.GetDateTime("created_at");
                                materia.Updated_At = reader.GetDateTime("updated_at");
                                
                                return materia;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al buscar materia ID: {idMateria}", ex);
            }
            return null;
        }

        public bool Update(Materia materia)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Materia SET 
                                     clave_materia = @clave_materia,
                                     nombre = @nombre,
                                     descripcion = @descripcion,
                                     creditos = @creditos,
                                     semestre = @semestre,
                                     horas_teoria = @horas_teoria,
                                     horas_practica = @horas_practica,
                                     status_materia = @status_materia
                                     WHERE id_materia = @id_materia";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", materia.Id_Materia);
                        cmd.Parameters.AddWithValue("@clave_materia", materia.Clave_Materia);
                        cmd.Parameters.AddWithValue("@nombre", materia.Nombre);
                        cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrEmpty(materia.Descripcion) ? (object)DBNull.Value : materia.Descripcion);
                        cmd.Parameters.AddWithValue("@creditos", materia.Creditos);
                        cmd.Parameters.AddWithValue("@semestre", materia.Semestre > 0 ? materia.Semestre : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@horas_teoria", materia.Horas_Teoria);
                        cmd.Parameters.AddWithValue("@horas_practica", materia.Horas_Practica);
                        cmd.Parameters.AddWithValue("@status_materia", string.IsNullOrEmpty(materia.Status_Materia) ? Estatus.MateriaActiva : materia.Status_Materia);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al actualizar materia", ex);
                return false;
            }
        }

        public bool Delete(int idMateria)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Materia WHERE id_materia = @id_materia";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_materia", idMateria);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar materia ID: {idMateria}", ex);
                return false;
            }
        }

        public bool ExisteClave(string clave, int? excluirId = null)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Materia WHERE clave_materia = @clave";
                    if (excluirId.HasValue)
                        query += " AND id_materia != @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@clave", clave);
                        if (excluirId.HasValue)
                            cmd.Parameters.AddWithValue("@id", excluirId.Value);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar clave_materia", ex);
                return false;
            }
        }

        public List<MateriaConProyectos> GetMateriasConProyectos()
        {
            List<MateriaConProyectos> lista = new List<MateriaConProyectos>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT 
                                        m.id_materia,
                                        m.clave_materia,
                                        m.nombre,
                                        m.creditos,
                                        m.semestre,
                                        COUNT(pm.id_proyecto) AS num_proyectos,
                                        GROUP_CONCAT(DISTINCT p.nombre_proyecto SEPARATOR ', ') AS proyectos,
                                        ROUND(SUM(pm.porcentaje_aplicado), 2) AS porcentaje_total
                                    FROM Materia m
                                    INNER JOIN Proyecto_Materia pm ON m.id_materia = pm.id_materia
                                    INNER JOIN Proyecto p ON pm.id_proyecto = p.id_proyecto
                                    GROUP BY m.id_materia
                                    ORDER BY m.semestre, m.clave_materia";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MateriaConProyectos item = new MateriaConProyectos();
                            item.Id_Materia = reader.GetInt32("id_materia");
                            item.Clave_Materia = reader.GetString("clave_materia");
                            item.Nombre = reader.GetString("nombre");
                            item.Creditos = reader.GetInt32("creditos");
                            
                            int idx = reader.GetOrdinal("semestre");
                            item.Semestre = reader.IsDBNull(idx) ? 0 : reader.GetInt32(idx);
                            
                            item.Num_Proyectos = reader.GetInt32("num_proyectos");
                            
                            idx = reader.GetOrdinal("proyectos");
                            item.Proyectos = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("porcentaje_total");
                            item.Porcentaje_Total = reader.IsDBNull(idx) ? 0 : reader.GetDecimal(idx);
                            
                            lista.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al buscar materias con proyectos", ex);
            }
            return lista;
        }
    }

    public class MateriaConProyectos
    {
        public int Id_Materia { get; set; }
        public string Clave_Materia { get; set; }
        public string Nombre { get; set; }
        public int Creditos { get; set; }
        public int Semestre { get; set; }
        public int Num_Proyectos { get; set; }
        public string Proyectos { get; set; }
        public decimal Porcentaje_Total { get; set; }
    }
}