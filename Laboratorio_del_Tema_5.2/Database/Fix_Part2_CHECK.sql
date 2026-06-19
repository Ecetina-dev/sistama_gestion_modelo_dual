-- ============================================================
-- FIX PART 2: CHECK constraints (sin procedures)
-- ============================================================

USE modelodualdb;

-- Alumno: constraints
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno' AND CONSTRAINT_NAME = 'chk_alumno_promedio') > 0,
    'ALTER TABLE Alumno DROP CHECK chk_alumno_promedio',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_promedio
    CHECK (promedio_general IS NULL OR (promedio_general >= 0.00 AND promedio_general <= 10.00));

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno' AND CONSTRAINT_NAME = 'chk_alumno_semestre') > 0,
    'ALTER TABLE Alumno DROP CHECK chk_alumno_semestre',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_semestre
    CHECK (semestre IS NULL OR (semestre >= 1 AND semestre <= 20));

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno' AND CONSTRAINT_NAME = 'chk_alumno_curp_len') > 0,
    'ALTER TABLE Alumno DROP CHECK chk_alumno_curp_len',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Alumno ADD CONSTRAINT chk_alumno_curp_len
    CHECK (curp IS NULL OR LENGTH(curp) = 18);

-- Materia: constraints
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Materia' AND CONSTRAINT_NAME = 'chk_materia_creditos') > 0,
    'ALTER TABLE Materia DROP CHECK chk_materia_creditos',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Materia ADD CONSTRAINT chk_materia_creditos
    CHECK (creditos >= 0 AND creditos <= 100);

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Materia' AND CONSTRAINT_NAME = 'chk_materia_semestre') > 0,
    'ALTER TABLE Materia DROP CHECK chk_materia_semestre',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Materia ADD CONSTRAINT chk_materia_semestre
    CHECK (semestre >= 0 AND semestre <= 12);

SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Materia' AND CONSTRAINT_NAME = 'chk_materia_horas') > 0,
    'ALTER TABLE Materia DROP CHECK chk_materia_horas',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Materia ADD CONSTRAINT chk_materia_horas
    CHECK (horas_teoria >= 0 AND horas_teoria <= 20 AND horas_practica >= 0 AND horas_practica <= 20);

-- Proyecto: constraints
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Proyecto' AND CONSTRAINT_NAME = 'chk_proyecto_horas') > 0,
    'ALTER TABLE Proyecto DROP CHECK chk_proyecto_horas',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Proyecto ADD CONSTRAINT chk_proyecto_horas
    CHECK (horas_totales IS NULL OR (horas_totales >= 0 AND horas_totales <= 10000));

-- Carrera: constraints
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Carrera' AND CONSTRAINT_NAME = 'chk_carrera_duracion') > 0,
    'ALTER TABLE Carrera DROP CHECK chk_carrera_duracion',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Carrera ADD CONSTRAINT chk_carrera_duracion
    CHECK (duracion_semestres >= 1 AND duracion_semestres <= 20);

-- Tema: constraints
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Tema' AND CONSTRAINT_NAME = 'chk_tema_numero') > 0,
    'ALTER TABLE Tema DROP CHECK chk_tema_numero',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Tema ADD CONSTRAINT chk_tema_numero
    CHECK (numero_tema >= 1 AND numero_tema <= 999);

-- Usuario: constraints
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Usuario' AND CONSTRAINT_NAME = 'chk_usuario_username') > 0,
    'ALTER TABLE Usuario DROP CHECK chk_usuario_username',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;
ALTER TABLE Usuario ADD CONSTRAINT chk_usuario_username
    CHECK (LENGTH(username) >= 3 AND LENGTH(username) <= 50);

SELECT '✅ CHECK constraints agregados' AS Status;
