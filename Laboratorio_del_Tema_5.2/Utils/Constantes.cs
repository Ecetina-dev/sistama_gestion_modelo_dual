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
        // Status de Carrera
        // ============================================
        public const string CarreraActiva = "activa";
        public const string CarreraInactiva = "inactiva";

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

    // ============================================
    // Genero
    // ============================================
    public static class Genero
    {
        public const string Masculino = "Masculino";
        public const string Femenino = "Femenino";
        public const string NoBinario = "No binario";
        public const string PrefieroNoDecir = "Prefiero no decir";
    }

    // ============================================
    // Turno
    // ============================================
    public static class Turno
    {
        public const string Matutino = "matutino";
        public const string Vespertino = "vespertino";
        public const string Nocturno = "nocturno";
        public const string Mixto = "mixto";
    }

    // ============================================
    // Tipos de documento de alumno
    // ============================================
    public static class DocumentoTipo
    {
        public const string Ine = "INE";
        public const string CurpPdf = "CURP_PDF";
        public const string ActaNacimiento = "Acta_Nacimiento";
        public const string ComprobanteDomicilio = "Comprobante_Domicilio";
        public const string CertificadoPreparatoria = "Certificado_Preparatoria";
        public const string ConstanciaEstudios = "Constancia_Estudios";
        public const string Fotografia = "Fotografia";
    }

    // ============================================
    // Motivos de baja / cambio de estado
    // ============================================
    public static class MotivoBaja
    {
        public const string SolicitudAlumno = "Solicitud del alumno";
        public const string BajaAcademica = "Baja academica";
        public const string BajaAdministrativa = "Baja administrativa";
        public const string Reingreso = "Reingreso";
    }

    // ============================================
    // Privilegios relacionados con alumnos
    // ============================================
    public static class Privilegio
    {
        public const string AdminCrudTodo = "admin.crud_todo";
        public const string AlumnoCrud = "profesor.crud_alumno";
        public const string AlumnoVerPropio = "alumno.ver_propio";
        public const string AlumnoEditarPropio = "alumno.editar_propio";
        public const string AlumnoVerProyectos = "alumno.ver_proyectos";
        public const string ProfesorCrudAlumno = "profesor.crud_alumno";
    }

    // ============================================
    // Configuracion de validaciones de alumno
    // ============================================
    public static class AlumnoConfig
    {
        public const int NoControlMinLength = 10;
        public const int NoControlMaxLength = 10;
        public const int CurpLength = 18;
        public const int RfcLength = 13;
        public const int TelefonoLength = 10;
        public const int CodigoPostalLength = 5;
        public const int EdadMinAlumno = 15;
        public const int EdadMaxAlumno = 100;

        // CURP mexicano: 18 caracteres alfanumericos.
        // Posiciones 0-3: letras (primer apellido, vocal interno, segundo apellido, nombre)
        // Posiciones 4-9: fecha nacimiento AAMMDD
        // Posicion 10: H/M (sexo)
        // Posiciones 11-12: codigo de estado (32 estados + XX para nacidos en el extranjero)
        // Posiciones 13-15: consonantes internas
        // Posiciones 16-17: digitos (consulta inicial + verificador)
        public const string CurpPattern = @"^[A-Z]{4}[0-9]{6}[HM]{1}[A-Z]{6}[0-9]{2}$";
        public const string RfcPattern = "^[A-ZÑ&]{4}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[A-Z0-9]{3}$";
        public const string EmailPattern = "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$";
        public const string TelefonoPattern = "^[0-9]{10}$";
        public const string NoControlPattern = "^[A-Z0-9]{10}$";
        public const string CodigoPostalPattern = "^[0-9]{5}$";
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

    /// <summary>
    /// Mensajes de validacion del modulo Alumno.
    /// </summary>
    public static class MensajesAlumno
    {
        public const string CurpInvalido = "El CURP no es valido.";
        public const string CurpLongitudInvalida = "El CURP debe tener 18 caracteres.";
        public const string CurpFormatoInvalido = "El CURP no tiene el formato correcto (ej: MMLC800101HNENSNS00).";
        public const string CurpChecksumInvalido = "El CURP tiene un digito verificador incorrecto.";
        public const string RfcInvalido = "El RFC no es valido.";
        public const string NoControlExiste = "El numero de control ya esta registrado.";
        public const string NoControlReservado = "El numero de control ya esta registrado (incluyendo registros eliminados).";
        public const string CurpExiste = "El CURP ya esta registrado.";
        public const string CurpReservado = "El CURP ya esta registrado (incluyendo registros eliminados).";
        public const string EmailExiste = "El email ya esta registrado.";
        public const string EmailReservado = "El email ya esta registrado (incluyendo registros eliminados).";
        public const string EmailInvalido = "El formato del email no es valido.";
        public const string TransicionStatusNoPermitida = "La transicion de estado no esta permitida.";
        public const string MotivoBajaRequerido = "El motivo de baja es requerido para dar de baja un alumno.";
        public const string MotivoEliminacionRequerido = "El motivo de eliminacion es requerido.";
        public const string AlumnoConAsignaciones = "No se puede eliminar el alumno porque tiene asignaciones activas.";
        public const string NoControlInvalido = "El numero de control debe tener exactamente 10 caracteres alfanumericos mayusculas/numeros.";
        public const string TelefonoInvalido = "El telefono debe contener 10 digitos.";
        public const string SemestreInvalido = "El semestre no es valido para la carrera.";
    }
}