# Auditoría del Módulo de Alumnos

**Proyecto:** Laboratorio_del_Tema_5.2 — C# WinForms .NET Framework 4.7.2  
**Ámbito:** FormAlumnos, AlumnoController, CarreraController, Modelos, Validadores, Esquema MySQL  
**Fecha:** 2026-06-21  
**Auditor:** Subagente de análisis  
**Base de datos inspeccionada:** `localhost:3307/ModeloDualDB`

---

## 1. Resumen Ejecutivo

Se realizó un análisis adversarial del módulo de Alumnos combinando lectura de código, inspección del esquema MySQL y validación de reglas de negocio. Se encontraron **múltiples fallas críticas de validación cruzada** entre el formulario, el controlador y la base de datos. La causa raíz más grave es que el formulario y el controlador permiten valores que violan directamente los `CHECK CONSTRAINTS` de la tabla `alumno`, lo que provocará errores de base de datos poco amigables para el usuario final.

**Hallazgos por severidad:**

| Severidad | Cantidad |
|-----------|----------|
| Crítico   | 3        |
| Alto      | 7        |
| Medio     | 12       |
| Bajo      | 8        |

---

## 2. Metodología

1. Lectura completa de los archivos solicitados en `Views/`, `Controllers/`, `Models/`, `Utils/` y `Data/`.
2. Inspección directa del esquema MySQL mediante `DESCRIBE`, `SHOW CREATE TABLE`, `SHOW TRIGGERS` y consultas a `information_schema`.
3. Validación de datos existentes contra restricciones de la base de datos.
4. Análisis de flujo de datos: `FormAlumnos` → `AlumnoController` → `AlumnoValidator` → MySQL.
5. Pruebas conceptuales de "romper el sistema" simulando entradas límite, nulas, malformadas y malintencionadas.

---

## 3. Hallazgos Críticos

### CRIT-01: Número de control — discrepancia fatal entre formulario, controlador y base de datos

- **Severidad:** Crítico
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 32-34, 348-352
  - `Laboratorio_del_Tema_5.2/Utils/Constantes.cs` líneas 149-152 (`AlumnoConfig`)
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` líneas 305-309 (`ValidarFormatos`)
  - Base de datos: constraint `chk_alumno_no_control` en tabla `alumno`
- **Descripción:**
  - El formulario permite números de control de **1 a 15 dígitos** (`MAX_NO_CONTROL = 15`, `EXT_MIN_NO_CONTROL = 1`, regex `^\d+$`).
  - `AlumnoValidator` / `AlumnoConfig` permiten **8 a 15 dígitos** (`^[0-9]{8,15}$`).
  - La base de datos exige **exactamente 10 caracteres alfanuméricos mayúsculas/números** (`length(no_control) = 10` y `^[A-Z0-9]{10}$`).
- **Impacto:**
  - Un usuario puede introducir `12345678` (8 dígitos), pasar todas las validaciones del formulario y del controlador, y recibir un error crudo de MySQL (`CHECK CONSTRAINT violation`) en lugar de un mensaje amigable.
  - El `NoControlPattern` del controlador no coincide con el `FORMATO_NO_CONTROL` de `parametro_sistema` (`^[A-Z0-9]{8,10}$`), que a su vez no coincide con el constraint de la DB.
- **Reproducción:**
  1. Abrir `FormAlumnos`.
  2. Clic en **Nuevo**.
  3. Ingresar `12345678` en No. Control.
  4. Completar nombre, apellido paterno, CURP, género, carrera, semestre, turno, fecha ingreso, email.
  5. Clic en **Guardar**.
  6. Se produce excepción MySQL con mensaje técnico.
- **Fix recomendado:**
  - Elegir **una única regla de negocio** y sincronizar formulario, validador, controlador y base de datos.
  - Recomendación: alinear todo con el constraint de BD (`^[A-Z0-9]{10}$`, exactamente 10 caracteres) o modificar el constraint de BD para que coincida con la regla de negocio real.
  - Actualizar `AlumnoConfig.NoControlPattern`, `AlumnoConfig.NoControlMinLength`, `AlumnoConfig.NoControlMaxLength` y los mensajes de error (`MensajesAlumno.NoControlInvalido`).
  - En el formulario, cambiar `SoloDigitos` para permitir también letras mayúsculas si se decide permitir alfanumérico, o mantener solo dígitos si el formato real es numérico.

---

### CRIT-02: Género "Otro" — valor aceptado por el formulario pero rechazado por la base de datos

- **Severidad:** Crítico
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 272-273 (`ConfigurarControlesEnterprise`)
  - `Laboratorio_del_Tema_5.2/Models/Alumno.cs` línea 64 (`[StringLength(30)]`)
  - Base de datos: constraint `chk_alumno_genero`
- **Descripción:**
  - El `ComboBox cmbGenero` carga: `"(Sin especificar)", "Masculino", "Femenino", "Otro"`.
  - La base de datos solo acepta: `'Masculino', 'Femenino', 'No binario', 'Prefiero no decir'`.
  - El valor `"Otro"` no existe en el dominio de la base de datos.
- **Impacto:**
  - Al guardar un nuevo alumno con género "Otro", el controlador valida el formulario como correcto, pero la inserción falla con error de constraint de MySQL.
- **Reproducción:**
  1. Nuevo alumno.
  2. Seleccionar género **Otro**.
  3. Completar datos obligatorios y guardar.
  4. Error de base de datos.
- **Fix recomendado:**
  - Reemplazar `"Otro"` por uno de los valores válidos de la base de datos (por ejemplo `"No binario"` o `"Prefiero no decir"`).
  - Centralizar los valores permitidos en `Constantes.cs` (ya existe `Genero` pero no se usa en el formulario) y rellenar el combo desde esa fuente.

---

### CRIT-03: Turno — valores en mayúscula enviados a una base de datos que espera minúsculas

- **Severidad:** Crítico
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 280-281 (`ConfigurarControlesEnterprise`)
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` línea 536 (`AgregarParametrosAlumno`)
  - Base de datos: constraint `chk_alumno_turno`
- **Descripción:**
  - El combo carga: `"Matutino", "Vespertino", "Nocturno"`.
  - La base de datos espera: `'matutino', 'vespertino', 'nocturno', 'mixto'`.
  - El controlador guarda el turno tal cual, sin `ToLowerInvariant()`.
- **Impacto:**
  - Cualquier alumno nuevo con turno distinto de null provocará violación de `chk_alumno_turno`.
- **Reproducción:**
  1. Nuevo alumno.
  2. Dejar turno en **Matutino** (por defecto).
  3. Guardar.
  4. Error de constraint de MySQL.
- **Fix recomendado:**
  - Normalizar turnos a minúsculas en `AgregarParametrosAlumno` (`alumno.Turno?.ToLowerInvariant()`).
  - O, preferiblemente, almacenar en `Constantes.cs` los valores normalizados (`Turno.Matutino = "matutino"`, etc.) y usarlos tanto en el combo como en el modelo.

---

## 4. Hallazgos de Severidad Alta

### ALTO-01: Email vacío permitido por el formulario pero rechazado por el controlador en edición

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 418-426 (`ValidarFormulario`)
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` líneas 305-309 (`ValidarFormatos`)
  - `Laboratorio_del_Tema_5.2/Utils/AlumnoValidator.cs` líneas 75-90 (`ValidarEmail`)
- **Descripción:**
  - El formulario permite dejar email vacío.
  - `ValidarCamposRequeridos` solo exige email para **nuevos** alumnos.
  - Pero `ValidarFormatos` siempre llama `AlumnoValidator.ValidarEmail(alumno.Email, ...)`, y este método devuelve `false` con error `"El formato del email no es valido"` cuando el email es nulo o vacío.
  - Por tanto, al editar un alumno existente y dejar email vacío, el controlador lanza una excepción de validación aunque el formulario no marque error.
- **Impacto:**
  - Inconsistencia de validación: el usuario no puede guardar un alumno sin email, a pesar de que el formulario lo permite visualmente.
- **Reproducción:**
  1. Seleccionar un alumno existente y editar.
  2. Borrar el campo Email.
  3. Guardar.
  4. Se muestra error `"El formato del email no es valido"`.
- **Fix recomendado:**
  - En `ValidarFormatos`, verificar `!string.IsNullOrWhiteSpace(alumno.Email)` antes de llamar `ValidarEmail`, igual que se hace con teléfono, CURP y RFC.

---

### ALTO-02: Teléfono — formulario permite 10-15 dígitos, controlador exige exactamente 10

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 36-37, 428-433
  - `Laboratorio_del_Tema_5.2/Utils/AlumnoConfig.cs` línea 155 (`TelefonoPattern = "^[0-9]{10}$"`)
  - `Laboratorio_del_Tema_5.2/Utils/AlumnoValidator.cs` líneas 93-108
- **Descripción:**
  - El formulario permite teléfonos de **10 a 15 dígitos** (`EXT_MIN_TELEFONO = 10`, `MAX_TELEFONO = 15`).
  - `AlumnoValidator` exige **exactamente 10 dígitos**.
  - La base de datos también exige **exactamente 10 dígitos** (`chk_alumno_telefono`).
- **Impacto:**
  - Teléfonos de 11-15 dígitos pasan la validación del formulario y fallan en el controlador/base de datos.
- **Reproducción:**
  1. Nuevo alumno.
  2. Ingresar teléfono `123456789012` (12 dígitos).
  3. Guardar.
  4. Error de validación o constraint.
- **Fix recomendado:**
  - Sincronizar el formulario con la regla de 10 dígitos o modificar `AlumnoConfig.TelefonoPattern` y el constraint de BD si la regla real es 10-15.
  - Actualizar tooltip y mensaje de error.

---

### ALTO-03: No. Control — formulario bloquea letras, pero la base de datos las permitiría

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 233-234 (`SoloDigitos`)
  - Base de datos: constraint `chk_alumno_no_control` (`^[A-Z0-9]{10}$`)
- **Descripción:**
  - El formulario solo permite dígitos en No. Control (`SoloDigitos`).
  - La base de datos permite letras mayúsculas y números.
  - Si más adelante se decide soportar alfanumérico, el formulario ya está bloqueando caracteres válidos.
- **Impacto:**
  - Limitación funcional y discrepancia entre capas.
- **Fix recomendado:**
  - Definir el formato oficial y aplicarlo consistentemente en todas las capas.

---

### ALTO-04: Vista "Ver Empresas" rompe la paginación y la configuración del grid

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 326-342 (`btnVerEmpresas_Click`, `CargarAlumnosConEmpresa`, `ActualizarContador`)
- **Descripción:**
  - `CargarAlumnosConEmpresa` asigna `dgvAlumnos.DataSource = controller.GetAlumnosConEmpresa()` (lista de `AlumnoConEmpresa`).
  - No llama a `ConfigurarGrid()`, por lo que el grid muestra columnas autogeneradas.
  - `ActualizarContador` llama `AplicarPagina()`, que depende de `_alumnosFiltrados` (que no se actualizó), causando comportamiento inconsistente.
  - Al alternar de vuelta a "Solo Alumnos", el estado se restaura, pero durante la vista empresas la experiencia es incorrecta.
- **Impacto:**
  - UI inconsistente: aparecen columnas no deseadas (Salario, Empresa, Puesto).
  - Paginación desfasada.
- **Reproducción:**
  1. Abrir el formulario.
  2. Clic en **Ver Empresas**.
  3. Observar que el grid cambia de columnas y la paginación no refleja el total real.
- **Fix recomendado:**
  - Cargar los datos de empresas en `_alumnosFiltrados` o en una colección separada.
  - Llamar a `ConfigurarGrid()` o crear un método `ConfigurarGridEmpresas()`.
  - Sincronizar la paginación con la colección activa.

---

### ALTO-05: Carreras inactivas aparecen seleccionables en el formulario

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 255-270 (`CargarCarreras`)
  - `Laboratorio_del_Tema_5.2/Controllers/CarreraController.cs` líneas 19-42 (`Read`)
- **Descripción:**
  - `CargarCarreras` invoca `new CarreraController().Read()`, que devuelve **todas** las carreras, activas e inactivas.
  - El combo no filtra por `status = 'activa'`.
  - `CarreraController.ReadActivas()` ya existe pero no se usa.
- **Impacto:**
  - Se pueden crear alumnos asociados a carreras inactivas, violando la regla de negocio esperada.
- **Reproducción:**
  1. Marcar una carrera como inactiva en la base de datos.
  2. Abrir `FormAlumnos`.
  3. La carrera inactiva sigue apareciendo en el combo.
- **Fix recomendado:**
  - Cambiar `CargarCarreras` para usar `CarreraController.ReadActivas()`.

---

### ALTO-06: No se valida que la carrera seleccionada exista realmente

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` líneas 280-303 (`ValidarCamposRequeridos`)
- **Descripción:**
  - El controlador valida que `Id_Carrera` tenga valor, pero no verifica su existencia en la tabla `carrera`.
  - La clave foránea `fk_alumno_carrera` lo validará, pero arrojará un error de base de datos técnico.
- **Impacto:**
  - Si el combo queda desfasado con la base de datos (por ejemplo, carrera eliminada mientras el formulario está abierto), el usuario recibe un error crudo.
- **Fix recomendado:**
  - Agregar validación explícita en el controlador: `if (!CarreraExiste(conn, alumno.Id_Carrera.Value)) throw new CrudOperationException("La carrera seleccionada no existe.", ...)`.

---

### ALTO-07: Sin transacciones explícitas en operaciones CRUD

- **Severidad:** Alto
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` métodos `Create`, `Update`, `Delete`
- **Descripción:**
  - Aunque se usa `using (MySqlConnection ...)`, no se envuelven las operaciones en `BEGIN TRANSACTION / COMMIT / ROLLBACK`.
  - Si la inserción del alumno tiene éxito pero falla la bitácora manual, la sincronización de email o `LAST_INSERT_ID`, no hay rollback automático.
- **Impacto:**
  - Posible inconsistencia de datos (alumno creado sin bitácora, o email desfasado).
- **Fix recomendado:**
  - Iniciar transacción al abrir la conexión y hacer `Commit()` solo al finalizar todas las operaciones relacionadas.

---

## 5. Hallazgos de Severidad Media

### MED-01: Sin verificación de privilegios / autorización en el formulario

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` todo el archivo
- **Descripción:**
  - El formulario no consulta `SesionActiva.Instance.TienePrivilegio(...)` para habilitar/deshabilitar acciones.
  - Cualquier usuario autenticado puede crear, editar y eliminar alumnos.
- **Impacto:**
  - Violación de privilegios: un alumno o empresa podría modificar datos de alumnos.
- **Fix recomendado:**
  - Al cargar el formulario, verificar `TienePrivilegio(Privilegio.ProfesorCrudAlumno)` o `EsAdmin`.
  - Deshabilitar botones de edición/eliminación según privilegios.

---

### MED-02: Doble registro en bitácora (trigger + código manual)

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` líneas 615-659 (`InsertarBitacora`)
  - Base de datos: triggers `trg_alumno_after_insert`, `trg_alumno_after_update`, `trg_alumno_after_delete`
- **Descripción:**
  - La base de datos ya tiene triggers que insertan en `bitacora`.
  - El controlador inserta **otra** entrada manual en la misma tabla.
  - Esto genera registros duplicados por cada operación.
- **Impacto:**
  - Ruido en auditoría, dificultad para trazabilidad.
- **Fix recomendado:**
  - Eliminar `InsertarBitacora` del controlador y confiar en los triggers, **o** desactivar los triggers y mantener la bitácora desde la aplicación.
  - Preferiblemente centralizar en un servicio de auditoría.

---

### MED-03: Inserción manual en bitácora usa tipo incompatible

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` líneas 615-659
  - Base de datos: `bitacora.datos_nuevos JSON`
- **Descripción:**
  - El controlador inserta un string plano en `datos_nuevos` (por ejemplo `"2021101001 - Juan Pérez"`).
  - La columna es de tipo `JSON`.
  - Si el string contiene comillas o caracteres especiales, la conversión implícita puede fallar o producir JSON inválido.
- **Impacto:**
  - Error silencioso (el controlador captura la excepción) pero pérdida de bitácora.
- **Fix recomendado:**
  - Usar `JSON_OBJECT()` en la consulta o serializar el objeto a JSON con comillas escapadas.

---

### MED-04: `updated_at` nunca se actualiza realmente

- **Severidad:** Media
- **Archivos y líneas:**
  - Base de datos: tabla `alumno.updated_at` (`DEFAULT CURRENT_TIMESTAMP` sin `ON UPDATE`)
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` consulta `UPDATE`
- **Descripción:**
  - El campo `updated_at` tiene `DEFAULT CURRENT_TIMESTAMP` pero **no** `ON UPDATE CURRENT_TIMESTAMP`.
  - El controlador no asigna `updated_at` en el `UPDATE`.
  - Como resultado, `updated_at` permanece con la fecha de alta.
- **Impacto:**
  - La fecha de última modificación es incorrecta, afectando auditoría y sincronización.
- **Fix recomendado:**
  - Modificar el constraint de la columna para incluir `ON UPDATE CURRENT_TIMESTAMP`, o establecer `updated_at = NOW()` explícitamente en el `UPDATE` del controlador.

---

### MED-05: No se puede cambiar el estado del alumno desde el formulario

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` línea 695 (`Status_Alumno = Estatus.AlumnoActivo`)
  - `Laboratorio_del_Tema_5.2/Controllers/AlumnoController.cs` líneas 389-416 (`ValidarTransicionStatus`)
- **Descripción:**
  - El formulario siempre fuerza `Status_Alumno = "activo"` al guardar.
  - No existe control para cambiar a `inactivo`, `suspendido`, `egresado` o `baja`.
  - `ValidarTransicionStatus` está preparado para validar transiciones, pero nunca se usa desde el formulario.
- **Impacto:**
  - Funcionalidad incompleta: no se puede gestionar el ciclo de vida del alumno.
- **Fix recomendado:**
  - Agregar un combo de estado en el formulario (solo visible para usuarios con privilegios).
  - Propagar `Status_Change_Reason` y `Motivo_Baja` cuando corresponda.

---

### MED-06: `HayCambiosSinGuardar` no detecta campos enterprise

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 448-451
- **Descripción:**
  - Solo verifica `NoControl`, `Nombre` y `ApellidoPaterno`.
  - No detecta cambios en CURP, género, carrera, semestre, turno, grupo, fecha ingreso, teléfono, email.
- **Impacto:**
  - El usuario puede perder cambios sin advertencia al cancelar.
- **Fix recomendado:**
  - Comparar todos los campos del formulario contra el objeto `Alumno` original cargado.

---

### MED-07: `MarcarError` solo resalta TextBox; no aplica a DateTimePicker, ComboBox ni NumericUpDown

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 453-454
- **Descripción:**
  - El método `MarcarError` solo acepta `TextBox`.
  - Cuando la validación falla en `dtpFechaNacimiento`, `cmbGenero`, `cmbCarrera`, `cmbTurno` o `nudSemestre`, no hay indicador visual.
- **Impacto:**
  - Usuario no identifica rápidamente qué campo corregir.
- **Fix recomendado:**
  - Sobrecargar `MarcarError` para `Control` y cambiar `BackColor` o agregar un borde rojo.

---

### MED-08: `RestaurarColores` no restaura todos los controles

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 456-461
- **Descripción:**
  - Solo restaura color de los TextBox básicos.
  - No incluye `txtCURP`, `txtGrupo`, ni controles enterprise.
- **Impacto:**
  - Los campos enterprise pueden quedar marcados en rojo después de corregir el error.
- **Fix recomendado:**
  - Iterar sobre todos los controles editables del `panelCardBody`.

---

### MED-09: Exportar CSV no escapa comas, saltos de línea ni comillas simples internas

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 570-600 (`ExportarCSV`, `EscapeCSV`)
- **Descripción:**
  - `EscapeCSV` envuelve en comillas dobles y duplica comillas dobles, pero:
    - No verifica que el delimitador sea coma (usa `,`).
    - No escapa comas dentro del valor (aunque las comillas dobles lo harían si están bien formadas).
    - No escapa saltos de línea.
    - Prefijo de fórmula (`'`) puede romper lectores que no lo reconozcan.
- **Impacto:**
  - Archivos CSV malformados si los datos contienen comas, saltos de línea o comillas.
- **Fix recomendado:**
  - Usar una librería CSV robusta (por ejemplo `CsvHelper`) o implementar escaping RFC 4180 completo.

---

### MED-10: `CarreraController.Read()` no filtra carreras borradas/activas

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Controllers/CarreraController.cs` líneas 19-42
- **Descripción:**
  - La tabla `carrera` no tiene `is_deleted`, pero sí `status`.
  - `Read()` devuelve activas e inactivas.
  - No existe versión que filtre por `status` combinado con otras reglas.
- **Impacto:**
  - El formulario muestra carreras inactivas (ya reportado en ALTO-05).
- **Fix recomendado:**
  - Usar `ReadActivas()` en el formulario.

---

### MED-11: `FormAlumnos` no valida sesión expirada

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs`
  - `Laboratorio_del_Tema_5.2/Models/Sesion.cs` (`IsSesionExpirada`)
- **Descripción:**
  - El formulario no llama a `SesionActiva.Instance.IsSesionExpirada()` antes de operaciones sensibles.
  - Si la sesión expira mientras el formulario está abierto, las operaciones de guardar/eliminar pueden fallar de forma confusa.
- **Impacto:**
  - Experiencia de usuario deficiente y potencial problema de seguridad.
- **Fix recomendado:**
  - Verificar expiración de sesión al inicio y antes de guardar/eliminar.

---

### MED-12: `ConfigurarGrid` oculta columnas inexistentes

- **Severidad:** Media
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 510-544
- **Descripción:**
  - El método hace `dgvAlumnos.Columns.Clear()` y luego itera sobre un arreglo de nombres para ocultarlos.
  - Como las columnas fueron borradas justo antes, el `foreach` no encuentra nada.
- **Impacto:**
  - Código muerto que da falsa sensación de seguridad.
- **Fix recomendado:**
  - Eliminar el bucle de ocultamiento o moverlo antes de `Columns.Clear()`.

---

## 6. Hallazgos de Severidad Baja

### BAJO-01: Tooltip de búsqueda contradice su comportamiento

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 163-167
- **Descripción:**
  - El icono de búsqueda (`lblBuscarIcono`) muestra tooltip implícito de búsqueda, pero al hacer clic **limpia** el campo.
- **Impacto:**
  - Confusión de UX.
- **Fix recomendado:**
  - Cambiar el tooltip a "Limpiar búsqueda" o mover la función de limpiar a `btnLimpiarBusqueda`.

---

### BAJO-02: `btnLimpiarBusqueda` es un `Label`, no un botón

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.Designer.cs` líneas 63-72
- **Descripción:**
  - El control se llama `btnLimpiarBusqueda` pero es de tipo `System.Windows.Forms.Label`.
  - No es accesible por teclado (no recibe foco, no se activa con Enter/Espacio).
- **Impacto:**
  - Problema de accesibilidad.
- **Fix recomendado:**
  - Cambiar a `Button` o asignarle `TabStop = true` y manejar teclas.

---

### BAJO-03: Tab order inconsistente con control invisible

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.Designer.cs` líneas 198-204 (`txtFechaNacimiento`)
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 82-83, 299-313
- **Descripción:**
  - `txtFechaNacimiento` sigue teniendo `TabIndex = 6` aunque esté invisible.
  - El `DateTimePicker` dinámico toma `TabIndex = 13`.
  - Esto puede causar que el foco "desaparezca" momentáneamente al navegar con Tab.
- **Fix recomendado:**
  - Asignar `TabStop = false` al textbox oculto o quitarlo del diseñador.

---

### BAJO-04: `dtpFechaNacimiento.MinDate` permite fechas antes de 1900 en algunos escenarios

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` línea 80
  - Base de datos: `chk_alumno_fecha_nac` (`year(fecha_nacimiento) >= 1900`)
- **Descripción:**
  - `MinDate = DateTime.Now.AddYears(-120)` (aprox. 1906-06-21 hoy) cumple con la regla de 1900, pero es frágil.
  - Si la regla de BD cambia a `>= 1920`, el formulario no se entera.
- **Fix recomendado:**
  - Derivar `MinDate` de una constante centralizada y documentarla.

---

### BAJO-05: Nombres y apellidos — formulario permite punto, base de datos no

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 735-749 (`SoloLetrasConAcentos`)
  - Base de datos: `chk_alumno_nombre`, `chk_alumno_apellido_paterno`, `chk_alumno_apellido_materno`
- **Descripción:**
  - `SoloLetrasConAcentos` permite espacio y punto (`.`).
  - Los constraints de BD rechazan puntuación (`[:punct:]`).
- **Impacto:**
  - Nombres como "Juan P." pasan el formulario y fallan en BD.
- **Fix recomendado:**
  - Quitar el punto de caracteres permitidos o alinear el constraint de BD.

---

### BAJO-06: `Logger.Error` no escribe excepción interna completa si es null

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Utils/Logger.cs` líneas 46-50
- **Descripción:**
  - El overload `Error(string, Exception)` siempre concatena `StackTrace`, que puede ser `null`.
- **Impacto:**
  - Posible `NullReferenceException` al loggear si `ex.StackTrace` es null (poco común pero posible).
- **Fix recomendado:**
  - Usar `ex.ToString()` o verificar nulidad de `StackTrace`.

---

### BAJO-07: `SesionActiva.CerrarSesion` no usa lock al asignar `_instance = null`

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Models/Sesion.cs` líneas 98-115
- **Descripción:**
  - La propiedad `Instance` usa doble-checked locking, pero `CerrarSesion` modifica `_instance` sin sincronización.
- **Impacto:**
  - Riesgo de condición de carrera en escenarios concurrentes.
- **Fix recomendado:**
  - Envolver la asignación en `lock (_lock)`.

---

### BAJO-08: Mensaje de error en eliminación puede ser engañoso

- **Severidad:** Baja
- **Archivos y líneas:**
  - `Laboratorio_del_Tema_5.2/Views/FormAlumnos.cs` líneas 389-391
- **Descripción:**
  - Si `controller.Delete` devuelve `false`, el mensaje dice: "Puede tener registros asociados."
  - `Delete` devuelve `false` cuando no se afecta ninguna fila (alumno no existe), no por asignaciones.
- **Fix recomendado:**
  - Distinguir entre "no encontrado", "asignaciones activas" y "error de BD".

---

## 7. Problemas de Arquitectura y Diseño

### ARQ-01: Responsabilidades mezcladas entre formulario y controlador

- El formulario valida formato de CURP, email, teléfono, etc.
- El controlador vuelve a validar formatos.
- La base de datos también valida con constraints.
- **Recomendación:** centralizar validaciones en una capa única (por ejemplo, `AlumnoValidator` + reglas de BD generadas a partir de un esquema común).

### ARQ-02: `AlumnoValidator.ValidarSemestre` no se utiliza

- Existe un validador que compara semestre con duración de carrera, pero nunca se invoca.
- **Recomendación:** Implementar la validación de semestre contra `carrera.duracion_semestres` en el controlador.

### ARQ-03: Código de bitácora duplicado y desfasado con el esquema

- El controlador inserta bitácora manualmente mientras la base de datos lo hace por triggers.
- El esquema de `bitacora` soporta JSON y operaciones como `CAMBIO_STATUS`, `RESTORE`, etc., pero el controlador solo usa `INSERT/UPDATE/DELETE` con string plano.
- **Recomendación:** Elegir una sola estrategia y eliminar la duplicación.

### ARQ-04: `Usuario_Alumno` vs `Usuario_Entidad`

- El controlador consulta `Usuario_Alumno` para sincronizar email (`ObtenerUsuarioIdPorAlumno`).
- El script `Auth_Migration.sql` crea `Usuario_Entidad`.
- Ambas tablas coexisten. Esto puede generar confusión sobre cuál es la fuente de verdad.
- **Recomendación:** Deprecar `Usuario_Alumno` en favor de `Usuario_Entidad` o documentar por qué se mantienen ambas.

---

## 8. Evidencia de la Base de Datos

### 8.1 Estructura de `alumno`

```sql
CREATE TABLE `alumno` (
  `id_alumno` int NOT NULL AUTO_INCREMENT,
  `no_control` varchar(15) NOT NULL,
  `curp` varchar(18) DEFAULT NULL,
  `rfc` varchar(13) DEFAULT NULL,
  `nss` varchar(20) DEFAULT NULL,
  `genero` varchar(30) DEFAULT NULL,
  `estado_civil` varchar(30) DEFAULT NULL,
  ...
  `status_alumno` varchar(20) NOT NULL DEFAULT 'activo',
  `is_deleted` tinyint DEFAULT '0',
  ...
  CONSTRAINT `chk_alumno_no_control` CHECK ((length(`no_control`) = 10) and regexp_like(`no_control`,_utf8mb4'^[A-Z0-9]{10}$')),
  CONSTRAINT `chk_alumno_genero` CHECK (((`genero` is null) or (`genero` in (_utf8mb4'Masculino',_utf8mb4'Femenino',_utf8mb4'No binario',_utf8mb4'Prefiero no decir')))),
  CONSTRAINT `chk_alumno_turno` CHECK (((`turno` is null) or (`turno` in (_utf8mb4'matutino',_utf8mb4'vepertino',_utf8mb4'nocturno',_utf8mb4'mixto')))),
  CONSTRAINT `chk_alumno_telefono` CHECK (((`telefono` is null) or ((length(`telefono`) = 10) and regexp_like(`telefono`,_utf8mb4'^[0-9]{10}$')))),
  CONSTRAINT `chk_alumno_email` CHECK (((`email` is null) or regexp_like(`email`,_utf8mb4'^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+[.][A-Za-z]{2,}$'))),
  CONSTRAINT `chk_alumno_nombre` CHECK (regexp_like(`nombre`,_utf8mb4'^[^0-9[:punct:][:digit:]]{2,100}$')),
  ...
  CONSTRAINT `fk_alumno_carrera` FOREIGN KEY (`id_carrera`) REFERENCES `carrera` (`id_carrera`)
) ENGINE=InnoDB;
```

### 8.2 Carreras existentes

```
id_carrera | clave        | nombre               | duracion_semestres | status
-----------|--------------|----------------------|--------------------|--------
1          | SIN-CARRERA  | Sin carrera asignada | 8                  | activa
```

Solo existe **una** carrera en la base de datos. Esto significa que, aunque el formulario pretenda cargar carreras, en la práctica solo habrá una opción real más el placeholder "(Sin carrera)".

### 8.3 Alumnos existentes (muestra)

```
no_control | email                        | curp  | genero | turno | id_carrera | semestre
-----------|------------------------------|-------|--------|-------|------------|----------
2021101001 | cgarcia@universidad.edu      | NULL  | NULL   | NULL  | NULL       | NULL
2021101002 | mrodriguez@universidad.edu   | NULL  | NULL   | NULL  | NULL       | NULL
...
```

Los datos históricos no tienen campos enterprise. Esto es compatible con la edición porque `ValidarCamposRequeridos` solo exige enterprise para **nuevos** registros, pero genera un estado mixto en el sistema.

---

## 9. Recomendaciones Prioritarias

| Prioridad | Acción |
|-----------|--------|
| 1 | Sincronizar reglas de No. Control, Género y Turno entre formulario, controlador y base de datos. |
| 2 | Corregir validación de email vacío en edición en `AlumnoController.ValidarFormatos`. |
| 3 | Sincronizar longitud de teléfono (10 dígitos). |
| 4 | Implementar transacciones en `Create`, `Update` y `Delete`. |
| 5 | Usar `CarreraController.ReadActivas()` en el formulario. |
| 6 | Arreglar vista "Ver Empresas" para que respete paginación y columnas. |
| 7 | Agregar verificación de privilegios en `FormAlumnos`. |
| 8 | Resolver duplicación de bitácora (triggers vs código manual). |
| 9 | Corregir `updated_at` para que refleje la última modificación. |
| 10 | Agregar controles para cambiar el estado del alumno y gestionar motivos. |

---

## 10. Conclusión

El módulo de Alumnos tiene una base funcional sólida, pero presenta **fallas críticas de sincronización entre capas**. Los tres problemas críticos (No. Control, Género y Turno) provocarán errores de base de datos directamente visibles para el usuario. Además, hay inconsistencias de validación, problemas de autorización, duplicación de bitácoras y carencias funcionales importantes (cambio de estado del alumno).

Se recomienda abordar primero los hallazgos críticos y altos antes de considerar el módulo estable para producción.
