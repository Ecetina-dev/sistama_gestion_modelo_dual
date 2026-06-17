-- ============================================================
-- MEJORAS ENTERPRISE DE BASE DE DATOS - Modelo Dual
-- Agrega: constraints, indices, validaciones y auditoria
-- ============================================================
-- Ejecutar en MySQL Workbench conectado a modelodualdb
-- ============================================================

USE modelodualdb;

-- ============================================================
-- 1. ASEGURAR COLLATION UTF8 (evita problemas con acentos)
-- ============================================================
ALTER DATABASE modelodualdb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Convertir todas las tablas a utf8mb4
SELECT CONCAT('ALTER TABLE ', TABLE_NAME, ' CONVERT TO CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;') 
FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_COLLATION != 'utf8mb4_unicode_ci';

-- ============================================================
-- 2. AGREGAR INDICES EN COLUMNAS DE BUSQUEDA FRECUENTE
-- ============================================================
-- Alumnos: buscar por nombre, apellido, email
ALTER TABLE Alumno ADD INDEX idx_alumno_nombre (nombre);
ALTER TABLE Alumno ADD INDEX idx_alumno_apellido (apellido_paterno);
ALTER TABLE Alumno ADD INDEX idx_alumno_status (status_alumno);

-- Empresas: buscar por RFC, nombre, ciudad
ALTER TABLE Empresa ADD INDEX idx_empresa_rfc (rfc);
ALTER TABLE Empresa ADD INDEX idx_empresa_ciudad (ciudad);
ALTER TABLE Empresa ADD INDEX idx_empresa_status (status_empresa);

-- Profesores: buscar por nombre, departamento, email
ALTER TABLE Profesor ADD INDEX idx_profesor_nombre (nombre);
ALTER TABLE Profesor ADD INDEX idx_profesor_departamento (departamento);

-- Materias: buscar por clave, semestre
ALTER TABLE Materia ADD INDEX idx_materia_semestre (semestre);

-- Temas: buscar por materia
ALTER TABLE Tema ADD INDEX idx_tema_materia (id_materia);

-- Proyectos: buscar por status
ALTER TABLE Proyecto ADD INDEX idx_proyecto_status (status);

-- Usuario: buscar por email, status (username y email ya tienen UNIQUE)
ALTER TABLE Usuario ADD INDEX idx_usuario_status (status);

-- Sesion: buscar por usuario y status
ALTER TABLE Sesion ADD INDEX idx_sesion_usuario_status (id_usuario, status);

-- ============================================================
-- 3. AGREGAR VALIDACIONES CON CHECK CONSTRAINTS
-- ============================================================
-- MySQL 8.0+ soporta CHECK constraints

-- Alumno: status solo valores validos
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_status 
CHECK (status_alumno IN ('activo', 'inactivo', 'egresado', 'suspendido'));

-- Empresa: status solo valores validos
ALTER TABLE Empresa ADD CONSTRAINT chk_empresa_status 
CHECK (status_empresa IN ('activa', 'inactiva', 'propuesta'));

-- Profesor: status solo valores validos
ALTER TABLE Profesor ADD CONSTRAINT chk_profesor_status 
CHECK (status_profesor IN ('activo', 'inactivo'));

-- Materia: creditos positivos, semestre valido
ALTER TABLE Materia ADD CONSTRAINT chk_materia_creditos CHECK (creditos > 0 AND creditos <= 20);
ALTER TABLE Materia ADD CONSTRAINT chk_materia_semestre CHECK (semestre >= 1 AND semestre <= 12);
ALTER TABLE Materia ADD CONSTRAINT chk_materia_status CHECK (status_materia IN ('activa', 'inactiva'));

-- Tema: numero valido
ALTER TABLE Tema ADD CONSTRAINT chk_tema_numero CHECK (numero_tema >= 1 AND numero_tema <= 999);

-- Proyecto: status valido
ALTER TABLE Proyecto ADD CONSTRAINT chk_proyecto_status 
CHECK (status IN ('propuesto', 'activo', 'en_revision', 'completado', 'cancelado'));

-- Usuario: status valido, email con @
ALTER TABLE Usuario ADD CONSTRAINT chk_usuario_status 
CHECK (status IN ('activo', 'inactivo', 'suspendido'));
ALTER TABLE Usuario ADD CONSTRAINT chk_usuario_email CHECK (email LIKE '%@%.%');

-- Sesion: status valido
ALTER TABLE Sesion ADD CONSTRAINT chk_sesion_status CHECK (status IN ('activa', 'expirada', 'cerrada'));

-- ============================================================
-- 4. VALORES POR DEFECTO
-- ============================================================
ALTER TABLE Alumno ALTER COLUMN status_alumno SET DEFAULT 'activo';
ALTER TABLE Empresa ALTER COLUMN status_empresa SET DEFAULT 'activa';
ALTER TABLE Profesor ALTER COLUMN status_profesor SET DEFAULT 'activo';
ALTER TABLE Materia ALTER COLUMN status_materia SET DEFAULT 'activa';
ALTER TABLE Materia ALTER COLUMN creditos SET DEFAULT 5;
ALTER TABLE Materia ALTER COLUMN semestre SET DEFAULT 1;
ALTER TABLE Proyecto ALTER COLUMN status SET DEFAULT 'propuesto';

-- ============================================================
-- 5. TABLA DE AUDITORIA (registra quién hizo qué y cuándo)
-- ============================================================
CREATE TABLE IF NOT EXISTS Auditoria (
    id_auditoria BIGINT PRIMARY KEY AUTO_INCREMENT,
    tabla_afectada VARCHAR(50) NOT NULL,
    operacion ENUM('INSERT', 'UPDATE', 'DELETE') NOT NULL,
    id_registro INT NOT NULL,
    id_usuario INT,
    fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    detalle TEXT,
    ip_origen VARCHAR(45),
    INDEX idx_auditoria_fecha (fecha),
    INDEX idx_auditoria_tabla (tabla_afectada),
    INDEX idx_auditoria_usuario (id_usuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================================
-- 6. TRIGGERS DE AUDITORIA PARA ALUMNO
-- ============================================================
DROP TRIGGER IF EXISTS trg_alumno_insert;
CREATE TRIGGER trg_alumno_insert AFTER INSERT ON Alumno
FOR EACH ROW
    INSERT INTO Auditoria (tabla_afectada, operacion, id_registro, detalle)
    VALUES ('Alumno', 'INSERT', NEW.id_alumno, CONCAT('Nuevo alumno: ', NEW.no_control, ' - ', NEW.nombre));

DROP TRIGGER IF EXISTS trg_alumno_update;
CREATE TRIGGER trg_alumno_update AFTER UPDATE ON Alumno
FOR EACH ROW
    INSERT INTO Auditoria (tabla_afectada, operacion, id_registro, detalle)
    VALUES ('Alumno', 'UPDATE', NEW.id_alumno, CONCAT('Alumno actualizado: ', NEW.no_control));

DROP TRIGGER IF EXISTS trg_alumno_delete;
CREATE TRIGGER trg_alumno_delete AFTER DELETE ON Alumno
FOR EACH ROW
    INSERT INTO Auditoria (tabla_afectada, operacion, id_registro, detalle)
    VALUES ('Alumno', 'DELETE', OLD.id_alumno, CONCAT('Alumno eliminado: ', OLD.no_control));

-- ============================================================
-- 7. TRIGGERS DE AUDITORIA PARA USUARIO
-- ============================================================
DROP TRIGGER IF EXISTS trg_usuario_insert;
CREATE TRIGGER trg_usuario_insert AFTER INSERT ON Usuario
FOR EACH ROW
    INSERT INTO Auditoria (tabla_afectada, operacion, id_registro, detalle)
    VALUES ('Usuario', 'INSERT', NEW.id_usuario, CONCAT('Nuevo usuario: ', NEW.username));

DROP TRIGGER IF EXISTS trg_usuario_update;
CREATE TRIGGER trg_usuario_update AFTER UPDATE ON Usuario
FOR EACH ROW
    INSERT INTO Auditoria (tabla_afectada, operacion, id_registro, detalle)
    VALUES ('Usuario', 'UPDATE', NEW.id_usuario, 
        CASE WHEN OLD.status != NEW.status 
            THEN CONCAT('Status cambiado: ', OLD.status, ' -> ', NEW.status, ' para ', NEW.username)
            ELSE CONCAT('Usuario actualizado: ', NEW.username)
        END);

-- ============================================================
-- 8. VISTAS PARA EL DASHBOARD DE ESTADISTICAS
-- ============================================================
CREATE OR REPLACE VIEW v_alumnos_activos AS
    SELECT id_alumno, no_control, nombre, apellido_paterno, apellido_materno, email
    FROM Alumno WHERE status_alumno = 'activo';

CREATE OR REPLACE VIEW v_empresas_activas AS
    SELECT id_empresa, rfc, razon_social, ciudad, telefono, email
    FROM Empresa WHERE status_empresa = 'activa';

CREATE OR REPLACE VIEW v_profesores_activos AS
    SELECT id_profesor, nombre, apellido_paterno, apellido_materno, email, departamento
    FROM Profesor WHERE status_profesor = 'activo';

-- ============================================================
-- 9. VERIFICACION FINAL
-- ============================================================
SELECT '========================================' AS '';
SELECT 'MEJORAS APLICADAS:' AS '';
SELECT '========================================' AS '';
SELECT '✅ Collation utf8mb4_unicode_ci' AS mejora;
SELECT '✅ Indices en columnas de busqueda' AS mejora;
SELECT '✅ CHECK constraints en status, creditos, email' AS mejora;
SELECT '✅ Valores DEFAULT en campos de status' AS mejora;
SELECT '✅ Tabla Auditoria creada' AS mejora;
SELECT '✅ Triggers de auditoria en Alumno y Usuario' AS mejora;
SELECT '✅ Base de datos NIVEL ENTERPRISE' AS mejora;
