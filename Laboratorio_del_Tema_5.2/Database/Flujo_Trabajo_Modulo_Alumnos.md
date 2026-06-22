# Flujo de Trabajo Profesional — Módulo Alumnos

**Proyecto:** Sistema Gestión Modelo Dual  
**Versión:** 2.0  
**Estándar:** Empresarial / MX (México)  
**BD:** MySQL — Tabla `alumno` con CHECK constraints

---

## 1. Flujo General del Módulo

```
[FormMenuPrincipal]
     │
     ▼
[FormAlumnos — Al Abrirse]
     │
     ├─ 1. Carga inicial de alumnos (Read)
     ├─ 2. Carga de carreras activas (ReadActivas)
     ├─ 3. Configuración de grid (7 columnas legacy)
     ├─ 4. Aplicación de filtros (por defecto: "Todos")
     ├─ 5. Paginación (50 registros/página)
     └─ 6. Reorganización de layout
```

### 1.1 Estado inicial del formulario
| Elemento | Estado |
|----------|--------|
| Grid (dgvAlumnos) | Visible con datos |
| Card de datos (panelCardDatos) | **Oculto** |
| Filtro de status | "Todos" |
| Campo de búsqueda | Vacío |
| Barra de herramientas | Visible |
| Botón "Ver Empresas" | Mostrar empresas |
| Botón "Nuevo" | Habilitado |
| Botón "Editar" | Habilitado (si hay selección) |
| Botón "Eliminar" | Habilitado (si hay selección) |

### 1.2 Navegación desde sidebar
| Botón | Acción |
|-------|--------|
| ➕ Nuevo | Limpia form, muestra card datos, foco en No. Control |
| ✏️ Editar | Carga datos del alumno seleccionado, muestra card |
| 🗑️ Eliminar | Soft-delete con confirmación + motivo |
| 🔄 Refrescar | Recarga lista completa |
| 👁️ Empresas | Alterna vista alumnos/empresas |
| 📥 Exportar CSV | Exporta datos filtrados a CSV |
| ← Volver al Menú | Cierra formulario |

---

## 2. Flujo de Creación (Nuevo Alumno)

```
[Click "Nuevo"]
     │
     ├─ isNewRecord = true
     ├─ LimpiarFormulario()
     ├─ HabilitarNoControl(true)
     ├─ Mostrar panelCardDatos
     └─ Foco en txtNoControl
          │
          ▼
     [Usuario llena campos]
          │
          ├─ Navegación por TAB (orden: 0→15)
          ├─ Enter → siguiente campo
          ├─ Ctrl+S → Guardar
          └─ Escape → Cancelar
               │
               ▼
          [Click "Guardar"]
               │
               ├─ ValidarFormulario()
               │    ├─ ¿No. Control vacío? → Error
               │    ├─ ¿No. Control ≠ 10 chars? → Error
               │    ├─ ¿No. Control alfanumérico? → Error
               │    ├─ ¿Nombre vacío? → Error
               │    ├─ ¿Nombre < 2 chars? → Error
               │    ├─ ¿Apellido Paterno vacío? → Error
               │    ├─ ¿Email inválido? → Error
               │    ├─ ¿Teléfono ≠ 10 dígitos? → Error
               │    ├─ ¿Fecha Nac. futura? → Error
               │    ├─ ¿CURP vacío (nuevo)? → Error
               │    ├─ ¿CURP inválido? → Error
               │    ├─ ¿Género sin seleccionar? → Error
               │    ├─ ¿Carrera sin seleccionar? → Error
               │    ├─ ¿Turno sin seleccionar? → Error
               │    ├─ ¿Fecha Ingreso futura? → Error
               │    ├─ ¿Fecha Ingreso < 1990? → Error
               │    └─ ¿Semestre fuera 1-20? → Error
               │
               ├─ ¿No. Control duplicado? → Error
               │
               ├─ Construir objeto Alumno
               ├─ controller.Create(alumno) [async]
               │    ├─ ValidarCamposRequeridos
               │    ├─ ValidarFormatos
               │    ├─ ValidarDuplicados (conn)
               │    ├─ BEGIN TRANSACTION
               │    ├─ INSERT INTO Alumno
               │    ├─ SincronizarEmailUsuario
               │    ├─ InsertarBitacora
               │    ├─ COMMIT
               │    └─ (o ROLLBACK en error)
               │
               ├─ ¿Éxito? → Mostrar "Alumno guardado"
               │    ├─ Ocultar card datos
               │    ├─ Recargar lista
               │    └─ Refrescar grid
               │
               └─ ¿Error? → Mostrar mensaje específico
```

---

## 3. Validaciones por Campo (Estándar MX / Empresarial)

### 3.1 No. Control
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | **Exactamente 10** | ✅ `MIN=MAX=10` |
| Caracteres | **Alfanumérico** mayúsculas | ✅ `SoloAlfanumericoMayusculas()` |
| Regex | `^[A-Z0-9]{10}$` | ✅ BD: `chk_alumno_no_control` |
| Tooltip | "10 caracteres alfanuméricos (ej: 2021101001)" | ✅ |
| Requerido | Sí | ✅ |
| BD VARCHAR | `VARCHAR(15)` | ✅ (pero CHECK exige length=10) |

### 3.2 Nombre
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | **2-100 caracteres** | ✅ `MAX_NOMBRE=100` |
| Caracteres | **Solo letras** + acentos + espacio | ✅ `SoloLetrasConAcentos()` |
| Números | ❌ **No permitidos** | ✅ |
| Puntuación | ❌ **No permitida** (. , - ' etc) | ✅ — Quitado el `.` del validador |
| Regex BD | `^[^0-9[:punct:][:digit:]]{2,100}$` | ✅ |
| Requerido | Sí | ✅ |

### 3.3 Apellido Paterno
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | **2-80 caracteres** | ✅ `MAX_APELLIDO=80` |
| Caracteres | Solo letras + acentos | ✅ `SoloLetrasConAcentos()` |
| Requerido | Sí | ✅ |
| Regex BD | `{2,80}$` sin números/puntuación | ✅ |

### 3.4 Apellido Materno
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | 0-80 (opcional) | ✅ |
| Caracteres | Solo letras + acentos | ✅ |
| Regex BD | Nullable, si tiene valor: `{2,80}$` | ✅ |

### 3.5 CURP (Clave Única de Registro de Población)
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | **Exactamente 18** | ✅ `txtCURP.MaxLength=18` |
| Formato | `^[A-Z]{4}[0-9]{6}[H,M][A-Z]{6}[0-9]{2}$` | ✅ `AlumnoConfig.CurpPattern` |
| Checksum | Dígito verificador (17° + 18°) | ✅ `ValidarChecksumCurp()` |
| Mayúsculas | **Obligatorio** | ✅ `CharacterCasing.Upper` |
| Regex BD | `length(curp)=18` | ✅ |
| Requerido | **Sí** (nuevos) / Opcional (edición) | ✅ |
| Unicidad | Sí (con soft-delete reserve) | ✅ `uk_alumno_curp` + `ExisteCurpEliminado` |
| Ejemplo | `GOMC800101HNENSN09` | ✅ |

### 3.6 Email
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | **Máx 254** (RFC 5321) | ✅ `MAX_EMAIL=254` |
| Formato | Regex email estándar | ✅ |
| BD VARCHAR | `VARCHAR(120)` | ✅ |
| Unicidad | **Sí** (con soft-delete reserve) | ✅ `uk_alumno_email` |
| Requerido | **Sí** (nuevos) / Opcional (edición) | ✅ |
| BD nullable | Sí | ✅ `chk_alumno_email: ((email is null) or regexp_like(...))` |

### 3.7 Teléfono
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | **Exactamente 10 dígitos** | ✅ `MAX_TELEFONO=10` |
| Formato | `^[0-9]{10}$` (2-3 dígitos área + 7-8 dígitos) | ✅ `SoloDigitos()` |
| Regex BD | `{10}$` con `^[0-9]{10}$` | ✅ `chk_alumno_telefono` |
| BD VARCHAR | `VARCHAR(15)` | ✅ (con CHECK length=10) |
| Requerido | Opcional | ✅ |
| Ejemplo | `5551234567` (CDMX) / `6621234567` (Sonora) | ✅ |

### 3.8 Género
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Valores | `Masculino, Femenino, No binario, Prefiero no decir` | ✅ `Genero.*` constantes |
| BD CHECK | `('Masculino','Femenino','No binario','Prefiero no decir')` | ✅ |
| Requerido | **Sí** (nuevos) | ✅ |
| UI | Combo dropdown con placeholder `(Sin especificar)` | ✅ |

### 3.9 Turno
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Valores | `Matutino, Vespertino, Nocturno, Mixto` | ✅ |
| BD CHECK | Minúsculas: `'matutino','vespertino','nocturno','mixto'` | ✅ `ToLowerInvariant()` |
| Requerido | **Sí** (nuevos) | ✅ |

### 3.10 Carrera
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Fuente | Catálogo activo (`ReadActivas()`) | ✅ |
| FK | `fk_alumno_carrera` | ✅ |
| Validación existencia | Controller verifica antes de insertar | ✅ |
| Requerido | **Sí** (nuevos) | ✅ |

### 3.11 Semestre
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Rango | **1-20** | ✅ `nudSemestre` min=1 max=20 |
| BD CHECK | `semestre >= 1 AND semestre <= 20` | ✅ |
| Requerido | **Sí** (nuevos) | ✅ |

### 3.12 Fecha de Ingreso
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Rango | **01/01/1990 a hoy** | ✅ |
| Formato | `dd/MM/yyyy` | ✅ |
| Requerido | **Sí** (nuevos) | ✅ |
| No futura | ✅ | ✅ |

### 3.13 Grupo
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Longitud | Máx 10 | ✅ `txtGrupo.MaxLength=10` |
| Requerido | No | ✅ |
| BD VARCHAR | `VARCHAR(20)` | ✅ |

### 3.14 Fecha de Nacimiento
| Aspecto | Estándar MX | Implementación |
|---------|------------|----------------|
| Rango | **Año ≥ 1900, no futura** | ✅ `MinDate=now-120a` |
| Requerido | No | ✅ datepicker checkbox |
| BD CHECK | `year(fecha_nacimiento) >= 1900` | ✅ |

---

## 4. Flujo de Edición

```
[Seleccionar alumno en grid → Click "Editar"]
     │
     ├─ isNewRecord = false
     ├─ HabilitarNoControl(false) → No. Control readonly
     ├─ CargarDatosFormulario()
     │    ├─ Obtener Alumno desde DataBoundItem
     │    ├─ No_Control → txtNoControl (readonly)
     │    ├─ Nombre → txtNombre
     │    ├─ Apellido_Paterno → txtApellidoPaterno
     │    ├─ Apellido_Materno → txtApellidoMaterno
     │    ├─ Email → txtEmail
     │    ├─ Telefono → txtTelefono
     │    ├─ Fecha_Nacimiento → dtpFechaNacimiento (o unchecked)
     │    ├─ CURP → txtCURP
     │    ├─ Género → cmbGenero (match por string)
     │    ├─ Carrera → cmbCarrera (match por SelectedValue)
     │    ├─ Semestre → nudSemestre (clamped 1-20)
     │    ├─ Turno → cmbTurno (TitleCase desde minusculas BD)
     │    ├─ Fecha_Ingreso → dtpFechaIngreso
     │    └─ Grupo → txtGrupo
     │
     ├─ Mostrar panelCardDatos
     └─ Foco en txtNombre
          │
          ▼
     [Usuario modifica campos → Click "Guardar"]
          │
          ├─ ValidarFormulario() (mismos checks)
          ├─ controller.Update(alumno) [async]
          │    ├─ BEGIN TRANSACTION
          │    ├─ ValidarCamposRequeridos (sin CURP/genero)
          │    ├─ ValidarFormatos
          │    ├─ ValidarDuplicados (excluyendo propio ID)
          │    ├─ ValidarTransicionStatus
          │    ├─ UPDATE Alumno SET ...
          │    ├─ SincronizarEmailUsuario (si cambió)
          │    ├─ InsertarBitacora
          │    └─ COMMIT
          │
          └─ ¿Éxito? → Recargar y ocultar card
```

### 4.1 Reglas de Edición
| Aspecto | Regla |
|---------|-------|
| No. Control | **No editable** al editar (readonly) |
| CURP | No obligatorio en edición (solo si se modifica) |
| Género | No obligatorio en edición |
| Carrera | No obligatorio en edición |
| Semestre | Siempre editable |
| Status | **Siempre "activo"** (no se puede cambiar desde el form actual) |

---

## 5. Flujo de Eliminación (Soft-Delete Empresarial)

```
[Seleccionar alumno → Click "Eliminar"]
     │
     ├─ ¿Hay fila seleccionada? → No → "Selecciona un alumno"
     │
     ├─ Obtener Alumno desde DataBoundItem
     │
     ├─ MessageBox: "¿Eliminar a 'Juan Pérez'?"
     │    └─ ¿No? → Cancelar
     │
     ├─ InputBox: "Motivo de eliminación (requerido):"
     │    └─ ¿Vacío? → "Cancelado, motivo requerido"
     │
     ├─ controller.Delete(id, razon)
     │    ├─ ¿Motivo vacío? → CrudOperationException
     │    ├─ BEGIN TRANSACTION
     │    ├─ ¿Tiene asignaciones activas? → CrudOperationException
     │    ├─ UPDATE Alumno SET
     │    │    is_deleted = 1,
     │    │    deleted_at = NOW(),
     │    │    deleted_by = @usuario,
     │    │    deleted_reason = @razon
     │    ├─ InsertarBitacora("DELETE", ...)
     │    └─ COMMIT
     │
     ├─ ¿Éxito? → "Alumno eliminado" + Recargar
     └─ ¿Error específico? → Mostrar mensaje
          ├─ "No se puede eliminar porque tiene asignaciones activas"
          └─ Otros errores de BD
```

### 5.1 Reglas de Eliminación
| Aspecto | Regla Empresarial |
|---------|-------------------|
| Tipo | **Soft-delete** (`is_deleted=1`) |
| Motivo | **Obligatorio** en todos los casos |
| Asignaciones activas | **Bloquea** la eliminación |
| Alumno no encontrado | Devuelve `false` |
| Datos preservados | Todos los campos se conservan en BD |
| Unicidad | `no_control_unico` (generado: NULL si deleted) permite re-uso controlado |
| Auditoría | `deleted_by`, `deleted_at`, `deleted_reason`, bitácora |

---

## 6. Flujo de Exportación CSV

```
[Click "Exportar CSV"]
     │
     ├─ ¿Hay datos filtrados? → No → "No hay datos para exportar"
     │
     ├─ SaveFileDialog (default: Alumnos_YYYYMMDD_HHmmss.csv)
     │    └─ ¿Cancela? → Salir
     │
     ├─ StreamWriter(UTF8)
     │
     ├─ Header: 15 columnas
     │    No. Control, Nombre, Ap. Paterno, Ap. Materno,
     │    Email, Teléfono, Fecha Nac., Status,
     │    CURP, Género, Carrera, Semestre, Turno,
     │    Fecha Ingreso, Grupo
     │
     ├─ Por cada alumno filtrado:
     │    ├─ EscapeCSV() → comillas + escape de fórmulas
     │    └─ Escribir línea CSV
     │
     └─ "Exportados N registros a: ruta"
```

### 6.1 Seguridad CSV
| Riesgo | Protección |
|--------|-----------|
| Inyección de fórmulas (`=`, `+`, `-`, `@`) | Prefijo `'` neutralizante |
| Comillas dobles en datos | `"` → `""` (RFC 4180) |
| Valores nulos | Cadena vacía |
| Caracteres especiales | UTF-8 encoding |

---

## 7. Flujo de Búsqueda y Filtros

```
[Búsqueda por texto]
     │
     ├─ Escribir en txtBuscar
     ├─ Filtra por: No_Control, Nombre,
     │    Apellido_Paterno, Apellido_Materno, Email
     ├─ Case-insensitive
     └─ Refresca página a 1
          │
          ▼
     [Filtro por Status]
          │
          ├─ cmbFiltroStatus (Dropdown)
          ├─ Opciones: Todos, Activos, Inactivos,
          │    Egresados, Suspendidos, Baja
          └─ Refresca página a 1
               │
               ▼
     [Ordenamiento por columna]
          │
          ├─ Click en header de columna
          ├─ Alterna ▲/▼ (asc/desc)
          ├── Ordena LINQ por propiedad (reflection)
          └── Refresca página a 1
               │
               ▼
     [Paginación]
          ├─ 50 registros por página
          ├─ ◀ Anterior / ▶ Siguiente
          └─ Indicador: "Pág X de Y (Z registros)"
```

---

## 8. Mapa de Teclado

| Tecla | Acción |
|-------|--------|
| **Tab** | Navegar al siguiente campo |
| **Enter** | Ir al siguiente campo / Guardar (en btnGuardar) |
| **Ctrl+N** | Nuevo alumno |
| **Ctrl+S** | Guardar |
| **Escape** | Cancelar / Cerrar card |
| **Double-click** en fila | Editar alumno |

---

## 9. Tab Order (Orden de Navegación)

```
0:  txtNoControl
1:  txtNombre
2:  txtApellidoPaterno
3:  txtApellidoMaterno
4:  txtCURP
5:  cmbGenero
6:  cmbCarrera
7:  nudSemestre
8:  cmbTurno
9:  dtpFechaIngreso
10: txtGrupo
11: txtEmail
12: txtTelefono
13: dtpFechaNacimiento
14: btnGuardar
15: btnCancelar
```

---

## 10. Estado del Grid por Status

| Status | Color de Fila |
|--------|---------------|
| activo | Por defecto (blanco) |
| inactivo | `#FFF0F0` (rojo claro) |
| egresado | `#F0FFF0` (verde claro) |
| suspendido | Por defecto |
| baja | Por defecto |

---

## 11. Arquitectura de BD (Columnas Enterprise)

```
alumno
├── id_alumno (PK, AUTO_INCREMENT)
├── no_control (VARCHAR(15), UNIQUE, CHECK: length=10, [A-Z0-9])
├── nombre (VARCHAR(100), NOT NULL, CHECK: sin números/puntuación)
├── apellido_paterno (VARCHAR(80), NOT NULL, CHECK)
├── apellido_materno (VARCHAR(80), CHECK nullable)
├── curp (VARCHAR(18), UNIQUE, CHECK: length=18)
├── email (VARCHAR(120), UNIQUE, CHECK: regex email)
├── telefono (VARCHAR(15), CHECK: length=10, [0-9])
├── fecha_nacimiento (DATE, CHECK: year>=1900)
├── genero (VARCHAR(30), CHECK: valores)
├── id_carrera (INT, FK → carrera)
├── semestre (INT, CHECK: 1-20)
├── turno (VARCHAR(20), CHECK: valores minúsculas)
├── fecha_ingreso (DATE)
├── grupo (VARCHAR(20))
├── status_alumno (VARCHAR(20), CHECK: valores)
├── is_deleted (TINYINT, default=0)
├── deleted_at (DATETIME)
├── deleted_by (INT, FK → usuario)
├── deleted_reason (VARCHAR(255))
├── created_at (TIMESTAMP)
├── updated_at (DATETIME)
├── created_by (INT, FK → usuario)
└── updated_by (INT, FK → usuario)
```

---

## 12. Resumen de Validación Cruzada (3 Capas)

| Componente | Capa 1: Formulario | Capa 2: Controller | Capa 3: BD |
|------------|-------------------|-------------------|------------|
| No. Control | 10 chars, A-Z0-9 | `ValidarNoControl` | `chk_alumno_no_control` |
| Nombre | Letras + acentos, min 2 | — | `chk_alumno_nombre` |
| Apellidos | Letras + acentos | — | `chk_alumno_apellido_*` |
| CURP | 18 chars, checksum | `ValidarCurp` | `chk_alumno_curp_len` |
| Email | Regex email | `ValidarEmail` | `chk_alumno_email` |
| Teléfono | 10 dígitos | `ValidarTelefono` | `chk_alumno_telefono` |
| Género | Combo con valores | — | `chk_alumno_genero` |
| Turno | Combo → ToLower | — | `chk_alumno_turno` |
| Carrera | Combo activas | carrera existe | `fk_alumno_carrera` |
| Semestre | 1-20 | — | `chk_alumno_semestre` |
| Fecha Nac. | No futura | Edad 15-100 | `chk_alumno_fecha_nac` |
| Fecha Ing. | 1990-hoy | — | — |

---

## 13. Conclusión

El módulo de Alumnos implementa un flujo **profesional y empresarial** con:

- ✅ **Validación en 3 capas**: Formulario → Controller → Base de datos
- ✅ **Estándares mexicanos**: CURP (18 chars con checksum), teléfono (10 dígitos), nombres (solo letras)
- ✅ **Seguridad**: Soft-delete con auditoría (quién, cuándo, por qué), transacciones, anti-SQL injection
- ✅ **UX Profesional**: Filtros, ordenamiento, paginación, exportación CSV segura, atajos de teclado
- ✅ **Consistencia**: 14 CHECK constraints en BD que refuerzan las validaciones del formulario
- ✅ **Integridad referencial**: FKs a usuario, carrera con validación de existencia

### Pendientes (mejoras futuras)
- Gestión de cambios de estado (activo ↔ inactivo ↔ egresado ↔ baja)
- Validación de semestre contra duración de la carrera
- Precarga de combos desde parámetros del sistema
