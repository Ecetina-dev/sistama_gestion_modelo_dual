using System;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Empresa.
    /// </summary>
    public class Empresa
    {
        public int Id_Empresa { get; set; }
        public string Nombre_Comercial { get; set; }
        public string Razon_Social { get; set; }
        public string RFC { get; set; }
        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Estado { get; set; }
        public string CP { get; set; }
        public string Telefono_Empresa { get; set; }
        public string Email_Empresa { get; set; }
        public string Nombre_Contacto { get; set; }
        public string Puesto_Contacto { get; set; }
        public string Status_Empresa { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        public Empresa()
        {
        }

        public Empresa(string nombreComercial, string razonSocial, string rfc, 
                       string direccion, string ciudad, string estado,
                       string telefono, string email, string contacto, string puesto)
        {
            Nombre_Comercial = nombreComercial;
            Razon_Social = razonSocial;
            RFC = rfc;
            Direccion = direccion;
            Ciudad = ciudad;
            Estado = estado;
            Telefono_Empresa = telefono;
            Email_Empresa = email;
            Nombre_Contacto = contacto;
            Puesto_Contacto = puesto;
            Status_Empresa = "activa";
        }

        public override string ToString()
        {
            return $"{Nombre_Comercial} - {Ciudad}";
        }
    }
}