using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Laboratorio_del_Tema_5_2.Data.EntityModel;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers.Services
{
    /// <summary>
    /// Servicio de autenticación — separa la lógica de negocio del controller.
    /// Usa Entity Framework para acceso a datos.
    /// </summary>
    public class AuthService
    {
        public ResultadoLogin ValidarCredenciales(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(password))
            {
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgCredencialesInvalidas
                };
            }

            login = login.Trim();

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
                using (var db = new ModeloDualContext())
                {
                    var user = db.Usuarios
                        .Include(u => u.Rol)
                        .AsNoTracking()
                        .FirstOrDefault(u => u.Username.ToLower() == login.ToLower()
                                          || u.Email.ToLower() == login.ToLower());

                    if (user == null)
                    {
                        try { BCrypt.Net.BCrypt.Verify("dummy", "$2a$11$aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"); } catch { }
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = Seguridad.MsgCredencialesInvalidas
                        };
                    }

                    string usernameBd = user.Username;
                    string passwordHash = user.PasswordHash;
                    string status = user.Status;
                    int idUsuario = user.Id_Usuario;
                    int intentosFallidos = user.Intentos_Fallidos ?? 0;
                    string rolNombre = user.Rol?.Nombre ?? "";

                    bool isDeleted = user.Is_Deleted == true;
                    bool debeCambiarPassword = user.Debe_Cambiar_Password == 1;
                    DateTime? fechaActivacion = user.Fecha_Activacion;

                    if (isDeleted)
                    {
                        Logger.Warning($"Intento de login en cuenta eliminada: '{usernameBd}'");
                        return new ResultadoLogin
                        {
                            Success = false,
                            CuentaEliminada = true,
                            Message = "Esta cuenta ha sido eliminada. Contacta al administrador."
                        };
                    }

                    int minutosBloqueo = ParametroSistemaService.Instance.GetInt(
                        Claves.TIEMPO_BLOQUEO_MINUTOS, Seguridad.MinutosBloqueo);

                    if (user.Bloqueado_Hasta.HasValue && DateTime.Now < user.Bloqueado_Hasta.Value)
                    {
                        TimeSpan restante = user.Bloqueado_Hasta.Value - DateTime.Now;
                        Logger.Warning($"Cuenta bloqueada: '{usernameBd}'. Restante: {restante.Minutes} min");
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = $"Demasiados intentos fallidos. Reintenta en {Math.Ceiling(restante.TotalMinutes)} minuto(s)."
                        };
                    }

                    if (status == "inactivo" || status == "suspendido")
                    {
                        Logger.Warning($"Intento de login en cuenta {status}: '{usernameBd}'");
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = Seguridad.MsgCuentaInactiva
                        };
                    }

                    if (!BCrypt.Net.BCrypt.Verify(password, passwordHash))
                    {
                        using (var dbWrite = new ModeloDualContext())
                        {
                            var userWrite = dbWrite.Usuarios.Find(idUsuario);
                            if (userWrite != null)
                            {
                                userWrite.Intentos_Fallidos = (userWrite.Intentos_Fallidos ?? 0) + 1;
                                int intentosActuales = userWrite.Intentos_Fallidos.Value;
                                int maxIntentos = ParametroSistemaService.Instance.GetInt(
                                    Claves.MAX_INTENTOS_LOGIN, Seguridad.MaxIntentosLogin);

                                if (intentosActuales >= maxIntentos)
                                {
                                    userWrite.Bloqueado_Hasta = DateTime.Now.AddMinutes(minutosBloqueo);
                                    Logger.Warning($"Cuenta bloqueada por {minutosBloqueo} min: '{usernameBd}'");
                                }
                                dbWrite.SaveChanges();
                            }
                        }

                        int restantes = ParametroSistemaService.Instance.GetInt(
                            Claves.MAX_INTENTOS_LOGIN, Seguridad.MaxIntentosLogin) - intentosFallidos - 1;
                        Logger.Warning($"Contrasena incorrecta para '{usernameBd}'. Intentos restantes: {restantes}");
                        return new ResultadoLogin
                        {
                            Success = false,
                            Message = restantes > 0
                                ? $"{Seguridad.MsgCredencialesInvalidas} (Intentos restantes: {restantes})"
                                : Seguridad.MsgCuentaBloqueada,
                            IntentosRestantes = restantes
                        };
                    }

                    if (debeCambiarPassword)
                    {
                        Logger.Info($"Cuenta requiere activacion: '{usernameBd}'");
                        return new ResultadoLogin
                        {
                            Success = true,
                            RequiereActivacion = true,
                            Message = "Debes activar tu cuenta con una nueva contraseña."
                        };
                    }

                    int diasCaducidad = ParametroSistemaService.Instance.GetInt(
                        Claves.DIAS_CADUCIDAD_PASSWORD, Seguridad.DiasCaducidadPassword);
                    bool passwordExpirado = false;
                    if (fechaActivacion.HasValue)
                    {
                        passwordExpirado = (DateTime.Now - fechaActivacion.Value).TotalDays > diasCaducidad;
                    }

                    using (var dbWrite = new ModeloDualContext())
                    using (var tx = dbWrite.Database.BeginTransaction())
                    {
                        try
                        {
                            var userWrite = dbWrite.Usuarios.Find(idUsuario);
                            if (userWrite == null)
                                return new ResultadoLogin { Success = false, Message = Seguridad.MsgErrorGenerico };

                            userWrite.Ultimo_Login = DateTime.Now;
                            userWrite.Intentos_Fallidos = 0;
                            userWrite.Bloqueado_Hasta = null;

                            var usuarioDto = new Usuario
                            {
                                Id_Usuario = userWrite.Id_Usuario,
                                Username = userWrite.Username,
                                Email = userWrite.Email,
                                Id_Rol = userWrite.Id_Rol,
                                Status = userWrite.Status,
                                PasswordHash = userWrite.PasswordHash,
                                Debe_Cambiar_Password = userWrite.Debe_Cambiar_Password == 1,
                                Fecha_Activacion = userWrite.Fecha_Activacion,
                                Created_At = userWrite.Created_At ?? DateTime.Now
                            };

                            var privilegios = ObtenerPrivilegios(dbWrite, userWrite.Id_Rol);
                            var (tipoEntidad, idEntidad) = ObtenerEntidadVinculada(dbWrite, idUsuario);

                            SesionActiva.Instance.IniciarSesion(usuarioDto, rolNombre, tipoEntidad, idEntidad, privilegios);

                            dbWrite.Sesiones.Add(new SesionEF
                            {
                                Id_Sesion = Guid.NewGuid().ToString("N"),
                                Id_Usuario = idUsuario,
                                Fecha_Inicio = DateTime.Now,
                                Fecha_Expiracion = DateTime.Now.AddHours(Seguridad.DuracionSesionHoras),
                                Ip_Address = "127.0.0.1",
                                User_Agent = "WinForms App",
                                Status = "activa"
                            });

                            string jsonDatos = new System.Web.Script.Serialization.JavaScriptSerializer()
                                .Serialize(new { username = usernameBd });

                            dbWrite.Database.ExecuteSqlCommand(
                                "INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, datos_nuevos) VALUES (@p0, @p1, @p2, @p3, @p4)",
                                "Usuario", idUsuario.ToString(), "LOGIN", usernameBd, jsonDatos);

                            dbWrite.SaveChanges();
                            tx.Commit();

                            Logger.Info($"Login exitoso para usuario '{usernameBd}' (rol: {rolNombre})");

                            return new ResultadoLogin
                            {
                                Success = true,
                                Message = "Login exitoso",
                                Usuario = usuarioDto,
                                RequiereCambioPassword = usuarioDto.Debe_Cambiar_Password,
                                PasswordExpirado = passwordExpirado
                            };
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error en ValidarCredenciales", ex);
                return new ResultadoLogin
                {
                    Success = false,
                    Message = Seguridad.MsgErrorGenerico
                };
            }
        }

        public List<Rol> ObtenerRoles()
        {
            try
            {
                using (var db = new ModeloDualContext())
                {
                    return db.Roles
                        .OrderBy(r => r.Nombre)
                        .AsNoTracking()
                        .Select(r => new Rol
                        {
                            Id_Rol = r.Id_Rol,
                            Nombre = r.Nombre,
                            Descripcion = r.Descripcion
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al obtener roles", ex);
                return new List<Rol>();
            }
        }

        private List<string> ObtenerPrivilegios(ModeloDualContext db, int idRol)
        {
            return (from rp in db.RolesPrivilegios
                    join p in db.Privilegios on rp.Id_Privilegio equals p.Id_Privilegio
                    where rp.Id_Rol == idRol
                    select p.Nombre).AsNoTracking().ToList();
        }

        private (string tipoEntidad, int? idEntidad) ObtenerEntidadVinculada(ModeloDualContext db, int idUsuario)
        {
            var alumno = db.UsuariosAlumnos.AsNoTracking()
                .FirstOrDefault(ua => ua.Id_Usuario == idUsuario);
            if (alumno != null) return ("alumno", alumno.Id_Alumno);

            var prof = db.UsuariosProfesores.AsNoTracking()
                .FirstOrDefault(up => up.Id_Usuario == idUsuario);
            if (prof != null) return ("profesor", prof.Id_Profesor);

            var emp = db.UsuariosEmpresas.AsNoTracking()
                .FirstOrDefault(ue => ue.Id_Usuario == idUsuario);
            if (emp != null) return ("empresa", emp.Id_Empresa);

            return (null, null);
        }

        public void CerrarSesion()
        {
            try
            {
                int idUsuario = SesionActiva.Instance.Id_Usuario;
                using (var db = new ModeloDualContext())
                {
                    var sesion = db.Sesiones
                        .FirstOrDefault(s => s.Id_Usuario == idUsuario && s.Status == "activa");
                    if (sesion != null)
                    {
                        sesion.Status = "cerrada";
                        db.SaveChanges();
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
    
        public ResultadoLogin ActivarCuenta(string username, string passwordTemporal, string passwordNuevo)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(passwordTemporal) || string.IsNullOrEmpty(passwordNuevo))
                return new ResultadoLogin { Success = false, Message = "Todos los campos son requeridos" };

            if (passwordNuevo.Length < Seguridad.PasswordMinLength)
                return new ResultadoLogin { Success = false, Message = Seguridad.MsgPasswordLongitud };

            var validacionComun = PasswordValidator.Validar(passwordNuevo, username, null);
            if (!validacionComun.EsValido)
            {
                Logger.Warning($"Intento de activar cuenta con password débil: '{username}'. Razón: {validacionComun.Razon}");
                return new ResultadoLogin { Success = false, Message = validacionComun.Razon };
            }

            username = username.Trim();

            try
            {
                using (var db = new ModeloDualContext())
                using (var tx = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        var user = db.Usuarios
                            .FirstOrDefault(u => u.Username.ToLower() == username.ToLower()
                                              && u.Is_Deleted != true);

                        if (user == null)
                        {
                            tx.Rollback();
                            return new ResultadoLogin { Success = false, Message = "Credenciales invalidas" };
                        }

                        if (user.Bloqueado_Hasta.HasValue && DateTime.Now < user.Bloqueado_Hasta.Value)
                        {
                            tx.Rollback();
                            TimeSpan restante = user.Bloqueado_Hasta.Value - DateTime.Now;
                            Logger.Warning($"Activacion bloqueada para '{username}'. Restante: {Math.Ceiling(restante.TotalMinutes)} min");
                            return new ResultadoLogin { Success = false, Message = $"Demasiados intentos fallidos. Reintenta en {Math.Ceiling(restante.TotalMinutes)} minuto(s)." };
                        }

                        if (user.Status != "activo")
                        {
                            tx.Rollback();
                            return new ResultadoLogin { Success = false, Message = "Tu cuenta no esta activa. Contacta al administrador." };
                        }

                        if (string.IsNullOrEmpty(user.Password_Temporal_Hash))
                        {
                            tx.Rollback();
                            return new ResultadoLogin { Success = false, Message = "Esta cuenta ya fue activada. Usa el login normal." };
                        }

                        if (!BCrypt.Net.BCrypt.Verify(passwordTemporal, user.Password_Temporal_Hash))
                        {
                            user.Intentos_Fallidos = (user.Intentos_Fallidos ?? 0) + 1;
                            int intentosActuales = user.Intentos_Fallidos.Value;
                            int maxIntentos = ParametroSistemaService.Instance.GetInt(Claves.MAX_INTENTOS_LOGIN, Seguridad.MaxIntentosLogin);

                            if (intentosActuales >= maxIntentos)
                                user.Bloqueado_Hasta = DateTime.Now.AddMinutes(
                                    ParametroSistemaService.Instance.GetInt(Claves.TIEMPO_BLOQUEO_MINUTOS, Seguridad.MinutosBloqueo));

                            db.SaveChanges();

                            int restantes = maxIntentos - intentosActuales;
                            Logger.Warning($"Intento de activacion con password temporal invalido: '{username}'. Restantes: {restantes}");
                            tx.Commit();
                            return new ResultadoLogin
                            {
                                Success = false,
                                Message = restantes > 0 ? $"Credenciales invalidas (Intentos restantes: {restantes})" : Seguridad.MsgCuentaBloqueada,
                                IntentosRestantes = restantes
                            };
                        }

                        // Validar contra historial de passwords reutilizados
                        var hashesAnteriores = db.Database.SqlQuery<string>(
                            "SELECT password_hash FROM password_history WHERE id_usuario = @p0 ORDER BY fecha_cambio DESC",
                            user.Id_Usuario).ToList();

                        if (hashesAnteriores.Any(h => BCrypt.Net.BCrypt.Verify(passwordNuevo, h)))
                        {
                            tx.Rollback();
                            Logger.Warning($"Activacion rechazada: password reutilizado (usuario {username})");
                            return new ResultadoLogin { Success = false, Message = "No podes reutilizar una contrasena reciente. Elegi una diferente." };
                        }

                        // Activar cuenta
                        string nuevoHash = BCrypt.Net.BCrypt.HashPassword(passwordNuevo, Seguridad.BcryptCostFactor);
                        user.PasswordHash = nuevoHash;
                        user.Password_Temporal_Hash = null;
                        user.Debe_Cambiar_Password = 0;
                        user.Fecha_Activacion = DateTime.Now;
                        user.Intentos_Fallidos = 0;
                        user.Bloqueado_Hasta = null;

                        db.SaveChanges();
                        tx.Commit();

                        Logger.Info($"Cuenta activada exitosamente: '{username}'");

                        var usuarioDto = new Usuario
                        {
                            Id_Usuario = user.Id_Usuario,
                            Username = user.Username,
                            Email = user.Email,
                            Id_Rol = user.Id_Rol,
                            Status = user.Status,
                            PasswordHash = nuevoHash,
                            Debe_Cambiar_Password = false,
                            Fecha_Activacion = DateTime.Now,
                            Created_At = user.Created_At ?? DateTime.Now
                        };

                        List<string> privilegios = ObtenerPrivilegios(db, user.Id_Rol);
                        var (tipoEntidad, idEntidad) = ObtenerEntidadVinculada(db, user.Id_Usuario);

                        SesionActiva.Instance.IniciarSesion(usuarioDto, "usuario", tipoEntidad, idEntidad, privilegios);

                        return new ResultadoLogin
                        {
                            Success = true,
                            Message = "Cuenta activada exitosamente",
                            Usuario = usuarioDto,
                            RequiereCambioPassword = false
                        };
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
                Logger.Error("Error en ActivarCuenta", ex);
                return new ResultadoLogin { Success = false, Message = Seguridad.MsgErrorGenerico };
            }
        }
    }
}
