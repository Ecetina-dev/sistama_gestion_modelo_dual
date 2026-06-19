-- ============================================================
-- FIX: Ampliar CHECK constraint de bitacora.operacion
-- El C# usa LOGIN, ACTIVAR_CUENTA pero el CHECK solo permite
-- INSERT/UPDATE/DELETE/RESTORE, rompiendo la auditoria silenciosamente
-- ============================================================

USE modelodualdb;

-- Drop + recreate del CHECK con valores ampliados
SET @sql = (SELECT IF(
    (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
     WHERE TABLE_SCHEMA = 'modelodualdb' AND TABLE_NAME = 'bitacora' AND CONSTRAINT_NAME = 'chk_bitacora_operacion') > 0,
    'ALTER TABLE bitacora DROP CHECK chk_bitacora_operacion',
    'SELECT 1'));
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

-- Ampliar columna operacion a VARCHAR(20) (ACTIVAR_CUENTA = 14 chars)
ALTER TABLE bitacora MODIFY COLUMN operacion VARCHAR(25) NOT NULL;

ALTER TABLE bitacora ADD CONSTRAINT chk_bitacora_operacion
    CHECK (operacion IN (
        'INSERT','UPDATE','DELETE','RESTORE',
        'LOGIN','LOGOUT','LOGIN_FALLIDO',
        'ACTIVAR_CUENTA','RESET_PASSWORD','CAMBIO_PASSWORD',
        'BLOQUEO','DESBLOQUEO','CAMBIO_STATUS',
        'CREAR_USUARIO','ELIMINAR_USUARIO','ACCESO_MODULO'
    ));

SELECT '✅ CHECK de bitacora ampliado con operacion de auth' AS Status;
