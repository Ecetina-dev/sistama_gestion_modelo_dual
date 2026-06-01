using System;

namespace Laboratorio_del_Tema_5_2.Models
{
    public class Profesor
    {
        public int Id_Profesor { get; set; }
        public string No_Empleado { get; set; }
        public string Nombre { get; set; }
        public string Apellido_Paterno { get; set; }
        public string Apellido_Materno { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Departamento { get; set; }
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