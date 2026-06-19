using System;
using System.ComponentModel.DataAnnotations;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Career lookup model. Careers are soft-deleted by status to preserve
    /// referential integrity with existing students.
    /// </summary>
    public class Carrera
    {
        public int Id_Carrera { get; set; }

        [Required(ErrorMessage = "La clave de carrera es requerida")]
        [StringLength(20, ErrorMessage = "La clave no puede exceder 20 caracteres")]
        public string Clave { get; set; }

        [Required(ErrorMessage = "El nombre de carrera es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        [Range(1, 20, ErrorMessage = "La duración debe estar entre 1 y 20 semestres")]
        public int Duracion_Semestres { get; set; } = 8;

        public string Status { get; set; } = Estatus.CarreraActiva;

        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Carrera()
        {
        }

        public Carrera(string clave, string nombre, string descripcion, int duracionSemestres)
        {
            Clave = clave;
            Nombre = nombre;
            Descripcion = descripcion;
            Duracion_Semestres = duracionSemestres;
            Status = Estatus.CarreraActiva;
        }

        public override string ToString()
        {
            return $"{Clave} - {Nombre}";
        }
    }
}
