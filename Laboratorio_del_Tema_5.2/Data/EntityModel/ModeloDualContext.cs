using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio_del_Tema_5_2.Data.EntityModel
{
    /// <summary>
    /// DbContext de Entity Framework 6 para ModeloDualDB_SQL (SQL Server).
    /// </summary>
    public class ModeloDualContext : DbContext
    {
        public ModeloDualContext() : base("name=ModeloDualDB_EF")
        {
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        public DbSet<AlumnoEF> Alumnos { get; set; }
        public DbSet<UsuarioEF> Usuarios { get; set; }
        public DbSet<RolEF> Roles { get; set; }
        public DbSet<PrivilegioEF> Privilegios { get; set; }
        public DbSet<CarreraEF> Carreras { get; set; }
        public DbSet<MateriaEF> Materias { get; set; }
        public DbSet<ProfesorEF> Profesores { get; set; }
        public DbSet<EmpresaEF> Empresas { get; set; }
        public DbSet<ProyectoEF> Proyectos { get; set; }
        public DbSet<TemaEF> Temas { get; set; }
        public DbSet<SesionEF> Sesiones { get; set; }
        public DbSet<RolPrivilegioEF> RolesPrivilegios { get; set; }
        public DbSet<UsuarioAlumnoEF> UsuariosAlumnos { get; set; }
        public DbSet<UsuarioProfesorEF> UsuariosProfesores { get; set; }
        public DbSet<UsuarioEmpresaEF> UsuariosEmpresas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");



            base.OnModelCreating(modelBuilder);
        }
    }

    // ==========================================
    // ENTIDADES
    // ==========================================

    [Table("alumno")]
    public class AlumnoEF
    {
        [Key]
        [Column("id_alumno")]
        public int Id_Alumno { get; set; }

        [Required]
        [StringLength(15)]
        [Column("no_control")]
        public string No_Control { get; set; }

        [StringLength(18)]
        [Column("curp")]
        public string Curp { get; set; }

        [StringLength(13)]
        [Column("rfc")]
        public string Rfc { get; set; }

        [StringLength(20)]
        [Column("nss")]
        public string Nss { get; set; }

        [StringLength(30)]
        [Column("genero")]
        public string Genero { get; set; }

        [StringLength(30)]
        [Column("estado_civil")]
        public string Estado_Civil { get; set; }

        [StringLength(50)]
        [Column("nacionalidad")]
        public string Nacionalidad { get; set; }

        [StringLength(100)]
        [Column("direccion_calle")]
        public string Direccion_Calle { get; set; }

        [StringLength(20)]
        [Column("direccion_numero")]
        public string Direccion_Numero { get; set; }

        [StringLength(100)]
        [Column("direccion_colonia")]
        public string Direccion_Colonia { get; set; }

        [StringLength(100)]
        [Column("direccion_ciudad")]
        public string Direccion_Ciudad { get; set; }

        [StringLength(100)]
        [Column("direccion_estado")]
        public string Direccion_Estado { get; set; }

        [StringLength(10)]
        [Column("direccion_cp")]
        public string Direccion_Cp { get; set; }

        [StringLength(15)]
        [Column("telefono_fijo")]
        public string Telefono_Fijo { get; set; }

        [StringLength(100)]
        [Column("contacto_emergencia_nombre")]
        public string Contacto_Emergencia_Nombre { get; set; }

        [StringLength(15)]
        [Column("contacto_emergencia_telefono")]
        public string Contacto_Emergencia_Telefono { get; set; }

        [StringLength(50)]
        [Column("contacto_emergencia_parentesco")]
        public string Contacto_Emergencia_Parentesco { get; set; }

        [Column("id_carrera")]
        public int? Id_Carrera { get; set; }

        [Column("semestre")]
        public int? Semestre { get; set; }

        [StringLength(20)]
        [Column("grupo")]
        public string Grupo { get; set; }

        [StringLength(20)]
        [Column("turno")]
        public string Turno { get; set; }

        [Column("fecha_ingreso", TypeName = "date")]
        public DateTime? Fecha_Ingreso { get; set; }

        [Column("fecha_egreso", TypeName = "date")]
        public DateTime? Fecha_Egreso { get; set; }

        [Column("fecha_baja", TypeName = "date")]
        public DateTime? Fecha_Baja { get; set; }

        [StringLength(255)]
        [Column("motivo_baja")]
        public string Motivo_Baja { get; set; }

        [Column("promedio_general")]
        public decimal? Promedio_General { get; set; }

        [Column("created_by")]
        public int? Created_By { get; set; }

        [Column("updated_by")]
        public int? Updated_By { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(80)]
        [Column("apellido_paterno")]
        public string Apellido_Paterno { get; set; }

        [StringLength(80)]
        [Column("apellido_materno")]
        public string Apellido_Materno { get; set; }

        [StringLength(120)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(15)]
        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("fecha_nacimiento", TypeName = "date")]
        public DateTime? Fecha_Nacimiento { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status_alumno")]
        public string Status_Alumno { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("is_deleted")]
        public short? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [Column("deleted_by")]
        public int? Deleted_By { get; set; }

        [StringLength(255)]
        [Column("deleted_reason")]
        public string Deleted_Reason { get; set; }

        [StringLength(255)]
        [Column("status_change_reason")]
        public string Status_Change_Reason { get; set; }

        [StringLength(15)]
        [Column("no_control_unico")]
        public string No_Control_Unico { get; set; }

        [ForeignKey("Id_Carrera")]
        public virtual CarreraEF Carrera { get; set; }
    }

    [Table("usuario")]
    public class UsuarioEF
    {
        [Key]
        [Column("id_usuario")]
        public int Id_Usuario { get; set; }

        [Required]
        [StringLength(50)]
        [Column("username")]
        public string Username { get; set; }

        [Required]
        [StringLength(254)]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("id_rol")]
        public int Id_Rol { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; }

        [Column("ultimo_login")]
        public DateTime? Ultimo_Login { get; set; }

        [Column("intentos_fallidos")]
        public int? Intentos_Fallidos { get; set; }

        [Column("bloqueado_hasta")]
        public DateTime? Bloqueado_Hasta { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Column("is_deleted")]
        public bool? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [StringLength(254)]
        [Column("deleted_by")]
        public string Deleted_By { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("debe_cambiar_password")]
        public short? Debe_Cambiar_Password { get; set; }

        [StringLength(255)]
        [Column("password_temporal_hash")]
        public string Password_Temporal_Hash { get; set; }

        [Column("fecha_activacion")]
        public DateTime? Fecha_Activacion { get; set; }

        [Column("creado_por")]
        public int? Creado_Por { get; set; }

        [ForeignKey("Id_Rol")]
        public virtual RolEF Rol { get; set; }
    }

    [Table("rol")]
    public class RolEF
    {
        [Key]
        [Column("id_rol")]
        public int Id_Rol { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(200)]
        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        public virtual ICollection<UsuarioEF> Usuarios { get; set; }
    }

    [Table("privilegio")]
    public class PrivilegioEF
    {
        [Key]
        [Column("id_privilegio")]
        public int Id_Privilegio { get; set; }

        [Required]
        [StringLength(80)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(200)]
        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }
    }

    [Table("carrera")]
    public class CarreraEF
    {
        [Key]
        [Column("id_carrera")]
        public int Id_Carrera { get; set; }

        [Required]
        [StringLength(20)]
        [Column("clave")]
        public string Clave { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("duracion_semestres")]
        public int Duracion_Semestres { get; set; }

        [Required]
        [StringLength(10)]
        [Column("status")]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        public virtual ICollection<AlumnoEF> Alumnos { get; set; }
    }

    [Table("materia")]
    public class MateriaEF
    {
        [Key]
        [Column("id_materia")]
        public int Id_Materia { get; set; }

        [Required]
        [StringLength(10)]
        [Column("clave_materia")]
        public string Clave_Materia { get; set; }

        [Required]
        [StringLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion", TypeName = "ntext")]
        public string Descripcion { get; set; }

        [Column("creditos")]
        public int? Creditos { get; set; }

        [Column("semestre")]
        public int? Semestre { get; set; }

        [Column("horas_teoria")]
        public int? Horas_Teoria { get; set; }

        [Column("horas_practica")]
        public int? Horas_Practica { get; set; }

        [Required]
        [StringLength(10)]
        [Column("status_materia")]
        public string Status_Materia { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("is_deleted")]
        public short? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [StringLength(120)]
        [Column("deleted_by")]
        public string Deleted_By { get; set; }
    }

    [Table("profesor")]
    public class ProfesorEF
    {
        [Key]
        [Column("id_profesor")]
        public int Id_Profesor { get; set; }

        [Required]
        [StringLength(15)]
        [Column("no_empleado")]
        public string No_Empleado { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(80)]
        [Column("apellido_paterno")]
        public string Apellido_Paterno { get; set; }

        [StringLength(80)]
        [Column("apellido_materno")]
        public string Apellido_Materno { get; set; }

        [StringLength(120)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(15)]
        [Column("telefono")]
        public string Telefono { get; set; }

        [StringLength(100)]
        [Column("departamento")]
        public string Departamento { get; set; }

        [StringLength(80)]
        [Column("puesto")]
        public string Puesto { get; set; }

        [Required]
        [StringLength(10)]
        [Column("status_profesor")]
        public string Status_Profesor { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("is_deleted")]
        public short? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [StringLength(120)]
        [Column("deleted_by")]
        public string Deleted_By { get; set; }
    }

    [Table("empresa")]
    public class EmpresaEF
    {
        [Key]
        [Column("id_empresa")]
        public int Id_Empresa { get; set; }

        [Required]
        [StringLength(150)]
        [Column("nombre_comercial")]
        public string Nombre_Comercial { get; set; }

        [StringLength(200)]
        [Column("razon_social")]
        public string Razon_Social { get; set; }

        [StringLength(20)]
        [Column("rfc")]
        public string Rfc { get; set; }

        [StringLength(250)]
        [Column("direccion")]
        public string Direccion { get; set; }

        [StringLength(100)]
        [Column("ciudad")]
        public string Ciudad { get; set; }

        [StringLength(100)]
        [Column("estado")]
        public string Estado { get; set; }

        [StringLength(10)]
        [Column("cp")]
        public string Cp { get; set; }

        [StringLength(15)]
        [Column("telefono_empresa")]
        public string Telefono_Empresa { get; set; }

        [StringLength(120)]
        [Column("email_empresa")]
        public string Email_Empresa { get; set; }

        [StringLength(100)]
        [Column("nombre_contacto")]
        public string Nombre_Contacto { get; set; }

        [StringLength(80)]
        [Column("puesto_contacto")]
        public string Puesto_Contacto { get; set; }

        [Required]
        [StringLength(15)]
        [Column("status_empresa")]
        public string Status_Empresa { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("is_deleted")]
        public short? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [StringLength(120)]
        [Column("deleted_by")]
        public string Deleted_By { get; set; }
    }

    [Table("proyecto")]
    public class ProyectoEF
    {
        [Key]
        [Column("id_proyecto")]
        public int Id_Proyecto { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre_proyecto")]
        public string Nombre_Proyecto { get; set; }

        [Column("descripcion", TypeName = "ntext")]
        public string Descripcion { get; set; }

        [Column("objetivos", TypeName = "ntext")]
        public string Objetivos { get; set; }

        [Column("fecha_inicio", TypeName = "date")]
        public DateTime? Fecha_Inicio { get; set; }

        [Column("fecha_fin", TypeName = "date")]
        public DateTime? Fecha_Fin { get; set; }

        [Column("horas_totales")]
        public int? Horas_Totales { get; set; }

        [Required]
        [StringLength(20)]
        [Column("status_proyecto")]
        public string Status_Proyecto { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("is_deleted")]
        public short? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [StringLength(120)]
        [Column("deleted_by")]
        public string Deleted_By { get; set; }
    }

    [Table("tema")]
    public class TemaEF
    {
        [Key]
        [Column("id_tema")]
        public int Id_Tema { get; set; }

        [Column("id_materia")]
        public int Id_Materia { get; set; }

        [Column("numero_tema")]
        public int Numero_Tema { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion", TypeName = "ntext")]
        public string Descripcion { get; set; }

        [Column("horas_estimadas")]
        public decimal? Horas_Estimadas { get; set; }

        [Required]
        [StringLength(10)]
        [Column("status_tema")]
        public string Status_Tema { get; set; }

        [Column("created_at")]
        public DateTime? Created_At { get; set; }

        [Required]
        [Column("updated_at")]
        public DateTime Updated_At { get; set; }

        [Column("is_deleted")]
        public short? Is_Deleted { get; set; }

        [Column("deleted_at")]
        public DateTime? Deleted_At { get; set; }

        [StringLength(120)]
        [Column("deleted_by")]
        public string Deleted_By { get; set; }

        [ForeignKey("Id_Materia")]
        public virtual MateriaEF Materia { get; set; }
    }


    [Table("sesion")]
    public class SesionEF
    {
        [Key]
        [StringLength(64)]
        [Column("id_sesion")]
        public string Id_Sesion { get; set; }

        [Column("id_usuario")]
        public int Id_Usuario { get; set; }

        [Column("fecha_inicio")]
        public DateTime Fecha_Inicio { get; set; }

        [Column("fecha_expiracion")]
        public DateTime Fecha_Expiracion { get; set; }

        [StringLength(45)]
        [Column("ip_address")]
        public string Ip_Address { get; set; }

        [StringLength(255)]
        [Column("user_agent")]
        public string User_Agent { get; set; }

        [Required]
        [StringLength(10)]
        [Column("status")]
        public string Status { get; set; }

        [ForeignKey("Id_Usuario")]
        public virtual UsuarioEF Usuario { get; set; }
    }

    [Table("rol_privilegio")]
    public class RolPrivilegioEF
    {
        [Key, Column("id_rol", Order = 0)]
        public int Id_Rol { get; set; }

        [Key, Column("id_privilegio", Order = 1)]
        public int Id_Privilegio { get; set; }

        [ForeignKey("Id_Rol")]
        public virtual RolEF Rol { get; set; }

        [ForeignKey("Id_Privilegio")]
        public virtual PrivilegioEF Privilegio { get; set; }
    }

    [Table("usuario_alumno")]
    public class UsuarioAlumnoEF
    {
        [Key, Column("id_usuario", Order = 0)]
        public int Id_Usuario { get; set; }

        [Key, Column("id_alumno", Order = 1)]
        public int Id_Alumno { get; set; }
    }

    [Table("usuario_profesor")]
    public class UsuarioProfesorEF
    {
        [Key, Column("id_usuario", Order = 0)]
        public int Id_Usuario { get; set; }

        [Key, Column("id_profesor", Order = 1)]
        public int Id_Profesor { get; set; }
    }

    [Table("usuario_empresa")]
    public class UsuarioEmpresaEF
    {
        [Key, Column("id_usuario", Order = 0)]
        public int Id_Usuario { get; set; }

        [Key, Column("id_empresa", Order = 1)]
        public int Id_Empresa { get; set; }
    }

}
