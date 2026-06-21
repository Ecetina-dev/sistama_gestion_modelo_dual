-- ============================================================
-- MOCK DATA: Carreras de ejemplo
-- Fecha: 2026-06-21
-- Proposito: Poblar el catalogo de carreras para evitar que
--            el combo de FormAlumnos aparezca vacio en entornos
--            de desarrollo/pruebas.
--
-- Nota: El script es idempotente (INSERT IGNORE + UPDATE).
-- ============================================================

USE modelodualdb;

INSERT IGNORE INTO Carrera (clave, nombre, duracion_semestres, status, created_at, updated_at)
VALUES
    ('ING-SIS', 'Ingeniería en Sistemas Computacionales', 9, 'activa', NOW(), NOW()),
    ('ING-IND', 'Ingeniería Industrial', 9, 'activa', NOW(), NOW()),
    ('ING-MEC', 'Ingeniería Mecatrónica', 9, 'activa', NOW(), NOW()),
    ('ING-CIV', 'Ingeniería Civil', 9, 'activa', NOW(), NOW()),
    ('LIC-ADM', 'Licenciatura en Administración', 8, 'activa', NOW(), NOW()),
    ('LIC-CON', 'Licenciatura en Contaduría', 8, 'activa', NOW(), NOW()),
    ('LIC-DER', 'Licenciatura en Derecho', 8, 'activa', NOW(), NOW()),
    ('LIC-PSI', 'Licenciatura en Psicología', 8, 'activa', NOW(), NOW()),
    ('ING-AMB', 'Ingeniería Ambiental', 9, 'activa', NOW(), NOW()),
    ('ING-QUI', 'Ingeniería Química', 9, 'activa', NOW(), NOW());

-- Asegurar que todas las carreras de mock data esten activas
UPDATE Carrera SET status = 'activa', updated_at = NOW()
WHERE clave IN ('ING-SIS','ING-IND','ING-MEC','ING-CIV','LIC-ADM','LIC-CON','LIC-DER','LIC-PSI','ING-AMB','ING-QUI');

SELECT '✅ Carreras de ejemplo cargadas' AS Status;
SELECT id_carrera, clave, nombre, duracion_semestres, status FROM Carrera WHERE status = 'activa' ORDER BY nombre;
