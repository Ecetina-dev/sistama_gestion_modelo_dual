-- ============================================================
-- MIGRACION ENTERPRISE - FASE 1: Fundamento de datos de alumnos
-- Requisitos:
--   * Ejecutar primero v_alumnos_conflictos.sql y resolver
--     duplicados activos de no_control.
--   * Realizar un respaldo de modelodualdb antes de ejecutar.
-- ============================================================

USE modelodualdb;

DELIMITER //

CREATE PROCEDURE IF NOT EXISTS _Run_Alumnos_Enterprise_Phase1()
BEGIN
    DECLARE v_msg VARCHAR(1000);

    START TRANSACTION;

    -- Guard: abort if active no_control duplicates exist
    SELECT GROUP_CONCAT(DISTINCT no_control ORDER BY no_control SEPARATOR ', ')
      INTO v_msg
    FROM Alumno
    WHERE is_deleted = 0
    GROUP BY no_control
    HAVING COUNT(*) > 1;

    IF v_msg IS NOT NULL THEN
        SET v_msg = CONCAT('Abort: active no_control duplicates exist: ', v_msg);
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = v_msg;
    END IF;

    -- Carrera lookup
    CREATE TABLE IF NOT EXISTS Carrera (
        id_carrera INT AUTO_INCREMENT PRIMARY KEY,
        clave VARCHAR(20) NOT NULL,
        nombre VARCHAR(200) NOT NULL,
        duracion_semestres INT NOT NULL DEFAULT 8,
        status ENUM('activa','inactiva') NOT NULL DEFAULT 'activa',
        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
        UNIQUE KEY uk_carrera_clave (clave),
        INDEX idx_carrera_status (status)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

    INSERT INTO Carrera (clave, nombre, duracion_semestres, status)
    VALUES ('SIN-CARRERA', 'Sin carrera asignada', 8, 'activa')
    ON DUPLICATE KEY UPDATE updated_at = CURRENT_TIMESTAMP;

    -- Expand Alumno
    ALTER TABLE Alumno
        ADD COLUMN IF NOT EXISTS curp VARCHAR(18) NULL AFTER no_control,
        ADD COLUMN IF NOT EXISTS rfc VARCHAR(13) NULL AFTER curp,
        ADD COLUMN IF NOT EXISTS nss VARCHAR(20) NULL AFTER rfc,
        ADD COLUMN IF NOT EXISTS genero ENUM('Masculino','Femenino','No binario','Prefiero no decir') NULL AFTER nss,
        ADD COLUMN IF NOT EXISTS estado_civil VARCHAR(30) NULL AFTER genero,
        ADD COLUMN IF NOT EXISTS nacionalidad VARCHAR(50) NULL AFTER estado_civil,
        ADD COLUMN IF NOT EXISTS direccion_calle VARCHAR(100) NULL AFTER nacionalidad,
        ADD COLUMN IF NOT EXISTS direccion_numero VARCHAR(20) NULL AFTER direccion_calle,
        ADD COLUMN IF NOT EXISTS direccion_colonia VARCHAR(100) NULL AFTER direccion_numero,
        ADD COLUMN IF NOT EXISTS direccion_ciudad VARCHAR(100) NULL AFTER direccion_colonia,
        ADD COLUMN IF NOT EXISTS direccion_estado VARCHAR(100) NULL AFTER direccion_ciudad,
        ADD COLUMN IF NOT EXISTS direccion_cp VARCHAR(10) NULL AFTER direccion_estado,
        ADD COLUMN IF NOT EXISTS telefono_fijo VARCHAR(15) NULL AFTER direccion_cp,
        ADD COLUMN IF NOT EXISTS contacto_emergencia_nombre VARCHAR(100) NULL AFTER telefono_fijo,
        ADD COLUMN IF NOT EXISTS contacto_emergencia_telefono VARCHAR(15) NULL AFTER contacto_emergencia_nombre,
        ADD COLUMN IF NOT EXISTS contacto_emergencia_parentesco VARCHAR(50) NULL AFTER contacto_emergencia_telefono,
        ADD COLUMN IF NOT EXISTS id_carrera INT NULL AFTER contacto_emergencia_parentesco,
        ADD COLUMN IF NOT EXISTS semestre INT NULL AFTER id_carrera,
        ADD COLUMN IF NOT EXISTS grupo VARCHAR(20) NULL AFTER semestre,
        ADD COLUMN IF NOT EXISTS turno ENUM('matutino','vespertino','nocturno','mixto') NULL AFTER grupo,
        ADD COLUMN IF NOT EXISTS fecha_ingreso DATE NULL AFTER turno,
        ADD COLUMN IF NOT EXISTS fecha_egreso DATE NULL AFTER fecha_ingreso,
        ADD COLUMN IF NOT EXISTS fecha_baja DATE NULL AFTER fecha_egreso,
        ADD COLUMN IF NOT EXISTS motivo_baja VARCHAR(255) NULL AFTER fecha_baja,
        ADD COLUMN IF NOT EXISTS promedio_general DECIMAL(4,2) NULL AFTER motivo_baja,
        ADD COLUMN IF NOT EXISTS created_by INT NULL AFTER promedio_general,
        ADD COLUMN IF NOT EXISTS updated_by INT NULL AFTER created_by,
        ADD COLUMN IF NOT EXISTS deleted_by INT NULL AFTER updated_by,
        ADD COLUMN IF NOT EXISTS deleted_reason VARCHAR(255) NULL AFTER deleted_by,
        ADD COLUMN IF NOT EXISTS status_change_reason VARCHAR(255) NULL AFTER deleted_reason,
        ADD COLUMN IF NOT EXISTS no_control_unico VARCHAR(15)
            GENERATED ALWAYS AS (IF(is_deleted = 1, NULL, no_control)) STORED
            AFTER status_change_reason;

    -- Indexes and constraints
    DROP INDEX IF EXISTS uk_alumno_curp ON Alumno;
    CREATE UNIQUE INDEX uk_alumno_curp ON Alumno(curp);

    DROP INDEX IF EXISTS uk_alumno_no_control_unico ON Alumno;
    CREATE UNIQUE INDEX uk_alumno_no_control_unico ON Alumno(no_control_unico);

    DROP INDEX IF EXISTS uk_alumno_email ON Alumno;
    CREATE UNIQUE INDEX uk_alumno_email ON Alumno(email);

    CREATE INDEX IF NOT EXISTS idx_alumno_no_control ON Alumno(no_control);
    CREATE INDEX IF NOT EXISTS idx_alumno_rfc ON Alumno(rfc);
    CREATE INDEX IF NOT EXISTS idx_alumno_nombre_completo ON Alumno(apellido_paterno, apellido_materno, nombre);
    CREATE INDEX IF NOT EXISTS idx_alumno_id_carrera ON Alumno(id_carrera);
    CREATE INDEX IF NOT EXISTS idx_alumno_semestre ON Alumno(semestre);
    CREATE INDEX IF NOT EXISTS idx_alumno_status ON Alumno(status_alumno);
    CREATE INDEX IF NOT EXISTS idx_alumno_fecha_ingreso ON Alumno(fecha_ingreso);
    CREATE INDEX IF NOT EXISTS idx_alumno_is_deleted ON Alumno(is_deleted);

    ALTER TABLE Alumno
        DROP FOREIGN KEY IF EXISTS fk_alumno_carrera,
        ADD CONSTRAINT fk_alumno_carrera FOREIGN KEY (id_carrera) REFERENCES Carrera(id_carrera);

    ALTER TABLE Alumno
        DROP FOREIGN KEY IF EXISTS fk_alumno_created_by,
        ADD CONSTRAINT fk_alumno_created_by FOREIGN KEY (created_by) REFERENCES Usuario(id_usuario);

    ALTER TABLE Alumno
        DROP FOREIGN KEY IF EXISTS fk_alumno_updated_by,
        ADD CONSTRAINT fk_alumno_updated_by FOREIGN KEY (updated_by) REFERENCES Usuario(id_usuario);

    ALTER TABLE Alumno
        DROP FOREIGN KEY IF EXISTS fk_alumno_deleted_by,
        ADD CONSTRAINT fk_alumno_deleted_by FOREIGN KEY (deleted_by) REFERENCES Usuario(id_usuario);

    -- Alumno_Documento
    CREATE TABLE IF NOT EXISTS Alumno_Documento (
        id_documento INT AUTO_INCREMENT PRIMARY KEY,
        id_alumno INT NOT NULL,
        tipo_documento VARCHAR(50) NOT NULL,
        nombre_archivo VARCHAR(255) NOT NULL,
        ruta_archivo VARCHAR(500) NOT NULL,
        mime_type VARCHAR(100) NULL,
        subido_por INT NULL,
        created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
        INDEX idx_alumno_documento_alumno (id_alumno),
        FOREIGN KEY (id_alumno) REFERENCES Alumno(id_alumno) ON DELETE CASCADE,
        FOREIGN KEY (subido_por) REFERENCES Usuario(id_usuario)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

    COMMIT;
END //

DELIMITER ;

CALL _Run_Alumnos_Enterprise_Phase1();
DROP PROCEDURE IF EXISTS _Run_Alumnos_Enterprise_Phase1;

/*
-- ============================================================
-- ROLLBACK: ejecutar solo si es necesario revertir Fase 1
-- ============================================================
USE modelodualdb;

ALTER TABLE Alumno
    DROP FOREIGN KEY IF EXISTS fk_alumno_carrera,
    DROP FOREIGN KEY IF EXISTS fk_alumno_created_by,
    DROP FOREIGN KEY IF EXISTS fk_alumno_updated_by,
    DROP FOREIGN KEY IF EXISTS fk_alumno_deleted_by;

DROP INDEX IF EXISTS uk_alumno_curp ON Alumno;
DROP INDEX IF EXISTS uk_alumno_no_control_unico ON Alumno;
DROP INDEX IF EXISTS uk_alumno_email ON Alumno;
DROP INDEX IF EXISTS idx_alumno_no_control ON Alumno;
DROP INDEX IF EXISTS idx_alumno_rfc ON Alumno;
DROP INDEX IF EXISTS idx_alumno_nombre_completo ON Alumno;
DROP INDEX IF EXISTS idx_alumno_id_carrera ON Alumno;
DROP INDEX IF EXISTS idx_alumno_semestre ON Alumno;
DROP INDEX IF EXISTS idx_alumno_status ON Alumno;
DROP INDEX IF EXISTS idx_alumno_fecha_ingreso ON Alumno;
DROP INDEX IF EXISTS idx_alumno_is_deleted ON Alumno;

ALTER TABLE Alumno
    DROP COLUMN IF EXISTS curp,
    DROP COLUMN IF EXISTS rfc,
    DROP COLUMN IF EXISTS nss,
    DROP COLUMN IF EXISTS genero,
    DROP COLUMN IF EXISTS estado_civil,
    DROP COLUMN IF EXISTS nacionalidad,
    DROP COLUMN IF EXISTS direccion_calle,
    DROP COLUMN IF EXISTS direccion_numero,
    DROP COLUMN IF EXISTS direccion_colonia,
    DROP COLUMN IF EXISTS direccion_ciudad,
    DROP COLUMN IF EXISTS direccion_estado,
    DROP COLUMN IF EXISTS direccion_cp,
    DROP COLUMN IF EXISTS telefono_fijo,
    DROP COLUMN IF EXISTS contacto_emergencia_nombre,
    DROP COLUMN IF EXISTS contacto_emergencia_telefono,
    DROP COLUMN IF EXISTS contacto_emergencia_parentesco,
    DROP COLUMN IF EXISTS id_carrera,
    DROP COLUMN IF EXISTS semestre,
    DROP COLUMN IF EXISTS grupo,
    DROP COLUMN IF EXISTS turno,
    DROP COLUMN IF EXISTS fecha_ingreso,
    DROP COLUMN IF EXISTS fecha_egreso,
    DROP COLUMN IF EXISTS fecha_baja,
    DROP COLUMN IF EXISTS motivo_baja,
    DROP COLUMN IF EXISTS promedio_general,
    DROP COLUMN IF EXISTS created_by,
    DROP COLUMN IF EXISTS updated_by,
    DROP COLUMN IF EXISTS deleted_by,
    DROP COLUMN IF EXISTS deleted_reason,
    DROP COLUMN IF EXISTS status_change_reason,
    DROP COLUMN IF EXISTS no_control_unico;

DROP TABLE IF EXISTS Alumno_Documento;
DROP TABLE IF EXISTS Carrera;
*/
