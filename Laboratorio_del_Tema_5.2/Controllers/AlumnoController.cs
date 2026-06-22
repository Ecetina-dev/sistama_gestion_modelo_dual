using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

using Laboratorio_del_Tema_5_2.Controllers.Services;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    /// <summary>
    /// Controller for student CRUD operations with enterprise validation,
    /// audit columns, status transitions, and soft-delete reservation.
    /// </summary>
    public class AlumnoController
    {
        private readonly AlumnoService _alumnoService = new AlumnoService();
        /// <summary>
        /// Inserts a new student after validating required fields, formats,
        /// and uniqueness (active and soft-deleted records).
        /// </summary>
        public bool Create(Alumno alumno)
        {
            try
            {
                return _alumnoService.Create(alumno);
            }
            catch (CrudOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("Error en controller Create", ex);
                throw new CrudOperationException("Ocurrio un error al crear el alumno.", "Create", alumno);
            }
        }

        public List<Alumno> Read()
        {
            return _alumnoService.Read();
        }

        public Alumno ReadById(int idAlumno)
        {
            return _alumnoService.ReadById(idAlumno);
        }

        public bool Update(Alumno alumno)
        {
            try
            {
                return _alumnoService.Update(alumno);
            }
            catch (CrudOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("Error en controller Update", ex);
                throw new CrudOperationException("Ocurrio un error al actualizar el alumno.", "Update", alumno);
            }
        }

        public bool Delete(int idAlumno, string deletedReason)
        {
            try
            {
                return _alumnoService.Delete(idAlumno, deletedReason);
            }
            catch (CrudOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Logger.Error("Error en controller Delete", ex);
                throw new CrudOperationException("Ocurrio un error al eliminar el alumno.", "Delete", null);
            }
        }

        public List<AlumnoConEmpresa> GetAlumnosConEmpresa()
        {
            List<AlumnoConEmpresa> lista = new List<AlumnoConEmpresa>();

            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
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

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@statusActivo", Estatus.AsignacionActiva);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlumnoConEmpresa item = new AlumnoConEmpresa();
                                item.Id_Alumno = reader.GetInt32(reader.GetOrdinal("id_alumno"));
                                item.No_Control = reader.GetString(reader.GetOrdinal("no_control"));
                                item.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
                                item.Apellido_Paterno = reader.GetString(reader.GetOrdinal("apellido_paterno"));

                                int idxApMaterno = reader.GetOrdinal("apellido_materno");
                                item.Apellido_Materno = reader.IsDBNull(idxApMaterno) ? null : reader.GetString(idxApMaterno);

                                int idxEmail = reader.GetOrdinal("email");
                                item.Email = reader.IsDBNull(idxEmail) ? null : reader.GetString(idxEmail);

                                int idxTelefono = reader.GetOrdinal("telefono");
                                item.Telefono = reader.IsDBNull(idxTelefono) ? null : reader.GetString(idxTelefono);

                                item.Status_Alumno = reader.GetString(reader.GetOrdinal("status_alumno"));
                                item.Empresa = reader.GetString(reader.GetOrdinal("empresa"));
                                item.Puesto = reader.GetString(reader.GetOrdinal("puesto"));

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
                Logger.Error("Error reading alumnos with empresa", ex);
            }
            return lista;
        }

        // PUBLIC EXISTENCE HELPERS
        public bool ExisteNoControl(string noControl, int? excluirId = null)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    return ExisteNoControl(conn, noControl, excluirId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking no_control", ex);
                return false;
            }
        }

        public bool ExisteCurp(string curp, int? excluirId = null)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    return ExisteCurp(conn, curp, excluirId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking CURP", ex);
                return false;
            }
        }

        public bool ExisteEmail(string email, int? excluirId = null)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    return ExisteEmail(conn, email, excluirId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking email", ex);
                return false;
            }
        }

        public bool ExisteNoControlEliminado(string noControl)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    return ExisteNoControlEliminado(conn, noControl);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking deleted no_control", ex);
                return false;
            }
        }

        public bool ExisteCurpEliminado(string curp)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    return ExisteCurpEliminado(conn, curp);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking deleted CURP", ex);
                return false;
            }
        }

        // PRIVATE HELPERS
        private Alumno ReadById(SqlConnection conn, int idAlumno)
        {
            string query = "SELECT * FROM Alumno WHERE id_alumno = @id_alumno AND is_deleted = 0";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_alumno", idAlumno);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapAlumnoFromReader(reader);
                }
            }
            return null;
        }

        private Alumno MapAlumnoFromReader(SqlDataReader reader)
        {
            Alumno alumno = new Alumno();
            alumno.Id_Alumno = reader.GetInt32(reader.GetOrdinal("id_alumno"));
            alumno.No_Control = reader.GetString(reader.GetOrdinal("no_control"));
            alumno.Nombre = reader.GetString(reader.GetOrdinal("nombre"));
            alumno.Apellido_Paterno = reader.GetString(reader.GetOrdinal("apellido_paterno"));

            alumno.Apellido_Materno = GetStringOrNull(reader, "apellido_materno");
            alumno.Email = GetStringOrNull(reader, "email");
            alumno.Telefono = GetStringOrNull(reader, "telefono");

            int idxFechaNac = reader.GetOrdinal("fecha_nacimiento");
            alumno.Fecha_Nacimiento = reader.IsDBNull(idxFechaNac) ? (DateTime?)null : reader.GetDateTime(idxFechaNac);

            alumno.Status_Alumno = reader.GetString(reader.GetOrdinal("status_alumno"));
            alumno.Created_At = reader.GetDateTime(reader.GetOrdinal("created_at"));
            alumno.Updated_At = reader.GetDateTime(reader.GetOrdinal("updated_at"));

            // Phase 1 enterprise columns
            alumno.Curp = GetStringOrNull(reader, "curp");
            alumno.Rfc = GetStringOrNull(reader, "rfc");
            alumno.Nss = GetStringOrNull(reader, "nss");
            alumno.Genero = GetStringOrNull(reader, "genero");
            alumno.Estado_Civil = GetStringOrNull(reader, "estado_civil");
            alumno.Nacionalidad = GetStringOrNull(reader, "nacionalidad");
            alumno.Direccion_Calle = GetStringOrNull(reader, "direccion_calle");
            alumno.Direccion_Numero = GetStringOrNull(reader, "direccion_numero");
            alumno.Direccion_Colonia = GetStringOrNull(reader, "direccion_colonia");
            alumno.Direccion_Ciudad = GetStringOrNull(reader, "direccion_ciudad");
            alumno.Direccion_Estado = GetStringOrNull(reader, "direccion_estado");
            alumno.Direccion_Cp = GetStringOrNull(reader, "direccion_cp");
            alumno.Telefono_Fijo = GetStringOrNull(reader, "telefono_fijo");
            alumno.Contacto_Emergencia_Nombre = GetStringOrNull(reader, "contacto_emergencia_nombre");
            alumno.Contacto_Emergencia_Telefono = GetStringOrNull(reader, "contacto_emergencia_telefono");
            alumno.Contacto_Emergencia_Parentesco = GetStringOrNull(reader, "contacto_emergencia_parentesco");

            alumno.Id_Carrera = GetIntOrNull(reader, "id_carrera");
            alumno.Semestre = GetIntOrNull(reader, "semestre");
            alumno.Grupo = GetStringOrNull(reader, "grupo");
            alumno.Turno = GetStringOrNull(reader, "turno");

            alumno.Fecha_Ingreso = GetDateOrNull(reader, "fecha_ingreso");
            alumno.Fecha_Egreso = GetDateOrNull(reader, "fecha_egreso");
            alumno.Fecha_Baja = GetDateOrNull(reader, "fecha_baja");

            alumno.Motivo_Baja = GetStringOrNull(reader, "motivo_baja");
            alumno.Promedio_General = GetDecimalOrNull(reader, "promedio_general");

            alumno.Created_By = GetIntOrNull(reader, "created_by");
            alumno.Updated_By = GetIntOrNull(reader, "updated_by");
            alumno.Deleted_By = GetIntOrNull(reader, "deleted_by");
            alumno.Deleted_Reason = GetStringOrNull(reader, "deleted_reason");
            alumno.Status_Change_Reason = GetStringOrNull(reader, "status_change_reason");

            return alumno;
        }

        private string GetStringOrNull(SqlDataReader reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? null : reader.GetString(idx);
        }

        private int? GetIntOrNull(SqlDataReader reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? (int?)null : reader.GetInt32(idx);
        }

        private DateTime? GetDateOrNull(SqlDataReader reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? (DateTime?)null : reader.GetDateTime(idx);
        }

        private decimal? GetDecimalOrNull(SqlDataReader reader, string column)
        {
            int idx = reader.GetOrdinal(column);
            return reader.IsDBNull(idx) ? (decimal?)null : reader.GetDecimal(idx);
        }

        private void AgregarParametrosAlumno(SqlCommand cmd, Alumno alumno, bool incluirAuditAlta, bool incluirAuditCambio)
        {
            cmd.Parameters.AddWithValue("@no_control", alumno.No_Control);
            cmd.Parameters.AddWithValue("@nombre", alumno.Nombre);
            cmd.Parameters.AddWithValue("@apellido_paterno", alumno.Apellido_Paterno);
            cmd.Parameters.AddWithValue("@apellido_materno", string.IsNullOrEmpty(alumno.Apellido_Materno) ? (object)DBNull.Value : alumno.Apellido_Materno);
            cmd.Parameters.AddWithValue("@email", string.IsNullOrEmpty(alumno.Email) ? (object)DBNull.Value : alumno.Email);
            cmd.Parameters.AddWithValue("@telefono", string.IsNullOrEmpty(alumno.Telefono) ? (object)DBNull.Value : alumno.Telefono);
            cmd.Parameters.AddWithValue("@fecha_nacimiento", alumno.Fecha_Nacimiento.HasValue ? (object)alumno.Fecha_Nacimiento.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@status_alumno", string.IsNullOrEmpty(alumno.Status_Alumno) ? Estatus.AlumnoActivo : alumno.Status_Alumno);

            cmd.Parameters.AddWithValue("@curp", string.IsNullOrEmpty(alumno.Curp) ? (object)DBNull.Value : alumno.Curp.ToUpperInvariant());
            cmd.Parameters.AddWithValue("@rfc", string.IsNullOrEmpty(alumno.Rfc) ? (object)DBNull.Value : alumno.Rfc.ToUpperInvariant());
            cmd.Parameters.AddWithValue("@nss", string.IsNullOrEmpty(alumno.Nss) ? (object)DBNull.Value : alumno.Nss);
            cmd.Parameters.AddWithValue("@genero", string.IsNullOrEmpty(alumno.Genero) ? (object)DBNull.Value : alumno.Genero);
            cmd.Parameters.AddWithValue("@estado_civil", string.IsNullOrEmpty(alumno.Estado_Civil) ? (object)DBNull.Value : alumno.Estado_Civil);
            cmd.Parameters.AddWithValue("@nacionalidad", string.IsNullOrEmpty(alumno.Nacionalidad) ? (object)DBNull.Value : alumno.Nacionalidad);

            cmd.Parameters.AddWithValue("@direccion_calle", string.IsNullOrEmpty(alumno.Direccion_Calle) ? (object)DBNull.Value : alumno.Direccion_Calle);
            cmd.Parameters.AddWithValue("@direccion_numero", string.IsNullOrEmpty(alumno.Direccion_Numero) ? (object)DBNull.Value : alumno.Direccion_Numero);
            cmd.Parameters.AddWithValue("@direccion_colonia", string.IsNullOrEmpty(alumno.Direccion_Colonia) ? (object)DBNull.Value : alumno.Direccion_Colonia);
            cmd.Parameters.AddWithValue("@direccion_ciudad", string.IsNullOrEmpty(alumno.Direccion_Ciudad) ? (object)DBNull.Value : alumno.Direccion_Ciudad);
            cmd.Parameters.AddWithValue("@direccion_estado", string.IsNullOrEmpty(alumno.Direccion_Estado) ? (object)DBNull.Value : alumno.Direccion_Estado);
            cmd.Parameters.AddWithValue("@direccion_cp", string.IsNullOrEmpty(alumno.Direccion_Cp) ? (object)DBNull.Value : alumno.Direccion_Cp);

            cmd.Parameters.AddWithValue("@telefono_fijo", string.IsNullOrEmpty(alumno.Telefono_Fijo) ? (object)DBNull.Value : alumno.Telefono_Fijo);
            cmd.Parameters.AddWithValue("@contacto_emergencia_nombre", string.IsNullOrEmpty(alumno.Contacto_Emergencia_Nombre) ? (object)DBNull.Value : alumno.Contacto_Emergencia_Nombre);
            cmd.Parameters.AddWithValue("@contacto_emergencia_telefono", string.IsNullOrEmpty(alumno.Contacto_Emergencia_Telefono) ? (object)DBNull.Value : alumno.Contacto_Emergencia_Telefono);
            cmd.Parameters.AddWithValue("@contacto_emergencia_parentesco", string.IsNullOrEmpty(alumno.Contacto_Emergencia_Parentesco) ? (object)DBNull.Value : alumno.Contacto_Emergencia_Parentesco);

            cmd.Parameters.AddWithValue("@id_carrera", alumno.Id_Carrera.HasValue ? (object)alumno.Id_Carrera.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@semestre", alumno.Semestre.HasValue ? (object)alumno.Semestre.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@grupo", string.IsNullOrEmpty(alumno.Grupo) ? (object)DBNull.Value : alumno.Grupo);
            cmd.Parameters.AddWithValue("@turno", string.IsNullOrEmpty(alumno.Turno) ? (object)DBNull.Value : alumno.Turno.ToLowerInvariant());

            cmd.Parameters.AddWithValue("@fecha_ingreso", alumno.Fecha_Ingreso.HasValue ? (object)alumno.Fecha_Ingreso.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha_egreso", alumno.Fecha_Egreso.HasValue ? (object)alumno.Fecha_Egreso.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha_baja", alumno.Fecha_Baja.HasValue ? (object)alumno.Fecha_Baja.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@motivo_baja", string.IsNullOrEmpty(alumno.Motivo_Baja) ? (object)DBNull.Value : alumno.Motivo_Baja);
            cmd.Parameters.AddWithValue("@promedio_general", alumno.Promedio_General.HasValue ? (object)alumno.Promedio_General.Value : DBNull.Value);

            if (incluirAuditAlta)
                cmd.Parameters.AddWithValue("@created_by", alumno.Created_By ?? (object)DBNull.Value);

            if (incluirAuditCambio)
            {
                cmd.Parameters.AddWithValue("@status_change_reason", string.IsNullOrEmpty(alumno.Status_Change_Reason) ? (object)DBNull.Value : alumno.Status_Change_Reason);
                cmd.Parameters.AddWithValue("@updated_by", alumno.Updated_By ?? (object)DBNull.Value);
            }
        }

        private int? ObtenerUsuarioAuditoria()
        {
            return SesionActiva.Instance?.Id_Usuario > 0 ? SesionActiva.Instance.Id_Usuario : (int?)null;
        }

        private void ValidarCamposRequeridos(SqlConnection conn, Alumno alumno, bool esNuevo)
        {
            if (string.IsNullOrWhiteSpace(alumno.No_Control))
                throw new CrudOperationException("El numero de control es requerido.", "Validate", alumno);

            if (string.IsNullOrWhiteSpace(alumno.Nombre))
                throw new CrudOperationException("El nombre es requerido.", "Validate", alumno);

            if (string.IsNullOrWhiteSpace(alumno.Apellido_Paterno))
                throw new CrudOperationException("El apellido paterno es requerido.", "Validate", alumno);

            if (esNuevo)
            {
                // Enterprise required fields only for new records.
                // Existing records are allowed to be updated incrementally until PR-4 adds the new inputs.
                if (string.IsNullOrWhiteSpace(alumno.Curp))
                    throw new CrudOperationException("El CURP es requerido.", "Validate", alumno);

                if (string.IsNullOrWhiteSpace(alumno.Genero))
                    throw new CrudOperationException("El genero es requerido.", "Validate", alumno);

                if (!alumno.Id_Carrera.HasValue)
                    throw new CrudOperationException("La carrera es requerida.", "Validate", alumno);

                if (!alumno.Semestre.HasValue)
                    throw new CrudOperationException("El semestre es requerido.", "Validate", alumno);

                if (!alumno.Fecha_Ingreso.HasValue)
                    throw new CrudOperationException("La fecha de ingreso es requerida.", "Validate", alumno);

                if (string.IsNullOrWhiteSpace(alumno.Email))
                    throw new CrudOperationException("El email es requerido.", "Validate", alumno);
            }

            // ALTO-06 fix: validar que la carrera seleccionada exista realmente
            if (alumno.Id_Carrera.HasValue && !CarreraExiste(conn, alumno.Id_Carrera.Value))
                throw new CrudOperationException("La carrera seleccionada no existe.", "Validate", alumno);
        }

        private void ValidarFormatos(Alumno alumno)
        {
            string error;

            if (!AlumnoValidator.ValidarNoControl(alumno.No_Control, out error))
                throw new CrudOperationException(error, "Validate", alumno);

            if (!string.IsNullOrWhiteSpace(alumno.Curp) && !AlumnoValidator.ValidarCurp(alumno.Curp, out error))
                throw new CrudOperationException(error, "Validate", alumno);

            if (!string.IsNullOrWhiteSpace(alumno.Rfc) && !AlumnoValidator.ValidarRfc(alumno.Rfc, out error))
                throw new CrudOperationException(error, "Validate", alumno);

            // ALTO-01 fix: no validar email si esta vacio en edicion
            if (!string.IsNullOrWhiteSpace(alumno.Email) && !AlumnoValidator.ValidarEmail(alumno.Email, out error))
                throw new CrudOperationException(error, "Validate", alumno);

            if (!string.IsNullOrWhiteSpace(alumno.Telefono) && !AlumnoValidator.ValidarTelefono(alumno.Telefono, out error))
                throw new CrudOperationException(error, "Validate", alumno);

            if (alumno.Fecha_Nacimiento.HasValue && !AlumnoValidator.ValidarEdad(alumno.Fecha_Nacimiento.Value, AlumnoConfig.EdadMinAlumno, AlumnoConfig.EdadMaxAlumno, out error))
                throw new CrudOperationException(error, "Validate", alumno);
        }

        private void ValidarDuplicados(SqlConnection conn, Alumno alumno, int? excluirId)
        {
            if (ExisteNoControl(conn, alumno.No_Control, excluirId))
                throw new CrudOperationException(MensajesAlumno.NoControlExiste, "Validate", alumno);

            if (ExisteNoControlEliminado(conn, alumno.No_Control))
                throw new CrudOperationException(MensajesAlumno.NoControlReservado, "Validate", alumno);

            if (!string.IsNullOrWhiteSpace(alumno.Curp))
            {
                if (ExisteCurp(conn, alumno.Curp, excluirId))
                    throw new CrudOperationException(MensajesAlumno.CurpExiste, "Validate", alumno);

                if (ExisteCurpEliminado(conn, alumno.Curp))
                    throw new CrudOperationException(MensajesAlumno.CurpReservado, "Validate", alumno);
            }

            if (!string.IsNullOrWhiteSpace(alumno.Email))
            {
                if (ExisteEmail(conn, alumno.Email, excluirId))
                    throw new CrudOperationException(MensajesAlumno.EmailExiste, "Validate", alumno);

                if (ExisteEmailEliminado(conn, alumno.Email))
                    throw new CrudOperationException(MensajesAlumno.EmailReservado, "Validate", alumno);
            }
        }

        private bool ExisteNoControl(SqlConnection conn, string noControl, int? excluirId)
        {
            string query = "SELECT COUNT(*) FROM Alumno WHERE no_control = @no_control AND is_deleted = 0";
            if (excluirId.HasValue)
                query += " AND id_alumno != @id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@no_control", noControl);
                if (excluirId.HasValue)
                    cmd.Parameters.AddWithValue("@id", excluirId.Value);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool ExisteCurp(SqlConnection conn, string curp, int? excluirId)
        {
            string query = "SELECT COUNT(*) FROM Alumno WHERE curp = @curp AND is_deleted = 0";
            if (excluirId.HasValue)
                query += " AND id_alumno != @id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@curp", curp);
                if (excluirId.HasValue)
                    cmd.Parameters.AddWithValue("@id", excluirId.Value);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool ExisteEmail(SqlConnection conn, string email, int? excluirId)
        {
            string query = "SELECT COUNT(*) FROM Alumno WHERE email = @email AND is_deleted = 0";
            if (excluirId.HasValue)
                query += " AND id_alumno != @id";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                if (excluirId.HasValue)
                    cmd.Parameters.AddWithValue("@id", excluirId.Value);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool ExisteEmailEliminado(SqlConnection conn, string email)
        {
            string query = "SELECT COUNT(*) FROM Alumno WHERE email = @email AND is_deleted = 1";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@email", email);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool ExisteNoControlEliminado(SqlConnection conn, string noControl)
        {
            string query = "SELECT COUNT(*) FROM Alumno WHERE no_control = @no_control AND is_deleted = 1";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@no_control", noControl);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool CarreraExiste(int idCarrera)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    return CarreraExiste(conn, idCarrera);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error checking carrera existence", ex);
                return false;
            }
        }

        private bool CarreraExiste(SqlConnection conn, int idCarrera)
        {
            string query = "SELECT COUNT(*) FROM Carrera WHERE id_carrera = @id_carrera";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_carrera", idCarrera);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool ExisteCurpEliminado(SqlConnection conn, string curp)
        {
            string query = "SELECT COUNT(*) FROM Alumno WHERE curp = @curp AND is_deleted = 1";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@curp", curp);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void ValidarTransicionStatus(SqlConnection conn, Alumno alumno, Alumno actual)
        {
            string statusNuevo = string.IsNullOrEmpty(alumno.Status_Alumno) ? actual.Status_Alumno : alumno.Status_Alumno;
            string statusActual = actual.Status_Alumno;

            if (string.Equals(statusActual, statusNuevo, StringComparison.OrdinalIgnoreCase))
                return;

            string error;
            if (!AlumnoValidator.ValidarTransicionStatus(statusActual, statusNuevo, out error))
                throw new CrudOperationException(error, "Validate", alumno);

            bool requiereMotivo =
                EsStatusFinalODesactivante(statusNuevo) ||
                EsReactivacion(statusActual, statusNuevo);

            if (requiereMotivo && string.IsNullOrWhiteSpace(alumno.Status_Change_Reason))
                throw new CrudOperationException("El motivo de cambio de estado es requerido.", "Validate", alumno);

            if (string.Equals(statusNuevo, Estatus.AlumnoBaja, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(alumno.Motivo_Baja))
                    throw new CrudOperationException(MensajesAlumno.MotivoBajaRequerido, "Validate", alumno);

                if (!alumno.Fecha_Baja.HasValue)
                    alumno.Fecha_Baja = DateTime.Today;
            }
        }

        private bool EsStatusFinalODesactivante(string status)
        {
            return string.Equals(status, Estatus.AlumnoBaja, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, Estatus.AlumnoSuspendido, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, Estatus.AlumnoEgresado, StringComparison.OrdinalIgnoreCase);
        }

        private bool EsReactivacion(string actual, string nuevo)
        {
            return string.Equals(nuevo, Estatus.AlumnoActivo, StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(actual, Estatus.AlumnoActivo, StringComparison.OrdinalIgnoreCase);
        }

        private bool TieneAsignacionesActivas(SqlConnection conn, int idAlumno)
        {
            string query = @"SELECT COUNT(*) FROM Alumno_Empresa
                              WHERE id_alumno = @id_alumno AND status_asignacion = @statusActivo";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_alumno", idAlumno);
                cmd.Parameters.AddWithValue("@statusActivo", Estatus.AsignacionActiva);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void SincronizarEmailUsuario(SqlConnection conn, int idAlumno, string nuevoEmail)
        {
            try
            {
                int? idUsuario = ObtenerUsuarioIdPorAlumno(conn, idAlumno);
                if (!idUsuario.HasValue || string.IsNullOrWhiteSpace(nuevoEmail))
                    return;

                string query = "UPDATE Usuario SET email = @email WHERE id_usuario = @id_usuario";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", nuevoEmail.Trim());
                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Email sync must not block the main CRUD operation.
                Logger.Error($"Error syncing email for alumno {idAlumno}", ex);
            }
        }

        private int? ObtenerUsuarioIdPorAlumno(SqlConnection conn, int idAlumno)
        {
            string query = @"SELECT id_usuario FROM Usuario_Alumno
                              WHERE id_alumno = @id_alumno
                              ";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_alumno", idAlumno);
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? (int?)null : Convert.ToInt32(result);
            }
        }

        private void InsertarBitacora(SqlConnection conn, string operacion, Alumno alumno)
        {
            try
            {
                string query = @"INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, datos_nuevos, fecha)
                                 VALUES ('Alumno', @id, @op, @usr, @datos, GETDATE())";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", alumno.Id_Alumno > 0 ? alumno.Id_Alumno : 0);
                    cmd.Parameters.AddWithValue("@op", operacion);
                    cmd.Parameters.AddWithValue("@usr", SesionActiva.Instance?.Username ?? "sistema");
                    string datos = $"{alumno.No_Control} - {alumno.Nombre} {alumno.Apellido_Paterno}";
                    cmd.Parameters.AddWithValue("@datos", datos);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Logging failures must not block CRUD operations.
            }
        }

        private void InsertarBitacora(SqlConnection conn, string operacion, int idAlumno, string datos)
        {
            try
            {
                string query = @"INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, datos_nuevos, fecha)
                                 VALUES ('Alumno', @id, @op, @usr, @datos, GETDATE())";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idAlumno);
                    cmd.Parameters.AddWithValue("@op", operacion);
                    cmd.Parameters.AddWithValue("@usr", SesionActiva.Instance?.Username ?? "sistema");
                    cmd.Parameters.AddWithValue("@datos", datos);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Logging failures must not block CRUD operations.
            }
        }
    }

    /// <summary>
    /// Auxiliary class for the Alumno-Empresa JOIN result.
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
