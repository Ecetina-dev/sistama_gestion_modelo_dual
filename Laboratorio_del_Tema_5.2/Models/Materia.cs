using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Materia.
    /// </summary>
    public class Materia
    {
        public int Id_Materia { get; set; }

        [Required(ErrorMessage = "La clave de materia es requerida")]
        [StringLength(20, ErrorMessage = "La clave no puede exceder 20 caracteres")]
        public string Clave_Materia { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        [Range(0, 100, ErrorMessage = "Los créditos deben estar entre 0 y 100")]
        public int Creditos { get; set; }

        [Range(0, 12, ErrorMessage = "El semestre debe estar entre 0 y 12")]
        public int Semestre { get; set; }

        [Range(0, 20, ErrorMessage = "Las horas de teoría deben estar entre 0 y 20")]
        public int Horas_Teoria { get; set; }

        [Range(0, 20, ErrorMessage = "Las horas de práctica deben estar entre 0 y 20")]
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