-- ============================================================
-- FIX: Tabla password_history
-- Guarda el historial de passwords hasheados de cada usuario
-- para prevenir reutilización de las últimas N contraseñas
-- ============================================================

USE modelodualdb;

CREATE TABLE IF NOT EXISTS password_history (
    id_historial BIGINT NOT NULL AUTO_INCREMENT,
    id_usuario INT NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    fecha_cambio DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_historial),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    INDEX idx_ph_usuario (id_usuario),
    INDEX idx_ph_fecha (fecha_cambio)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

SELECT '✅ Tabla password_history creada' AS Status;
