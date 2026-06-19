-- ============================================================
-- FIX PART 3: ENUM → VARCHAR + CHECK
-- ============================================================

USE modelodualdb;

-- Usuario.status
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Usuario' AND CONSTRAINT_NAME = 'chk_usuario_status') > 0,
    'ALTER TABLE Usuario DROP CHECK chk_usuario_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Usuario MODIFY COLUMN status VARCHAR(20) NOT NULL DEFAULT 'activo';
ALTER TABLE Usuario ADD CONSTRAINT chk_usuario_status
    CHECK (status IN ('activo', 'inactivo', 'suspendido'));

-- Alumno.genero
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno' AND CONSTRAINT_NAME = 'chk_alumno_genero') > 0,
    'ALTER TABLE Alumno DROP CHECK chk_alumno_genero',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Alumno MODIFY COLUMN genero VARCHAR(30) NULL;
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_genero
    CHECK (genero IS NULL OR genero IN ('Masculino', 'Femenino', 'No binario', 'Prefiero no decir'));

-- Alumno.turno
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno' AND CONSTRAINT_NAME = 'chk_alumno_turno') > 0,
    'ALTER TABLE Alumno DROP CHECK chk_alumno_turno',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Alumno MODIFY COLUMN turno VARCHAR(20) NULL;
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_turno
    CHECK (turno IS NULL OR turno IN ('matutino', 'vespertino', 'nocturno', 'mixto'));

-- Alumno.status_alumno
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno' AND CONSTRAINT_NAME = 'chk_alumno_status') > 0,
    'ALTER TABLE Alumno DROP CHECK chk_alumno_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Alumno MODIFY COLUMN status_alumno VARCHAR(20) NOT NULL DEFAULT 'activo';
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_status
    CHECK (status_alumno IN ('activo', 'inactivo', 'egresado', 'suspendido', 'baja'));

-- Carrera.status
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Carrera' AND CONSTRAINT_NAME = 'chk_carrera_status') > 0,
    'ALTER TABLE Carrera DROP CHECK chk_carrera_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Carrera MODIFY COLUMN status VARCHAR(10) NOT NULL DEFAULT 'activa';
ALTER TABLE Carrera ADD CONSTRAINT chk_carrera_status
    CHECK (status IN ('activa', 'inactiva'));

-- Sesion.status
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Sesion' AND CONSTRAINT_NAME = 'chk_sesion_status') > 0,
    'ALTER TABLE Sesion DROP CHECK chk_sesion_status',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Sesion MODIFY COLUMN status VARCHAR(10) NOT NULL DEFAULT 'activa';
ALTER TABLE Sesion ADD CONSTRAINT chk_sesion_status
    CHECK (status IN ('activa', 'expirada', 'cerrada'));

-- Usuario_Entidad.tipo_entidad
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Usuario_Entidad' AND CONSTRAINT_NAME = 'chk_ue_tipo') > 0,
    'ALTER TABLE Usuario_Entidad DROP CHECK chk_ue_tipo',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Usuario_Entidad MODIFY COLUMN tipo_entidad VARCHAR(10) NOT NULL;
ALTER TABLE Usuario_Entidad ADD CONSTRAINT chk_ue_tipo
    CHECK (tipo_entidad IN ('alumno', 'profesor', 'empresa'));

SELECT '✅ ENUMs convertidos a VARCHAR + CHECK' AS Status;
