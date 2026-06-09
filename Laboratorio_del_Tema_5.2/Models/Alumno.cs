using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Alumno.
    /// </summary>
    public class Alumno
    {
        public int Id_Alumno { get; set; }

        [Required(ErrorMessage = "El número de control es requerido")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El número de control debe tener 8 dígitos")]
        public string No_Control { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es requerido")]
        [StringLength(100, ErrorMessage = "El apellido paterno no puede exceder 100 caracteres")]
        public string Apellido_Paterno { get; set; }

        [StringLength(100, ErrorMessage = "El apellido materno no puede exceder 100 caracteres")]
        public string Apellido_Materno { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; }

        [StringLength(10, MinimumLength = 10, ErrorMessage = "El teléfono debe tener 10 dígitos")]
        public string Telefono { get; set; }

        public DateTime? Fecha_Nacimiento { get; set; }

        public string Status_Alumno { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Alumno()
        {
        }

        public Alumno(string noControl, string nombre, string apellidoPaterno, 
                     string apellidoMaterno, string email, string telefono)
        {
            No_Control = noControl;
            Nombre = nombre;
            Apellido_Paterno = apellidoPaterno;
            Apellido_Materno = apellidoMaterno;
            Email = email;
            Telefono = telefono;
            Status_Alumno = "activo";
            Fecha_Nacimiento = null;
        }

        public override string ToString()
        {
            return $"{No_Control} - {Nombre} {Apellido_Paterno} {Apellido_Materno}";
        }
    }
}