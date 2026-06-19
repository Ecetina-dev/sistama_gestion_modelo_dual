-- ============================================================
-- FIX PART 6: Convertir ENUMs restantes + ON UPDATE
-- ============================================================

USE modelodualdb;

-- ============================================================
-- ENUMs restantes → VARCHAR + CHECK
-- ============================================================

-- alumno_empresa.status_asignacion
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'alumno_empresa' AND CONSTRAINT_NAME = 'chk_ae_status') > 0,
    'ALTER TABLE alumno_empresa DROP CHECK chk_ae_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE alumno_empresa MODIFY COLUMN status_asignacion VARCHAR(15) NOT NULL DEFAULT 'activa';
ALTER TABLE alumno_empresa ADD CONSTRAINT chk_ae_status
    CHECK (status_asignacion IN ('activa','finalizada','pausada'));

-- alumno_proyecto.status_participacion
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'alumno_proyecto' AND CONSTRAINT_NAME = 'chk_ap_status') > 0,
    'ALTER TABLE alumno_proyecto DROP CHECK chk_ap_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE alumno_proyecto MODIFY COLUMN status_participacion VARCHAR(15) NOT NULL DEFAULT 'activo';
ALTER TABLE alumno_proyecto ADD CONSTRAINT chk_ap_status
    CHECK (status_participacion IN ('activo','completado','retirado'));

-- bitacora.operacion
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'bitacora' AND CONSTRAINT_NAME = 'chk_bitacora_operacion') > 0,
    'ALTER TABLE bitacora DROP CHECK chk_bitacora_operacion',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE bitacora MODIFY COLUMN operacion VARCHAR(10) NOT NULL;
ALTER TABLE bitacora ADD CONSTRAINT chk_bitacora_operacion
    CHECK (operacion IN ('INSERT','UPDATE','DELETE','RESTORE'));

-- direccion.tipo_direccion
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'direccion' AND CONSTRAINT_NAME = 'chk_direccion_tipo') > 0,
    'ALTER TABLE direccion DROP CHECK chk_direccion_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE direccion MODIFY COLUMN tipo_direccion VARCHAR(15) NOT NULL;
ALTER TABLE direccion ADD CONSTRAINT chk_direccion_tipo
    CHECK (tipo_direccion IN ('fiscal','personal','laboral','sucursal'));

-- documento.tipo_documento
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'documento' AND CONSTRAINT_NAME = 'chk_documento_tipo') > 0,
    'ALTER TABLE documento DROP CHECK chk_documento_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE documento MODIFY COLUMN tipo_documento VARCHAR(30) NOT NULL;
ALTER TABLE documento ADD CONSTRAINT chk_documento_tipo
    CHECK (tipo_documento IN ('contrato','factura','identificacion','constancia','certificado','evidencia','otro'));

-- email.tipo_email
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'email' AND CONSTRAINT_NAME = 'chk_email_tipo') > 0,
    'ALTER TABLE email DROP CHECK chk_email_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE email MODIFY COLUMN tipo_email VARCHAR(15) NOT NULL;
ALTER TABLE email ADD CONSTRAINT chk_email_tipo
    CHECK (tipo_email IN ('personal','laboral','institucional','emergencia'));

-- empresa.status_empresa
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'empresa' AND CONSTRAINT_NAME = 'chk_empresa_status') > 0,
    'ALTER TABLE empresa DROP CHECK chk_empresa_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE empresa MODIFY COLUMN status_empresa VARCHAR(15) NOT NULL DEFAULT 'activa';
ALTER TABLE empresa ADD CONSTRAINT chk_empresa_status
    CHECK (status_empresa IN ('activa','inactiva','pendiente'));

-- materia.status_materia
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'materia' AND CONSTRAINT_NAME = 'chk_materia_status') > 0,
    'ALTER TABLE materia DROP CHECK chk_materia_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE materia MODIFY COLUMN status_materia VARCHAR(10) NOT NULL DEFAULT 'activa';
ALTER TABLE materia ADD CONSTRAINT chk_materia_status
    CHECK (status_materia IN ('activa','inactiva'));

-- notificacion.tipo_notificacion
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'notificacion' AND CONSTRAINT_NAME = 'chk_notif_tipo') > 0,
    'ALTER TABLE notificacion DROP CHECK chk_notif_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE notificacion MODIFY COLUMN tipo_notificacion VARCHAR(10) NOT NULL;
ALTER TABLE notificacion ADD CONSTRAINT chk_notif_tipo
    CHECK (tipo_notificacion IN ('email','sms','push','sistema'));

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'notificacion' AND CONSTRAINT_NAME = 'chk_notif_prioridad') > 0,
    'ALTER TABLE notificacion DROP CHECK chk_notif_prioridad',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE notificacion MODIFY COLUMN prioridad VARCHAR(10) NOT NULL DEFAULT 'normal';
ALTER TABLE notificacion ADD CONSTRAINT chk_notif_prioridad
    CHECK (prioridad IN ('baja','normal','alta','urgente'));

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'notificacion' AND CONSTRAINT_NAME = 'chk_notif_canal') > 0,
    'ALTER TABLE notificacion DROP CHECK chk_notif_canal',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE notificacion MODIFY COLUMN canal_envio VARCHAR(10) NOT NULL DEFAULT 'email';
ALTER TABLE notificacion ADD CONSTRAINT chk_notif_canal
    CHECK (canal_envio IN ('email','sms','push','in_app'));

-- profesor.status_profesor
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'profesor' AND CONSTRAINT_NAME = 'chk_profesor_status') > 0,
    'ALTER TABLE profesor DROP CHECK chk_profesor_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE profesor MODIFY COLUMN status_profesor VARCHAR(10) NOT NULL DEFAULT 'activo';
ALTER TABLE profesor ADD CONSTRAINT chk_profesor_status
    CHECK (status_profesor IN ('activo','permiso','baja'));

-- proyecto.status_proyecto
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'proyecto' AND CONSTRAINT_NAME = 'chk_proyecto_status') > 0,
    'ALTER TABLE proyecto DROP CHECK chk_proyecto_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE proyecto MODIFY COLUMN status_proyecto VARCHAR(20) NOT NULL DEFAULT 'propuesto';
ALTER TABLE proyecto ADD CONSTRAINT chk_proyecto_status
    CHECK (status_proyecto IN ('propuesto','en_progreso','completado','cancelado'));

-- proyecto_profesor.tipo_supervision
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'proyecto_profesor' AND CONSTRAINT_NAME = 'chk_pp_tipo') > 0,
    'ALTER TABLE proyecto_profesor DROP CHECK chk_pp_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE proyecto_profesor MODIFY COLUMN tipo_supervision VARCHAR(15) NOT NULL;
ALTER TABLE proyecto_profesor ADD CONSTRAINT chk_pp_tipo
    CHECK (tipo_supervision IN ('director','asesor','revisor'));

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'proyecto_profesor' AND CONSTRAINT_NAME = 'chk_pp_status') > 0,
    'ALTER TABLE proyecto_profesor DROP CHECK chk_pp_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE proyecto_profesor MODIFY COLUMN status_supervision VARCHAR(15) NOT NULL DEFAULT 'activa';
ALTER TABLE proyecto_profesor ADD CONSTRAINT chk_pp_status
    CHECK (status_supervision IN ('activa','concluida'));

-- telefono.tipo_telefono
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'telefono' AND CONSTRAINT_NAME = 'chk_tel_tipo') > 0,
    'ALTER TABLE telefono DROP CHECK chk_tel_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE telefono MODIFY COLUMN tipo_telefono VARCHAR(15) NOT NULL;
ALTER TABLE telefono ADD CONSTRAINT chk_tel_tipo
    CHECK (tipo_telefono IN ('movil','casa','oficina','fax','emergencia'));

-- tema.status_tema
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'tema' AND CONSTRAINT_NAME = 'chk_tema_status') > 0,
    'ALTER TABLE tema DROP CHECK chk_tema_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE tema MODIFY COLUMN status_tema VARCHAR(10) NOT NULL DEFAULT 'activo';
ALTER TABLE tema ADD CONSTRAINT chk_tema_status
    CHECK (status_tema IN ('activo','inactivo'));

-- ============================================================
-- ON UPDATE CURRENT_TIMESTAMP restantes
-- ============================================================

ALTER TABLE alumno_empresa MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE alumno_proyecto MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE direccion MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE documento MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE email MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE notificacion MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE proyecto_materia MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE proyecto_profesor MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE rol MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;
ALTER TABLE telefono MODIFY COLUMN updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP;

SELECT '✅ ENUMs restantes convertidos + ON UPDATE removidos' AS Status;
