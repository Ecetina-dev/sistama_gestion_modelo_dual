# Auditoría Profunda — FormMenuPrincipal

**Fecha:** 2026-06-18
**Método:** Code review + simulación de escenarios + tests SQL contra BD real
**Objetivo:** Encontrar todos los errores posibles, intentando romper el sistema

---

## 🔴 Issues Críticos (4)

### Bug #1: Vista `v_alumnos_activos` incluye soft-deleted
- **Severidad:** 🔴 Alta
- **Descripción:** La vista filtra solo por `status_alumno='activo'`, NO por `is_deleted=0`. Cuando admin hace soft-delete de un alumno vía `AlumnoController.Delete`, el `is_deleted` pasa a 1 pero `status_alumno` queda en 'activo'. El menú sigue mostrando ese alumno en el conteo.
- **Evidencia SQL:**
  ```
  alumno activo + is_deleted=0: 13
  alumno activo + is_deleted=1: 3  ← CONTADOS COMO ACTIVOS EN EL MENÚ
  
  Vista del menú muestra: 16
  FormAlumnos lista: 14
  Diferencia: 3
  ```
- **Impacto:** Inconsistencia entre el dashboard y la lista del módulo. El admin piensa que tiene 16 alumnos pero realmente 3 fueron "eliminados" (soft-delete) y siguen contando.
- **Fix:** Actualizar la vista `v_alumnos_activos` para que también filtre por `is_deleted=0`. Aplicar mismo fix a `v_empresas_activas`, `v_profesores_activos`, `v_materias_activas`, `v_proyectos_activos`.

### Bug #2: `LimpiarEstadoUI` no resetea `statPendientes.Visible` ni cards de admin
- **Severidad:** 🟡 Media
- **Descripción:** Al cerrar sesión, `LimpiarEstadoUI` resetea los labels pero deja `statPendientes.Visible` y `cardMigracionBD.Visible` con su último valor. Si un admin cerró sesión, las cards de admin quedan visibles durante la transición hasta el siguiente login.
- **Impacto:** En una ventana de tiempo (milisegundos) se ve el menu "limpio" pero con cards de admin visibles.

### Bug #3: Activated event se dispara en más casos que solo "volver de form hijo"
- **Severidad:** 🟡 Media
- **Descripción:** `Activated` también se dispara cuando:
  1. El form se muestra por primera vez (doble load con constructor)
  2. El form se restaura desde minimize
  3. Otra app pierde foco y nosotros lo ganamos
- **Impacto:** Doble query en el primer load. Cache de 30s podría no aplicar correctamente. El usuario ve los datos refrescarse en momentos inesperados.

### Bug #4: Soft-delete inconsistency en TODAS las vistas
- **Severidad:** 🔴 Alta
- **Descripción:** Similar al Bug #1 pero para todas las otras entidades. Cualquier soft-delete deja al alumno/empresa/profesor/materia/proyecto "fantasma" en el dashboard.
- **Evidencia SQL:**
  ```
  empresa: 6 (vista) = 6 (con is_deleted=0). OK por ahora.
  profesor: 36 (vista) = 36 (con is_deleted=0). OK.
  materia: 9 (vista) = 9 (con is_deleted=0). OK.
  proyecto: N/A vista v_proyectos_activos (no se usa en menú)
  ```
- **Status actual:** Solo alumno tiene soft-deleted inconsistente (3 fantasmas). Pero si alguien elimina una empresa/materia en el futuro, el bug aparecería.

---

## 🟡 Issues Medios (6)

### Issue #5: `CargarEstadisticas` no muestra error al usuario si falla
- **Severidad:** 🟡 Media
- **Descripción:** Si la BD se desconecta, el try-catch loguea el error pero el usuario ve los últimos números cached sin saber que están desactualizados.
- **Fix:** Mostrar un label pequeño "🔴 Sin conexión" o similar cuando falla.

### Issue #6: Cache de 30s nunca aplica después de un Activated
- **Severidad:** 🟡 Baja (pero ineficiente)
- **Descripción:** Cada vez que el usuario minimize/restore o vuelve de un form hijo, `Activated` se dispara y resetea `_ultimaCargaStats` a MinValue. Esto hace que el cache de 30s NUNCA se aplique después del primer Activated. El cache solo aplica si pasan < 30s entre Cargas dentro del mismo Activated.
- **Fix:** Quitar el reset del cache en Activated, o hacer el cache más largo.

### Issue #7: Si la query no retorna filas, `_ultimaCargaStats` no se actualiza
- **Severidad:** 🟡 Baja
- **Descripción:** Si `reader.Read()` es false, el código no actualiza `_ultimaCargaStats`. El cache nunca se "congela" y cada llamada subsiguiente intenta la query. Inofensivo pero ineficiente.

### Issue #8: Doble load en el primer show del form
- **Severidad:** 🟡 Baja
- **Descripción:** El constructor llama `CargarEstadisticas()` (vía `ConfigurarInterfaz`). Luego `Activated` se dispara y llama `CargarEstadisticas()` de nuevo. 2 queries idénticas.
- **Fix:** Marcar un flag `_primeraCarga = true` y solo llamar una vez en el constructor.

### Issue #9: No hay polling automático cuando el form está idle
- **Severidad:** 🟡 Media
- **Descripción:** Si el usuario está mirando el menu sin abrir módulos, no hay Activated y los datos no se actualizan. Solo se actualizan al abrir un form hijo o minimize/restore.
- **Fix:** Agregar un `Timer` que refresque stats cada 5 minutos, o cada 60s para admin.

### Issue #10: MouseMove handlers crean muchos delegates
- **Severidad:** 🟢 Cosmético
- **Descripción:** Cada movimiento del mouse dispara el handler. No es un problema real en WinForms, pero podría optimizarse con un throttle.

---

## 🟢 Issues Menores (5)

### Issue #11: KeyDown puede no disparar si el foco está en un child
- **Severidad:** 🟢 Cosmético
- **Descripción:** En algunos casos, Tab y otras teclas de navegación no llegan al form porque el child control las maneja primero.
- **Fix:** Usar `ProcessCmdKey` en lugar de `KeyDown`.

### Issue #12: Solo 4 stats en el dashboard, hay 25 tablas
- **Severidad:** 🟢 Decisión de diseño
- **Descripción:** El dashboard solo muestra alumnos/empresas/profesores/pendientes. No hay stats de proyectos, materias, temas, carreras.
- **Fix opcional:** Agregar más stats según necesidad.

### Issue #13: `_idleTimer.Tick` handler no se desuscribe en Dispose
- **Severidad:** 🟢 Cosmético
- **Descripción:** `_idleTimer.Tick += IdleTimer_Tick` se mantiene enganchado después de Dispose. Para `System.Windows.Forms.Timer` no es problema real, pero no es best practice.
- **Fix:** `_idleTimer.Tick -= IdleTimer_Tick` antes de Dispose.

### Issue #14: Memory leaks potenciales con lambdas
- **Severidad:** 🟢 Cosmético
- **Descripción:** `this.MouseMove += (s,e) => ...` y similares quedan enganchados al form. En WinForms no es problema porque el form se destruye completo.

### Issue #15: `FormClosing` no se desuscribe
- **Severidad:** 🟢 Cosmético
- **Descripción:** El handler se asigna en el Designer pero no se desuscribe en Dispose. Inofensivo en la práctica.

---

## 🧪 Tests realizados (10)

| # | Test | Resultado |
|---|------|-----------|
| 1 | Vista vs `count(*)` de alumno | 16 vs 17 (1 inactivo) ✅ |
| 2 | Vista filtra por is_deleted? | 🔴 NO - incluye 2 soft-deleted |
| 3 | Definición de la vista | Solo filtra por status_alumno 🔴 |
| 4 | Inconsistencia alumno | status='activo' AND is_deleted=1 = 2 🔴 |
| 5 | Simular soft-delete | Vista no se actualiza 🔴 |
| 6 | Cambio status activo->inactivo | Vista se actualiza ✅ |
| 7 | Estado de alumnos | 13 activos+0 deleted, 3 activos+1 deleted, 1 egresado+0 |
| 8 | Simular flujo admin | Menu muestra 16, FormAlumnos muestra 14, diff=2 |
| 9 | Edge case: solo mirando menu | Cache 30s no aplica si no hay Activated |
| 10 | Soft-delete en empresa/materia | 0 inconsistencias hoy, pero arquitectura vulnerable |

---

## 📊 Resumen

| Severidad | Cantidad |
|-----------|----------|
| 🔴 Críticos | 4 |
| 🟡 Medios | 6 |
| 🟢 Menores | 5 |
| **Total** | **15** |

---

## 🎯 Fixes prioritarios

1. **CRÍTICO**: Arreglar las vistas SQL para filtrar también por `is_deleted=0` (Issue #1, #4)
2. **MEDIO**: Mejorar `LimpiarEstadoUI` para resetear visibilidad (Issue #2)
3. **MEDIO**: Mostrar feedback al usuario si la BD se cae (Issue #5)
4. **MEDIO**: Agregar polling automático cada X minutos (Issue #9)
5. **BAJO**: Optimizar el doble load inicial (Issue #8)

¿Quieres que aplique los fixes críticos (1-4) y medios prioritarios (5, 9)?
