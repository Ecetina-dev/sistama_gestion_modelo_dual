using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Tema.
    /// </summary>
    public class Tema
    {
        public int Id_Tema { get; set; }

        [Required(ErrorMessage = "La materia es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione una materia")]
        public int Id_Materia { get; set; }

        [Required(ErrorMessage = "El número de tema es requerido")]
        [Range(1, 999, ErrorMessage = "El número de tema debe estar entre 1 y 999")]
        public int Numero_Tema { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        [MinLength(3, ErrorMessage = "El nombre debe tener al menos 3 caracteres")]
        public string Nombre { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        public string Status_Tema { get; set; }
        public string Nombre_Materia { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public override string ToString()
        {
            return $"{Numero_Tema}. {Nombre}";
        }
    }
}