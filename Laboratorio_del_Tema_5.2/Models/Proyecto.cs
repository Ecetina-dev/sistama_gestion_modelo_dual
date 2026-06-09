using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Proyecto.
    /// </summary>
    public class Proyecto
    {
        public int Id_Proyecto { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; }

        [StringLength(1000, ErrorMessage = "Los objetivos no pueden exceder 1000 caracteres")]
        public string Objetivos { get; set; }

        public DateTime? Fecha_Inicio { get; set; }
        public DateTime? Fecha_Fin { get; set; }

        [Range(0, 10000, ErrorMessage = "Las horas totales deben estar entre 0 y 10000")]
        public int? Horas_Totales { get; set; }

        public string Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public override string ToString()
        {
            return $"{Nombre} ({Status})";
        }
    }
}