-- ============================================================
-- FIX: sp_Crear_Usuario - usar nuevas tablas de vinculo
-- Antes: insertaba en Usuario_Entidad (polimórfica)
-- Ahora: inserta en Usuario_Alumno / Usuario_Profesor / Usuario_Empresa
-- ============================================================

USE modelodualdb;

DROP PROCEDURE IF EXISTS sp_Crear_Usuario;

DELIMITER $$

CREATE PROCEDURE sp_Crear_Usuario(
    IN p_username VARCHAR(50),
    IN p_email VARCHAR(120),
    IN p_password_plain VARCHAR(255),
    IN p_id_rol INT,
    IN p_tipo_entidad VARCHAR(10),
    IN p_id_entidad INT
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        SELECT 'Error al crear usuario' AS resultado;
    END;

    START TRANSACTION;

    -- Insertar usuario (el password se debe pasar ya hasheado desde la app)
    INSERT INTO Usuario (username, email, password_hash, id_rol)
    VALUES (p_username, p_email, p_password_plain, p_id_rol);

    -- Vincular con entidad (tabla especifica segun el tipo, FK real)
    IF p_tipo_entidad IS NOT NULL AND p_id_entidad IS NOT NULL THEN
        IF p_tipo_entidad = 'alumno' THEN
            INSERT INTO Usuario_Alumno (id_usuario, id_alumno)
            VALUES (LAST_INSERT_ID(), p_id_entidad);
        ELSEIF p_tipo_entidad = 'profesor' THEN
            INSERT INTO Usuario_Profesor (id_usuario, id_profesor)
            VALUES (LAST_INSERT_ID(), p_id_entidad);
        ELSEIF p_tipo_entidad = 'empresa' THEN
            INSERT INTO Usuario_Empresa (id_usuario, id_empresa)
            VALUES (LAST_INSERT_ID(), p_id_entidad);
        END IF;
    END IF;

    COMMIT;
    SELECT 'Usuario creado exitosamente' AS resultado;
END$$

DELIMITER ;
