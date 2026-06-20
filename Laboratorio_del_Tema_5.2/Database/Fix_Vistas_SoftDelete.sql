-- ============================================================
-- FIX CRÍTICO: Vistas deben filtrar también por is_deleted
-- Bug #1 y #4 de Auditoria_Profunda_MenuPrincipal
--
-- ANTES: v_alumnos_activos mostraba 16 (incluia 2 soft-deleted)
--        FormAlumnos mostraba 14 (filtraba is_deleted=0)
--        DIFERENCIA: 2 alumnos fantasma en el menu
--
-- AHORA: ambas vistas filtran is_deleted=0, consistencia total
-- ============================================================

USE modelodualdb;

-- Drop views existentes
DROP VIEW IF EXISTS v_alumnos_activos;
DROP VIEW IF EXISTS v_empresas_activas;
DROP VIEW IF EXISTS v_profesores_activos;
DROP VIEW IF EXISTS v_materias_activas;
DROP VIEW IF EXISTS v_proyectos_activos;

-- Recreate con filtro is_deleted=0
CREATE VIEW v_alumnos_activos AS
SELECT *
FROM alumno
WHERE status_alumno = 'activo' AND is_deleted = 0;

CREATE VIEW v_empresas_activas AS
SELECT *
FROM empresa
WHERE status_empresa = 'activa' AND is_deleted = 0;

CREATE VIEW v_profesores_activos AS
SELECT *
FROM profesor
WHERE status_profesor = 'activo' AND is_deleted = 0;

CREATE VIEW v_materias_activas AS
SELECT *
FROM materia
WHERE status_materia = 'activa' AND is_deleted = 0;

CREATE VIEW v_proyectos_activos AS
SELECT *
FROM proyecto
WHERE status_proyecto IN ('propuesto', 'en_progreso') AND is_deleted = 0;

SELECT '✅ Vistas corregidas - ahora filtran is_deleted=0' AS Status;

-- Verificación
SELECT 'alumnos activos (debe ser 13, no 16):' AS '';
SELECT COUNT(*) AS conteo FROM v_alumnos_activos;
