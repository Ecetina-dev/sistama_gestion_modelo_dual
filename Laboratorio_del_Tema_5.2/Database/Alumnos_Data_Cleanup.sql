-- ============================================================
-- LIMPIEZA DE DATOS DE ALUMNO (PREPARATORIA A NOT NULL CURP)
-- Este script solo CONSULTA y marca registros problematicos.
-- NO modifica datos salvo que el administrador descomente y
-- edite explicitamente las sentencias UPDATE/DELETE al final.
-- Ejecutar en MySQL Workbench / CLI conectado a modelodualdb.
-- ============================================================

USE modelodualdb;

-- ------------------------------------------------------------
-- 1. Alumnos activos sin CURP (requieren captura manual)
-- ------------------------------------------------------------
SELECT
    id_alumno,
    no_control,
    nombre,
    apellido_paterno,
    apellido_materno,
    email,
    status_alumno
FROM Alumno
WHERE is_deleted = 0
  AND (curp IS NULL OR curp = '')
ORDER BY apellido_paterno, nombre;

-- ------------------------------------------------------------
-- 2. CURPs duplicadas (incluye activos y eliminados)
-- ------------------------------------------------------------
SELECT
    curp,
    COUNT(*) AS total,
    GROUP_CONCAT(DISTINCT id_alumno ORDER BY id_alumno SEPARATOR ', ') AS ids_alumno,
    SUM(CASE WHEN is_deleted = 0 THEN 1 ELSE 0 END) AS activos
FROM Alumno
WHERE curp IS NOT NULL
  AND curp != ''
GROUP BY curp
HAVING COUNT(*) > 1;

-- ------------------------------------------------------------
-- 3. no_control duplicados entre registros activos
-- ------------------------------------------------------------
SELECT
    no_control,
    COUNT(*) AS total,
    GROUP_CONCAT(DISTINCT id_alumno ORDER BY id_alumno SEPARATOR ', ') AS ids_alumno
FROM Alumno
WHERE is_deleted = 0
GROUP BY no_control
HAVING COUNT(*) > 1;

-- ------------------------------------------------------------
-- 4. Emails duplicados entre registros activos
-- ------------------------------------------------------------
SELECT
    email,
    COUNT(*) AS total,
    GROUP_CONCAT(DISTINCT id_alumno ORDER BY id_alumno SEPARATOR ', ') AS ids_alumno
FROM Alumno
WHERE is_deleted = 0
  AND email IS NOT NULL
  AND email != ''
GROUP BY email
HAVING COUNT(*) > 1;

-- ------------------------------------------------------------
-- 5. no_control activos invalidos (no numerico o vacio)
-- ------------------------------------------------------------
SELECT
    id_alumno,
    no_control,
    nombre,
    apellido_paterno,
    apellido_materno
FROM Alumno
WHERE is_deleted = 0
  AND (no_control IS NULL OR no_control = '' OR no_control REGEXP '^[0-9]+$' = 0)
ORDER BY apellido_paterno, nombre;

-- ------------------------------------------------------------
-- PLANTILLAS DE CORRECCION MANUAL (descomentar y editar antes de ejecutar)
-- ------------------------------------------------------------

-- Actualizar un CURP faltante una vez que se tenga el documento oficial.
-- Reemplazar <ID> por el id_alumno y <CURP-VALIDO> por el CURP correcto:
-- UPDATE Alumno SET curp = '<CURP-VALIDO>' WHERE id_alumno = <ID>;

-- Corregir un numero de control duplicado/invalido:
-- UPDATE Alumno SET no_control = '<NUEVO-NO-CONTROL>' WHERE id_alumno = <ID>;

-- Marcar como eliminado un regros duplicado obsoleto (requiere motivo):
-- UPDATE Alumno
-- SET is_deleted = 1, deleted_at = NOW(), deleted_reason = 'Registro duplicado'
-- WHERE id_alumno = <ID>;
