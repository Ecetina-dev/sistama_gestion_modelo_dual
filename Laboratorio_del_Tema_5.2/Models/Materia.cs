using System;

namespace Laboratorio_del_Tema_5_2.Models
{
    public class Materia
    {
        public int Id_Materia { get; set; }
        public string Clave_Materia { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Creditos { get; set; }
        public int Semestre { get; set; }
        public int Horas_Teoria { get; set; }
        public int Horas_Practica { get; set; }
        public string Status_Materia { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Materia()
        {
        }

        public Materia(string clave, string nombre, string descripcion, int creditos, int semestre)
        {
            Clave_Materia = clave;
            Nombre = nombre;
            Descripcion = descripcion;
            Creditos = creditos;
            Semestre = semestre;
            Status_Materia = "activa";
        }

        public override string ToString()
        {
            return $"{Clave_Materia} - {Nombre}";
        }
    }
}