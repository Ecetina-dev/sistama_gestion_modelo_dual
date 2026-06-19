using System;
using System.ComponentModel.DataAnnotations;

namespace Laboratorio_del_Tema_5_2.Models
{
    /// <summary>
    /// Attachment metadata model for student documents.
    /// Actual file storage is handled outside the database.
    /// </summary>
    public class AlumnoDocumento
    {
        public int Id_Documento { get; set; }

        [Required(ErrorMessage = "El alumno es requerido")]
        public int Id_Alumno { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [StringLength(50, ErrorMessage = "El tipo de documento no puede exceder 50 caracteres")]
        public string Tipo_Documento { get; set; }

        [Required(ErrorMessage = "El nombre de archivo es requerido")]
        [StringLength(255, ErrorMessage = "El nombre de archivo no puede exceder 255 caracteres")]
        public string Nombre_Archivo { get; set; }

        [Required(ErrorMessage = "La ruta de archivo es requerida")]
        [StringLength(500, ErrorMessage = "La ruta de archivo no puede exceder 500 caracteres")]
        public string Ruta_Archivo { get; set; }

        [StringLength(100, ErrorMessage = "El MIME type no puede exceder 100 caracteres")]
        public string Mime_Type { get; set; }

        public long? Tamanio_Bytes { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        public int? Uploaded_By { get; set; }
        public DateTime Created_At { get; set; }

        public AlumnoDocumento()
        {
        }

        public AlumnoDocumento(int idAlumno, string tipoDocumento, string nombreArchivo,
                               string rutaArchivo, string mimeType, long? tamanioBytes, int? uploadedBy)
        {
            Id_Alumno = idAlumno;
            Tipo_Documento = tipoDocumento;
            Nombre_Archivo = nombreArchivo;
            Ruta_Archivo = rutaArchivo;
            Mime_Type = mimeType;
            Tamanio_Bytes = tamanioBytes;
            Uploaded_By = uploadedBy;
        }

        public override string ToString()
        {
            return $"{Tipo_Documento} - {Nombre_Archivo}";
        }
    }
}
