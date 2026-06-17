-- ============================================
-- DIAGNÓSTICO DE BD - Modelo Dual
-- ============================================
USE modelodualdb;

SELECT '=== TABLAS ===' AS info;
SHOW TABLES;

SELECT '=== ESTRUCTURA Alumno ===' AS info;
SHOW COLUMNS FROM Alumno;

SELECT '=== ESTRUCTURA Usuario ===' AS info;
SHOW COLUMNS FROM Usuario;

SELECT '=== ESTRUCTURA Empresa ===' AS info;
SHOW COLUMNS FROM Empresa;

SELECT '=== ESTRUCTURA Profesor ===' AS info;
SHOW COLUMNS FROM Profesor;

SELECT '=== ESTRUCTURA Materia ===' AS info;
SHOW COLUMNS FROM Materia;

SELECT '=== ESTRUCTURA Tema ===' AS info;
SHOW COLUMNS FROM Tema;

SELECT '=== ESTRUCTURA Proyecto ===' AS info;
SHOW COLUMNS FROM Proyecto;

SELECT '=== INDICES ===' AS info;
SELECT TABLE_NAME, INDEX_NAME, COLUMN_NAME FROM information_schema.STATISTICS 
WHERE TABLE_SCHEMA = 'modelodualdb' ORDER BY TABLE_NAME, INDEX_NAME;

SELECT '=== CONSTRAINTS ===' AS info;
SELECT TABLE_NAME, CONSTRAINT_NAME, CONSTRAINT_TYPE 
FROM information_schema.TABLE_CONSTRAINTS 
WHERE TABLE_SCHEMA = 'modelodualdb' ORDER BY TABLE_NAME;

SELECT '=== COLLATION ===' AS info;
SELECT TABLE_NAME, TABLE_COLLATION FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'modelodualdb';
