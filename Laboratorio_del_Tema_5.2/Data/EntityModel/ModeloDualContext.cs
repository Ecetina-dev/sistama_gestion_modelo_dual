using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio_del_Tema_5_2.Data.EntityModel
{
    /// <summary>
    /// DbContext de Entity Framework 6 para ModeloDualDB.
    /// 
    /// ⚠️ ACTUALMENTE EN MODO "DATABASE FIRST" (apunta a MySQL).
    /// Cuando migres a SQL Server, solo cambía la connection string en App.config:
    ///   <add name="ModeloDualDB_EF"
    ///        connectionString="Server=localhost;Database=ModeloDualDB_SQL;Integrated Security=True;"
    ///        providerName="System.Data.SqlClient" />
    ///
    /// 📌 Si preferís usar el diseñador visual (EDMX Designer):
    ///   1. Abrí el proyecto en Visual Studio
    ///   2. Botón derecho en "Data/EntityModel/" → Add → New Item
    ///   3. Elegí "ADO.NET Entity Data Model" → "EF Designer from database"
    ///   4. Seleccioná la conexión "ModeloDualDB_EF"
    ///   5. Elegí las tablas → Finish
    ///   6. Se genera ModeloDualModel.edmx + .tt templates con las clases
    ///   7. Borrá este DbContext manual y usá el generado
    /// </summary>
    public class ModeloDualContext : DbContext
    {
        // Usar la connection string "ModeloDualDB_EF" de App.config
        public ModeloDualContext() : base("name=ModeloDualDB_EF")
        {
            // Configurar lazy loading y proxy creation
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        // DbSets (entidades)
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configurar el nombre del esquema por defecto
            // (MySQL no usa dbo, pero EF lo requiere)
            modelBuilder.HasDefaultSchema("dbo");

            // Mapeos específicos para tablas existentes
            modelBuilder.Entity<AlumnoEF>().ToTable("Alumno");

            base.OnModelCreating(modelBuilder);
        }
    }

    // ==========================================
    // ENTIDADES (parciales - compatibles con EDMX)
    // ==========================================

    [Table("Alumno")]
    public class AlumnoEF
    {
        [Key]
        [Column("id_alumno")]
        public int Id_Alumno { get; set; }

        [Required]
        [StringLength(15)]
        [Column("no_control")]
        public string No_Control { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        [Column("apellido_paterno")]
        public string Apellido_Paterno { get; set; }

        [StringLength(100)]
        [Column("apellido_materno")]
        public string Apellido_Materno { get; set; }

        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(15)]
        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("fecha_nacimiento")]
        public DateTime? Fecha_Nacimiento { get; set; }

        [StringLength(20)]
        [Column("status_alumno")]
        public string Status_Alumno { get; set; }

        [StringLength(18)]
        [Column("curp")]
        public string Curp { get; set; }

        [Column("id_carrera")]
        public int? Id_Carrera { get; set; }

        [Column("semestre")]
        public int? Semestre { get; set; }

        [Column("created_at")]
        public DateTime Created_At { get; set; }

        [Column("updated_at")]
        public DateTime Updated_At { get; set; }
    }

    [Table("Usuario")]
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
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [StringLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; }

        [Column("id_rol")]
        public int Id_Rol { get; set; }

        [StringLength(20)]
        [Column("status")]
        public string Status { get; set; }

        [Column("created_at")]
        public DateTime Created_At { get; set; }

        [ForeignKey("Id_Rol")]
        public virtual RolEF Rol { get; set; }
    }

    [Table("Rol")]
    public class RolEF
    {
        [Key]
        [Column("id_rol")]
        public int Id_Rol { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(255)]
        [Column("descripcion")]
        public string Descripcion { get; set; }
    }

    [Table("Privilegio")]
    public class PrivilegioEF
    {
        [Key]
        [Column("id_privilegio")]
        public int Id_Privilegio { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(255)]
        [Column("descripcion")]
        public string Descripcion { get; set; }
    }

    [Table("Carrera")]
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
    }

    [Table("Materia")]
    public class MateriaEF
    {
        [Key]
        [Column("id_materia")]
        public int Id_Materia { get; set; }

        [Required]
        [StringLength(20)]
        [Column("clave_materia")]
        public string Clave_Materia { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("creditos")]
        public int Creditos { get; set; }

        [Column("semestre")]
        public int Semestre { get; set; }
    }

    [Table("Profesor")]
    public class ProfesorEF
    {
        [Key]
        [Column("id_profesor")]
        public int Id_Profesor { get; set; }

        [Required]
        [StringLength(20)]
        [Column("no_empleado")]
        public string No_Empleado { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [StringLength(100)]
        [Column("apellido_paterno")]
        public string Apellido_Paterno { get; set; }
    }

    [Table("Empresa")]
    public class EmpresaEF
    {
        [Key]
        [Column("id_empresa")]
        public int Id_Empresa { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre_comercial")]
        public string Nombre_Comercial { get; set; }

        [StringLength(13)]
        [Column("rfc")]
        public string RFC { get; set; }
    }

    [Table("Proyecto")]
    public class ProyectoEF
    {
        [Key]
        [Column("id_proyecto")]
        public int Id_Proyecto { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre_proyecto")]
        public string Nombre { get; set; }

        [Column("status_proyecto")]
        public string Status { get; set; }
    }

    [Table("Tema")]
    public class TemaEF
    {
        [Key]
        [Column("id_tema")]
        public int Id_Tema { get; set; }

        [Column("id_materia")]
        public int Id_Materia { get; set; }

        [Required]
        [StringLength(200)]
        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("numero_tema")]
        public int Numero_Tema { get; set; }

        [Column("status_tema")]
        public string Status_Tema { get; set; }
    }
}
