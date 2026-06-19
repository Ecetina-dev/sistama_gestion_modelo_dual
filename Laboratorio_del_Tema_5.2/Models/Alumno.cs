using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Student data model. Phase 1 adds enterprise identity, demographic,
    /// contact, academic, lifecycle, and audit columns while preserving
    /// all existing properties and constructors.
    /// </summary>
    public class Alumno
    {
        public int Id_Alumno { get; set; }

        [Required(ErrorMessage = "El número de control es requerido")]
        [StringLength(15, MinimumLength = 8, ErrorMessage = "El número de control debe tener entre 8 y 15 dígitos")]
        public string No_Control { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es requerido")]
        [StringLength(100, ErrorMessage = "El apellido paterno no puede exceder 100 caracteres")]
        public string Apellido_Paterno { get; set; }

        [StringLength(100, ErrorMessage = "El apellido materno no puede exceder 100 caracteres")]
        public string Apellido_Materno { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [StringLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
        public string Email { get; set; }

        [StringLength(15, MinimumLength = 10, ErrorMessage = "El teléfono debe tener entre 10 y 15 dígitos")]
        public string Telefono { get; set; }

        public DateTime? Fecha_Nacimiento { get; set; }

        public string Status_Alumno { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }

        // Phase 1 enterprise identity fields
        [StringLength(18, MinimumLength = 18, ErrorMessage = "El CURP debe tener 18 caracteres")]
        public string Curp { get; set; }

        [StringLength(13, ErrorMessage = "El RFC no puede exceder 13 caracteres")]
        public string Rfc { get; set; }

        [StringLength(20, ErrorMessage = "El NSS no puede exceder 20 caracteres")]
        public string Nss { get; set; }

        [StringLength(30, ErrorMessage = "El género no puede exceder 30 caracteres")]
        public string Genero { get; set; }

        [StringLength(30, ErrorMessage = "El estado civil no puede exceder 30 caracteres")]
        public string Estado_Civil { get; set; }

        [StringLength(50, ErrorMessage = "La nacionalidad no puede exceder 50 caracteres")]
        public string Nacionalidad { get; set; }

        // Phase 1 address fields
        [StringLength(100, ErrorMessage = "La calle no puede exceder 100 caracteres")]
        public string Direccion_Calle { get; set; }

        [StringLength(20, ErrorMessage = "El número exterior no puede exceder 20 caracteres")]
        public string Direccion_Numero { get; set; }

        [StringLength(100, ErrorMessage = "La colonia no puede exceder 100 caracteres")]
        public string Direccion_Colonia { get; set; }

        [StringLength(100, ErrorMessage = "La ciudad no puede exceder 100 caracteres")]
        public string Direccion_Ciudad { get; set; }

        [StringLength(100, ErrorMessage = "El estado no puede exceder 100 caracteres")]
        public string Direccion_Estado { get; set; }

        [StringLength(10, ErrorMessage = "El código postal no puede exceder 10 caracteres")]
        public string Direccion_Cp { get; set; }

        [StringLength(15, ErrorMessage = "El teléfono fijo no puede exceder 15 caracteres")]
        public string Telefono_Fijo { get; set; }

        // Phase 1 emergency contact fields
        [StringLength(100, ErrorMessage = "El nombre del contacto no puede exceder 100 caracteres")]
        public string Contacto_Emergencia_Nombre { get; set; }

        [StringLength(15, ErrorMessage = "El teléfono de emergencia no puede exceder 15 caracteres")]
        public string Contacto_Emergencia_Telefono { get; set; }

        [StringLength(50, ErrorMessage = "El parentesco no puede exceder 50 caracteres")]
        public string Contacto_Emergencia_Parentesco { get; set; }

        // Phase 1 academic fields
        public int? Id_Carrera { get; set; }

        [Range(1, 20, ErrorMessage = "El semestre debe estar entre 1 y 20")]
        public int? Semestre { get; set; }

        [StringLength(20, ErrorMessage = "El grupo no puede exceder 20 caracteres")]
        public string Grupo { get; set; }

        [StringLength(30, ErrorMessage = "El turno no puede exceder 30 caracteres")]
        public string Turno { get; set; }

        public DateTime? Fecha_Ingreso { get; set; }
        public DateTime? Fecha_Egreso { get; set; }
        public DateTime? Fecha_Baja { get; set; }

        [StringLength(255, ErrorMessage = "El motivo de baja no puede exceder 255 caracteres")]
        public string Motivo_Baja { get; set; }

        [Range(0.0, 10.0, ErrorMessage = "El promedio general debe estar entre 0.00 y 10.00")]
        public decimal? Promedio_General { get; set; }

        // Phase 1 audit / soft-delete support
        public int? Created_By { get; set; }
        public int? Updated_By { get; set; }
        public int? Deleted_By { get; set; }

        [StringLength(255, ErrorMessage = "El motivo de eliminación no puede exceder 255 caracteres")]
        public string Deleted_Reason { get; set; }

        [StringLength(255, ErrorMessage = "El motivo de cambio de estado no puede exceder 255 caracteres")]
        public string Status_Change_Reason { get; set; }

        public Alumno()
        {
        }

        public Alumno(string noControl, string nombre, string apellidoPaterno,
                     string apellidoMaterno, string email, string telefono)
        {
            No_Control = noControl;
            Nombre = nombre;
            Apellido_Paterno = apellidoPaterno;
            Apellido_Materno = apellidoMaterno;
            Email = email;
            Telefono = telefono;
            Status_Alumno = "activo";
            Fecha_Nacimiento = null;
        }

        public override string ToString()
        {
            return $"{No_Control} - {Nombre} {Apellido_Paterno} {Apellido_Materno}";
        }
    }
}
