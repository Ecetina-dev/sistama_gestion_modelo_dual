using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers
{
    public class AuthController
    {
        public ResultadoLogin ValidarCredenciales(string login, string password)
        {
            // Sanitizar inputs
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgCredencialesInvalidas
                };
            }

            login = login.Trim();

            // Limitar longitud para evitar ataques de tipo DoS
            if (login.Length > Seguridad.LoginMaxLength || password.Length > Seguridad.PasswordMaxLength)
            {
                Logger.Warning($"Intento de login con input excesivo. Login length: {login.Length}");
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgCredencialesInvalidas
                };
            }

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT u.id_usuario, u.username, u.email, u.password_hash, 
                                            u.id_rol, u.status, u.intentos_fallidos, u.bloqueado_hasta,
                                            r.nombre AS rol_nombre
                                     FROM Usuario u
                                     INNER JOIN Rol r ON u.id_rol = r.id_rol
                                     WHERE u.username = @login OR u.email = @login";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@login", login);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                // No revelar si el usuario existe o no
                                Logger.Warning($"Intento de login con usuario inexistente: '{login}'");
                                return new ResultadoLogin
                                {
                                    Success = false,
                                    Message = Seguridad.MsgCredencialesInvalidas
                                };
                            }

                            int idUsuario = reader.GetInt32("id_usuario");
                            string usernameBd = reader.GetString("username");
                            string passwordHash = reader.GetString("password_hash");
                            string status = reader.GetString("status");
                            int intentosFallidos = reader.GetInt32("intentos_fallidos");
                            string rolNombre = reader.GetString("rol_nombre");

                            if (!reader.IsDBNull(reader.GetOrdinal("bloqueado_hasta")))
                            {
                                DateTime bloqueadoHasta = reader.GetDateTime("bloqueado_hasta");
                                if (DateTime.Now < bloqueadoHasta)
                                {
                                    TimeSpan restante = bloqueadoHasta - DateTime.Now;
                                    Logger.Warning($"Cuenta bloqueada: '{usernameBd}'. Restante: {restante.Minutes} min");
                                    return new ResultadoLogin
                                    {
                                        Success = false,
                                        Message = Seguridad.MsgCuentaBloqueada
                                    };
                                }
                            }

                            if (status == "inactivo")
                            {
                                Logger.Warning($"Intento de login en cuenta inactiva: '{usernameBd}'");
                                return new ResultadoLogin
                                {
                                    Success = false,
                                    Message = Seguridad.MsgCuentaInactiva
                                };
                            }

                            if (status == "suspendido")
                            {
                                Logger.Warning($"Intento de login en cuenta suspendida: '{usernameBd}'");
                                return new ResultadoLogin
                                {
                                    Success = false,
                                    Message = Seguridad.MsgCuentaInactiva
                                };
                            }

                            if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
                            {
                                reader.Close();
                                ActualizarIntentosFallidos(conn, idUsuario, intentosFallidos);
                                int restantes = Seguridad.MaxIntentosLogin - intentosFallidos - 1;
                                Logger.Warning($"Contrasena incorrecta para '{usernameBd}'. Intentos restantes: {restantes}");
                                return new ResultadoLogin
                                {
                                    Success = false,
                                    // Mensaje generico - no revela info
                                    Message = restantes > 0
                                        ? $"{Seguridad.MsgCredencialesInvalidas} (Intentos restantes: {restantes})"
                                        : Seguridad.MsgCuentaBloqueada
                                };
                            }

                            reader.Close();

                            ActualizarIntentosFallidos(conn, idUsuario, 0);
                            ActualizarUltimoLogin(conn, idUsuario);

                            Usuario usuario = ObtenerUsuarioPorId(conn, idUsuario);
                            List<string> privilegios = ObtenerPrivilegios(conn, usuario.Id_Rol);
                            var (tipoEntidad, idEntidad) = ObtenerEntidadVinculada(conn, idUsuario);

                            SesionActiva.Instance.IniciarSesion(usuario, rolNombre, tipoEntidad, idEntidad, privilegios);

                            CrearSesion(conn, idUsuario);

                            Logger.Info($"Login exitoso para usuario '{usernameBd}' (rol: {rolNombre})");

                            return new ResultadoLogin
                            {
                                Success = true,
                                Message = "Login exitoso",
                                Usuario = usuario,
                                RequiereCambioPassword = usuario.Debe_Cambiar_Password
                            };
                        }
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                Logger.Error("Error de MySQL en ValidarCredenciales", mysqlEx);
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgErrorConexion
                };
            }
            catch (Exception ex)
            {
                Logger.Error("Error inesperado en ValidarCredenciales", ex);
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgErrorGenerico
                };
            }
        }

        public bool CrearUsuario(string username, string email, string password, int idRol, string tipoEntidad, int idEntidad)
        {
            // Validaciones basicas de entrada
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

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string queryUsuario = @"INSERT INTO Usuario (username, email, password_hash, id_rol, status)
                                                   VALUES (@username, @email, @password_hash, @id_rol, 'activo')";

                            int idUsuario;
                            using (MySqlCommand cmd = new MySqlCommand(queryUsuario, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@username", username);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@password_hash", passwordHash);
                                cmd.Parameters.AddWithValue("@id_rol", idRol);
                                cmd.ExecuteNonQuery();

                                using (MySqlCommand cmdId = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, transaction))
                                {
                                    idUsuario = Convert.ToInt32(cmdId.ExecuteScalar());
                                }
                            }

                            if (!string.IsNullOrEmpty(tipoEntidad) && idEntidad > 0)
                            {
                                string queryVinculo = @"INSERT INTO Usuario_Entidad (id_usuario, tipo_entidad, id_entidad)
                                                        VALUES (@id_usuario, @tipo_entidad, @id_entidad)";
                                using (MySqlCommand cmd = new MySqlCommand(queryVinculo, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                                    cmd.Parameters.AddWithValue("@tipo_entidad", tipoEntidad);
                                    cmd.Parameters.AddWithValue("@id_entidad", idEntidad);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            Logger.Info($"Usuario creado exitosamente: '{username}' (rol_id: {idRol}, tipo: {tipoEntidad ?? "N/A"})");
                            return true;
                        }
                        catch (MySqlException mysqlEx) when (mysqlEx.Number == 1062) // Duplicate entry
                        {
                            transaction.Rollback();
                            Logger.Warning($"Intento de crear usuario duplicado: '{username}' o '{email}'");
                            return false;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                Logger.Error("Error de MySQL al crear usuario", mysqlEx);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("Error inesperado al crear usuario", ex);
                return false;
            }
        }

        public bool CambiarPassword(int idUsuario, string passwordActual, string passwordNuevo)
        {
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT password_hash FROM Usuario WHERE id_usuario = @id_usuario";
                    string hashActual;
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        hashActual = cmd.ExecuteScalar()?.ToString();
                    }

                    if (hashActual == null || !BCrypt.Net.BCrypt.Verify(passwordActual, hashActual))
                    {
                        return false;
                    }

                    string hashNuevo = BCrypt.Net.BCrypt.HashPassword(passwordNuevo, Seguridad.BcryptCostFactor);
                    string updateQuery = "UPDATE Usuario SET password_hash = @hash WHERE id_usuario = @id_usuario";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@hash", hashNuevo);
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
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
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = $"SELECT COUNT(*) FROM Usuario WHERE {campo} = @valor";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

        private Usuario ObtenerUsuarioPorId(MySqlConnection conn, int idUsuario)
        {
            string query = @"SELECT id_usuario, username, email, id_rol, status, ultimo_login, created_at,
                                    debe_cambiar_password, fecha_activacion
                             FROM Usuario WHERE id_usuario = @id_usuario";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Usuario
                        {
                            Id_Usuario = reader.GetInt32("id_usuario"),
                            Username = reader.GetString("username"),
                            Email = reader.GetString("email"),
                            Id_Rol = reader.GetInt32("id_rol"),
                            Status = reader.GetString("status"),
                            Ultimo_Login = reader.IsDBNull(reader.GetOrdinal("ultimo_login")) ? (DateTime?)null : reader.GetDateTime("ultimo_login"),
                            Created_At = reader.GetDateTime("created_at"),
                            Debe_Cambiar_Password = reader.GetBoolean("debe_cambiar_password"),
                            Fecha_Activacion = reader.IsDBNull(reader.GetOrdinal("fecha_activacion")) ? (DateTime?)null : reader.GetDateTime("fecha_activacion")
                        };
                    }
                }
            }
            return null;
        }

        private List<string> ObtenerPrivilegios(MySqlConnection conn, int idRol)
        {
            List<string> privilegios = new List<string>();
            string query = @"SELECT p.nombre 
                             FROM Privilegio p
                             INNER JOIN Rol_Privilegio rp ON p.id_privilegio = rp.id_privilegio
                             WHERE rp.id_rol = @id_rol";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_rol", idRol);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        privilegios.Add(reader.GetString("nombre"));
                    }
                }
            }
            return privilegios;
        }

        private (string tipoEntidad, int? idEntidad) ObtenerEntidadVinculada(MySqlConnection conn, int idUsuario)
        {
            string query = @"SELECT tipo_entidad, id_entidad FROM Usuario_Entidad WHERE id_usuario = @id_usuario LIMIT 1";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string tipo = reader.GetString("tipo_entidad");
                        int id = reader.GetInt32("id_entidad");
                        return (tipo, id);
                    }
                }
            }
            return (null, null);
        }

        private void ActualizarIntentosFallidos(MySqlConnection conn, int idUsuario, int intentos)
        {
            int nuevosIntentos = intentos + 1;
            DateTime? bloqueo = null;

            if (nuevosIntentos >= Seguridad.MaxIntentosLogin)
            {
                bloqueo = DateTime.Now.AddMinutes(Seguridad.MinutosBloqueo);
            }

            string query = @"UPDATE Usuario SET intentos_fallidos = @intentos, bloqueado_hasta = @bloqueo 
                             WHERE id_usuario = @id_usuario";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@intentos", nuevosIntentos);
                cmd.Parameters.AddWithValue("@bloqueo", bloqueo.HasValue ? (object)bloqueo.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        private void ActualizarUltimoLogin(MySqlConnection conn, int idUsuario)
        {
            string query = "UPDATE Usuario SET ultimo_login = NOW(), intentos_fallidos = 0, bloqueado_hasta = NULL WHERE id_usuario = @id_usuario";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.ExecuteNonQuery();
            }
        }

        private void CrearSesion(MySqlConnection conn, int idUsuario)
        {
            string tokenSesion = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString("X");
            DateTime expiracion = DateTime.Now.AddHours(8);

            string query = @"INSERT INTO Sesion (id_sesion, id_usuario, fecha_inicio, fecha_expiracion, status)
                             VALUES (@token, @id_usuario, NOW(), @expiracion, 'activa')";

            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@token", tokenSesion);
                cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                cmd.Parameters.AddWithValue("@expiracion", expiracion);
                cmd.ExecuteNonQuery();
            }
        }

        public void CerrarSesion()
        {
            try
            {
                int idUsuario = SesionActiva.Instance.Id_Usuario;

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Sesion SET status = 'cerrada' WHERE id_usuario = @id_usuario AND status = 'activa'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al cerrar sesion", ex);
            }
            finally
            {
                SesionActiva.Instance.CerrarSesion();
            }
        }

        public List<Rol> ObtenerRoles()
        {
            List<Rol> roles = new List<Rol>();
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT id_rol, nombre, descripcion FROM Rol ORDER BY nombre";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            roles.Add(new Rol
                            {
                                Id_Rol = reader.GetInt32("id_rol"),
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("descripcion")) ? null : reader.GetString("descripcion")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al obtener roles", ex);
            }
            return roles;
        }

        /// <summary>
        /// Obtiene los roles disponibles segun el tipo de entidad.
        /// admin no se puede vincular, solo puede ser "Sin vincular".
        /// </summary>
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

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Usuario_Entidad WHERE tipo_entidad = @tipo AND id_entidad = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@tipo", tipoEntidad);
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

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Usuario WHERE LOWER(username) = LOWER(@username)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Usuario WHERE LOWER(email) = LOWER(@email)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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

        // ============================================================
        // FLUJO PROFESIONAL: Gestion de usuarios por admin
        // ============================================================

        /// <summary>
        /// Genera un password temporal aleatorio de longitud dada.
        /// </summary>
        public string GenerarPasswordTemporal(int longitud = 10)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789!@#$%";
            var random = new Random();
            var pwd = new char[longitud];
            for (int i = 0; i < longitud; i++)
                pwd[i] = chars[random.Next(chars.Length)];
            return new string(pwd);
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

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    using (MySqlTransaction transaction = conn.BeginTransaction())
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
                                 TRUE, @temp_hash, @creado_por)";

                            int idUsuario;
                            using (MySqlCommand cmd = new MySqlCommand(queryUsuario, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@username", username);
                                cmd.Parameters.AddWithValue("@email", email);
                                cmd.Parameters.AddWithValue("@temp_hash", passwordTemporalHash);
                                cmd.Parameters.AddWithValue("@id_rol", idRol);
                                cmd.Parameters.AddWithValue("@creado_por", creadoPor);
                                cmd.ExecuteNonQuery();

                                using (MySqlCommand cmdId = new MySqlCommand("SELECT LAST_INSERT_ID()", conn, transaction))
                                {
                                    idUsuario = Convert.ToInt32(cmdId.ExecuteScalar());
                                }
                            }

                            // Vincular con entidad si corresponde
                            if (!string.IsNullOrEmpty(tipoEntidad) && idEntidad.HasValue && idEntidad.Value > 0)
                            {
                                string queryVinculo = @"INSERT INTO Usuario_Entidad (id_usuario, tipo_entidad, id_entidad)
                                                        VALUES (@id_usuario, @tipo_entidad, @id_entidad)";
                                using (MySqlCommand cmd = new MySqlCommand(queryVinculo, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@id_usuario", idUsuario);
                                    cmd.Parameters.AddWithValue("@tipo_entidad", tipoEntidad);
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
            catch (MySqlException mysqlEx) when (mysqlEx.Number == 1062)
            {
                Logger.Warning($"Intento de crear duplicado: '{username}' o '{email}'");
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
            // Validaciones
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(passwordTemporal) || string.IsNullOrEmpty(passwordNuevo))
            {
                return new ResultadoLogin
                {
                    Success = false,
                    Message = "Todos los campos son requeridos"
                };
            }

            if (passwordNuevo.Length < Seguridad.PasswordMinLength)
            {
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgPasswordLongitud
                };
            }

            username = username.Trim();

            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();

                    // Buscar usuario
                    string query = @"SELECT id_usuario, username, email, password_temporal_hash, status
                                     FROM Usuario
                                     WHERE LOWER(username) = LOWER(@username)";

                    int idUsuario;
                    string email;
                    string passwordTempHash;
                    string status;
                    bool yaActivado = false;

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                return new ResultadoLogin
                                {
                                    Success = false,
                                    Message = "Credenciales invalidas"
                                };
                            }

                            idUsuario = reader.GetInt32("id_usuario");
                            email = reader.GetString("email");
                            status = reader.GetString("status");

                            if (reader.IsDBNull(reader.GetOrdinal("password_temporal_hash")))
                            {
                                yaActivado = true;
                                passwordTempHash = null;
                            }
                            else
                            {
                                passwordTempHash = reader.GetString("password_temporal_hash");
                            }
                        }
                    }

                    if (status != "activo")
                    {
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = "Tu cuenta no esta activa. Contacta al administrador."
                        };
                    }

                    if (yaActivado)
                    {
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = "Esta cuenta ya fue activada. Usa el login normal."
                        };
                    }

                    // Verificar password temporal
                    if (string.IsNullOrEmpty(passwordTempHash) ||
                        !BCrypt.Net.BCrypt.Verify(passwordTemporal, passwordTempHash))
                    {
                        Logger.Warning($"Intento de activacion con password temporal invalido: '{username}'");
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = "Credenciales invalidas"
                        };
                    }

                    // Activar cuenta: hashear nuevo password y limpiar password temporal
                    string nuevoHash = BCrypt.Net.BCrypt.HashPassword(passwordNuevo, Seguridad.BcryptCostFactor);

                    string updateQuery = @"UPDATE Usuario
                                          SET password_hash = @nuevo_hash,
                                              password_temporal_hash = NULL,
                                              debe_cambiar_password = FALSE,
                                              fecha_activacion = NOW()
                                          WHERE id_usuario = @id";

                    using (MySqlCommand cmd = new MySqlCommand(updateQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@nuevo_hash", nuevoHash);
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.ExecuteNonQuery();
                    }

                    // Cargar usuario completo y sus privilegios para iniciar sesion
                    Usuario usuario = ObtenerUsuarioPorId(conn, idUsuario);
                    usuario.Debe_Cambiar_Password = false;

                    List<string> privilegios = ObtenerPrivilegios(conn, usuario.Id_Rol);
                    var (tipoEntidad, idEntidad) = ObtenerEntidadVinculada(conn, idUsuario);

                    // Obtener nombre del rol
                    string nombreRol = "";
                    using (MySqlCommand cmd = new MySqlCommand("SELECT nombre FROM Rol WHERE id_rol = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuario.Id_Rol);
                        nombreRol = cmd.ExecuteScalar()?.ToString() ?? "";
                    }

                    SesionActiva.Instance.IniciarSesion(usuario, nombreRol, tipoEntidad, idEntidad, privilegios);
                    CrearSesion(conn, idUsuario);

                    Logger.Info($"Cuenta activada: '{username}'");

                    return new ResultadoLogin
                    {
                        Success = true,
                        Message = "Cuenta activada exitosamente",
                        Usuario = usuario,
                        RequiereCambioPassword = false
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al activar cuenta", ex);
                return new ResultadoLogin
                {
                    Success = false,
                    Message = "Error al activar la cuenta"
                };
            }
        }

        /// <summary>
        /// Resetea la contraseña de un usuario (admin).
        /// Genera un nuevo password temporal y lo devuelve.
        /// </summary>
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

                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE Usuario
                                    SET password_temporal_hash = @temp_hash,
                                        password_hash = @temp_hash,
                                        debe_cambiar_password = TRUE,
                                        fecha_activacion = NULL
                                    WHERE id_usuario = @id";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
            List<Usuario> usuarios = new List<Usuario>();
            try
            {
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = @"SELECT u.id_usuario, u.username, u.email, u.id_rol, u.status,
                                            u.debe_cambiar_password, u.fecha_activacion, u.created_at,
                                            r.nombre AS rol_nombre
                                     FROM Usuario u
                                     INNER JOIN Rol r ON u.id_rol = r.id_rol
                                     ORDER BY u.created_at DESC";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var usuario = new Usuario
                            {
                                Id_Usuario = reader.GetInt32("id_usuario"),
                                Username = reader.GetString("username"),
                                Email = reader.GetString("email"),
                                Id_Rol = reader.GetInt32("id_rol"),
                                Status = reader.GetString("status"),
                                Debe_Cambiar_Password = reader.GetBoolean("debe_cambiar_password"),
                                Fecha_Activacion = reader.IsDBNull(reader.GetOrdinal("fecha_activacion"))
                                    ? (DateTime?)null : reader.GetDateTime("fecha_activacion"),
                                Created_At = reader.GetDateTime("created_at"),
                                Rol = new Rol { Nombre = reader.GetString("rol_nombre") }
                            };
                            usuarios.Add(usuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al listar usuarios", ex);
            }
            return usuarios;
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
                using (MySqlConnection conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE Usuario SET status = @status WHERE id_usuario = @id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
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
