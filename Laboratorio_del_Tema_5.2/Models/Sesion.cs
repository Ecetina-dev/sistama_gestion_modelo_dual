using System;
using System.Collections.Generic;
using MySqlConnector;
using Laboratorio_del_Tema_5_2.Data;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Models
{
    public class ResultadoLogin
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Usuario Usuario { get; set; }
        public bool RequiereCambioPassword { get; set; }
        public bool RequiereActivacion { get; set; }
        public bool PasswordExpirado { get; set; }
        public bool CuentaEliminada { get; set; }
        /// <summary>Intentos restantes (-1 si no aplica, 0 = bloqueado)</summary>
        public int IntentosRestantes { get; set; } = -1;
    }

    public class SesionActiva
    {
        private static SesionActiva _instance;
        private static readonly object _lock = new object();

        public int Id_Usuario { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public int Id_Rol { get; private set; }
        public string Nombre_Rol { get; private set; }
        public string Tipo_Entidad { get; private set; }
        public int? Id_Entidad { get; private set; }
        public List<string> Privilegios { get; private set; }
        public DateTime Fecha_Login { get; private set; }
        public bool Debe_Cambiar_Password { get; private set; }

        private SesionActiva() { }

        /// <summary>
        /// Fuerza el flag de cambio de password cuando está expirado,
        /// preservando el resto de la sesión (rol, privilegios, entidad).
        /// </summary>
        public void IniciarSesionForzarCambioPassword(Usuario usuario, string nombreRol, string tipoEntidad, int? idEntidad, List<string> privilegios)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));

            Id_Usuario = usuario.Id_Usuario;
            Username = usuario.Username;
            Email = usuario.Email;
            Id_Rol = usuario.Id_Rol;
            Nombre_Rol = nombreRol;
            Tipo_Entidad = tipoEntidad;
            Id_Entidad = idEntidad;
            Privilegios = privilegios ?? new List<string>();
            Fecha_Login = DateTime.Now;
            Debe_Cambiar_Password = true;
        }

        /// <summary>
        /// Sobrecarga mínima: solo setea identidad y marca cambio obligatorio.
        /// Usar solo si NO se dispone de los datos de rol/entidad.
        /// </summary>
        public void IniciarSesionForzarCambioPassword(int idUsuario, string username)
        {
            Id_Usuario = idUsuario;
            Username = username;
            Debe_Cambiar_Password = true;
            Fecha_Login = DateTime.Now;
        }

        public static SesionActiva Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SesionActiva();
                        }
                    }
                }
                return _instance;
            }
        }

        public void IniciarSesion(Usuario usuario, string nombreRol, string tipoEntidad, int? idEntidad, List<string> privilegios)
        {
            Id_Usuario = usuario.Id_Usuario;
            Username = usuario.Username;
            Email = usuario.Email;
            Id_Rol = usuario.Id_Rol;
            Nombre_Rol = nombreRol;
            Tipo_Entidad = tipoEntidad;
            Id_Entidad = idEntidad;
            Privilegios = privilegios;
            Fecha_Login = DateTime.Now;
            Debe_Cambiar_Password = usuario.Debe_Cambiar_Password;
        }

        public void CerrarSesion()
        {
            // Resetear intentos fallidos al cerrar sesión (el usuario ya salió del sistema)
            if (_instance != null && _instance.Id_Usuario > 0)
            {
                try
                {
                    using (var conn = Laboratorio_del_Tema_5_2.Data.MySQLConnection.GetConnection())
                    {
                        conn.Open();
                        using (var cmd = new MySqlConnector.MySqlCommand(
                            "UPDATE Usuario SET intentos_fallidos = 0, bloqueado_hasta = NULL WHERE id_usuario = @id", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", _instance.Id_Usuario);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { /* no bloquear el cierre por error de BD */ }
            }
            _instance = null;
        }

        public bool TienePrivilegio(string privilegio)
        {
            if (Privilegios == null) return false;
            return Privilegios.Contains(privilegio) || Privilegios.Contains("admin.crud_todo");
        }

        public bool EsAdmin => Nombre_Rol == "admin";
        public bool EsAlumno => Nombre_Rol == "alumno";
        public bool EsProfesor => Nombre_Rol == "profesor";
        public bool EsEmpresa => Nombre_Rol == "empresa";

        /// <summary>
        /// Verifica si la sesión actual ha expirado por tiempo.
        /// </summary>
        public bool IsSesionExpirada(int duracionHoras = 8)
        {
            if (Id_Usuario == 0 || Fecha_Login == default)
                return false;
            return DateTime.Now > Fecha_Login.AddHours(duracionHoras);
        }
    }
}