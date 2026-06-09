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
    }
}