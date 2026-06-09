using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class ProyectoController
    {
        // ============================================
        // CREATE - Insertar nuevo proyecto
        // ============================================
        public bool Create(Proyecto proyecto)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Proyecto 
                                     (nombre_proyecto, descripcion, objetivos, 
                                      fecha_inicio, fecha_fin, horas_totales, 
                                      status_proyecto, created_at, updated_at) 
                                     VALUES 
                                     (@nombre_proyecto, @descripcion, @objetivos,
                                      @fecha_inicio, @fecha_fin, @horas_totales,
                                      @status_proyecto, @created_at, @updated_at)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre_proyecto", proyecto.Nombre);
                        var descripcionCreate = string.IsNullOrEmpty(proyecto.Descripcion) ? DBNull.Value : (object)proyecto.Descripcion;
                        var objetivosCreate = string.IsNullOrEmpty(proyecto.Objetivos) ? DBNull.Value : (object)proyecto.Objetivos;
                        cmd.Parameters.AddWithValue("@descripcion", descripcionCreate);
                        cmd.Parameters.AddWithValue("@objetivos", objetivosCreate);
                        cmd.Parameters.AddWithValue("@fecha_inicio", proyecto.Fecha_Inicio.HasValue ? (object)proyecto.Fecha_Inicio.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@fecha_fin", proyecto.Fecha_Fin.HasValue ? (object)proyecto.Fecha_Fin.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@horas_totales", proyecto.Horas_Totales.HasValue ? (object)proyecto.Horas_Totales.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@status_proyecto", string.IsNullOrEmpty(proyecto.Status) ? Estatus.ProyectoPropuesto : proyecto.Status);
                        cmd.Parameters.AddWithValue("@created_at", DateTime.Now);
                        cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear proyecto", ex);
                return false;
            }
        }

        // ============================================
        // READ - Obtener todos los proyectos
        // ============================================
        public List<Proyecto> Read()
        {
            List<Proyecto> proyectos = new List<Proyecto>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Proyecto ORDER BY fecha_inicio DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Proyecto proyecto = new Proyecto();
                            proyecto.Id_Proyecto = reader.GetInt32("id_proyecto");
                            proyecto.Nombre = reader.GetString("nombre_proyecto");
                            
                            int idx = reader.GetOrdinal("descripcion");
                            proyecto.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("objetivos");
                            proyecto.Objetivos = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("fecha_inicio");
                            proyecto.Fecha_Inicio = reader.IsDBNull(idx) ? (DateTime?)null : reader.GetDateTime(idx);
                            
                            idx = reader.GetOrdinal("fecha_fin");
                            proyecto.Fecha_Fin = reader.IsDBNull(idx) ? (DateTime?)null : reader.GetDateTime(idx);
                            
                            idx = reader.GetOrdinal("horas_totales");
                            proyecto.Horas_Totales = reader.IsDBNull(idx) ? (int?)null : reader.GetInt32(idx);
                            
                            proyecto.Status = reader.GetString("status_proyecto");
                            proyecto.Created_At = reader.GetDateTime("created_at");
                            proyecto.Updated_At = reader.GetDateTime("updated_at");
                            
                            proyectos.Add(proyecto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer proyectos", ex);
            }
            return proyectos;
        }

        // ============================================
        // READ by ID - Obtener proyecto específico
        // ============================================
        public Proyecto ReadById(int idProyecto)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Proyecto WHERE id_proyecto = @id_proyecto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_proyecto", idProyecto);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Proyecto proyecto = new Proyecto();
                                proyecto.Id_Proyecto = reader.GetInt32("id_proyecto");
                                proyecto.Nombre = reader.GetString("nombre_proyecto");
                                
                                int idx = reader.GetOrdinal("descripcion");
                                proyecto.Descripcion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("objetivos");
                                proyecto.Objetivos = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("fecha_inicio");
                                proyecto.Fecha_Inicio = reader.IsDBNull(idx) ? (DateTime?)null : reader.GetDateTime(idx);
                                
                                idx = reader.GetOrdinal("fecha_fin");
                                proyecto.Fecha_Fin = reader.IsDBNull(idx) ? (DateTime?)null : reader.GetDateTime(idx);
                                
                                idx = reader.GetOrdinal("horas_totales");
                                proyecto.Horas_Totales = reader.IsDBNull(idx) ? (int?)null : reader.GetInt32(idx);
                                
                                proyecto.Status = reader.GetString("status_proyecto");
                                proyecto.Created_At = reader.GetDateTime("created_at");
                                proyecto.Updated_At = reader.GetDateTime("updated_at");
                                
                                return proyecto;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al buscar proyecto ID: {idProyecto}", ex);
            }
            return null;
        }

        // ============================================
        // UPDATE - Actualizar proyecto
        // ============================================
        public bool Update(Proyecto proyecto)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Proyecto SET 
                                     nombre_proyecto = @nombre_proyecto,
                                     descripcion = @descripcion,
                                     objetivos = @objetivos,
                                     fecha_inicio = @fecha_inicio,
                                     fecha_fin = @fecha_fin,
                                     horas_totales = @horas_totales,
                                     status_proyecto = @status_proyecto,
                                     updated_at = @updated_at
                                     WHERE id_proyecto = @id_proyecto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_proyecto", proyecto.Id_Proyecto);
                        cmd.Parameters.AddWithValue("@nombre_proyecto", proyecto.Nombre);
                        var descripcionValue = string.IsNullOrEmpty(proyecto.Descripcion) ? DBNull.Value : (object)proyecto.Descripcion;
                        var objetivosValue = string.IsNullOrEmpty(proyecto.Objetivos) ? DBNull.Value : (object)proyecto.Objetivos;
                        cmd.Parameters.AddWithValue("@descripcion", descripcionValue);
                        cmd.Parameters.AddWithValue("@objetivos", objetivosValue);
                        cmd.Parameters.AddWithValue("@fecha_inicio", proyecto.Fecha_Inicio.HasValue ? (object)proyecto.Fecha_Inicio.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@fecha_fin", proyecto.Fecha_Fin.HasValue ? (object)proyecto.Fecha_Fin.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@horas_totales", proyecto.Horas_Totales.HasValue ? (object)proyecto.Horas_Totales.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@status_proyecto", string.IsNullOrEmpty(proyecto.Status) ? Estatus.ProyectoPropuesto : proyecto.Status);
                        cmd.Parameters.AddWithValue("@updated_at", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al actualizar proyecto", ex);
                return false;
            }
        }

        // ============================================
        // DELETE - Eliminar proyecto
        // ============================================
        public bool Delete(int idProyecto)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Proyecto WHERE id_proyecto = @id_proyecto";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_proyecto", idProyecto);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar proyecto ID: {idProyecto}", ex);
                return false;
            }
        }
    }
}