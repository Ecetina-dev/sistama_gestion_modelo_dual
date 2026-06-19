-- ============================================================
-- FIX PART 4: Missing FK indexes (sin IF NOT EXISTS)
-- ============================================================

USE modelodualdb;

-- idx_tema_id_materia
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Tema' AND INDEX_NAME = 'idx_tema_id_materia') = 0,
    'CREATE INDEX idx_tema_id_materia ON Tema(id_materia)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_alumno_empresa_id_alumno
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno_Empresa' AND INDEX_NAME = 'idx_alumno_empresa_id_alumno') = 0,
    'CREATE INDEX idx_alumno_empresa_id_alumno ON Alumno_Empresa(id_alumno)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_alumno_empresa_id_empresa
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno_Empresa' AND INDEX_NAME = 'idx_alumno_empresa_id_empresa') = 0,
    'CREATE INDEX idx_alumno_empresa_id_empresa ON Alumno_Empresa(id_empresa)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_proyecto_materia_id_proyecto
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Proyecto_Materia' AND INDEX_NAME = 'idx_proyecto_materia_id_proyecto') = 0,
    'CREATE INDEX idx_proyecto_materia_id_proyecto ON Proyecto_Materia(id_proyecto)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_proyecto_materia_id_materia
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Proyecto_Materia' AND INDEX_NAME = 'idx_proyecto_materia_id_materia') = 0,
    'CREATE INDEX idx_proyecto_materia_id_materia ON Proyecto_Materia(id_materia)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_proyecto_profesor_id_proyecto
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Proyecto_Profesor' AND INDEX_NAME = 'idx_proyecto_profesor_id_proyecto') = 0,
    'CREATE INDEX idx_proyecto_profesor_id_proyecto ON Proyecto_Profesor(id_proyecto)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_proyecto_profesor_id_profesor
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Proyecto_Profesor' AND INDEX_NAME = 'idx_proyecto_profesor_id_profesor') = 0,
    'CREATE INDEX idx_proyecto_profesor_id_profesor ON Proyecto_Profesor(id_profesor)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_alumno_proyecto_id_alumno
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno_Proyecto' AND INDEX_NAME = 'idx_alumno_proyecto_id_alumno') = 0,
    'CREATE INDEX idx_alumno_proyecto_id_alumno ON Alumno_Proyecto(id_alumno)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_alumno_proyecto_id_proyecto
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Alumno_Proyecto' AND INDEX_NAME = 'idx_alumno_proyecto_id_proyecto') = 0,
    'CREATE INDEX idx_alumno_proyecto_id_proyecto ON Alumno_Proyecto(id_proyecto)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_rol_privilegio_id_rol
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Rol_Privilegio' AND INDEX_NAME = 'idx_rol_privilegio_id_rol') = 0,
    'CREATE INDEX idx_rol_privilegio_id_rol ON Rol_Privilegio(id_rol)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- idx_rol_privilegio_id_privilegio
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.STATISTICS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'Rol_Privilegio' AND INDEX_NAME = 'idx_rol_privilegio_id_privilegio') = 0,
    'CREATE INDEX idx_rol_privilegio_id_privilegio ON Rol_Privilegio(id_privilegio)',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

SELECT '✅ Índices en FKs agregados' AS Status;
