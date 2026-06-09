using System;
using System.Collections.Generic;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Enums para los status utilizados en la aplicacion.
    /// </summary>
    
    // ============================================
    // Status de Empresa
    // ============================================
    public enum StatusEmpresa
    {
        Activa,
        Inactiva,
        Propuesta
    }

    // ============================================
    // Status de Alumno
    // ============================================
    public enum StatusAlumno
    {
        Activo,
        Inactivo,
        Egresado,
        Suspendido
    }

    // ============================================
    // Status de Profesor
    // ============================================
    public enum StatusProfesor
    {
        Activo,
        Inactivo
    }

    // ============================================
    // Status de Materia
    // ============================================
    public enum StatusMateria
    {
        Activa,
        Inactiva
    }

    // ============================================
    // Status de Asignacion (Alumno-Empresa)
    // ============================================
    public enum StatusAsignacion
    {
        Activa,
        Finalizada,
        Cancelada
    }

    // ============================================
    // Status de Proyecto
    // ============================================
    public enum StatusProyecto
    {
        Activo,
        Completado,
        Cancelado,
        EnRevision,
        Propuesto
    }

    // ============================================
    // Status de Tema
    // ============================================
    public enum StatusTema
    {
        Activo,
        Disponible,
        Asignado,
        Completado
    }

    // ============================================
    // Extension methods para conversiones
    // ============================================
    public static class StatusExtensions
    {
        /// <summary>
        /// Convierte StatusEmpresa a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusEmpresa status)
        {
            return status.ToString().ToLower();
        }

        /// <summary>
        /// Convierte un string de la BD a StatusEmpresa.
        /// </summary>
        public static StatusEmpresa ToStatusEmpresa(this string status)
        {
            switch (status.ToLower())
            {
                case "activa": return StatusEmpresa.Activa;
                case "inactiva": return StatusEmpresa.Inactiva;
                case "propuesta": return StatusEmpresa.Propuesta;
                default: return StatusEmpresa.Activa;
            }
        }

        /// <summary>
        /// Convierte StatusAlumno a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusAlumno status)
        {
            return status.ToString().ToLower();
        }

        /// <summary>
        /// Convierte un string de la BD a StatusAlumno.
        /// </summary>
        public static StatusAlumno ToStatusAlumno(this string status)
        {
            switch (status.ToLower())
            {
                case "activo": return StatusAlumno.Activo;
                case "inactivo": return StatusAlumno.Inactivo;
                case "egresado": return StatusAlumno.Egresado;
                case "suspendido": return StatusAlumno.Suspendido;
                default: return StatusAlumno.Activo;
            }
        }

        /// <summary>
        /// Convierte StatusProfesor a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusProfesor status)
        {
            return status.ToString().ToLower();
        }

        /// <summary>
        /// Convierte StatusMateria a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusMateria status)
        {
            return status.ToString().ToLower();
        }

        /// <summary>
        /// Convierte StatusProyecto a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusProyecto status)
        {
            if (status == StatusProyecto.EnRevision) return "en_revision";
            return status.ToString().ToLower();
        }

        /// <summary>
        /// Convierte un string de la BD a StatusProyecto.
        /// </summary>
        public static StatusProyecto ToStatusProyecto(this string status)
        {
            switch (status.ToLower())
            {
                case "activo": return StatusProyecto.Activo;
                case "completado": return StatusProyecto.Completado;
                case "cancelado": return StatusProyecto.Cancelado;
                case "en_revision": return StatusProyecto.EnRevision;
                case "propuesto": return StatusProyecto.Propuesto;
                default: return StatusProyecto.Propuesto;
            }
        }

        /// <summary>
        /// Convierte StatusTema a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusTema status)
        {
            return status.ToString().ToLower();
        }

        /// <summary>
        /// Convierte StatusAsignacion a string para la base de datos.
        /// </summary>
        public static string ToDbString(this StatusAsignacion status)
        {
            return status.ToString().ToLower();
        }
    }
}