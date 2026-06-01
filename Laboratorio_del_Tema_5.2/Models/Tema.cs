using System;

namespace Laboratorio_del_Tema_5_2.Models
{
    public class Tema
    {
        public int Id_Tema { get; set; }
        public int Id_Materia { get; set; }
        public int Numero_Tema { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Horas_Estimadas { get; set; }
        public string Status_Tema { get; set; }
        public string Nombre_Materia { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
    }
}