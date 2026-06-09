using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class ProfesorController
    {
        // ============================================
        // CREATE - Insertar nuevo profesor
        // ============================================
        public bool Create(Profesor profesor)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Profesor 
                                     (no_empleado, nombre, apellido_paterno, apellido_materno, 
                                      email, telefono, departamento, puesto, status_profesor) 
                                     VALUES 
                                     (@no_empleado, @nombre, @apellido_paterno, @apellido_materno,
                                      @email, @telefono, @departamento, @puesto, @status_profesor)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no_empleado", profesor.No_Empleado);
                        cmd.Parameters.AddWithValue("@nombre", profesor.Nombre);
                        cmd.Parameters.AddWithValue("@apellido_paterno", profesor.Apellido_Paterno);
                        
                        if (string.IsNullOrEmpty(profesor.Apellido_Materno))
                            cmd.Parameters.AddWithValue("@apellido_materno", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@apellido_materno", profesor.Apellido_Materno);
                        
                        if (string.IsNullOrEmpty(profesor.Email))
                            cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@email", profesor.Email);
                        
                        if (string.IsNullOrEmpty(profesor.Telefono))
                            cmd.Parameters.AddWithValue("@telefono", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@telefono", profesor.Telefono);
                        
                        if (string.IsNullOrEmpty(profesor.Departamento))
                            cmd.Parameters.AddWithValue("@departamento", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@departamento", profesor.Departamento);
                        
                        if (string.IsNullOrEmpty(profesor.Puesto))
                            cmd.Parameters.AddWithValue("@puesto", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@puesto", profesor.Puesto);
                        
                        cmd.Parameters.AddWithValue("@status_profesor", string.IsNullOrEmpty(profesor.Status_Profesor) ? Estatus.ProfesorActivo : profesor.Status_Profesor);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear profesor", ex);
                return false;
            }
        }

        // ============================================
        // READ - Obtener todos los profesores
        // ============================================
        public List<Profesor> Read()
        {
            List<Profesor> profesores = new List<Profesor>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Profesor ORDER BY apellido_paterno, nombre";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Profesor profesor = new Profesor();
                            profesor.Id_Profesor = reader.GetInt32("id_profesor");
                            profesor.No_Empleado = reader.GetString("no_empleado");
                            profesor.Nombre = reader.GetString("nombre");
                            profesor.Apellido_Paterno = reader.GetString("apellido_paterno");
                            
                            int idx = reader.GetOrdinal("apellido_materno");
                            profesor.Apellido_Materno = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("email");
                            profesor.Email = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("telefono");
                            profesor.Telefono = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("departamento");
                            profesor.Departamento = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("puesto");
                            profesor.Puesto = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            profesor.Status_Profesor = reader.GetString("status_profesor");
                            profesor.Created_At = reader.GetDateTime("created_at");
                            profesor.Updated_At = reader.GetDateTime("updated_at");
                            
                            profesores.Add(profesor);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer profesores", ex);
            }
            return profesores;
        }

        // ============================================
        // READ by ID - Obtener profesor específico
        // ============================================
        public Profesor ReadById(int idProfesor)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Profesor WHERE id_profesor = @id_profesor";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_profesor", idProfesor);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Profesor profesor = new Profesor();
                                profesor.Id_Profesor = reader.GetInt32("id_profesor");
                                profesor.No_Empleado = reader.GetString("no_empleado");
                                profesor.Nombre = reader.GetString("nombre");
                                profesor.Apellido_Paterno = reader.GetString("apellido_paterno");
                                
                                int idx = reader.GetOrdinal("apellido_materno");
                                profesor.Apellido_Materno = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("email");
                                profesor.Email = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("telefono");
                                profesor.Telefono = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("departamento");
                                profesor.Departamento = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("puesto");
                                profesor.Puesto = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                profesor.Status_Profesor = reader.GetString("status_profesor");
                                profesor.Created_At = reader.GetDateTime("created_at");
                                profesor.Updated_At = reader.GetDateTime("updated_at");
                                
                                return profesor;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al buscar profesor ID: {idProfesor}", ex);
            }
            return null;
        }

        // ============================================
        // UPDATE - Actualizar profesor
        // ============================================
        public bool Update(Profesor profesor)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Profesor SET 
                                     no_empleado = @no_empleado,
                                     nombre = @nombre,
                                     apellido_paterno = @apellido_paterno,
                                     apellido_materno = @apellido_materno,
                                     email = @email,
                                     telefono = @telefono,
                                     departamento = @departamento,
                                     puesto = @puesto,
                                     status_profesor = @status_profesor
                                     WHERE id_profesor = @id_profesor";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_profesor", profesor.Id_Profesor);
                        cmd.Parameters.AddWithValue("@no_empleado", profesor.No_Empleado);
                        cmd.Parameters.AddWithValue("@nombre", profesor.Nombre);
                        cmd.Parameters.AddWithValue("@apellido_paterno", profesor.Apellido_Paterno);
                        
                        if (string.IsNullOrEmpty(profesor.Apellido_Materno))
                            cmd.Parameters.AddWithValue("@apellido_materno", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@apellido_materno", profesor.Apellido_Materno);
                        
                        if (string.IsNullOrEmpty(profesor.Email))
                            cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@email", profesor.Email);
                        
                        if (string.IsNullOrEmpty(profesor.Telefono))
                            cmd.Parameters.AddWithValue("@telefono", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@telefono", profesor.Telefono);
                        
                        if (string.IsNullOrEmpty(profesor.Departamento))
                            cmd.Parameters.AddWithValue("@departamento", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@departamento", profesor.Departamento);
                        
                        if (string.IsNullOrEmpty(profesor.Puesto))
                            cmd.Parameters.AddWithValue("@puesto", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@puesto", profesor.Puesto);
                        
                        cmd.Parameters.AddWithValue("@status_profesor", string.IsNullOrEmpty(profesor.Status_Profesor) ? Estatus.ProfesorActivo : profesor.Status_Profesor);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al actualizar profesor", ex);
                return false;
            }
        }

        // ============================================
        // DELETE - Eliminar profesor
        // ============================================
        public bool Delete(int idProfesor)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Profesor WHERE id_profesor = @id_profesor";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_profesor", idProfesor);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar profesor ID: {idProfesor}", ex);
                return false;
            }
        }

        // ============================================
        // EXISTS - Verificar si ya existe el No_Empleado
        // ============================================
        public bool ExisteNoEmpleado(string noEmpleado, int? excluirId = null)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Profesor WHERE no_empleado = @no_empleado";
                    if (excluirId.HasValue)
                        query += " AND id_profesor != @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no_empleado", noEmpleado);
                        if (excluirId.HasValue)
                            cmd.Parameters.AddWithValue("@id", excluirId.Value);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar no_empleado", ex);
                return false;
            }
        }

        // ============================================
        // JOIN: Profesores con Proyectos (como directores/asesores)
        // ============================================
        public List<ProfesorConProyectos> GetProfesoresConProyectos()
        {
            List<ProfesorConProyectos> lista = new List<ProfesorConProyectos>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT 
                                        p.id_profesor,
                                        p.no_empleado,
                                        p.nombre,
                                        p.apellido_paterno,
                                        p.apellido_materno,
                                        p.departamento,
                                        COUNT(DISTINCT pr.id_proyecto) AS num_proyectos,
                                        GROUP_CONCAT(DISTINCT pp.tipo_supervision SEPARATOR ', ') AS tipos_supervision,
                                        GROUP_CONCAT(DISTINCT CONCAT(pp.tipo_supervision, ': ', pr.nombre_proyecto) SEPARATOR ', ') AS proyectos
                                    FROM Profesor p
                                    INNER JOIN Proyecto_Profesor pp ON p.id_profesor = pp.id_profesor
                                    INNER JOIN Proyecto pr ON pp.id_proyecto = pr.id_proyecto
                                    WHERE pp.status_supervision = @statusActiva
                                    GROUP BY p.id_profesor, p.no_empleado, p.nombre, p.apellido_paterno, p.apellido_materno, p.departamento";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@statusActiva", Estatus.AsignacionActiva);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ProfesorConProyectos item = new ProfesorConProyectos();
                                item.Id_Profesor = reader.GetInt32("id_profesor");
                                item.No_Empleado = reader.GetString("no_empleado");
                                item.Nombre = reader.GetString("nombre");
                                item.Apellido_Paterno = reader.GetString("apellido_paterno");
                                
                                int idx = reader.GetOrdinal("apellido_materno");
                                item.Apellido_Materno = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("departamento");
                                item.Departamento = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                item.Num_Proyectos = reader.GetInt32("num_proyectos");
                                
                                idx = reader.GetOrdinal("tipos_supervision");
                                item.Tipos_Supervision = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("proyectos");
                                item.Proyectos = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                lista.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al buscar profesores con proyectos", ex);
            }
            return lista;
        }
    }

    // ============================================
    // CLASE AUXILIAR PARA EL JOIN
    // ============================================
    public class ProfesorConProyectos
    {
        public int Id_Profesor { get; set; }
        public string No_Empleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Departamento { get; set; }
        public string Tipos_Supervision { get; set; }
        public int Num_Proyectos { get; set; }
        public string Proyectos { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido_Paterno} {Apellido_Materno}";
    }
}