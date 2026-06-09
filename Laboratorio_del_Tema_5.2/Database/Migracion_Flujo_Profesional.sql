-- =====================================================
-- MIGRACION: Flujo profesional de activacion de cuentas
-- =====================================================
-- Este script agrega los campos necesarios para que el
-- sistema funcione con el flujo profesional:
-- 1. Admin carga datos del usuario (matricula, datos)
-- 2. Sistema genera password temporal
-- 3. Usuario activa su cuenta con el password temporal
-- 4. Usuario establece su password definitivo
-- =====================================================

USE ModeloDualDB;

-- Verificar si las columnas ya existen antes de agregarlas
SET @col_debe_cambiar := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'ModeloDualDB'
    AND TABLE_NAME = 'Usuario'
    AND COLUMN_NAME = 'debe_cambiar_password');

SET @col_pwd_temp := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'ModeloDualDB'
    AND TABLE_NAME = 'Usuario'
    AND COLUMN_NAME = 'password_temporal_hash');

SET @col_fecha_activ := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'ModeloDualDB'
    AND TABLE_NAME = 'Usuario'
    AND COLUMN_NAME = 'fecha_activacion');

SET @col_creado_por := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_SCHEMA = 'ModeloDualDB'
    AND TABLE_NAME = 'Usuario'
    AND COLUMN_NAME = 'creado_por');

-- Agregar columna debe_cambiar_password si no existe
SET @sql = IF(@col_debe_cambiar = 0,
    'ALTER TABLE Usuario ADD COLUMN debe_cambiar_password BOOLEAN DEFAULT TRUE',
    'SELECT "Columna debe_cambiar_password ya existe" AS info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Agregar columna password_temporal_hash si no existe
SET @sql = IF(@col_pwd_temp = 0,
    'ALTER TABLE Usuario ADD COLUMN password_temporal_hash VARCHAR(255) NULL',
    'SELECT "Columna password_temporal_hash ya existe" AS info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Agregar columna fecha_activacion si no existe
SET @sql = IF(@col_fecha_activ = 0,
    'ALTER TABLE Usuario ADD COLUMN fecha_activacion DATETIME NULL',
    'SELECT "Columna fecha_activacion ya existe" AS info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Agregar columna creado_por si no existe
SET @sql = IF(@col_creado_por = 0,
    'ALTER TABLE Usuario ADD COLUMN creado_por INT NULL',
    'SELECT "Columna creado_por ya existe" AS info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Intentar agregar FK a creado_por (puede fallar si ya existe)
SET @fk_existe := (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = 'ModeloDualDB'
    AND TABLE_NAME = 'Usuario'
    AND CONSTRAINT_NAME = 'fk_usuario_creado_por');

SET @sql = IF(@fk_existe = 0,
    'ALTER TABLE Usuario ADD CONSTRAINT fk_usuario_creado_por FOREIGN KEY (creado_por) REFERENCES Usuario(id_usuario)',
    'SELECT "FK fk_usuario_creado_por ya existe" AS info');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Marcar usuarios existentes como ya activados (no deben cambiar password)
-- Esto preserva el usuario admin y cualquier otro que ya exista
UPDATE Usuario
SET debe_cambiar_password = FALSE,
    fecha_activacion = COALESCE(fecha_activacion, created_at, NOW())
WHERE fecha_activacion IS NOT NULL OR username = 'admin';

-- Si el admin no tiene fecha_activacion, ponersela
UPDATE Usuario
SET fecha_activacion = NOW()
WHERE username = 'admin' AND fecha_activacion IS NULL;

-- Verificar resultado
SELECT '=== USUARIOS ACTIVOS ===' AS estado;
SELECT id_usuario, username, email, status,
       debe_cambiar_password, fecha_activacion, creado_por
FROM Usuario;

SELECT '=== MIGRACION COMPLETADA ===' AS resultado;
