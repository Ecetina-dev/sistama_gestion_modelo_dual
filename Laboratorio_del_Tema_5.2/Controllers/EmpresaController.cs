using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    /// <summary>
    /// Controlador para operaciones CRUD de la entidad Empresa.
    /// </summary>
    public class EmpresaController
    {
        /// <summary>
        /// Inserta una nueva empresa en la base de datos.
        /// </summary>
        public bool Create(Empresa empresa)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO Empresa 
                                     (nombre_comercial, razon_social, rfc, direccion, ciudad, 
                                      estado, cp, telefono_empresa, email_empresa, 
                                      nombre_contacto, puesto_contacto, status_empresa) 
                                     VALUES 
                                     (@nombre_comercial, @razon_social, @rfc, @direccion, @ciudad,
                                      @estado, @cp, @telefono_empresa, @email_empresa,
                                      @nombre_contacto, @puesto_contacto, @status_empresa)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre_comercial", empresa.Nombre_Comercial);
                        cmd.Parameters.AddWithValue("@razon_social", string.IsNullOrEmpty(empresa.Razon_Social) ? (object)DBNull.Value : empresa.Razon_Social);
                        cmd.Parameters.AddWithValue("@rfc", string.IsNullOrEmpty(empresa.RFC) ? (object)DBNull.Value : empresa.RFC);
                        cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(empresa.Direccion) ? (object)DBNull.Value : empresa.Direccion);
                        cmd.Parameters.AddWithValue("@ciudad", string.IsNullOrEmpty(empresa.Ciudad) ? (object)DBNull.Value : empresa.Ciudad);
                        cmd.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(empresa.Estado) ? (object)DBNull.Value : empresa.Estado);
                        cmd.Parameters.AddWithValue("@cp", string.IsNullOrEmpty(empresa.CP) ? (object)DBNull.Value : empresa.CP);
                        cmd.Parameters.AddWithValue("@telefono_empresa", string.IsNullOrEmpty(empresa.Telefono_Empresa) ? (object)DBNull.Value : empresa.Telefono_Empresa);
                        cmd.Parameters.AddWithValue("@email_empresa", string.IsNullOrEmpty(empresa.Email_Empresa) ? (object)DBNull.Value : empresa.Email_Empresa);
                        cmd.Parameters.AddWithValue("@nombre_contacto", string.IsNullOrEmpty(empresa.Nombre_Contacto) ? (object)DBNull.Value : empresa.Nombre_Contacto);
                        cmd.Parameters.AddWithValue("@puesto_contacto", string.IsNullOrEmpty(empresa.Puesto_Contacto) ? (object)DBNull.Value : empresa.Puesto_Contacto);
                        cmd.Parameters.AddWithValue("@status_empresa", string.IsNullOrEmpty(empresa.Status_Empresa) ? Estatus.EmpresaActiva : empresa.Status_Empresa);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear empresa", ex);
                return false;
            }
        }

        /// <summary>
        /// Obtiene todas las empresas de la base de datos.
        /// </summary>
        public List<Empresa> Read()
        {
            List<Empresa> empresas = new List<Empresa>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Empresa ORDER BY nombre_comercial";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Empresa empresa = new Empresa();
                            empresa.Id_Empresa = reader.GetInt32("id_empresa");
                            empresa.Nombre_Comercial = reader.GetString("nombre_comercial");
                            
                            int idx = reader.GetOrdinal("razon_social");
                            empresa.Razon_Social = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("rfc");
                            empresa.RFC = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("direccion");
                            empresa.Direccion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("ciudad");
                            empresa.Ciudad = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("estado");
                            empresa.Estado = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("cp");
                            empresa.CP = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("telefono_empresa");
                            empresa.Telefono_Empresa = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("email_empresa");
                            empresa.Email_Empresa = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("nombre_contacto");
                            empresa.Nombre_Contacto = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            idx = reader.GetOrdinal("puesto_contacto");
                            empresa.Puesto_Contacto = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                            
                            empresa.Status_Empresa = reader.GetString("status_empresa");
                            empresa.Created_At = reader.GetDateTime("created_at");
                            empresa.Updated_At = reader.GetDateTime("updated_at");
                            
                            empresas.Add(empresa);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer empresas", ex);
            }
            return empresas;
        }

        /// <summary>
        /// Obtiene una empresa especifica por su ID.
        /// </summary>
        public Empresa ReadById(int idEmpresa)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM Empresa WHERE id_empresa = @id_empresa";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_empresa", idEmpresa);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Empresa empresa = new Empresa();
                                empresa.Id_Empresa = reader.GetInt32("id_empresa");
                                empresa.Nombre_Comercial = reader.GetString("nombre_comercial");
                                
                                int idx = reader.GetOrdinal("razon_social");
                                empresa.Razon_Social = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("rfc");
                                empresa.RFC = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("direccion");
                                empresa.Direccion = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("ciudad");
                                empresa.Ciudad = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("estado");
                                empresa.Estado = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("cp");
                                empresa.CP = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("telefono_empresa");
                                empresa.Telefono_Empresa = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("email_empresa");
                                empresa.Email_Empresa = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("nombre_contacto");
                                empresa.Nombre_Contacto = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                idx = reader.GetOrdinal("puesto_contacto");
                                empresa.Puesto_Contacto = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                empresa.Status_Empresa = reader.GetString("status_empresa");
                                empresa.Created_At = reader.GetDateTime("created_at");
                                empresa.Updated_At = reader.GetDateTime("updated_at");
                                
                                return empresa;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al buscar empresa", ex);
            }
            return null;
        }

        /// <summary>
        /// Actualiza los datos de una empresa existente.
        /// </summary>
        public bool Update(Empresa empresa)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Empresa SET 
                                     nombre_comercial = @nombre_comercial,
                                     razon_social = @razon_social,
                                     rfc = @rfc,
                                     direccion = @direccion,
                                     ciudad = @ciudad,
                                     estado = @estado,
                                     cp = @cp,
                                     telefono_empresa = @telefono_empresa,
                                     email_empresa = @email_empresa,
                                     nombre_contacto = @nombre_contacto,
                                     puesto_contacto = @puesto_contacto,
                                     status_empresa = @status_empresa
                                     WHERE id_empresa = @id_empresa";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_empresa", empresa.Id_Empresa);
                        cmd.Parameters.AddWithValue("@nombre_comercial", empresa.Nombre_Comercial);
                        cmd.Parameters.AddWithValue("@razon_social", string.IsNullOrEmpty(empresa.Razon_Social) ? (object)DBNull.Value : empresa.Razon_Social);
                        cmd.Parameters.AddWithValue("@rfc", string.IsNullOrEmpty(empresa.RFC) ? (object)DBNull.Value : empresa.RFC);
                        cmd.Parameters.AddWithValue("@direccion", string.IsNullOrEmpty(empresa.Direccion) ? (object)DBNull.Value : empresa.Direccion);
                        cmd.Parameters.AddWithValue("@ciudad", string.IsNullOrEmpty(empresa.Ciudad) ? (object)DBNull.Value : empresa.Ciudad);
                        cmd.Parameters.AddWithValue("@estado", string.IsNullOrEmpty(empresa.Estado) ? (object)DBNull.Value : empresa.Estado);
                        cmd.Parameters.AddWithValue("@cp", string.IsNullOrEmpty(empresa.CP) ? (object)DBNull.Value : empresa.CP);
                        cmd.Parameters.AddWithValue("@telefono_empresa", string.IsNullOrEmpty(empresa.Telefono_Empresa) ? (object)DBNull.Value : empresa.Telefono_Empresa);
                        cmd.Parameters.AddWithValue("@email_empresa", string.IsNullOrEmpty(empresa.Email_Empresa) ? (object)DBNull.Value : empresa.Email_Empresa);
                        cmd.Parameters.AddWithValue("@nombre_contacto", string.IsNullOrEmpty(empresa.Nombre_Contacto) ? (object)DBNull.Value : empresa.Nombre_Contacto);
                        cmd.Parameters.AddWithValue("@puesto_contacto", string.IsNullOrEmpty(empresa.Puesto_Contacto) ? (object)DBNull.Value : empresa.Puesto_Contacto);
                        cmd.Parameters.AddWithValue("@status_empresa", string.IsNullOrEmpty(empresa.Status_Empresa) ? Estatus.EmpresaActiva : empresa.Status_Empresa);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al actualizar empresa", ex);
                return false;
            }
        }

        /// <summary>
        /// Elimina una empresa de la base de datos por su ID.
        /// </summary>
        public bool Delete(int idEmpresa)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "DELETE FROM Empresa WHERE id_empresa = @id_empresa";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_empresa", idEmpresa);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar empresa ID: {idEmpresa}", ex);
                return false;
            }
        }

        /// <summary>
        /// Verifica si ya existe una empresa con el mismo RFC.
        /// </summary>
        public bool ExisteRFC(string rfc, int? excluirId = null)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Empresa WHERE rfc = @rfc";
                    if (excluirId.HasValue)
                        query += " AND id_empresa != @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@rfc", rfc);
                        if (excluirId.HasValue)
                            cmd.Parameters.AddWithValue("@id", excluirId.Value);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar RFC", ex);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la lista de empresas con sus alumnos asignados usando JOIN.
        /// </summary>
        public List<EmpresaConAlumnos> GetEmpresasConAlumnos()
        {
            List<EmpresaConAlumnos> lista = new List<EmpresaConAlumnos>();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
string query = @"SELECT 
                                        e.id_empresa,
                                        e.nombre_comercial,
                                        e.ciudad,
                                        e.status_empresa,
                                        COUNT(ae.id_alumno) AS num_alumnos,
                                        GROUP_CONCAT(CONCAT(a.nombre, ' ', a.apellido_paterno) SEPARATOR ', ') AS alumnos
                                    FROM Empresa e
                                    INNER JOIN Alumno_Empresa ae ON e.id_empresa = ae.id_empresa
                                    INNER JOIN Alumno a ON ae.id_alumno = a.id_alumno
                                    WHERE ae.status_asignacion = @statusActivo
                                    GROUP BY e.id_empresa";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@statusActivo", Estatus.AsignacionActiva);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmpresaConAlumnos item = new EmpresaConAlumnos();
                                item.Id_Empresa = reader.GetInt32("id_empresa");
                                item.Nombre_Comercial = reader.GetString("nombre_comercial");
                                
                                int idx = reader.GetOrdinal("ciudad");
                                item.Ciudad = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                item.Status_Empresa = reader.GetString("status_empresa");
                                item.Num_Alumnos = reader.GetInt32("num_alumnos");
                                
                                idx = reader.GetOrdinal("alumnos");
                                item.Alumnos = reader.IsDBNull(idx) ? null : reader.GetString(idx);
                                
                                lista.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al buscar empresas con alumnos", ex);
            }
            return lista;
        }
    }

    /// <summary>
    /// Clase auxiliar para almacenar el resultado de la consulta JOIN.
    /// </summary>
    public class EmpresaConAlumnos
    {
        public int Id_Empresa { get; set; }
        public string Nombre_Comercial { get; set; }
        public string Ciudad { get; set; }
        public string Status_Empresa { get; set; }
        public int Num_Alumnos { get; set; }
        public string Alumnos { get; set; }
    }
}