-- =====================================================
-- Script para corregir errores de ortografia en MySQL
-- Ejecutar en MySQL Workbench o linea de comandos
-- =====================================================

-- Verificar si existen las columnas con errores ortograficos
-- Tabla Alumno
-- DESCRIBE Alumno;

-- Corregir columna 'apellido_patermo' -> 'apellido_paterno'
-- Corregir columna 'apellido_matemo' -> 'apellido_materno'

-- IMPORTANTE: Ejecutar solo si las columnas con errores existen
-- Primero verificar con: DESCRIBE Alumno;

-- Si existe la columna mal escrita, ejecutar:
-- ALTER TABLE Alumno CHANGE COLUMN apellido_patermo apellido_paterno VARCHAR(100) NOT NULL;
-- ALTER TABLE Alumno CHANGE COLUMN apellido_matemo apellido_materno VARCHAR(100) NULL;

-- Verificar si existen las columnas con errores ortograficos
-- Tabla Profesor
-- DESCRIBE Profesor;

-- Corregir columna 'apellido_patermo' -> 'apellido_paterno'
-- Corregir columna 'apellido_matemo' -> 'apellido_materno'

-- IMPORTANTE: Ejecutar solo si las columnas con errores existen
-- Primero verificar con: DESCRIBE Profesor;

-- Si existe la columna mal escrita, ejecutar:
-- ALTER TABLE Profesor CHANGE COLUMN apellido_patermo apellido_paterno VARCHAR(100) NOT NULL;
-- ALTER TABLE Profesor CHANGE COLUMN apellido_matemo apellido_materno VARCHAR(100) NULL;

-- =====================================================
-- Script de verificacion - NO MODIFICAR
-- =====================================================

-- Verificar que todas las columnas esperadas existen
SELECT 'Verificando estructura de Alumno...' AS mensaje;
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'ModeloDualDB' 
AND TABLE_NAME = 'Alumno'
ORDER BY ORDINAL_POSITION;

SELECT 'Verificando estructura de Profesor...' AS mensaje;
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'ModeloDualDB' 
AND TABLE_NAME = 'Profesor'
ORDER BY ORDINAL_POSITION;

-- Verificar si hay valores nulos en created_at (si existe la columna)
-- SELECT COUNT(*) AS registros_sin_created_at FROM Alumno WHERE created_at IS NULL;