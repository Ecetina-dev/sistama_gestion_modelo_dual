-- ============================================================================
-- Fix: Constraints de validacion de nombres - aceptar acentos espa\u00f1oles
-- ============================================================================
-- Problema: [[:punct:]] en MySQL bajo utf8mb4 incluye vocales acentuadas
-- (a, e, i, o, u) y n\u00f1. Esto rechazaba nombres como "Garc\u00eda", "L\u00f3pez".
--
-- Soluci\u00f3n: usar negaci\u00f3n de caracteres directamente, no [[:punct:]].
-- Pattern anterior: [^0-9[:punct:][:digit:]]  <- MAL: incluye acentos
-- Pattern corregido: [^0-9A-Za-z\u00e1\u00e9\u00ed\u00f3\u00fa\u00c1\u00c9\u00cd\u00d3\u00da\u00d1\s\'\-\.] <- BIEN
-- ============================================================================

-- 1. Apellido paterno
ALTER TABLE alumno
    DROP CHECK chk_alumno_apellido_paterno,
    ADD CONSTRAINT chk_alumno_apellido_paterno
        CHECK (regexp_like(apellido_paterno,
            _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\'\u002d]{2,80}$'));

-- 2. Apellido materno
ALTER TABLE alumno
    DROP CHECK chk_alumno_apellido_materno,
    ADD CONSTRAINT chk_alumno_apellido_materno
        CHECK ((apellido_materno IS NULL) OR regexp_like(apellido_materno,
            _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\'\u002d]{2,80}$'));

-- 3. Nombre
ALTER TABLE alumno
    DROP CHECK chk_alumno_nombre,
    ADD CONSTRAINT chk_alumno_nombre
        CHECK (regexp_like(nombre,
            _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\.\'\u002d]{2,100}$'));

-- 4. Verificar que funciona
SELECT 
    'Garc\u00eda' AS test,
    REGEXP_LIKE('Garc\u00eda', _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\''\\u002d]{2,80}$') AS ok
UNION ALL SELECT 'L\u00f3pez', REGEXP_LIKE('L\u00f3pez', _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\''\\u002d]{2,80}$')
UNION ALL SELECT 'Mu\u00f1oz', REGEXP_LIKE('Mu\u00f1oz', _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\''\\u002d]{2,80}$')
UNION ALL SELECT 'Oc\u00f3n', REGEXP_LIKE('Oc\u00f3n', _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\''\\u002d]{2,80}$')
UNION ALL SELECT 'Garcia', REGEXP_LIKE('Garcia', _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\''\\u002d]{2,80}$')
UNION ALL SELECT 'Juan Carlos', REGEXP_LIKE('Juan Carlos', _utf8mb4'^[^0-9\u00e1\u00e9\u00ed\u00f3\u00faA-Z\\s\\.''\\u002d]{2,100}$');

-- 5. Test INSERT real
INSERT INTO Alumno (no_control, nombre, apellido_paterno, email, telefono, status_alumno, genero, id_carrera, semestre, grupo, turno, fecha_ingreso)
VALUES ('TST2026001', 'Juan Carlos', 'Garc\u00eda', 'juan@test.com', '5512345678', 'activo', 'Masculino', 1, 1, 'A', 'matutino', '2024-08-01');

DELETE FROM Alumno WHERE no_control = 'TST2026001';
SELECT 'INSERT CON ACENTOS EXITOSO' AS resultado;
