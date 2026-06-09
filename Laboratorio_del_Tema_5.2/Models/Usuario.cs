using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Laboratorio_del_Tema_5_2.Models
{
    public class Usuario
    {
        public int Id_Usuario { get; set; }

        [Required(ErrorMessage = "El username es requerido")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El username debe tener entre 3 y 50 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contrasena es requerida")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contrasena debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        public string PasswordHash { get; set; }

        public int Id_Rol { get; set; }

        public string Status { get; set; }

        public DateTime? Ultimo_Login { get; set; }

        public int Intentos_Fallidos { get; set; }

        public DateTime? Bloqueado_Hasta { get; set; }

        public DateTime Created_At { get; set; }

        public DateTime Updated_At { get; set; }

        // Nuevos campos para flujo profesional
        public bool Debe_Cambiar_Password { get; set; }

        public string Password_Temporal_Hash { get; set; }

        public DateTime? Fecha_Activacion { get; set; }

        public int? Creado_Por { get; set; }

        public Rol Rol { get; set; }

        public List<Privilegio> Privilegios { get; set; }
    }

    public class Rol
    {
        public int Id_Rol { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }

    public class Privilegio
    {
        public int Id_Privilegio { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Created_At { get; set; }
    }

    public class UsuarioEntidad
    {
        public int Id_Usuario { get; set; }
        public string Tipo_Entidad { get; set; }
        public int Id_Entidad { get; set; }
        public DateTime Created_At { get; set; }
    }

    public class Sesion
    {
        public string Id_Sesion { get; set; }
        public int Id_Usuario { get; set; }
        public DateTime Fecha_Inicio { get; set; }
        public DateTime Fecha_Expiracion { get; set; }
        public string Ip_Address { get; set; }
        public string User_Agent { get; set; }
        public string Status { get; set; }
    }
}