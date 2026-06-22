using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Data.EntityModel;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;
using Laboratorio_del_Tema_5_2.Controllers.Services;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class AuthController
    {
            private readonly AuthService _authService = new AuthService();

        public ResultadoLogin ValidarCredenciales(string login, string password)
        {
            return _authService.ValidarCredenciales(login, password);
        }

        public bool CrearUsuario(string username, string email, string password, int idRol, string tipoEntidad, int idEntidad)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            {
                Logger.Warning("CrearUsuario: parametros invalidos");
                return false;
            }

            if (password.Length < Seguridad.PasswordMinLength)
            {
                Logger.Warning("CrearUsuario: password muy corto");
                return false;
            }

            username = username.Trim();
            email = email.Trim().ToLower();

            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password, Seguridad.BcryptCostFactor);

                using (var db = new ModeloDualContext())
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        var nuevoUsuario = new UsuarioEF
                        {
                            Username = username,
                            Email = email,
                            PasswordHash = passwordHash,
                            Id_Rol = idRol,
                            Status = "activo",
                            Created_At = DateTime.Now,
                            Updated_At = DateTime.Now,
                            Is_Deleted = false,
                            Debe_Cambiar_Password = 1
                        };
                        db.Usuarios.Add(nuevoUsuario);
                        db.SaveChanges();

                        int idUsuario = nuevoUsuario.Id_Usuario;

                        if (!string.IsNullOrEmpty(tipoEntidad) && idEntidad > 0)
                        {
                            switch (tipoEntidad.ToLower())
                            {
                                case "alumno":
                                    db.UsuariosAlumnos.Add(new UsuarioAlumnoEF { Id_Usuario = idUsuario, Id_Alumno = idEntidad });
                                    break;
                                case "profesor":
                                    db.UsuariosProfesores.Add(new UsuarioProfesorEF { Id_Usuario = idUsuario, Id_Profesor = idEntidad });
                                    break;
                                case "empresa":
                                    db.UsuariosEmpresas.Add(new UsuarioEmpresaEF { Id_Usuario = idUsuario, Id_Empresa = idEntidad });
                                    break;
                            }
                            db.SaveChanges();
                        }

                        tx.Commit();
                        Logger.Info($"Usuario creado exitosamente: '{username}' (rol_id: {idRol}, tipo: {tipoEntidad ?? "N/A"})");
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al crear usuario", ex);
                return false;
            }
        }

        public bool CambiarPassword(int idUsuario, string passwordActual, string passwordNuevo)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT password_hash, username, email FROM Usuario WHERE id_usuario = @id_usuario";
                    string hashActual;
                    string username;
                    string email;
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read()) return false;
                            hashActual = reader.GetString(reader.GetOrdinal("password_hash"));
                            username = reader.GetString(reader.GetOrdinal("username"));
                            email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email"));
                        }
                    }

                    if (hashActual == null || !BCrypt.Net.BCrypt.Verify(passwordActual, hashActual))
                    {
                        return false;
                    }

                    // No permitir que el nuevo sea igual al actual
                    if (BCrypt.Net.BCrypt.Verify(passwordNuevo, hashActual))
                    {
                        Logger.Warning($"Intento de cambiar password por el mismo: usuario {idUsuario}");
                        return false;
                    }

                    // Validar contra diccionario de comunes
                    var validacionComun = PasswordValidator.Validar(passwordNuevo, username, email);
                    if (!validacionComun.EsValido)
                    {
                        Logger.Warning($"Cambio de password débil rechazado: usuario {idUsuario}. Razón: {validacionComun.Razon}");
                        throw new InvalidOperationException(validacionComun.Razon);
                    }

                    // Validar contra historial de passwords reutilizados
                    if (EsPasswordReutilizado(conn, idUsuario, passwordNuevo))
                    {
                        Logger.Warning($"Cambio de password rechazado: reutilización (usuario {idUsuario})");
                        throw new InvalidOperationException("No podés reutilizar una contraseña reciente. Elegí una diferente.");
                    }

                    string hashNuevo = BCrypt.Net.BCrypt.HashPassword(passwordNuevo, Seguridad.BcryptCostFactor);

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string updateQuery = "UPDATE Usuario SET password_hash = @hash WHERE id_usuario = @id_usuario";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@hash", hashNuevo);
                                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                                cmd.ExecuteNonQuery();
                            }

                            // Guardar hash anterior en historial
                            RegistrarEnHistorial(conn, transaction, idUsuario, hashActual);

                            // Auditar cambio de password
                            try
                            {
                                string queryBitacora = @"INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, ip_address, navegador, fecha)
                                                         VALUES (@tabla, @id_registro, @operacion, @usuario, @ip, @navegador, GETDATE())";
                                using (SqlCommand cmd = new SqlCommand(queryBitacora, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@tabla", "Usuario");
                                    cmd.Parameters.AddWithValue("@id_registro", idUsuario.ToString());
                                    cmd.Parameters.AddWithValue("@operacion", "CAMBIO_PASSWORD");
                                    cmd.Parameters.AddWithValue("@usuario", username);
                                    cmd.Parameters.AddWithValue("@ip", "127.0.0.1");
                                    cmd.Parameters.AddWithValue("@navegador", "WinForms App");
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            catch (Exception ex) { Logger.Error("Error al auditar CAMBIO_PASSWORD", ex); }

                            transaction.Commit();
                        }
                        catch
                        {
                            try { transaction.Rollback(); } catch { }
                            throw;
                        }
                    }
                    return true;
                }
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception ex)
            {
                Logger.Error("Error al cambiar password", ex);
                return false;
            }
        }

        public bool ExisteUsername(string username)
        {
            return ExisteEnBD("username", username);
        }

        public bool ExisteEmail(string email)
        {
            return ExisteEnBD("email", email);
        }

        private bool ExisteEnBD(string campo, string valor)
        {
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM Usuario WHERE {campo} = @valor";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@valor", valor);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al verificar {campo}", ex);
                return false;
            }
        }

        private Usuario ObtenerUsuarioPorId(SqlConnection conn, int idUsuario)
        {
            string query = @"SELECT id_usuario, username, email, id_rol, status, ultimo_login, created_at,
                                    debe_cambiar_password, fecha_activacion
                             FROM Usuario WHERE id_usuario = @id_usuario";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            Id_Usuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Id_Rol = reader.GetInt32(reader.GetOrdinal("id_rol")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            Ultimo_Login = reader.IsDBNull(reader.GetOrdinal("ultimo_login")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("ultimo_login")),
                            Created_At = reader.GetDateTime(reader.GetOrdinal("created_at")),
                            Debe_Cambiar_Password = reader.GetInt16(reader.GetOrdinal("debe_cambiar_password")) != 0,
                            Fecha_Activacion = reader.IsDBNull(reader.GetOrdinal("fecha_activacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_activacion"))
                        };
                    }
                }
            }
            return null;
        }

        private List<string> ObtenerPrivilegios(SqlConnection conn, int idRol)
        {
            List<string> privilegios = new List<string>();
            string query = @"SELECT p.nombre 
                             FROM Privilegio p
                             INNER JOIN Rol_Privilegio rp ON p.id_privilegio = rp.id_privilegio
                             WHERE rp.id_rol = @id_rol";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_rol", idRol);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        privilegios.Add(reader.GetString(reader.GetOrdinal("nombre")));
                    }
                }
            }
            return privilegios;
        }

        private (string tipoEntidad, int? idEntidad) ObtenerEntidadVinculada(SqlConnection conn, int idUsuario)
        {
            // Usuario_Alumno (nueva tabla con FK real)
            string queryAlumno = "SELECT TOP 1 id_alumno FROM Usuario_Alumno WHERE id_usuario = @id";
            using (var cmd = new SqlCommand(queryAlumno, conn))
            {
                cmd.Parameters.AddWithValue("@id", idUsuario);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return ("alumno", Convert.ToInt32(result));
            }

            // Usuario_Profesor
            string queryProf = "SELECT TOP 1 id_profesor FROM Usuario_Profesor WHERE id_usuario = @id";
            using (var cmd = new SqlCommand(queryProf, conn))
            {
                cmd.Parameters.AddWithValue("@id", idUsuario);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return ("profesor", Convert.ToInt32(result));
            }

            // Usuario_Empresa
            string queryEmp = "SELECT TOP 1 id_empresa FROM Usuario_Empresa WHERE id_usuario = @id";
            using (var cmd = new SqlCommand(queryEmp, conn))
            {
                cmd.Parameters.AddWithValue("@id", idUsuario);
                var result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return ("empresa", Convert.ToInt32(result));
            }

            return (null, null);
        }

        private List<string> ObtenerPrivilegiosEF(ModeloDualContext db, int idRol)
        {
            return (from rp in db.RolesPrivilegios
                    join p in db.Privilegios on rp.Id_Privilegio equals p.Id_Privilegio
                    where rp.Id_Rol == idRol
                    select p.Nombre).ToList();
        }

        private (string tipoEntidad, int? idEntidad) ObtenerEntidadVinculadaEF(ModeloDualContext db, int idUsuario)
        {
            var alumno = db.UsuariosAlumnos
                .FirstOrDefault(ua => ua.Id_Usuario == idUsuario);
            if (alumno != null)
                return ("alumno", alumno.Id_Alumno);

            var prof = db.UsuariosProfesores
                .FirstOrDefault(up => up.Id_Usuario == idUsuario);
            if (prof != null)
                return ("profesor", prof.Id_Profesor);

            var emp = db.UsuariosEmpresas
                .FirstOrDefault(ue => ue.Id_Usuario == idUsuario);
            if (emp != null)
                return ("empresa", emp.Id_Empresa);

            return (null, null);
        }

        /// <summary>
        /// Incrementa el contador de intentos fallidos.
        /// Cuando se supera el máximo, establece bloqueo_hasta.
        /// </summary>
        private void IncrementarIntentosFallidos(SqlConnection conn, int idUsuario, int intentosActuales)
        {
            IncrementarIntentosFallidos(conn, null, idUsuario, intentosActuales);
        }

        private void IncrementarIntentosFallidos(SqlConnection conn, SqlTransaction transaction, int idUsuario, int intentosActuales)
        {
            int nuevosIntentos = intentosActuales + 1;
            DateTime? bloqueo = null;

            int maxIntentos = ParametroSistemaService.Instance.GetInt(
                Claves.MAX_INTENTOS_LOGIN, Seguridad.MaxIntentosLogin);
            int minutosBloqueo = ParametroSistemaService.Instance.GetInt(
                Claves.TIEMPO_BLOQUEO_MINUTOS, Seguridad.MinutosBloqueo);

            if (nuevosIntentos >= maxIntentos)
            {
                bloqueo = DateTime.Now.AddMinutes(minutosBloqueo);
            }

            string query = @"UPDATE Usuario SET intentos_fallidos = @intentos, bloqueado_hasta = @bloqueo
                             WHERE id_usuario = @id_usuario";

            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@intentos", nuevosIntentos);
                cmd.Parameters.AddWithValue("@bloqueo", bloqueo.HasValue ? (object)bloqueo.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizarUltimoLogin(SqlConnection conn, int idUsuario)
        {
            string query = "UPDATE Usuario SET ultimo_login = GETDATE(), intentos_fallidos = 0, bloqueado_hasta = NULL WHERE id_usuario = @id_usuario";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        private void CrearSesion(SqlConnection conn, int idUsuario)
        {
            string tokenSesion = Guid.NewGuid().ToString("N"); // 32 hex chars
            DateTime expiracion = DateTime.Now.AddHours(Seguridad.DuracionSesionHoras);

            string query = @"INSERT INTO Sesion (id_sesion, id_usuario, fecha_inicio, fecha_expiracion, ip_address, user_agent, status)
                             VALUES (@token, @id_usuario, GETDATE(), @expiracion, @ip, @user_agent, 'activa')";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@token", tokenSesion);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@expiracion", expiracion);
                cmd.Parameters.AddWithValue("@ip", "127.0.0.1");
                cmd.Parameters.AddWithValue("@user_agent", "WinForms App");
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Valida que el nuevo password no esté en el historial reciente del usuario.
        /// Por defecto verifica las últimas 5 contraseñas.
        /// </summary>
        private bool EsPasswordReutilizado(SqlConnection conn, int idUsuario, string passwordNuevo, int ultimas = 5)
        {
            try
            {
                string query = @"SELECT password_hash FROM password_history
                                 WHERE id_usuario = @id
                                 ORDER BY fecha_cambio DESC
                                 OFFSET 0 ROWS FETCH NEXT @limite ROWS ONLY";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cmd.Parameters.AddWithValue("@limite", ultimas);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string hashAnterior = reader.GetString(reader.GetOrdinal("password_hash"));
                            if (BCrypt.Net.BCrypt.Verify(passwordNuevo, hashAnterior))
                                return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al validar historial de passwords", ex);
            }
            return false;
        }

        /// <summary>
        /// Registra el cambio de password en el historial.
        /// </summary>
        private void RegistrarEnHistorial(SqlConnection conn, SqlTransaction transaction, int idUsuario, string passwordHashAnterior)
        {
            if (string.IsNullOrEmpty(passwordHashAnterior)) return;
            try
            {
                string query = @"INSERT INTO password_history (id_usuario, password_hash) VALUES (@id, @hash)";
                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", idUsuario);
                    cmd.Parameters.AddWithValue("@hash", passwordHashAnterior);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al registrar password en historial", ex);
            }
        }

        /// <summary>
        /// Registra una operacion en la tabla bitacora (auditoria enterprise).
        /// </summary>
        private void InsertarBitacora(SqlConnection conn, string tabla, string idRegistro, string operacion, string usuario)
        {
            InsertarBitacora(conn, null, tabla, idRegistro, operacion, usuario);
        }

        private void InsertarBitacora(SqlConnection conn, SqlTransaction transaction, string tabla, string idRegistro, string operacion, string usuario)
        {
            try
            {
                string query = @"INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, ip_address, navegador, fecha)
                                 VALUES (@tabla, @id_registro, @operacion, @usuario, @ip, @navegador, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@tabla", tabla);
                    cmd.Parameters.AddWithValue("@id_registro", idRegistro);
                    cmd.Parameters.AddWithValue("@operacion", operacion);
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@ip", "127.0.0.1");
                    cmd.Parameters.AddWithValue("@navegador", "WinForms App");
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // La bitacora no debe bloquear el login si falla
                Logger.Error("Error al insertar en bitacora", ex);
            }
        }

        public void CerrarSesion()
        {
            _authService.CerrarSesion();
        }

        public List<Rol> ObtenerRoles()
        {
            return _authService.ObtenerRoles();
        }

        public List<Rol> ObtenerRolesPorTipo(string tipoEntidad)
        {
            List<Rol> todosLosRoles = ObtenerRoles();
            List<Rol> rolesFiltrados = new List<Rol>();

            // admin solo si no tiene entidad
            // Los demas roles solo si tienen entidad
            foreach (var rol in todosLosRoles)
            {
                if (tipoEntidad == "none")
                {
                    if (rol.Nombre == "admin")
                        rolesFiltrados.Add(rol);
                }
                else
                {
                    if (rol.Nombre == tipoEntidad)
                        rolesFiltrados.Add(rol);
                }
            }

            return rolesFiltrados;
        }

        /// <summary>
        /// Verifica si una entidad ya esta vinculada a otro usuario.
        /// Devuelve true si esta disponible (no vinculada), false si ya tiene usuario.
        /// </summary>
        public bool EntidadEstaDisponible(string tipoEntidad, int idEntidad)
        {
            try
            {
                if (string.IsNullOrEmpty(tipoEntidad) || idEntidad <= 0)
                    return false;

                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string tabla = tipoEntidad switch
                    {
                        "alumno" => "Usuario_Alumno",
                        "profesor" => "Usuario_Profesor",
                        "empresa" => "Usuario_Empresa",
                        _ => throw new ArgumentException($"Tipo de entidad no válido: {tipoEntidad}")
                    };
                    string columna = tipoEntidad switch
                    {
                        "alumno" => "id_alumno",
                        "profesor" => "id_profesor",
                        "empresa" => "id_empresa",
                        _ => "id_entidad"
                    };
                    string query = $"SELECT COUNT(*) FROM {tabla} WHERE {columna} = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idEntidad);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar disponibilidad de entidad", ex);
                return false;
            }
        }

        /// <summary>
        /// Verifica disponibilidad de username case-insensitive.
        /// </summary>
        public bool UsernameDisponible(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                    return false;

                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Usuario WHERE LOWER(username) = LOWER(@username)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username.Trim());
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar username", ex);
                return false;
            }
        }

        /// <summary>
        /// Verifica disponibilidad de email case-insensitive.
        /// </summary>
        public bool EmailDisponible(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Usuario WHERE LOWER(email) = LOWER(@email)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email.Trim());
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count == 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al verificar email", ex);
                return false;
            }
        }


        /// <summary>
        /// Genera un password temporal aleatorio de longitud dada.
        /// </summary>
        public string GenerarPasswordTemporal(int longitud = 10)
        {
            return PasswordService.GenerarPasswordTemporal(longitud);
        }

        /// <summary>
        /// Carga un nuevo usuario al sistema con password temporal.
        /// Lo hace el admin/RH. El usuario debera activarlo despues.
        /// Devuelve el password temporal generado (para entregar al usuario).
        /// </summary>
        public ResultadoCarga CargarUsuario(string username, string email, int idRol,
            string tipoEntidad, int? idEntidad, int creadoPor)
        {
            // Validaciones basicas
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
            {
                return new ResultadoCarga { Success = false, Message = "Username y email son requeridos" };
            }

            if (creadoPor <= 0)
            {
                return new ResultadoCarga { Success = false, Message = "Se requiere el ID del admin que crea el usuario" };
            }

            username = username.Trim();
            email = email.Trim().ToLower();

            // Verificar duplicados
            if (!UsernameDisponible(username))
            {
                return new ResultadoCarga { Success = false, Message = $"El username '{username}' ya esta registrado" };
            }

            if (!EmailDisponible(email))
            {
                return new ResultadoCarga { Success = false, Message = $"El email '{email}' ya esta registrado" };
            }

            // Si se especifica entidad, validar que no este ya vinculada
            if (!string.IsNullOrEmpty(tipoEntidad) && idEntidad.HasValue && idEntidad.Value > 0)
            {
                if (!EntidadEstaDisponible(tipoEntidad, idEntidad.Value))
                {
                    return new ResultadoCarga { Success = false, Message = "La entidad seleccionada ya esta vinculada a otro usuario" };
                }
            }

            try
            {
                // Generar password temporal y su hash
                string passwordTemporal = GenerarPasswordTemporal();
                string passwordTemporalHash = BCrypt.Net.BCrypt.HashPassword(passwordTemporal, Seguridad.BcryptCostFactor);

                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Insertar usuario con password_temporal_hash y password_hash vacio
                            // El password_hash real se setea cuando activa
                            string queryUsuario = @"INSERT INTO Usuario
                                (username, email, password_hash, id_rol, status,
                                 debe_cambiar_password, password_temporal_hash, creado_por)
                                VALUES
                                (@username, @email, @temp_hash, @id_rol, 'activo',
                                 1, @temp_hash, @creado_por)";

                            int idUsuario;
                            using (SqlCommand cmd = new SqlCommand(queryUsuario, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@username", username);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@temp_hash", passwordTemporalHash);
                                cmd.Parameters.AddWithValue("@id_rol", idRol);
                                cmd.Parameters.AddWithValue("@creado_por", creadoPor);
                                cmd.ExecuteNonQuery();

                                using (SqlCommand cmdId = new SqlCommand("SELECT SCOPE_IDENTITY()", conn, transaction))
                                {
                                    idUsuario = Convert.ToInt32(cmdId.ExecuteScalar());
                                }
                            }

                            // Vincular con entidad si corresponde
                            if (!string.IsNullOrEmpty(tipoEntidad) && idEntidad.HasValue && idEntidad.Value > 0)
                            {
                                string tablaVinculo2 = ObtenerTablaVinculo(tipoEntidad);
                                string columnaVinculo2 = ObtenerColumnaVinculo(tipoEntidad);
                                string queryVinculo = $"INSERT INTO {tablaVinculo2} (id_usuario, {columnaVinculo2}) VALUES (@id_usuario, @id_entidad)";
                                using (SqlCommand cmd = new SqlCommand(queryVinculo, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                                    cmd.Parameters.AddWithValue("@id_entidad", idEntidad.Value);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            Logger.Info($"Usuario cargado por admin {creadoPor}: '{username}' (password temporal generado)");

                            return new ResultadoCarga
                            {
                                Success = true,
                                Message = "Usuario creado. Entrega el password temporal al usuario.",
                                PasswordTemporal = passwordTemporal,
                                Id_Usuario = idUsuario,
                                Username = username
                            };
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (SqlException sqlEx) when (sqlEx.Number == 2627)
            {
                Logger.Warning($"Intento de crear duplicado (CargarUsuario): '{username}' o '{email}'");
                return new ResultadoCarga { Success = false, Message = "Username o email duplicado" };
            }
            catch (Exception ex)
            {
                Logger.Error("Error al cargar usuario", ex);
                return new ResultadoCarga { Success = false, Message = "Error al crear el usuario" };
            }
        }

        /// <summary>
        /// Activa la cuenta de un usuario validando su password temporal
        /// y estableciendo su nueva contrasena definitiva.
        /// </summary>
        public ResultadoLogin ActivarCuenta(string username, string passwordTemporal, string passwordNuevo)
        {
            return _authService.ActivarCuenta(username, passwordTemporal, passwordNuevo);
        }

        public ResultadoCarga ResetearPassword(int idUsuario, int adminId)
        {
            if (idUsuario <= 0)
            {
                return new ResultadoCarga { Success = false, Message = "ID de usuario invalido" };
            }

            try
            {
                string passwordTemporal = GenerarPasswordTemporal();
                string tempHash = BCrypt.Net.BCrypt.HashPassword(passwordTemporal, Seguridad.BcryptCostFactor);

                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Usuario
                                    SET password_temporal_hash = @temp_hash,
                                        password_hash = @temp_hash,
                                        debe_cambiar_password = 1,
                                        fecha_activacion = NULL
                                    WHERE id_usuario = @id";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@temp_hash", tempHash);
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            return new ResultadoCarga { Success = false, Message = "Usuario no encontrado" };
                        }
                    }

                    Logger.Info($"Password reseteado por admin {adminId} para usuario {idUsuario}");

                    return new ResultadoCarga
                    {
                        Success = true,
                        Message = "Password reseteado. Entrega el nuevo password al usuario.",
                        PasswordTemporal = passwordTemporal,
                        Id_Usuario = idUsuario
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al resetear password", ex);
                return new ResultadoCarga { Success = false, Message = "Error al resetear password" };
            }
        }

        /// <summary>
        /// Lista todos los usuarios (solo admin).
        /// </summary>
        public List<Usuario> ListarUsuarios()
        {
            return _authService.ListarUsuarios();
        }

        /// <summary>
        /// Cambia el estado de un usuario (activar/desactivar).
        /// </summary>
        public bool CambiarStatusUsuario(int idUsuario, string nuevoStatus, int adminId)
        {
            if (nuevoStatus != "activo" && nuevoStatus != "inactivo" && nuevoStatus != "suspendido")
                return false;

            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Usuario SET status = @status WHERE id_usuario = @id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@status", nuevoStatus);
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.ExecuteNonQuery();
                    }
                    Logger.Info($"Admin {adminId} cambio status de usuario {idUsuario} a {nuevoStatus}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al cambiar status de usuario {idUsuario}", ex);
                return false;
            }
        }

        /// <summary>
        /// Prepara la sesión cuando el password del usuario expiró pero ya validó
        /// su identidad. Carga rol, privilegios y entidad vinculada para que el
        /// menú principal pueda aplicar la autorización correctamente.
        /// </summary>
        public bool IniciarSesionPorPasswordExpirado(Usuario usuario)
        {
            if (usuario == null) return false;
            try
            {
                using (SqlConnection conn = SqlServerConnection.GetConnection())
                {
                    conn.Open();
                    List<string> privilegios = ObtenerPrivilegios(conn, usuario.Id_Rol);
                    var (tipoEntidad, idEntidad) = ObtenerEntidadVinculada(conn, usuario.Id_Usuario);

                    string nombreRol = "";
                    using (SqlCommand cmd = new SqlCommand("SELECT nombre FROM Rol WHERE id_rol = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuario.Id_Rol);
                        nombreRol = cmd.ExecuteScalar()?.ToString() ?? "";
                    }

                    SesionActiva.Instance.IniciarSesionForzarCambioPassword(
                        usuario, nombreRol, tipoEntidad, idEntidad, privilegios);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al iniciar sesión por password expirado", ex);
                return false;
            }
        }

        /// <summary>
        /// Versión asíncrona de ValidarCredenciales para operaciones no bloqueantes.
        /// </summary>
        public async System.Threading.Tasks.Task<ResultadoLogin> ValidarCredencialesAsync(string login, string password)
        {
            return await System.Threading.Tasks.Task.Run(() => ValidarCredenciales(login, password));
        }

        private static string ObtenerTablaVinculo(string tipoEntidad)
        {
            return tipoEntidad switch
            {
                "alumno" => "Usuario_Alumno",
                "profesor" => "Usuario_Profesor",
                "empresa" => "Usuario_Empresa",
                _ => throw new ArgumentException($"Tipo de entidad no válido: {tipoEntidad}")
            };
        }

        private static string ObtenerColumnaVinculo(string tipoEntidad)
        {
            return tipoEntidad switch
            {
                "alumno" => "id_alumno",
                "profesor" => "id_profesor",
                "empresa" => "id_empresa",
                _ => throw new ArgumentException($"Tipo de entidad no válido: {tipoEntidad}")
            };
        }
    }

    /// <summary>
    /// Resultado de cargar un usuario nuevo (incluye password temporal).
    /// </summary>
    public class ResultadoCarga
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string PasswordTemporal { get; set; }
        public int Id_Usuario { get; set; }
        public string Username { get; set; }
    }
}
