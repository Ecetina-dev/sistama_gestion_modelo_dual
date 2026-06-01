using System;

namespace Laboratorio_del_Tema_5_2.Models
{
    public class Proyecto
    {
        public int Id_Proyecto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Objetivos { get; set; }
        public DateTime? Fecha_Inicio { get; set; }
        public DateTime? Fecha_Fin { get; set; }
        public int? Horas_Totales { get; set; }
        public string Status { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}