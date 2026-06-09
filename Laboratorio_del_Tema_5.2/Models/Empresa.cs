using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Modelo de datos para la entidad Empresa.
    /// </summary>
    public class Empresa
    {
        public int Id_Empresa { get; set; }

        [Required(ErrorMessage = "El nombre comercial es requerido")]
        [StringLength(200, ErrorMessage = "El nombre comercial no puede exceder 200 caracteres")]
        public string Nombre_Comercial { get; set; }

        [StringLength(200, ErrorMessage = "La razón social no puede exceder 200 caracteres")]
        public string Razon_Social { get; set; }

        [StringLength(13, MinimumLength = 12, ErrorMessage = "El RFC debe tener 12 o 13 caracteres")]
        public string RFC { get; set; }

        [StringLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
        public string Direccion { get; set; }

        [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
        public string Ciudad { get; set; }

        [StringLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres")]
        public string Estado { get; set; }

        [StringLength(5, ErrorMessage = "El código postal debe tener 5 dígitos")]
        public string CP { get; set; }

        [StringLength(10, ErrorMessage = "El teléfono no puede exceder 10 caracteres")]
        public string Telefono_Empresa { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email_Empresa { get; set; }

        [StringLength(100, ErrorMessage = "El nombre de contacto no puede exceder 100 caracteres")]
        public string Nombre_Contacto { get; set; }

        [StringLength(100, ErrorMessage = "El puesto de contacto no puede exceder 100 caracteres")]
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