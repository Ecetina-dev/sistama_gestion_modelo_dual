using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    /// <summary>
    /// Controlador para operaciones CRUD de la entidad Alumno.
    /// </summary>
    public class AlumnoController
    {
        /// <summary>
        /// Inserta un nuevo alumno en la base de datos.
        /// </summary>
        public bool Create(Alumno alumno)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    // Validar formato de email (capa server-side, doble defensa)
                    if (!string.IsNullOrEmpty(alumno.Email) && 
                        !System.Text.RegularExpressions.Regex.IsMatch(alumno.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new CrudOperationException("El formato del email no es válido.", "Validate", alumno);
                    }

                    string query = @"INSERT INTO Alumno 
                                     (no_control, nombre, apellido_paterno, apellido_materno, 
                                      email, telefono, fecha_nacimiento, status_alumno) 
                                     VALUES 
                                     (@no_control, @nombre, @apellido_paterno, @apellido_materno,
                                      @email, @telefono, @fecha_nacimiento, @status_alumno)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no_control", alumno.No_Control);
                        cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                        cmd.Parameters.AddWithValue("@apellido_paterno", alumno.Apellido_Paterno);
                        
                        if (string.IsNullOrEmpty(alumno.Apellido_Materno))
                            cmd.Parameters.AddWithValue("@apellido_materno", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@apellido_materno", alumno.Apellido_Materno);
                        
                        if (string.IsNullOrEmpty(alumno.Email))
                            cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@email", alumno.Email);
                        
                        if (string.IsNullOrEmpty(alumno.Telefono))
                            cmd.Parameters.AddWithValue("@telefono", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@telefono", alumno.Telefono);
                        
                        if (alumno.Fecha_Nacimiento.HasValue)
                            cmd.Parameters.AddWithValue("@fecha_nacimiento", alumno.Fecha_Nacimiento.Value);
                        else
                            cmd.Parameters.AddWithValue("@fecha_nacimiento", DBNull.Value);
                        
                        cmd.Parameters.AddWithValue("@status_alumno", string.IsNullOrEmpty(alumno.Status_Alumno) ? Estatus.AlumnoActivo : alumno.Status_Alumno);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            InsertarBitacora(conn, "INSERT", alumno);
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear alumno", ex);
                throw new CrudOperationException($"Error al crear el alumno: {ex.Message}", "Create", alumno);
            }
        }

        /// <summary>
        /// Obtiene todos los alumnos de la base de datos.
        /// </summary>
        public List<Alumno> Read()
        {
            List<Alumno> alumnos = new List<Alumno>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Alumno WHERE is_deleted = 0 ORDER BY apellido_paterno, nombre";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Alumno alumno = new Alumno();
                            alumno.Id_Alumno = reader.GetInt32("id_alumno");
                            alumno.No_Control = reader.GetString("no_control");
                            alumno.Nombre = reader.GetString("nombre");
                            alumno.Apellido_Paterno = reader.GetString("apellido_paterno");
                            
                            int idxApMaterno = reader.GetOrdinal("apellido_materno");
                            alumno.Apellido_Materno = reader.IsDBNull(idxApMaterno) ? null : reader.GetString(idxApMaterno);
                            
                            int idxEmail = reader.GetOrdinal("email");
                            alumno.Email = reader.IsDBNull(idxEmail) ? null : reader.GetString(idxEmail);
                            
                            int idxTelefono = reader.GetOrdinal("telefono");
                            alumno.Telefono = reader.IsDBNull(idxTelefono) ? null : reader.GetString(idxTelefono);
                            
                            int idxFechaNac = reader.GetOrdinal("fecha_nacimiento");
                            alumno.Fecha_Nacimiento = reader.IsDBNull(idxFechaNac) ? (DateTime?)null : reader.GetDateTime(idxFechaNac);
                            
                            alumno.Status_Alumno = reader.GetString("status_alumno");
                            alumno.Created_At = reader.GetDateTime("created_at");
                            alumno.Updated_At = reader.GetDateTime("updated_at");
                            
                            alumnos.Add(alumno);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer alumnos", ex);
            }
            return alumnos;
        }

        /// <summary>
        /// Obtiene un alumno especifico por su ID.
        /// </summary>
        public Alumno ReadById(int idAlumno)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Alumno WHERE id_alumno = @id_alumno AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_alumno", idAlumno);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Alumno alumno = new Alumno();
                                alumno.Id_Alumno = reader.GetInt32("id_alumno");
                                alumno.No_Control = reader.GetString("no_control");
                                alumno.Nombre = reader.GetString("nombre");
                                alumno.Apellido_Paterno = reader.GetString("apellido_paterno");
                                
                                int idxApMaterno = reader.GetOrdinal("apellido_materno");
                                alumno.Apellido_Materno = reader.IsDBNull(idxApMaterno) ? null : reader.GetString(idxApMaterno);
                                
                                int idxEmail = reader.GetOrdinal("email");
                                alumno.Email = reader.IsDBNull(idxEmail) ? null : reader.GetString(idxEmail);
                                
                                int idxTelefono = reader.GetOrdinal("telefono");
                                alumno.Telefono = reader.IsDBNull(idxTelefono) ? null : reader.GetString(idxTelefono);
                                
                                int idxFechaNac = reader.GetOrdinal("fecha_nacimiento");
                                alumno.Fecha_Nacimiento = reader.IsDBNull(idxFechaNac) ? (DateTime?)null : reader.GetDateTime(idxFechaNac);
                                
                                alumno.Status_Alumno = reader.GetString("status_alumno");
                                alumno.Created_At = reader.GetDateTime("created_at");
                                alumno.Updated_At = reader.GetDateTime("updated_at");
                                
                                return alumno;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al buscar alumno", ex);
            }
            return null;
        }

        /// <summary>
        /// Actualiza los datos de un alumno existente.
        /// </summary>
        public bool Update(Alumno alumno)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    // Validar formato de email (capa server-side, doble defensa)
                    if (!string.IsNullOrEmpty(alumno.Email) && 
                        !System.Text.RegularExpressions.Regex.IsMatch(alumno.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    {
                        throw new CrudOperationException("El formato del email no es válido.", "Validate", alumno);
                    }

                    string query = @"UPDATE Alumno SET 
                                     no_control = @no_control,
                                     nombre = @nombre,
                                     apellido_paterno = @apellido_paterno,
                                     apellido_materno = @apellido_materno,
                                     email = @email,
                                     telefono = @telefono,
                                     fecha_nacimiento = @fecha_nacimiento,
                                     status_alumno = @status_alumno
                                     WHERE id_alumno = @id_alumno AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_alumno", alumno.Id_Alumno);
                        cmd.Parameters.AddWithValue("@no_control", alumno.No_Control);
                        cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
                        cmd.Parameters.AddWithValue("@apellido_paterno", alumno.Apellido_Paterno);
                        
                        if (string.IsNullOrEmpty(alumno.Apellido_Materno))
                            cmd.Parameters.AddWithValue("@apellido_materno", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@apellido_materno", alumno.Apellido_Materno);
                        
                        if (string.IsNullOrEmpty(alumno.Email))
                            cmd.Parameters.AddWithValue("@email", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@email", alumno.Email);
                        
                        if (string.IsNullOrEmpty(alumno.Telefono))
                            cmd.Parameters.AddWithValue("@telefono", DBNull.Value);
                        else
                            cmd.Parameters.AddWithValue("@telefono", alumno.Telefono);
                        
                        if (alumno.Fecha_Nacimiento.HasValue)
                            cmd.Parameters.AddWithValue("@fecha_nacimiento", alumno.Fecha_Nacimiento.Value);
                        else
                            cmd.Parameters.AddWithValue("@fecha_nacimiento", DBNull.Value);
                        
                        cmd.Parameters.AddWithValue("@status_alumno", string.IsNullOrEmpty(alumno.Status_Alumno) ? Estatus.AlumnoActivo : alumno.Status_Alumno);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            InsertarBitacora(conn, "UPDATE", alumno);
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al actualizar alumno", ex);
                throw new CrudOperationException($"Error al actualizar el alumno: {ex.Message}", "Update", alumno);
            }
        }

        /// <summary>
        /// Realiza un soft delete del alumno (no elimina físicamente).
        /// </summary>
        public bool Delete(int idAlumno)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    // SOFT DELETE — no borrar físicamente
                    string query = @"UPDATE Alumno SET is_deleted = 1, deleted_at = NOW() 
                                     WHERE id_alumno = @id_alumno AND is_deleted = 0";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_alumno", idAlumno);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        
                        if (rowsAffected > 0)
                            InsertarBitacora(conn, "DELETE", idAlumno, $"Alumno ID: {idAlumno}");
                        
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar (soft) alumno ID: {idAlumno}", ex);
                throw new CrudOperationException($"Error al eliminar el alumno: {ex.Message}", "Delete", null);
            }
        }

        /// <summary>
        /// Obtiene la lista de alumnos con su empresa asignada usando JOIN.
        /// </summary>
        public List<AlumnoConEmpresa> GetAlumnosConEmpresa()
        {
            List<AlumnoConEmpresa> lista = new List<AlumnoConEmpresa>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT 
                                        a.id_alumno,
                                        a.no_control,
                                        a.nombre,
                                        a.apellido_paterno,
                                        a.apellido_materno,
                                        a.email,
                                        a.telefono,
                                        a.status_alumno,
                                        e.nombre_comercial AS empresa,
                                        ae.puesto,
                                        ae.salario
                                    FROM Alumno a
                                    INNER JOIN Alumno_Empresa ae ON a.id_alumno = ae.id_alumno
                                    INNER JOIN Empresa e ON ae.id_empresa = e.id_empresa
                                     WHERE ae.status_asignacion = @statusActivo
                                     AND a.is_deleted = 0
                                     ORDER BY a.apellido_paterno, a.nombre";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@statusActivo", Estatus.AsignacionActiva);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlumnoConEmpresa item = new AlumnoConEmpresa();
                                item.Id_Alumno = reader.GetInt32("id_alumno");
                                item.No_Control = reader.GetString("no_control");
                                item.Nombre = reader.GetString("nombre");
                                item.Apellido_Paterno = reader.GetString("apellido_paterno");
                                
                                int idxApMaterno = reader.GetOrdinal("apellido_materno");
                                item.Apellido_Materno = reader.IsDBNull(idxApMaterno) ? null : reader.GetString(idxApMaterno);
                                
                                int idxEmail = reader.GetOrdinal("email");
                                item.Email = reader.IsDBNull(idxEmail) ? null : reader.GetString(idxEmail);
                                
                                int idxTelefono = reader.GetOrdinal("telefono");
                                item.Telefono = reader.IsDBNull(idxTelefono) ? null : reader.GetString(idxTelefono);
                                
                                item.Status_Alumno = reader.GetString("status_alumno");
                                item.Empresa = reader.GetString("empresa");
                                item.Puesto = reader.GetString("puesto");
                                
                                int idxSalario = reader.GetOrdinal("salario");
                                item.Salario = reader.IsDBNull(idxSalario) ? 0 : reader.GetDecimal(idxSalario);
                                
                                lista.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al buscar alumnos con empresa", ex);
            }
            return lista;
        }
        /// <summary>
        /// Verifica si ya existe un alumno con el mismo número de control.
        /// </summary>
        public bool ExisteNoControl(string noControl, int? excluirId = null)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Alumno WHERE no_control = @no_control AND is_deleted = 0";
                    if (excluirId.HasValue)
                        query += " AND id_alumno != @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@no_control", noControl);
                        if (excluirId.HasValue)
                            cmd.Parameters.AddWithValue("@id", excluirId.Value);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar no_control", ex);
                return false;
            }
        }

        /// <summary>
        /// Registra una operacion en la bitacora del sistema.
        /// </summary>
        private void InsertarBitacora(MySqlConnection conn, string operacion, Alumno alumno)
        {
            try
            {
                string query = @"INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, datos_nuevos, fecha)
                                 VALUES ('Alumno', @id, @op, @usr, @datos, NOW())";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", alumno.Id_Alumno > 0 ? alumno.Id_Alumno : 0);
                    cmd.Parameters.AddWithValue("@op", operacion);
                    cmd.Parameters.AddWithValue("@usr", SesionActiva.Instance?.Username ?? "sistema");
                    string datos = $"{alumno.No_Control} - {alumno.Nombre} {alumno.Apellido_Paterno}";
                    cmd.Parameters.AddWithValue("@datos", datos);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { /* no bloquear CRUD por error de bitácora */ }
        }

        /// <summary>
        /// Registra una eliminacion en la bitacora usando solo el ID.
        /// </summary>
        private void InsertarBitacora(MySqlConnection conn, string operacion, int idAlumno, string datos)
        {
            try
            {
                string query = @"INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, datos_nuevos, fecha)
                                 VALUES ('Alumno', @id, @op, @usr, @datos, NOW())";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idAlumno);
                    cmd.Parameters.AddWithValue("@op", operacion);
                    cmd.Parameters.AddWithValue("@usr", SesionActiva.Instance?.Username ?? "sistema");
                    cmd.Parameters.AddWithValue("@datos", datos);
                    cmd.ExecuteNonQuery();
                }
            }
            catch { /* no bloquear CRUD por error de bitácora */ }
        }
    }

    /// <summary>
    /// Clase auxiliar para almacenar el resultado de la consulta JOIN.
    /// </summary>
    public class AlumnoConEmpresa
    {
        public int Id_Alumno { get; set; }
        public string No_Control { get; set; }
        public string Nombre { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Status_Alumno { get; set; }
        public string Empresa { get; set; }
        public string Puesto { get; set; }
        public decimal Salario { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellido_Paterno} {Apellido_Materno}";
    }
}