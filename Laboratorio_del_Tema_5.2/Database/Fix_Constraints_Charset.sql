-- ============================================================================
-- Fix: Constraints de validacion de nombres con acentos espa\u00f1oles
-- ============================================================================
-- El problema: los constraints actuales usan [[:punct:]] que bajo utf8mb4
-- puede rechazar caracteres acentuados (\u00e1=a, \u00e9=e, \u00ed=i, \u00f3=o, \u00fa=u, \u00f1=n).
-- La soluci\u00f3n: usar expl\u00edcitamente los caracteres acentuados permitidos
-- en el pattern en vez de [[:punct:]].
--
-- Ejemplo: '[^\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1A-Z0-9[:digit:]]'
-- es mejor que '[^[:punct:][:digit:]]'
-- ============================================================================

-- 1. Verificar constraint actual
-- SELECT CONSTRAINT_NAME, CHECK_CLAUSE
-- FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS
-- WHERE TABLE_NAME = 'alumno' AND TABLE_SCHEMA = 'modelodualdb';

-- 2. Alterar los constraints para aceptar acentos espa\u00f1oles
-- El collation utf8mb4_0900_ai_ci ya es accent-insensitive pero los constraints
-- usan regexp que trata los acentos como [[:punct:]].
-- Soluci\u00f3n: exclude solo d\u00edgitos y puntuaci\u00f3n b\u00e1sica, no acentos.

ALTER TABLE alumno
    DROP CHECK chk_alumno_nombre,
    ADD CONSTRAINT chk_alumno_nombre
        CHECK (regexp_like(nombre, _utf8mb4'^[^\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1A-Z\\s]{2,100}$')
              OR regexp_like(nombre, _utf8mb4'^[A-Z\u00c1\u00c9\u00cd\u00d3\u00da\u00d1][A-Z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1\\s\\'\u002d]{1,99}$'));

ALTER TABLE alumno
    DROP CHECK chk_alumno_apellido_paterno,
    ADD CONSTRAINT chk_alumno_apellido_paterno
        CHECK (regexp_like(apellido_paterno, _utf8mb4'^[A-Z\u00c1\u00c9\u00cd\u00d3\u00da\u00d1][A-Z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1\\s\\'\u002d]{1,79}$'));

ALTER TABLE alumno
    DROP CHECK chk_alumno_apellido_materno,
    ADD CONSTRAINT chk_alumno_apellido_materno
        CHECK ((apellido_materno IS NULL)
            OR regexp_like(apellido_materno, _utf8mb4'^[A-Z\u00c1\u00c9\u00cd\u00d3\u00da\u00d1][A-Z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1\\s\\'\u002d]{1,79}$'));

-- 3. Verificar que funciona con acentos
-- SELECT nombre, regexp_like(nombre, _utf8mb4'^[A-Z\u00c1\u00c9\u00cd\u00d3\u00da\u00d1][A-Z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1\\s\\''\\u002d]{1,79}$') AS matcheo
-- FROM (SELECT 'Garc\u00eda' AS nombre UNION SELECT 'L\u00f3pez' UNION SELECT 'Mu\u00f1oz' UNION SELECT 'Oc\u00f3n' UNION SELECT 'Hern\u00e1ndez') t;
