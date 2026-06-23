using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Laboratorio_del_Tema_5_2.Data.EntityModel;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Controllers.Services
{
    /// <summary>
    /// Servicio de alumnos con Entity Framework.
    /// Separado del controller legacy para cumplir el estandar de capas.
    /// </summary>
    public class AlumnoService
    {
        public bool Create(Alumno alumno)
        {
            try
            {
                using (var db = new ModeloDualContext())
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Validaciones (se mantienen en el controller legacy por su complejidad con SqlConnection)
                        if (string.IsNullOrEmpty(alumno.Status_Alumno))
                            alumno.Status_Alumno = Estatus.AlumnoActivo;

                        var entity = new AlumnoEF
                        {
                            No_Control = alumno.No_Control?.ToUpperInvariant(),
                            Nombre = alumno.Nombre,
                            Apellido_Paterno = alumno.Apellido_Paterno,
                            Apellido_Materno = alumno.Apellido_Materno,
                            Email = alumno.Email,
                            Telefono = alumno.Telefono,
                            Fecha_Nacimiento = alumno.Fecha_Nacimiento,
                            Status_Alumno = alumno.Status_Alumno,
                            Curp = alumno.Curp?.ToUpperInvariant(),
                            Rfc = alumno.Rfc?.ToUpperInvariant(),
                            Nss = alumno.Nss,
                            Genero = alumno.Genero,
                            Estado_Civil = alumno.Estado_Civil,
                            Nacionalidad = alumno.Nacionalidad,
                            Direccion_Calle = alumno.Direccion_Calle,
                            Direccion_Numero = alumno.Direccion_Numero,
                            Direccion_Colonia = alumno.Direccion_Colonia,
                            Direccion_Ciudad = alumno.Direccion_Ciudad,
                            Direccion_Estado = alumno.Direccion_Estado,
                            Direccion_Cp = alumno.Direccion_Cp,
                            Telefono_Fijo = alumno.Telefono_Fijo,
                            Contacto_Emergencia_Nombre = alumno.Contacto_Emergencia_Nombre,
                            Contacto_Emergencia_Telefono = alumno.Contacto_Emergencia_Telefono,
                            Contacto_Emergencia_Parentesco = alumno.Contacto_Emergencia_Parentesco,
                            Id_Carrera = alumno.Id_Carrera,
                            Semestre = alumno.Semestre,
                            Grupo = alumno.Grupo,
                            Turno = alumno.Turno,
                            Fecha_Ingreso = alumno.Fecha_Ingreso,
                            Fecha_Egreso = alumno.Fecha_Egreso,
                            Fecha_Baja = alumno.Fecha_Baja,
                            Motivo_Baja = alumno.Motivo_Baja,
                            Promedio_General = alumno.Promedio_General,
                            Created_By = ObtenerUsuarioAuditoria(),
                            Created_At = DateTime.Now,
                            Updated_At = DateTime.Now,
                            No_Control_Unico = alumno.No_Control?.ToUpperInvariant()
                        };

                        // Verificar duplicados antes de insertar
                        if (db.Alumnos.Any(a => a.No_Control == entity.No_Control))
                            throw new CrudOperationException(MensajesAlumno.NoControlExiste, "Create", alumno);

                        if (!string.IsNullOrEmpty(entity.Curp) && db.Alumnos.Any(a => a.Curp == entity.Curp))
                            throw new CrudOperationException(MensajesAlumno.CurpExiste, "Create", alumno);

                        if (!string.IsNullOrEmpty(entity.Email) && db.Alumnos.Any(a => a.Email == entity.Email))
                            throw new CrudOperationException(MensajesAlumno.EmailExiste, "Create", alumno);

                        db.Alumnos.Add(entity);
                        db.SaveChanges();
                        alumno.Id_Alumno = entity.Id_Alumno;

                        tx.Commit();

                        Logger.Info($"Alumno creado: ID={alumno.Id_Alumno}, no_control={alumno.No_Control}");
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al crear alumno: {ex}", ex);

                // Capturar errores de validacion de EF
                if (ex is System.Data.Entity.Validation.DbEntityValidationException valEx)
                {
                    var detalles = valEx.EntityValidationErrors
                        .SelectMany(e => e.ValidationErrors)
                        .Select(e => $"{e.PropertyName}: {e.ErrorMessage}");
                    string msg = string.Join("; ", detalles);
                    Logger.Error($"Error de validacion EF: {msg}");
                    throw new CrudOperationException($"Error de validacion: {msg}", "Create", alumno);
                }

                // Capturar error SQL real (inner exception mas profunda)
                var baseEx = ex.GetBaseException();
                string detalle = baseEx?.Message ?? ex.Message;

                // Loggear el SQL que fallo si hay info
                if (ex is System.Data.Entity.Infrastructure.DbUpdateException dbEx)
                {
                    Logger.Error($"DbUpdateException. Inner: {dbEx.InnerException?.Message}");
                    Logger.Error($"BaseException: {baseEx?.Message}");
                }

                throw new CrudOperationException($"Error BD: {detalle}", "Create", alumno);
            }
        }

        public List<Alumno> Read()
        {
            try
            {
                using (var db = new ModeloDualContext())
                {
                    return db.Alumnos
                        .Where(a => a.Is_Deleted == null || a.Is_Deleted == 0)
                        .OrderBy(a => a.Apellido_Paterno)
                        .ThenBy(a => a.Nombre)
                        .AsNoTracking()
                        .ToList()
                        .Select(a => MapToAlumno(a))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error al leer alumnos", ex);
                throw new CrudOperationException("Ocurrio un error al obtener la lista de alumnos.", "Read", null);
            }
        }

        public Alumno ReadById(int idAlumno)
        {
            try
            {
                using (var db = new ModeloDualContext())
                {
                    var entity = db.Alumnos
                        .AsNoTracking()
                        .FirstOrDefault(a => a.Id_Alumno == idAlumno
                                          && (a.Is_Deleted == null || a.Is_Deleted == 0));

                    return entity != null ? MapToAlumno(entity) : null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al leer alumno ID: {idAlumno}", ex);
                throw new CrudOperationException("Ocurrio un error al buscar el alumno.", "ReadById", null);
            }
        }

        public bool Update(Alumno alumno)
        {
            try
            {
                using (var db = new ModeloDualContext())
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = db.Alumnos.Find(alumno.Id_Alumno);
                        if (entity == null || (entity.Is_Deleted != null && entity.Is_Deleted != 0))
                            return false;

                        // Actualizar propiedades (normalizar mayusculas)
                        entity.No_Control = alumno.No_Control?.ToUpperInvariant();
                        entity.Nombre = alumno.Nombre;
                        entity.Apellido_Paterno = alumno.Apellido_Paterno;
                        entity.Apellido_Materno = alumno.Apellido_Materno;
                        entity.Email = alumno.Email;
                        entity.Telefono = alumno.Telefono;
                        entity.Fecha_Nacimiento = alumno.Fecha_Nacimiento;
                        entity.Status_Alumno = alumno.Status_Alumno;
                        entity.Curp = alumno.Curp?.ToUpperInvariant();
                        entity.Rfc = alumno.Rfc?.ToUpperInvariant();
                        entity.Nss = alumno.Nss;
                        entity.Genero = alumno.Genero;
                        entity.Estado_Civil = alumno.Estado_Civil;
                        entity.Nacionalidad = alumno.Nacionalidad;
                        entity.Direccion_Calle = alumno.Direccion_Calle;
                        entity.Direccion_Numero = alumno.Direccion_Numero;
                        entity.Direccion_Colonia = alumno.Direccion_Colonia;
                        entity.Direccion_Ciudad = alumno.Direccion_Ciudad;
                        entity.Direccion_Estado = alumno.Direccion_Estado;
                        entity.Direccion_Cp = alumno.Direccion_Cp;
                        entity.Telefono_Fijo = alumno.Telefono_Fijo;
                        entity.Contacto_Emergencia_Nombre = alumno.Contacto_Emergencia_Nombre;
                        entity.Contacto_Emergencia_Telefono = alumno.Contacto_Emergencia_Telefono;
                        entity.Contacto_Emergencia_Parentesco = alumno.Contacto_Emergencia_Parentesco;
                        entity.Id_Carrera = alumno.Id_Carrera;
                        entity.Semestre = alumno.Semestre;
                        entity.Grupo = alumno.Grupo;
                        entity.Turno = alumno.Turno;
                        entity.Fecha_Ingreso = alumno.Fecha_Ingreso;
                        entity.Fecha_Egreso = alumno.Fecha_Egreso;
                        entity.Fecha_Baja = alumno.Fecha_Baja;
                        entity.Motivo_Baja = alumno.Motivo_Baja;
                        entity.Promedio_General = alumno.Promedio_General;
                        entity.No_Control_Unico = alumno.No_Control?.ToUpperInvariant();
                        entity.Updated_At = DateTime.Now;

                        entity.Updated_By = ObtenerUsuarioAuditoria();

                        // Verificar duplicados (excluyendo el registro actual)
                        if (db.Alumnos.Any(a => a.No_Control == entity.No_Control && a.Id_Alumno != entity.Id_Alumno))
                            throw new CrudOperationException(MensajesAlumno.NoControlExiste, "Update", alumno);

                        if (!string.IsNullOrEmpty(entity.Curp) && db.Alumnos.Any(a => a.Curp == entity.Curp && a.Id_Alumno != entity.Id_Alumno))
                            throw new CrudOperationException(MensajesAlumno.CurpExiste, "Update", alumno);

                        if (!string.IsNullOrEmpty(entity.Email) && db.Alumnos.Any(a => a.Email == entity.Email && a.Id_Alumno != entity.Id_Alumno))
                            throw new CrudOperationException(MensajesAlumno.EmailExiste, "Update", alumno);

                        db.SaveChanges();
                        tx.Commit();

                        Logger.Info($"Alumno actualizado: ID={alumno.Id_Alumno}");
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al actualizar alumno ID={alumno.Id_Alumno}: {ex.Message}", ex);
                throw new CrudOperationException("Ocurrio un error al actualizar el alumno.", "Update", alumno);
            }
        }

        public bool Delete(int idAlumno, string deletedReason)
        {
            try
            {
                using (var db = new ModeloDualContext())
                using (var tx = db.Database.BeginTransaction())
                {
                    try
                    {
                        var entity = db.Alumnos.Find(idAlumno);
                        if (entity == null || (entity.Is_Deleted != null && entity.Is_Deleted != 0))
                            return false;

                        entity.Is_Deleted = 1;
                        entity.Deleted_At = DateTime.Now;
                        entity.Deleted_By = ObtenerUsuarioAuditoria();
                        entity.Deleted_Reason = deletedReason;
                        entity.Status_Alumno = "baja";

                        db.SaveChanges();
                        tx.Commit();

                        Logger.Info($"Alumno eliminado (soft): ID={idAlumno}, motivo={deletedReason}");
                        return true;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error al eliminar alumno ID={idAlumno}: {ex.Message}", ex);
                throw new CrudOperationException("Ocurrio un error al eliminar el alumno.", "Delete", null);
            }
        }

        // ================ HELPERS ================

        private static Alumno MapToAlumno(AlumnoEF entity)
        {
            return new Alumno
            {
                Id_Alumno = entity.Id_Alumno,
                No_Control = entity.No_Control,
                Nombre = entity.Nombre,
                Apellido_Paterno = entity.Apellido_Paterno,
                Apellido_Materno = entity.Apellido_Materno,
                Email = entity.Email,
                Telefono = entity.Telefono,
                Fecha_Nacimiento = entity.Fecha_Nacimiento,
                Status_Alumno = entity.Status_Alumno,
                Curp = entity.Curp,
                Rfc = entity.Rfc,
                Nss = entity.Nss,
                Genero = entity.Genero,
                Estado_Civil = entity.Estado_Civil,
                Nacionalidad = entity.Nacionalidad,
                Direccion_Calle = entity.Direccion_Calle,
                Direccion_Numero = entity.Direccion_Numero,
                Direccion_Colonia = entity.Direccion_Colonia,
                Direccion_Ciudad = entity.Direccion_Ciudad,
                Direccion_Estado = entity.Direccion_Estado,
                Direccion_Cp = entity.Direccion_Cp,
                Telefono_Fijo = entity.Telefono_Fijo,
                Contacto_Emergencia_Nombre = entity.Contacto_Emergencia_Nombre,
                Contacto_Emergencia_Telefono = entity.Contacto_Emergencia_Telefono,
                Contacto_Emergencia_Parentesco = entity.Contacto_Emergencia_Parentesco,
                Id_Carrera = entity.Id_Carrera,
                Semestre = entity.Semestre,
                Grupo = entity.Grupo,
                Turno = entity.Turno,
                Fecha_Ingreso = entity.Fecha_Ingreso,
                Fecha_Egreso = entity.Fecha_Egreso,
                Fecha_Baja = entity.Fecha_Baja,
                Motivo_Baja = entity.Motivo_Baja,
                Promedio_General = entity.Promedio_General,
                Created_By = entity.Created_By,
                // La bitacora y sincronizacion de email se manejan via triggers
            };
        }

        private static int? ObtenerUsuarioAuditoria()
        {
            try
            {
                return SesionActiva.Instance?.Id_Usuario;
            }
            catch
            {
                return null;
            }
        }
    }
}
