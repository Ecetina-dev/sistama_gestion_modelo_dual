using System;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Alumno.
    /// </summary>
    public class Alumno
    {
        public int Id_Alumno { get; set; }
        public string No_Control { get; set; }
        public string Nombre { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Email { get; set; }
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