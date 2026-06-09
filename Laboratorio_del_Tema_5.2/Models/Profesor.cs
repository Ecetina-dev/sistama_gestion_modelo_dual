using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Profesor.
    /// </summary>
    public class Profesor
    {
        public int Id_Profesor { get; set; }

        [Required(ErrorMessage = "El número de empleado es requerido")]
        [StringLength(20, ErrorMessage = "El número de empleado no puede exceder 20 caracteres")]
        public string No_Empleado { get; set; }

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

        [StringLength(10, ErrorMessage = "El teléfono no puede exceder 10 caracteres")]
        public string Telefono { get; set; }

        [StringLength(100, ErrorMessage = "El departamento no puede exceder 100 caracteres")]
        public string Departamento { get; set; }

        [StringLength(100, ErrorMessage = "El puesto no puede exceder 100 caracteres")]
        public string Puesto { get; set; }

        public string Status_Profesor { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Profesor()
        {
        }

        public Profesor(string noEmpleado, string nombre, string apellidoPaterno, 
                        string apellidoMaterno, string email, string telefono,
                        string departamento, string puesto)
        {
            No_Empleado = noEmpleado;
            Nombre = nombre;
            Apellido_Paterno = apellidoPaterno;
            Apellido_Materno = apellidoMaterno;
            Email = email;
            Telefono = telefono;
            Departamento = departamento;
            Puesto = puesto;
            Status_Profesor = "activo";
        }

        public override string ToString()
        {
            return $"{No_Empleado} - {Nombre} {Apellido_Paterno}";
        }
    }
}