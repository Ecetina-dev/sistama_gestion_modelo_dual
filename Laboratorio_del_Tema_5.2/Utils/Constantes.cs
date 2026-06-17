using System;
using System.Windows.Forms;

namespace Laboratorio_del_Tema_5_2.Utils
{
    /// <summary>
    /// Constantes de estado utilizadas en la aplicacion.
    /// </summary>
    public static class Estatus
    {
        // ============================================
        // Status de Empresa
        // ============================================
        public const string EmpresaActiva = "activa";
        public const string EmpresaInactiva = "inactiva";
        public const string EmpresaPropuesta = "propuesta";

        // ============================================
        // Status de Alumno
        // ============================================
        public const string AlumnoActivo = "activo";
        public const string AlumnoInactivo = "inactivo";
        public const string AlumnoEgresado = "egresado";
        public const string AlumnoSuspendido = "suspendido";
        public const string AlumnoBaja = "baja";

        // ============================================
        // Status de Profesor
        // ============================================
        public const string ProfesorActivo = "activo";
        public const string ProfesorInactivo = "inactivo";

        // ============================================
        // Status de Materia
        // ============================================
        public const string MateriaActiva = "activa";
        public const string MateriaInactiva = "inactiva";

        // ============================================
        // Status de Asignacion (Alumno-Empresa)
        // ============================================
        public const string AsignacionActiva = "activa";
        public const string AsignacionFinalizada = "finalizada";
        public const string AsignacionCancelada = "cancelada";

        // ============================================
        // Status de Proyecto
        // ============================================
        public const string ProyectoActivo = "activo";
        public const string ProyectoCompletado = "completado";
        public const string ProyectoCancelado = "cancelado";
        public const string ProyectoEnRevision = "en_revision";
        public const string ProyectoPropuesto = "propuesto";

        // ============================================
        // Status de Tema
        // ============================================
        public const string TemaActivo = "activo";
        public const string TemaDisponible = "disponible";
        public const string TemaAsignado = "asignado";
        public const string TemaCompletado = "completado";
    }

    /// <summary>
    /// Mensajes de la aplicacion.
    /// </summary>
    public static class Mensajes
    {
        public const string RegistroGuardado = "Registro guardado correctamente.";
        public const string RegistroEliminado = "Registro eliminado correctamente.";
        public const string ErrorGuardar = "Error al guardar el registro.";
        public const string ErrorEliminar = "Error al eliminar el registro.";
        public const string SeleccionarRegistro = "Seleccione un registro.";
        public const string ConfirmarEliminacion = "Esta seguro de eliminar este registro?";
        public const string CampoRequerido = "Este campo es requerido.";
        public const string FormatoInvalido = "El formato es invalido.";
    }

    /// <summary>
    /// Configuracion de la aplicacion.
    /// </summary>
    public static class Config
    {
        public const string NombreApp = "Sistema Gestion Modelo Dual";
        public const int TimeoutConexion = 30;
        public const string FormatoFecha = "yyyy-MM-dd";
        public const string FormatoFechaHora = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Pide confirmacion al usuario.
        /// </summary>
        public static bool Confirmar(string mensaje, string titulo = "Confirmar")
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }

    /// <summary>
    /// Constantes de seguridad y autenticacion.
    /// Centralizadas para facil mantenimiento.
    /// 
    /// IMPORTANTE: Estos valores son fallbacks por defecto.
    /// Los valores reales se obtienen de la tabla parametro_sistema
    /// a traves de ParametroSistemaService.Instance cuando la BD esta disponible.
    /// </summary>
    public static class Seguridad
    {
        // ============================================
        // Roles del sistema
        // ============================================
        public const string RolAdmin = "admin";
        public const string RolAlumno = "alumno";
        public const string RolProfesor = "profesor";
        public const string RolEmpresa = "empresa";

        // ============================================
        // Tipos de entidad
        // ============================================
        public const string TipoAlumno = "alumno";
        public const string TipoProfesor = "profesor";
        public const string TipoEmpresa = "empresa";
        public const string TipoSinVincular = "none";

        // ============================================
        // Limites de seguridad (fallbacks - DB manda)
        // ============================================
        public const int MaxIntentosLogin = 5;
        public const int MinutosBloqueo = 1;
        public const int BcryptCostFactor = 11;
        public const int DuracionSesionHoras = 8;

        // ============================================
        // Validaciones de input (fallbacks - DB manda)
        // ============================================
        public const int UsernameMinLength = 3;
        public const int UsernameMaxLength = 50;
        public const int EmailMaxLength = 254;       // RFC 5321
        public const int PasswordMinLength = 8;       // NIST SP 800-63B
        public const int PasswordMaxLength = 128;     // OWASP ASVS 4.0
        public const int LoginMaxLength = 254;        // username o email (email max RFC 5321)

        // ============================================
        // Password requirements (fallbacks)
        // ============================================
        public const bool ExigirCaracterEspecialPassword = true;
        public const int DiasCaducidadPassword = 90;

        // ============================================
        // Mensajes de error genericos (no leak info)
        // ============================================
        public const string MsgCredencialesInvalidas = "Usuario o contrasena incorrectos";
        public const string MsgErrorConexion = "No se pudo conectar al servidor. Verifica tu conexion.";
        public const string MsgErrorGenerico = "Ocurrio un error inesperado. Intenta de nuevo.";
        public const string MsgCuentaBloqueada = "Demasiados intentos fallidos. Cuenta bloqueada temporalmente.";
        public const string MsgCuentaInactiva = "Cuenta inactiva. Contacta al administrador.";
        public const string MsgSesionExpirada = "Tu sesion ha expirado. Inicia sesion de nuevo.";

        // ============================================
        // Mensajes de validacion
        // ============================================
        public const string MsgUsernameRequerido = "El usuario es requerido";
        public const string MsgUsernameLongitud = "Entre 3 y 50 caracteres";
        public const string MsgUsernameFormato = "Solo letras, numeros, guion y guion bajo";
        public const string MsgUsernameExiste = "El usuario ya existe";
        public const string MsgEmailRequerido = "El email es requerido";
        public const string MsgEmailFormato = "Formato de email invalido";
        public const string MsgEmailExiste = "El email ya esta registrado";
        public const string MsgPasswordRequerido = "La contrasena es requerida";
        public const string MsgPasswordLongitud = "Minimo 8 caracteres";
        public const string MsgPasswordCoincide = "Las contrasenas no coinciden";
    }
}