-- ============================================================
-- VISTA DIAGNOSTICA: v_alumnos_conflictos
-- Reporta conflictos en la tabla Alumno antes de aplicar la
-- migracion enterprise del modulo alumnos.
-- Ejecutar en MySQL Workbench / CLI conectado a modelodualdb.
-- ============================================================

USE modelodualdb;

CREATE OR REPLACE VIEW v_alumnos_conflictos AS
SELECT
    (SELECT COUNT(*) FROM Alumno WHERE is_deleted = 0) AS total_alumnos_activos,
    (SELECT COUNT(*) FROM (
        SELECT no_control FROM Alumno WHERE is_deleted = 0
        GROUP BY no_control HAVING COUNT(*) > 1
    ) d) AS duplicados_no_control_activos,
    (SELECT COUNT(*) FROM Alumno
     WHERE is_deleted = 0
       AND (no_control IS NULL OR no_control = '' OR no_control REGEXP '^[0-9]+$' = 0)
    ) AS valores_no_control_invalidos,
    (SELECT COUNT(*) FROM (
        SELECT curp FROM Alumno WHERE curp IS NOT NULL
        GROUP BY curp HAVING COUNT(*) > 1
    ) d) AS curp_duplicadas,
    (SELECT COUNT(*) FROM Alumno
     WHERE curp IS NOT NULL
       AND curp NOT REGEXP '^[A-Z]{1}[AEIOUX]{1}[A-Z]{2}[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])[HM]{1}(AS|BC|BS|CC|CS|CH|CL|CM|DF|DG|GT|GR|HG|JC|MC|MN|MS|NT|NL|OC|PL|QT|QR|SP|SL|SR|TC|TS|TL|VZ|YN|ZS|NE)[B-DF-HJ-NP-TV-Z]{3}[0-9A-Z]{1}[0-9]{1}$'
    ) AS curp_invalidas,
    (SELECT COUNT(*) FROM (
        SELECT email FROM Alumno WHERE email IS NOT NULL AND is_deleted = 0
        GROUP BY email HAVING COUNT(*) > 1
    ) d) AS emails_duplicados;
