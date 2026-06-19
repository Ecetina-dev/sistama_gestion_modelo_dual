# Auditoría Login ↔ Base de Datos

**Fecha:** 2026-06-18
**Componentes auditados:** AuthController.cs, Models/Usuario.cs, Models/Sesion.cs, Views/FormLogin.cs, Views/FormActivarCuenta.cs, ParametroSistemaService.cs, SistemaTests/Program.cs, stored procedures de MySQL.

---

## Resumen Ejecutivo

| Categoría | Estado | Detalle |
|-----------|--------|---------|
| Modelos C# ↔ columnas DB | ✅ ALINEADO | Todos los campos del modelo existen en DB |
| Queries inline en C# | ✅ ALINEADO | Todas las queries usan columnas existentes |
| Stored procedures | 🔴 1 bug encontrado | `sp_Crear_Usuario` usaba tabla legacy |
| `Usuario_Entidad` legacy | ✅ Migrado | C# ya no la referencia (excepto tests de cleanup) |
| Enums (status) | ✅ ALINEADO | C# usa strings hardcoded matching los CHECK constraints |
| Parametros del sistema | ✅ ALINEADO | Las 7 claves de `Claves` existen en `parametro_sistema` |
| `ResultadoLogin` model | ✅ ALINEADO | Todos los flags se setean en el controller |
| Tests de sistema | ⚠️ Cleanup legacy | Borra de `Usuario_Entidad` (no destructivo, la tabla existe) |

---

## Verificación 1: Modelo Usuario vs DB

### Columnas en `Usuario` (MySQL)
```
id_usuario, username, email, password_hash, id_rol, status, ultimo_login,
intentos_fallidos, bloqueado_hasta, created_at, is_deleted, deleted_at,
deleted_by, updated_at, debe_cambiar_password, password_temporal_hash,
fecha_activacion, creado_por
```

### Propiedades en `Usuario` (C#)
```
Id_Usuario, Username, Email, Password*, PasswordHash, Id_Rol, Status,
Ultimo_Login, Intentos_Fallidos, Bloqueado_Hasta, Created_At, Updated_At,
Debe_Cambiar_Password, Password_Temporal_Hash, Fecha_Activacion, Creado_Por,
Is_Deleted, Deleted_At, Deleted_By
```

✅ **Resultado:** Todas las columnas DB están mapeadas en el modelo. `Password` (C#) es solo para validación/UI, no se persiste.

---

## Verificación 2: Modelo Sesion vs DB

### Columnas en `Sesion` (MySQL)
```
id_sesion, id_usuario, fecha_inicio, fecha_expiracion, ip_address,
user_agent, status
```

### Propiedades en `Sesion` (C#)
```
Id_Sesion, Id_Usuario, Fecha_Inicio, Fecha_Expiracion, Ip_Address,
User_Agent, Status
```

✅ **Resultado:** Totalmente alineado.

---

## Verificación 3: Modelo Rol/Privilegio vs DB

### `rol` (MySQL)
```
id_rol, nombre, descripcion, created_at, updated_at
```

### `Rol` (C#)
```
Id_Rol, Nombre, Descripcion, Created_At, Updated_At
```

✅ **Resultado:** Alineado.

### `privilegio` (MySQL)
```
id_privilegio, nombre, descripcion, created_at
```

### `Privilegio` (C#)
```
Id_Privilegio, Nombre, Descripcion, Created_At
```

✅ **Resultado:** Alineado.

---

## Verificación 4: Queries inline de `AuthController`

### `ValidarCredenciales` (línea 44)
```sql
SELECT u.id_usuario, u.username, u.email, u.password_hash, 
       u.id_rol, u.status, u.intentos_fallidos, u.bloqueado_hasta,
       u.is_deleted, u.debe_cambiar_password, u.fecha_activacion,
       u.deleted_at,
       r.nombre AS rol_nombre
FROM Usuario u
INNER JOIN Rol r ON u.id_rol = r.id_rol
WHERE u.username = @login OR u.email = @login
```
✅ Todas las columnas existen en `Usuario` y `Rol`.

### `CrearUsuario` (línea 255)
```sql
INSERT INTO Usuario (username, email, password_hash, id_rol, status)
VALUES (@username, @email, @password_hash, @id_rol, 'activo')
```
✅ Columnas válidas.

### `CambiarPassword` (líneas 324, 338)
```sql
SELECT password_hash FROM Usuario WHERE id_usuario = @id_usuario
UPDATE Usuario SET password_hash = @hash WHERE id_usuario = @id_usuario
```
✅ Columnas válidas.

### `ActivarCuenta` (línea 923)
```sql
SELECT id_usuario, username, email, password_temporal_hash, 
       status, intentos_fallidos, bloqueado_hasta
FROM Usuario
WHERE LOWER(username) = LOWER(@username) AND is_deleted = 0
```
✅ Columnas válidas.

### `ActivarCuenta` UPDATE (línea ~1023)
```sql
UPDATE Usuario
SET password_hash = @nuevo_hash,
    password_temporal_hash = NULL,
    debe_cambiar_password = FALSE,
    fecha_activacion = NOW(),
    intentos_fallidos = 0,
    bloqueado_hasta = NULL
WHERE id_usuario = @id
```
✅ Columnas válidas.

### `ResetearPassword` (línea ~1100)
```sql
UPDATE Usuario
SET password_temporal_hash = @temp_hash,
    password_hash = @temp_hash,
    debe_cambiar_password = TRUE,
    fecha_activacion = NULL
WHERE id_usuario = @id
```
✅ Columnas válidas.

### `ObtenerUsuarioPorId` (línea 390)
```sql
SELECT id_usuario, username, email, id_rol, status, ultimo_login, created_at,
       debe_cambiar_password, fecha_activacion
FROM Usuario WHERE id_usuario = @id_usuario
```
✅ Columnas válidas.

### `ObtenerPrivilegios` (línea 421)
```sql
SELECT p.nombre 
FROM Privilegio p
INNER JOIN Rol_Privilegio rp ON p.id_privilegio = rp.id_privilegio
WHERE rp.id_rol = @id_rol
```
✅ Columnas y tablas válidas.

### `ObtenerEntidadVinculada` (línea 446)
```sql
SELECT id_alumno FROM Usuario_Alumno WHERE id_usuario = @id LIMIT 1
SELECT id_profesor FROM Usuario_Profesor WHERE id_usuario = @id LIMIT 1
SELECT id_empresa FROM Usuario_Empresa WHERE id_usuario = @id LIMIT 1
```
✅ Usa las nuevas tablas de vinculo (no `Usuario_Entidad`).

### `CrearSesion` (línea 519)
```sql
INSERT INTO Sesion (id_sesion, id_usuario, fecha_inicio, fecha_expiracion, ip_address, user_agent, status)
VALUES (@token, @id_usuario, NOW(), @expiracion, @ip, @user_agent, 'activa')
```
✅ Columnas válidas.

### `CerrarSesion` (línea ~580)
```sql
UPDATE Sesion SET status = 'cerrada' WHERE id_usuario = @id_usuario AND status = 'activa'
UPDATE Usuario SET intentos_fallidos = 0, bloqueado_hasta = NULL WHERE id_usuario = @id
```
✅ Columnas válidas.

### `InsertarBitacora` (línea 541)
```sql
INSERT INTO bitacora (tabla_afectada, id_registro, operacion, usuario, ip_address, navegador, fecha)
VALUES (@tabla, @id_registro, @operacion, @usuario, @ip, @navegador, NOW())
```
✅ Columnas válidas.

### `IncrementarIntentosFallidos` (línea 482)
```sql
UPDATE Usuario SET intentos_fallidos = @intentos, bloqueado_hasta = @bloqueo 
WHERE id_usuario = @id_usuario
```
✅ Columnas válidas.

### `ActualizarUltimoLogin` (línea 509)
```sql
UPDATE Usuario SET ultimo_login = NOW(), intentos_fallidos = 0, bloqueado_hasta = NULL 
WHERE id_usuario = @id_usuario
```
✅ Columnas válidas.

### `CargarUsuario` (línea 821)
```sql
INSERT INTO Usuario (username, email, password_hash, id_rol, status,
                     debe_cambiar_password, password_temporal_hash, creado_por)
VALUES (@username, @email, @temp_hash, @id_rol, 'activo',
        TRUE, @temp_hash, @creado_por)
```
✅ Columnas válidas.

---

## Verificación 5: `parametro_sistema` ↔ `Claves`

| Clave en C# (`Claves.X`) | Existe en DB | Valor | Default en C# (`Seguridad.X`) |
|--------------------------|--------------|-------|-------------------------------|
| `MAX_INTENTOS_LOGIN` | ✅ | 5 | `MaxIntentosLogin` |
| `TIEMPO_BLOQUEO_MINUTOS` | ✅ | 1 | `MinutosBloqueo` |
| `MIN_CARACTERES_PASSWORD` | ✅ | 8 | (n/a) |
| `EXIGIR_MAYUSCULAS_PASSWORD` | ✅ | 1 | (n/a) |
| `EXIGIR_NUMEROS_PASSWORD` | ✅ | 1 | (n/a) |
| `EXIGIR_CARACTER_ESPECIAL_PASSWORD` | ✅ | 1 | (n/a) |
| `DIAS_CADUCIDAD_PASSWORD` | ✅ | 90 | `DiasCaducidadPassword` |

✅ **Resultado:** Las 7 claves están sincronizadas con la tabla.

---

## Verificación 6: Status strings

| C# usa | DB permite (CHECK) |
|--------|---------------------|
| `"activo"` | ✅ activo |
| `"inactivo"` | ✅ inactivo |
| `"suspendido"` | ✅ suspendido |

✅ **Resultado:** Todos los status strings usados en C# están permitidos por los CHECK constraints.

---

## 🔴 Issues encontrados y arreglados

### Issue #1: `sp_Crear_Usuario` usaba tabla legacy

**Antes:**
```sql
INSERT INTO Usuario_Entidad (id_usuario, tipo_entidad, id_entidad) VALUES (...)
```

**Después (Fix_sp_Crear_Usuario.sql):**
```sql
IF p_tipo_entidad = 'alumno' THEN
    INSERT INTO Usuario_Alumno (id_usuario, id_alumno) VALUES (...);
ELSEIF p_tipo_entidad = 'profesor' THEN
    INSERT INTO Usuario_Profesor (id_usuario, id_profesor) VALUES (...);
ELSEIF p_tipo_entidad = 'empresa' THEN
    INSERT INTO Usuario_Empresa (id_usuario, id_empresa) VALUES (...);
END IF;
```

✅ **Aplicado y verificado:** La nueva versión inserta en la tabla correcta.

---

## 🟢 Tests funcionales recomendados

Para verificar el flujo completo de login contra el estado actual de la BD:

1. **Login admin** (`admin` / password del script original)
   - ✅ Verifica SELECT principal funciona
   - ✅ Verifica JOIN con `Rol` funciona
   - ✅ Verifica `ObtenerPrivilegios` devuelve los privilegios correctos
   - ✅ Verifica `ObtenerEntidadVinculada` no devuelve nada (admin sin entidad)
   - ✅ Verifica `InsertarBitacora` registra el login

2. **Login alumno vinculado** (`maria23` / su password)
   - ✅ Verifica `ObtenerEntidadVinculada` devuelve `("alumno", 2)`
   - ✅ Verifica carga de privilegios
   - ✅ Verifica creación de sesión

3. **Activación de cuenta**
   - Admin crea usuario → inserta en `Usuario_Alumno` (no legacy)
   - Usuario activa con password temporal
   - `debe_cambiar_password` pasa a FALSE
   - `fecha_activacion` se setea con NOW()

4. **Bloqueo por intentos**
   - 5 intentos fallidos → `intentos_fallidos = 5`
   - `bloqueado_hasta` se setea con NOW() + `TIEMPO_BLOQUEO_MINUTOS`
   - 6to intento antes del desbloqueo → mensaje de bloqueo

---

## 📊 Conclusión

**El sistema de login está completamente alineado con la base de datos** después de las correcciones de esta sesión:

- ✅ Todos los modelos C# matchean las columnas DB
- ✅ Todas las queries inline de `AuthController` usan columnas existentes
- ✅ Todos los status strings son válidos según CHECK constraints
- ✅ Todos los parámetros del sistema existen en la tabla
- ✅ El bug del stored procedure `sp_Crear_Usuario` fue arreglado
- ✅ Las nuevas tablas de vinculo se usan correctamente en todo el código
