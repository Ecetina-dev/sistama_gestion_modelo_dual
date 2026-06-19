-- ============================================================
-- FIX ENTERPRISE DB ISSUES - VERSION SIN PROCEDURES
-- Se ejecuta en partes porque el DELIMITER no funciona por stdin
-- ============================================================

USE modelodualdb;

-- ============================================================
-- ISSUE #1: Fix polymorphic Usuario_Entidad
-- ============================================================

CREATE TABLE IF NOT EXISTS Usuario_Alumno (
    id_usuario INT NOT NULL,
    id_alumno INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_usuario, id_alumno),
    UNIQUE KEY uk_alumno_vinc (id_alumno),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    FOREIGN KEY (id_alumno) REFERENCES Alumno(id_alumno) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Usuario_Profesor (
    id_usuario INT NOT NULL,
    id_profesor INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_usuario, id_profesor),
    UNIQUE KEY uk_profesor_vinc (id_profesor),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    FOREIGN KEY (id_profesor) REFERENCES Profesor(id_profesor) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS Usuario_Empresa (
    id_usuario INT NOT NULL,
    id_empresa INT NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_usuario, id_empresa),
    UNIQUE KEY uk_empresa_vinc (id_empresa),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE,
    FOREIGN KEY (id_empresa) REFERENCES Empresa(id_empresa) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT IGNORE INTO Usuario_Alumno (id_usuario, id_alumno, created_at)
SELECT id_usuario, id_entidad, created_at
FROM Usuario_Entidad WHERE tipo_entidad = 'alumno';

INSERT IGNORE INTO Usuario_Profesor (id_usuario, id_profesor, created_at)
SELECT id_usuario, id_entidad, created_at
FROM Usuario_Entidad WHERE tipo_entidad = 'profesor';

INSERT IGNORE INTO Usuario_Empresa (id_usuario, id_empresa, created_at)
SELECT id_usuario, id_entidad, created_at
FROM Usuario_Entidad WHERE tipo_entidad = 'empresa';

SELECT '✅ Tablas Usuario_*_Entidad creadas y pobladas' AS Status;
