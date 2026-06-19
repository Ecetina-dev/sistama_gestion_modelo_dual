# 🧪 CHECKLIST DE VERIFICACIÓN MANUAL - Sistema Modelo Dual

## Objetivo
Verificar manualmente que toda la UI funciona correctamente con el nuevo sistema de autenticación.

## Credenciales
- **Usuario**: admin
- **Password**: Admin123*

---

## ✅ CHECKLIST

### 🔐 LOGIN (5 min)

- [ ] **L1**: Login con credenciales válidas (admin/Admin123*) → abre menú principal
- [ ] **L2**: Login con usuario inexistente → mensaje genérico "Credenciales inválidas"
- [ ] **L3**: Login con contraseña incorrecta → mismo mensaje genérico
- [ ] **L4**: Login con campos vacíos → mensaje específico por campo
- [ ] **L5**: 5 intentos fallidos → cuenta bloqueada
- [ ] **L6**: Click en "¿Olvidaste tu contraseña?" → mensaje informativo
- [ ] **L7**: Click en "Crear cuenta" → abre FormCrearCuenta
- [ ] **L8**: Presionar Enter en password → intenta login
- [ ] **L9**: Presionar Escape → cierra con confirmación
- [ ] **L10**: Activar Caps Lock mientras escribes → aparece warning "Caps Lock activo"

---

### 📝 CREACIÓN DE CUENTAS (10 min)

- [ ] **C1**: Crear cuenta admin sin entidad → éxito
- [ ] **C2**: Crear cuenta alumno + seleccionar entidad → éxito
- [ ] **C3**: Crear cuenta profesor + seleccionar entidad → éxito
- [ ] **C4**: Crear cuenta empresa + seleccionar entidad → éxito
- [ ] **C5**: Intentar crear con username duplicado → error específico
- [ ] **C6**: Intentar crear con email duplicado → error específico
- [ ] **C7**: Intentar crear con passwords que no coinciden → error
- [ ] **C8**: Cambiar tipo de usuario → combos se filtran automáticamente
- [ ] **C9**: Tipo = "Sin vincular (Admin)" → oculta combo de entidad
- [ ] **C10**: Username muy corto (<3) → error de validación
- [ ] **C11**: Email con formato inválido → error
- [ ] **C12**: Password muy corto (<6) → error
- [ ] **C13**: Escribir password → indicador de fuerza aparece
- [ ] **C14**: Presionar Escape → cancela con confirmación
- [ ] **C15**: Intentar cerrar ventana durante carga → confirmación

---

### 🏠 MENÚ PRINCIPAL (5 min)

- [ ] **M1**: Login como admin → ve TODOS los botones
- [ ] **M2**: Logout desde menú → vuelve a login
- [ ] **M3**: Botón "Cambiar contraseña" → abre FormCambiarPassword
- [ ] **M4**: Botón "Salir" → confirma antes de salir
- [ ] **M5**: Botón "Test Connection" → muestra versión MySQL

---

### 👨‍🎓 CRUD ALUMNOS (15 min)

- [ ] **A1**: Abrir FormAlumnos → carga lista y grid muestra solo columnas legacy (No. Control, Nombre, Ap. Paterno, Ap. Materno, Email, Teléfono, Status)
- [ ] **A2**: Click "Nuevo" → limpia formulario
- [ ] **A3**: Llenar campos y guardar → aparece en lista
- [ ] **A4**: Seleccionar alumno de la lista → se carga en formulario
- [ ] **A5**: Modificar campos y guardar → lista se actualiza
- [ ] **A6**: Seleccionar y eliminar → solicita motivo de eliminación → confirma → alumno desaparece de la lista (soft delete)
- [ ] **A7**: Intentar guardar con número de control duplicado → error
- [ ] **A8**: Validar campos obligatorios
- [ ] **A9**: Crear alumno con CURP válida (formato y checksum correctos) → se guarda y `created_by` se llena
- [ ] **A10**: Crear alumno con CURP inválida (checksum incorrecto) → rechazada con mensaje "El CURP no es válido"
- [ ] **A11**: Intentar crear alumno con `no_control` de un registro previamente eliminado (soft-deleted) → rechazada como "El número de control ya está registrado (incluyendo registros eliminados)"
- [ ] **A12**: Cambiar status de un alumno a "baja" sin proporcionar `motivo_baja` → rechazada con mensaje de motivo requerido
- [ ] **A13**: Intentar eliminar (soft-delete) un alumno que tiene asignaciones activas en `Alumno_Empresa` → rechazada con mensaje de asignaciones activas
- [ ] **A14**: Modificar el email de un alumno que tiene vínculo `Usuario_Entidad` → verificar que `Usuario.email` se sincroniza con el nuevo valor
- [ ] **A15**: Abrir FormAlumnos → en el grid NO son visibles CURP, RFC, NSS, dirección, ni campos de contacto de emergencia (Phase 1 los almacena pero no los muestra)

---

---

### 👨‍🏫 CRUD PROFESORES (10 min)

- [ ] **P1**: Abrir FormProfesores → carga lista
- [ ] **P2**: Crear nuevo profesor → éxito
- [ ] **P3**: Editar profesor existente → se actualiza
- [ ] **P4**: Eliminar profesor → confirma
- [ ] **P5**: Validar número empleado único

---

### 🏢 CRUD EMPRESAS (10 min)

- [ ] **E1**: Abrir FormEmpresas → carga lista
- [ ] **E2**: Crear nueva empresa → éxito
- [ ] **E3**: Editar empresa → se actualiza
- [ ] **E4**: Eliminar empresa → confirma
- [ ] **E5**: Validar RFC único

---

### 📚 CRUD MATERIAS (5 min)

- [ ] **MA1**: Abrir FormMaterias → carga lista
- [ ] **MA2**: Crear materia → éxito
- [ ] **MA3**: Editar materia → se actualiza
- [ ] **MA4**: Eliminar materia → confirma

---

### 📖 CRUD TEMAS (5 min)

- [ ] **T1**: Abrir FormTemas → carga lista con su materia
- [ ] **T2**: Crear tema asignándolo a materia → éxito
- [ ] **T3**: Editar tema → se actualiza
- [ ] **T4**: Eliminar tema → confirma

---

### 💼 CRUD PROYECTOS (5 min)

- [ ] **PR1**: Abrir FormProyectos → carga lista
- [ ] **PR2**: Crear proyecto → éxito
- [ ] **PR3**: Editar proyecto → se actualiza
- [ ] **PR4**: Eliminar proyecto → confirma

---

### 🔐 CAMBIO DE CONTRASEÑA (3 min)

- [ ] **CP1**: Click "Cambiar contraseña" desde menú
- [ ] **CP2**: Ingresar contraseña actual incorrecta → error
- [ ] **CP3**: Ingresar contraseña actual correcta + nueva → éxito
- [ ] **CP4**: Logout y login con la nueva contraseña → funciona
- [ ] **CP5**: Verificar que NO se puede usar la contraseña anterior

---

### 👥 SESIÓN (3 min)

- [ ] **S1**: Después de login, la barra de título muestra el usuario
- [ ] **S2**: Después de 8 horas (o simular), la sesión expira
- [ ] **S3**: Logout limpia la sesión correctamente
- [ ] **S4**: Re-abrir la app sin hacer login → muestra FormLogin
- [ ] **S5**: Verificar que al cerrar la app se registra fin de sesión

---

### 🎓 CARRERA (5 min)

- [ ] **C1**: CRUD completo de carreras funciona (crear, leer, actualizar, eliminar lógicamente via status = 'inactiva')

---

### 🔍 SEGURIDAD (5 min)

- [ ] **SE1**: Inspeccionar logs en `bin/Debug/net472/Logs/app_YYYYMMDD.log`
- [ ] **SE2**: Verificar que NO se loguean contraseñas
- [ ] **SE3**: Intentar SQL injection en login → no funciona
- [ ] **SE4**: Verificar que mensajes de error NO leak info del usuario
- [ ] **SE5**: Verificar que la contraseña se oculta con asteriscos

---

## 📊 RESUMEN DE PRUEBAS

| Sección | Total tests | Pasados | Fallados | Notas |
|---------|-------------|---------|----------|-------|
| Login | 10 | ___ | ___ | |
| Creación cuentas | 15 | ___ | ___ | |
| Menú principal | 5 | ___ | ___ | |
| CRUD Alumnos | 15 | ___ | ___ | Phase 1 enterprise fields: A9-A15 |
| CRUD Profesores | 5 | ___ | ___ | |
| CRUD Empresas | 5 | ___ | ___ | |
| CRUD Materias | 4 | ___ | ___ | |
| CRUD Temas | 4 | ___ | ___ | |
| CRUD Proyectos | 4 | ___ | ___ | |
| Cambio contraseña | 5 | ___ | ___ | |
| Sesión | 5 | ___ | ___ | |
| Seguridad | 5 | ___ | ___ | |
| Carrera | 1 | ___ | ___ | Phase 1 new entity |
| **TOTAL** | **83** | **___** | **___** | |

---

## 🐛 CÓMO REPORTAR UN BUG

Si encontrás un bug durante las pruebas:

```
BUG #[número]
- Qué estabas haciendo:
- Qué esperabas que pase:
- Qué pasó realmente:
- Mensaje de error (si hay):
- Pasos para reproducir:
- Severidad: [Crítico / Alto / Medio / Bajo]
```

---

## ✅ CRITERIO DE ACEPTACIÓN

El sistema está **APTO PARA PRODUCCIÓN** cuando:
- [ ] Todos los tests de Login pasan
- [ ] Todos los tests de Creación de cuentas pasan
- [ ] Todos los CRUD funcionan sin errores
- [ ] Cambio de contraseña funciona
- [ ] La sesión se mantiene correctamente
- [ ] No hay bugs críticos
- [ ] Los logs muestran los eventos esperados

---

## 📝 NOTAS ADICIONALES

### Datos de prueba sugeridos

**Alumno de prueba:**
- No_Control: TEST001
- Nombre: Juan
- Apellido: Pérez
- Email: juan@test.com
- Teléfono: 555-1234
- Status: activo

**Profesor de prueba:**
- No_Empleado: P001
- Nombre: María
- Apellido: García
- Email: maria@test.com
- Status: activo

**Empresa de prueba:**
- Nombre: TestCorp SA
- RFC: TST010101AAA
- Status: activa

**Materia de prueba:**
- Clave: TST001
- Nombre: Programación I
- Créditos: 5
- Semestre: 1

---

### Comandos útiles

```bash
# Ver logs del sistema
type "bin\Debug\net472\Logs\app_20260608.log"

# Correr tests automatizados
cd SistemaTests
dotnet run

# Build
dotnet build
```

---

**Versión del checklist**: 1.0  
**Fecha**: 2026-06-08  
**Sistema**: Modelo Dual v1.0
