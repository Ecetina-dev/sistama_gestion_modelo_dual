# Auditoría Completa — FormMenuPrincipal

**Fecha:** 2026-06-18
**Componentes auditados:** `FormMenuPrincipal.cs`, `FormMenuPrincipal.Designer.cs`, `Program.cs`, integración con `SesionActiva`, `AuthController`, vistas de MySQL.

---

## 1. Inventario de funcionalidades

### 1.1 Sidebar (panel lateral)
- Logo + nombre del sistema
- Avatar con inicial del usuario
- Nombre y rol del usuario
- Estado "En línea"
- 7 botones de navegación (Alumnos, Empresas, Proyectos, Profesores, Materias, Temas, Gestión Usuarios)
- 3 botones inferiores (Cambiar Password, Cerrar Sesión, Salir)

### 1.2 Dashboard (panel principal)
- Header con saludo personalizado
- 4 estadísticas (Alumnos, Empresas, Profesores, Pendientes)
- 8 cards de módulos (Alumnos, Empresas, Proyectos, Profesores, Materias, Temas, Gestión Usuarios, Migración BD)

---

## 2. Verificación de permisos

### 2.1 Visibilidad de cards (ConfigurarPermisos)

| Card | esAdmin | esProfesor | esAlumno | esEmpresa | Estado |
|------|---------|------------|----------|-----------|--------|
| cardAlumnos | ✅ | ✅ | ❌ | ❌ | OK |
| cardEmpresas | ✅ | ❌ | ❌ | ✅ | OK |
| cardProyectos | ✅ | ✅ | ✅ | ✅ | OK (todos) |
| cardProfesores | ✅ | ❌ | ❌ | ❌ | OK (solo admin) |
| cardMaterias | ✅ | ✅ | ❌ | ❌ | OK |
| cardTemas | ✅ | ✅ | ❌ | ❌ | OK |
| cardGestionUsuarios | ✅ | ❌ | ❌ | ❌ | OK |
| cardMigracionBD | ✅ | ❌ | ❌ | ❌ | OK (solo admin) |

### 2.2 🔴 INCONSISTENCIA: Permission check en click handlers

| Card | Visibility check | Click check | Match? |
|------|------------------|-------------|--------|
| cardAlumnos | admin/profesor | `TienePrivilegio("profesor.crud_alumno")` OR admin | ✅ |
| cardEmpresas | admin/empresa | `EsAdmin` OR `EsEmpresa` | ✅ |
| cardProyectos | todos | **NINGUNO** | 🔴 |
| cardProfesores | solo admin | `EsAdmin` | ✅ |
| cardMaterias | admin/profesor | `TienePrivilegio("profesor.crud_materia")` OR admin | ✅ |
| cardTemas | admin/profesor | `TienePrivilegio("profesor.crud_tema")` OR admin | ✅ |
| cardGestionUsuarios | solo admin | `EsAdmin` | ✅ |
| cardMigracionBD | solo admin | **NINGUNO** | 🔴 |

**Impacto:** Un usuario sin permisos puede abrir un form si la card está visible. Esto es defensa en profundidad, no debería faltar.

---

## 3. Verificación de alineación con BD

### 3.1 Query de estadísticas (CargarEstadisticas)

```sql
SELECT 
    (SELECT COUNT(*) FROM v_alumnos_activos) AS total_alumnos,
    (SELECT COUNT(*) FROM v_empresas_activas) AS total_empresas,
    (SELECT COUNT(*) FROM v_profesores_activos) AS total_profesores,
    (SELECT COUNT(*) FROM Usuario WHERE debe_cambiar_password = 1 AND is_deleted = 0) AS pendientes_activacion
```

**Verificación:**
- ✅ `v_alumnos_activos` existe
- ✅ `v_empresas_activas` existe
- ✅ `v_profesores_activos` existe
- ✅ `Usuario.debe_cambiar_password` existe (TINYINT)
- ✅ `Usuario.is_deleted` existe (TINYINT)

**Resultado:** Query funcional y bien formada.

---

## 4. Issues encontrados

### 🔴 Críticos (3)

#### Issue #1: Permission check ausente en `btnProyectos_Click`
- **Línea:** FormMenuPrincipal.cs ~155
- **Impacto:** Un usuario sin permisos puede abrir FormProyectos si la card es visible
- **Mitigación actual:** `cardProyectos.Visible = esAdmin || esProfesor || esAlumno || esEmpresa` (todos)
- **Severidad:** 🟡 Media (bajo riesgo real, pero mala práctica)

#### Issue #2: Permission check ausente en `btnMigracionBD_Click`
- **Línea:** FormMenuPrincipal.cs ~275
- **Impacto:** La card es `Visible = esAdmin`, pero no hay check en el click handler
- **Mitigación actual:** Solo el card visibility (defensa única)
- **Severidad:** 🟡 Media (mejorable)

#### Issue #3: Sin auto-logout por inactividad
- **Descripción:** `IsSesionExpirada()` se evalúa solo en `ConfigurarInterfaz()` (al abrir el form). Si el usuario deja el form abierto por >8 horas, no se cierra automáticamente.
- **Severidad:** 🟡 Media
- **Fix recomendado:** Agregar `Timer` que verifique cada X minutos y cierre sesión.

---

### 🟡 Medios (6)

#### Issue #4: Click handlers no se propagan a los labels internos
- **Descripción:** Las cards tienen `Click += btnAlumnos_Click`, pero los labels internos (`iconoAlumnos`, `tituloAlumnos`, `descAlumnos`, `permAlumnos`) NO propagan clicks.
- **Impacto:** Si el usuario hace click en el ícono o el título (no en el área vacía de la card), no se abre el form.
- **Severidad:** 🟡 Media (UX confusa)

#### Issue #5: Sin accessibility (Teclado)
- **Descripción:** Las cards no son `TabStop`, no se pueden navegar con teclado, no responden a `Enter`.
- **Severidad:** 🟡 Media (accesibilidad)

#### Issue #6: `Application.Exit()` no dispara FormClosing
- **Descripción:** Al hacer `Application.Exit()` desde `btnSalir_Click`, los forms abiertos no reciben `FormClosing` event. Esto puede dejar sesiones sin cerrar.
- **Severidad:** 🟡 Media

#### Issue #7: Estado no se limpia entre sesiones
- **Descripción:** Cuando un usuario cierra sesión y otro inicia, los labels (`lblStatAlumnosNum`, `descAlumnos`, etc.) retienen datos del usuario anterior.
- **Severidad:** 🟡 Media (UX)

#### Issue #8: Avatar usa `username[0]`, falla con emails
- **Descripción:** `lblAvatar.Text = username.Substring(0, 1).ToUpper();` Si el username es un email (ej. `admin@modelodual.edu`), la inicial es `@`.
- **Severidad:** 🟡 Baja

#### Issue #9: `CargarEstadisticas` ejecuta para todos los usuarios
- **Descripción:** Cualquier usuario (alumno, empresa) ejecuta la query pesada contra todas las vistas. No hay caché, no hay rate limit.
- **Severidad:** 🟡 Baja

---

### 🟢 Hardening (3)

#### Issue #10: No hay feedback visual al cambiar de módulo
- **Descripción:** Después de abrir un form hijo y volver, no hay confirmación visual de que el menú está "limpio".
- **Severidad:** 🟢 Cosmético

#### Issue #11: Sin telemetría de uso
- **Descripción:** No se registra qué módulos abre cada usuario, frecuencia, etc.
- **Severidad:** 🟢 Futura mejora

#### Issue #12: Sin búsqueda rápida
- **Descripción:** Para usuarios con muchos módulos, no hay filtro/búsqueda.
- **Severidad:** 🟢 Futura mejora

---

## 5. Tabla resumen

| # | Issue | Severidad | Estado | Acción |
|---|-------|-----------|--------|--------|
| 1 | Falta check en btnProyectos | 🟡 Media | Pendiente | Agregar guard |
| 2 | Falta check en btnMigracionBD | 🟡 Media | Pendiente | Agregar guard |
| 3 | Sin auto-logout por inactividad | 🟡 Media | Pendiente | Timer |
| 4 | Click no propaga a labels | 🟡 Media | Pendiente | Propagar |
| 5 | Sin soporte teclado | 🟡 Media | Pendiente | TabStop + KeyDown |
| 6 | Application.Exit sin FormClosing | 🟡 Media | Pendiente | this.Close() en cada form |
| 7 | Estado no se limpia | 🟡 Media | Pendiente | Reset en FormClosing |
| 8 | Avatar con email | 🟡 Baja | Pendiente | Trim/extraer inicial |
| 9 | Stats query pesada | 🟡 Baja | Pendiente | Caché 30s |
| 10 | Sin feedback de cambio | 🟢 Cosmético | Opcional | - |
| 11 | Sin telemetría | 🟢 Futuro | Opcional | - |
| 12 | Sin búsqueda | 🟢 Futuro | Opcional | - |

---

## 6. Tests funcionales recomendados

1. **Login admin** → todas las cards visibles
2. **Login profesor** → solo Alumnos, Materias, Temas, Proyectos (no admin)
3. **Login alumno** → solo Proyectos
4. **Click en card Alumnos siendo profesor** → debe abrir FormAlumnos
5. **Click en card Gestión Usuarios siendo profesor** → debe bloquear
6. **Salir y volver a entrar** → labels deben limpiarse
7. **Dejar menu abierto 8+ horas** → debe cerrar sesión automáticamente (Issue #3, no implementado)

---

## 7. Conclusión

**El menú funciona** pero tiene **3 issues críticos/medios** que deben arreglarse:
- Falta de defense-in-depth en 2 botones (Proyectos, Migración BD)
- Sin auto-logout por inactividad

Y **varios issues de UX/robustez** que pueden esperar.
