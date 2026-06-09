-- =====================================================
-- SCRIPT DE MIGRACION: Sistema de Autenticacion
-- Modelo Dual - Usuario, Roles y Privilegios
-- =====================================================

USE ModeloDualDB;

-- =====================================================
-- TABLA: Rol
-- =====================================================
CREATE TABLE IF NOT EXISTS Rol (
    id_rol INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(50) NOT NULL UNIQUE,
    descripcion VARCHAR(200),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Privilegio
-- =====================================================
CREATE TABLE IF NOT EXISTS Privilegio (
    id_privilegio INT PRIMARY KEY AUTO_INCREMENT,
    nombre VARCHAR(80) NOT NULL UNIQUE,
    descripcion VARCHAR(200),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Rol_Privilegio (N:N)
-- =====================================================
CREATE TABLE IF NOT EXISTS Rol_Privilegio (
    id_rol INT NOT NULL,
    id_privilegio INT NOT NULL,
    PRIMARY KEY (id_rol, id_privilegio),
    FOREIGN KEY (id_rol) REFERENCES Rol(id_rol) ON DELETE CASCADE,
    FOREIGN KEY (id_privilegio) REFERENCES Privilegio(id_privilegio) ON DELETE CASCADE
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Usuario
-- =====================================================
CREATE TABLE IF NOT EXISTS Usuario (
    id_usuario INT PRIMARY KEY AUTO_INCREMENT,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(120) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    id_rol INT NOT NULL,
    status ENUM('activo', 'inactivo', 'suspendido') DEFAULT 'activo',
    ultimo_login DATETIME,
    intentos_fallidos INT DEFAULT 0,
    bloqueado_hasta DATETIME,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (id_rol) REFERENCES Rol(id_rol)
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Usuario_Entidad (Vinculo Usuario - Persona)
-- =====================================================
CREATE TABLE IF NOT EXISTS Usuario_Entidad (
    id_usuario INT NOT NULL,
    tipo_entidad ENUM('alumno', 'profesor', 'empresa') NOT NULL,
    id_entidad INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id_usuario, tipo_entidad, id_entidad),
    UNIQUE KEY unique_entidad (tipo_entidad, id_entidad),
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB;

-- =====================================================
-- TABLA: Sesion
-- =====================================================
CREATE TABLE IF NOT EXISTS Sesion (
    id_sesion VARCHAR(64) PRIMARY KEY,
    id_usuario INT NOT NULL,
    fecha_inicio DATETIME NOT NULL,
    fecha_expiracion DATETIME NOT NULL,
    ip_address VARCHAR(45),
    user_agent VARCHAR(255),
    status ENUM('activa', 'expirada', 'cerrada') DEFAULT 'activa',
    FOREIGN KEY (id_usuario) REFERENCES Usuario(id_usuario) ON DELETE CASCADE
) ENGINE=InnoDB;

-- =====================================================
-- DATOS: Roles
-- =====================================================
INSERT INTO Rol (nombre, descripcion) VALUES
('admin', 'Administrador del sistema - acceso total'),
('alumno', 'Alumno - acceso a su informacion y proyectos'),
('profesor', 'Profesor - gestiona alumnos, materias y proyectos'),
('empresa', 'Empresa - gestiona sus proyectos y ve alumnos asignados');

-- =====================================================
-- DATOS: Privilegios
-- =====================================================
INSERT INTO Privilegio (nombre, descripcion) VALUES
-- Alumno
('alumno.ver_propio', 'Ver su propia informacion'),
('alumno.editar_propio', 'Editar su propia informacion'),
('alumno.ver_proyectos', 'Ver sus proyectos asignados'),
-- Profesor
('profesor.crud_alumno', 'Crear, leer, actualizar, eliminar alumnos'),
('profesor.ver_todos_alumnos', 'Ver todos los alumnos'),
('profesor.crud_materia', 'Gestionar materias'),
('profesor.crud_tema', 'Gestionar temas'),
('profesor.ver_proyectos', 'Ver todos los proyectos'),
-- Empresa
('empresa.ver_asignaciones', 'Ver alumnos asignados'),
('empresa.editar_perfil', 'Editar perfil de empresa'),
('empresa.crud_proyecto', 'Gestionar proyectos propios'),
-- Admin
('admin.crud_usuario', 'Gestionar usuarios'),
('admin.crud_rol', 'Gestionar roles y privilegios'),
('admin.crud_todo', 'Acceso total a todas las entidades');

-- =====================================================
-- DATOS: Rol_Privilegio (Asignacion de permisos)
-- =====================================================
-- Admin: todos los privilegios
INSERT INTO Rol_Privilegio (id_rol, id_privilegio)
SELECT 1, id_privilegio FROM Privilegio;

-- Alumno: privilegios propios
INSERT INTO Rol_Privilegio (id_rol, id_privilegio)
SELECT 2, id_privilegio FROM Privilegio WHERE nombre IN ('alumno.ver_propio', 'alumno.editar_propio', 'alumno.ver_proyectos');

-- Profesor: privilegios de gestion
INSERT INTO Rol_Privilegio (id_rol, id_privilegio)
SELECT 3, id_privilegio FROM Privilegio WHERE nombre IN ('alumno.ver_propio', 'profesor.crud_alumno', 'profesor.ver_todos_alumnos', 'profesor.crud_materia', 'profesor.crud_tema', 'profesor.ver_proyectos');

-- Empresa: privilegios de empresa
INSERT INTO Rol_Privilegio (id_rol, id_privilegio)
SELECT 4, id_privilegio FROM Privilegio WHERE nombre IN ('empresa.ver_asignaciones', 'empresa.editar_perfil', 'empresa.crud_proyecto');

-- =====================================================
-- DATOS: Usuario Admin por defecto
-- Password: Admin123* (hash BCrypt)
-- =====================================================
INSERT INTO Usuario (username, email, password_hash, id_rol, status) VALUES
('admin', 'admin@modelodual.edu', '$2a$11$rBVjvgMZQ0vZ3bG8kHJH/.kE8xqU0G1hC0dX0kO9YqD7Z5X4wW2Vy', 1, 'activo');

-- =====================================================
-- VINCULos: Admin linked to admin (no entity)
-- =====================================================
-- Los admins no necesitan entidad vinculada, solo el rol

-- =====================================================
-- PROCEDIMIENTOS ALMACENADOS
-- =====================================================

DELIMITER //

-- Procedure: Crear usuario con contrasena BCrypt
CREATE PROCEDURE sp_Crear_Usuario(
    IN p_username VARCHAR(50),
    IN p_email VARCHAR(120),
    IN p_password_plain VARCHAR(255),
    IN p_id_rol INT,
    IN p_tipo_entidad ENUM('alumno', 'profesor', 'empresa'),
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

    -- Vincular con entidad
    IF p_tipo_entidad IS NOT NULL AND p_id_entidad IS NOT NULL THEN
        INSERT INTO Usuario_Entidad (id_usuario, tipo_entidad, id_entidad)
        VALUES (LAST_INSERT_ID(), p_tipo_entidad, p_id_entidad);
    END IF;

    COMMIT;
    SELECT 'Usuario creado exitosamente' AS resultado;
END //

-- Procedure: Validar credenciales
CREATE PROCEDURE sp_Validar_Credenciales(
    IN p_login VARCHAR(120),
    OUT p_id_usuario INT,
    OUT p_password_hash VARCHAR(255),
    OUT p_id_rol INT,
    OUT p_status VARCHAR(20)
)
BEGIN
    SELECT id_usuario, password_hash, id_rol, status
    INTO p_id_usuario, p_password_hash, p_id_rol, p_status
    FROM Usuario
    WHERE username = p_login OR email = p_login;
END //

-- Procedure: Actualizar ultimo login
CREATE PROCEDURE sp_Actualizar_Ultimo_Login(IN p_id_usuario INT)
BEGIN
    UPDATE Usuario SET ultimo_login = NOW() WHERE id_usuario = p_id_usuario;
END //

DELIMITER ;

-- =====================================================
-- INDICES
-- =====================================================
CREATE INDEX idx_usuario_username ON Usuario(username);
CREATE INDEX idx_usuario_email ON Usuario(email);
CREATE INDEX idx_usuario_rol ON Usuario(id_rol);
CREATE INDEX idx_sesion_usuario ON Sesion(id_usuario);
CREATE INDEX idx_sesion_token ON Sesion(id_sesion);
CREATE INDEX idx_usuario_entidad_tipo ON Usuario_Entidad(tipo_entidad, id_entidad);