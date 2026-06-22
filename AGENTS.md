# AGENTS.md — Laboratorio_del_Tema_5.2

Professional context for AI coding agents working on this project.
Read this before touching code. Last updated: 2026-06-21.

---

## Project Overview

- **Type**: C# Windows Forms desktop application
- **Target framework**: `net481` (.NET Framework 4.8.1) — SDK-style project
- **Output type**: `WinExe`
- **Architecture**: MVC-ish (Models + Controllers + Views), thin controllers with direct data access
- **Database**: MySQL `ModeloDualDB` on `localhost:3307` (credentials in local `App.config`, gitignored)
- **Solution**: `Laboratorio_del_Tema_5.2.slnx` (new XML format) — **only the main project is listed**
- **Real entry point**: `Laboratorio_del_Tema_5.2/Program.cs` (boots login → menu loop), **not** `FormMenuPrincipal.cs`

### App lifecycle

```
Program.cs
  └─ FormLogin.ShowDialog()        ← AuthController.ValidarCredenciales (BCrypt)
      └─ if Debe_Cambiar_Password  ← FormCambiarPassword loop (forced)
          └─ FormMenuPrincipal.ShowDialog()  ← opens entity forms by privilege
              └─ repeat until Program.SalirDelSistema
```

---

## Build & Run Commands

```bash
# Build the main app (does NOT build SistemaTests — not in .slnx)
dotnet build Laboratorio_del_Tema_5.2.slnx

# Run the app
dotnet run --project Laboratorio_del_Tema_5.2/Laboratorio_del_Tema_5.2.csproj

# Run manual integration tests (console runner, NOT dotnet test)
# ⚠️ Destructive: deletes test_* rows before each run — use dev DB only
dotnet run --project SistemaTests/SistemaTests.csproj
```

**Do NOT run `dotnet test`** — there is no test project. The `SistemaTests` project is a console runner with a `Main` method that calls integration tests against a live MySQL.

---

## Project Structure

```
Laboratorio_del_Tema_5.2/
├── Program.cs                          # WinForms bootstrap, login loop
├── App.config                          # Connection strings (GITIGNORED)
├── Controllers/
│   ├── AlumnoController.cs             # Canonical CRUD pattern (mirror this)
│   ├── AuthController.cs               # 1467 LOC — auth + user provisioning
│   ├── CarreraController.cs
│   ├── EmpresaController.cs
│   ├── MateriaController.cs
│   ├── ProfesorController.cs
│   ├── ProyectoController.cs
│   ├── TemaController.cs
│   └── Services/
│       ├── PasswordService.cs          # Crypto-secure temp passwords
│       └── PasswordValidator.cs
├── Data/
│   ├── MySQLConnection.cs              # Static connection factory (LIVE PATH)
│   ├── SqlServerConnection.cs          # Future SQL Server migration
│   └── EntityModel/
│       └── ModeloDualContext.cs        # EF6 DbContext (SCAFFOLDED, UNUSED)
├── Database/
│   ├── MigracionHelper.cs              # 967 LOC — migration orchestration
│   ├── Auth_Migration.sql              # MUST RUN FIRST — Usuario, Rol, Privilegio, Sesion
│   └── Fix_Errores_Ortografia.sql
├── Models/                             # POCOs with DataAnnotations
│   ├── Alumno.cs, AlumnoDocumento.cs
│   ├── Carrera.cs, Empresa.cs
│   ├── Materia.cs, Profesor.cs
│   ├── Proyecto.cs, Tema.cs
│   ├── Usuario.cs                      # Usuario, Rol, Privilegio, Sesion models
│   └── Sesion.cs                       # SesionActiva singleton + ResultadoLogin
├── Utils/
│   ├── AlumnoValidator.cs              # Imperative validation (302 LOC)
│   ├── Constantes.cs                   # Status string constants
│   ├── CustomExceptions.cs             # 5 exception types
│   ├── Logger.cs                       # Thread-safe file logger
│   ├── ParametroSistemaService.cs
│   ├── ServiceLocator.cs               # ⚠️ DEAD — unused DI container
│   └── TransactionHelper.cs
└── Views/                              # 12 WinForms (code-behind + Designer.cs)
    ├── FormLogin.cs                    # Login with logo
    ├── FormActivarCuenta.cs            # User activation
    ├── FormCambiarPassword.cs          # Forced password change
    ├── FormGestionUsuarios.cs          # User admin
    ├── FormMenuPrincipal.cs            # Main menu (629 LOC)
    ├── FormAlumnos.cs                  # 1171 LOC — largest view
    ├── FormCarreras.cs, FormEmpresas.cs
    ├── FormMaterias.cs, FormProfesores.cs
    ├── FormProyectos.cs, FormTemas.cs
    └── FormMigracionBD.cs
```

### Test project (separate, not in .slnx)

```
SistemaTests/
├── SistemaTests.csproj                 # net472, Exe, OLDER package versions
└── Program.cs                          # ~700 LOC console runner, 6 test phases
```

---

## NuGet Packages

| Package | Version | Purpose |
|---------|---------|---------|
| `MySqlConnector` | **2.6.0** | Primary ADO.NET driver (LIVE PATH) |
| `MySql.Data.EntityFramework` | 9.7.0 | EF6 MySQL provider (scaffolded only) |
| `BCrypt.Net-Next` | **4.2.0** | Password hashing, cost factor 11 |

⚠️ **Package drift**: `SistemaTests` uses `MySqlConnector 2.3.1` + `BCrypt.Net-Next 4.0.3` — older than production. Tests run against different driver versions.

⚠️ **No testing framework** (xUnit/NUnit/MSTest) is installed.

---

## Architecture Patterns

### Data access — raw SQL is the live path

**Primary**: parameterized SQL via `MySqlConnector` through `MySQLConnection.GetConnection()`.

Canonical pattern (mirror `AlumnoController.cs`):

```csharp
using (var conn = MySQLConnection.GetConnection())
{
    conn.Open();
    using (var transaction = conn.BeginTransaction())
    try
    {
        // duplicate + soft-deleted checks
        // INSERT/UPDATE/DELETE with @param placeholders
        // cmd.Parameters.AddWithValue("@x", value);
        // InsertarBitacora(conn, "INSERT", ...);
        transaction.Commit();
    }
    catch (MySqlException ex) when (ex.Number == 1062)
    {
        transaction.Rollback();
        throw new DuplicateRecordException(...);
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        Logger.Error(msg, ex);
        throw new CrudOperationException(msg, "Operation", entity);
    }
}
```

**Secondary (UNUSED)**: EF6 `ModeloDualContext : DbContext` in `Data/EntityModel/` — scaffolded database-first, reserved for a future SQL Server migration. **Do not use it in new code unless explicitly told to.**

### Error handling

- Controllers wrap every public method in try/catch
- On exception: `Logger.Error()` then `throw new CrudOperationException(message, operation, entity)`
- `MySqlException.Number == 1062` → `DuplicateRecordException`
- Views catch `CrudOperationException` and surface via `MessageBox.Show`
- Custom exceptions in `Utils/CustomExceptions.cs`:
  - `DatabaseConnectionException`
  - `CrudOperationException` (carries `Operation` + `Entity`) — **most common**
  - `ValidationException`
  - `DuplicateRecordException`
  - `ReferentialIntegrityException`

### Validation — two layers

1. **DataAnnotations** on models (`[Required]`, `[StringLength]`, `[EmailAddress]`, `[Range]`) — declarative, used for documentation/UI hints. **Not actively invoked** via `Validator.TryValidateObject` in controllers.
2. **Imperative validation** via static `AlumnoValidator` methods: `ValidarNoControl`, `ValidarCurp`, `ValidarRfc`, `ValidarEmail`, `ValidarTelefono`, `ValidarEdad`, `ValidarTransicionStatus` (with `out string error`). Throws `CrudOperationException` on failure.

When adding a new entity: create a `<Entity>Validator` static class mirroring `AlumnoValidator`.

### Soft-delete + audit pattern (used on all entities)

- Columns: `is_deleted`, `deleted_at`, `deleted_by`, `deleted_reason`
- Delete requires a reason and blocks if `Alumno_Empresa` has active assignments
- Audit columns: `created_by`, `updated_by`, `deleted_by` ← `SesionActiva.Instance.Id_Usuario` via `ObtenerUsuarioAuditoria()`
- `InsertarBitacora(conn, "INSERT"|"UPDATE"|"DELETE", ...)` writes to `bitacora` audit table; failures swallowed (never blocks CRUD)

### Status transition validation

`ValidarTransicionStatus` enforces allowed status transitions. Terminal states require `Status_Change_Reason` / `Motivo_Baja` / `Fecha_Baja`. Use `Constantes.cs` status constants — **never** magic strings.

### Email sync

Changing a student email propagates to the linked `Usuario.email` via `SincronizarEmailUsuario`.

---

## Authentication System

### Password storage

- **BCrypt** with auto-salt, cost factor 11
- Hash stored in `Usuario.password_hash`
- `PasswordService.GenerarPasswordTemporal` uses `RandomNumberGenerator` (crypto-secure, **not** `Random`)

### Auth flow

1. `FormLogin` → `AuthController.ValidarCredenciales()`
2. On success: `SesionActiva.Instance.IniciarSesion()` stores session
3. `FormMenuPrincipal` checks `SesionActiva.Instance.TienePrivilegio("...")` for authorization
4. Logout: `AuthController.CerrarSesion()` clears session

### Roles & privileges

| Role | Privileges |
|------|-----------|
| `admin` | All (`admin.crud_todo` short-circuits to true) |
| `alumno` | `alumno.ver_propio`, `alumno.editar_propio`, `alumno.ver_proyectos` |
| `profesor` | `profesor.crud_alumno`, `profesor.crud_materia`, `profesor.crud_tema`, ... |
| `empresa` | `empresa.ver_asignaciones`, `empresa.editar_perfil`, `empresa.crud_proyecto` |

### Session singleton

`SesionActiva.Instance` (double-check-locked singleton):
- Properties: `Id_Usuario`, `Username`, `Nombre_Rol`, `Tipo_Entidad`, `Id_Entidad`, `Privilegios`, `Debe_Cambiar_Password`
- Role flags: `EsAdmin`, `EsAlumno`, `EsProfesor`, `EsEmpresa`
- `TienePrivilegio(name)` — admin short-circuits to `true`
- 8-hour session expiry check

### Account lockout

After `Seguridad.MaxIntentosLogin` failed attempts.

---

## Database Setup

### First-run SQL (run manually via MySQL Workbench or CLI)

1. **`Database/Auth_Migration.sql`** — MUST RUN FIRST. Creates `Usuario`, `Rol`, `Privilegio`, `Sesion` tables.
2. **`Database/Fix_Errores_Ortografia.sql`** — corrections for misspelled column names.

### Connection strings (in local `App.config`, gitignored)

- `MySQL` — primary MySqlConnector connection (localhost:3307, ModeloDualDB, pooled)
- `ModeloDualDB_EF` — EF6 connection

**Do not commit `App.config` or `appsettings.json`** — they contain credentials.

---

## Testing

**There is no unit test framework.** `SistemaTests/Program.cs` is a console-based manual integration runner:

- 6 phases: auth, user provisioning, account activation, roles, CRUD, session
- Each test via `EjecutarTest(name, Func<bool>)` → prints `[PASS]`/`[FAIL]` with summary
- **Requires live MySQL** on localhost:3307 with seed data (admin/Admin123*)
- **Destructive**: deletes rows matching `username LIKE 'test_%'` before each run — dev DB only
- Not in `.slnx` → `dotnet build Laboratorio_del_Tema_5.2.slnx` will **not** build it

### Manual test checklist

See `CHECKLIST_VERIFICACION.md` (8.8KB) for the manual verification flow.

---

## OpenSpec / SDD Setup

`openspec/config.yaml`:
- `project.name: sistama_gestion_modelo_dual`
- `language: C#`, `architecture: MVC`
- **`strict_tdd: false`** — no unit test framework available
- `verify.build_command: "dotnet build Laboratorio_del_Tema_5.2.slnx"`
- `verify.manual_test_checklist: CHECKLIST_VERIFICACION.md`
- Proposal rules require rollback plan for risky DB/auth changes and privilege/role impact identification

### Active SDD changes

Two changes in progress under `openspec/changes/`:

1. **`formulario-alumnos-enterprise/`** — near-complete (PR-1..PR-4 done, `verify-report.md` exists)
2. **`modulo-alumnos-enterprise/`** — mid-apply (manual SQL migration + TC-1..TC-14 pending)

Run `/sdd-status` inside an agent for current phase readiness.

---

## Git Conventions

- **Commit style**: Conventional Commits in Spanish, scoped by entity
  - `fix(alumnos): patron CURP corregido - 7 chars alphanumericos tras H/M`
  - `feat(auth): ...`, `refactor(data): ...`
- **Branch**: `main` (direct push, no PR workflow currently enforced)
- **Gitignore**: `App.config`, `appsettings.json`, `bin/`, `obj/`, `.vs/`

---

## Skills to Load When Working Here

Inject these skill paths into subagent prompts for this project:

| Skill | Trigger | Path |
|-------|---------|------|
| `csharp-dotnet-enterprise` | Any C#/.NET code work | `C:\Users\minec\.pi\agent\skills\csharp-dotnet-enterprise\SKILL.md` |
| `branch-pr` | Creating/opening PRs | `~/.pi/agent/npm/node_modules/gentle-pi/skills/branch-pr/SKILL.md` |
| `chained-pr` | PRs >400 lines | `.../chained-pr/SKILL.md` |
| `work-unit-commits` | Commit planning | `.../work-unit-commits/SKILL.md` |
| `judgment-day` | Adversarial review | `.../judgment-day/SKILL.md` |
| `cognitive-doc-design` | Writing docs/reviews | `.../cognitive-doc-design/SKILL.md` |

Refresh the registry after skill changes: `gentle-ai skill-registry refresh`

---

## Learning References — Senior C# / .NET Open-Source Projects

Study these to level up your .NET enterprise skills. Each is the canonical reference for its pattern.

### Architecture & runtime

| Project | What to learn |
|---------|--------------|
| [.NET runtime](https://github.com/dotnet/runtime) | CLR, GC, BCL — the most senior C# code that exists |
| [ASP.NET Core](https://github.com/dotnet/aspnetcore) | Middleware pipeline, DI, MVC — how a real framework is built |
| [eShopOnContainers](https://github.com/dotnet-architecture/eShopOnContainers) | Microsoft's official microservices reference for .NET — CQRS, Docker, event bus |

### Patterns used in this project

| Project | Pattern in this project | What to learn |
|---------|------------------------|--------------|
| [CleanArchitecture (Jason Taylor)](https://github.com/jasontaylordev/CleanArchitecture) | MVC + controllers | Clean Arch + CQRS + MediatR — the enterprise template |
| [MediatR](https://github.com/jbogard/MediatR) | Controllers dispatching operations | CQRS in C# — small, didactic codebase |
| [Polly](https://github.com/App-vNext/Polly) | `try/catch` + retry in controllers | Resilience patterns (retry, circuit breaker, timeout) |
| [FluentValidation](https://github.com/FluentValidation/FluentValidation) | `AlumnoValidator` static class | Fluent declarative validation — cleaner than imperative |
| [Ardalis.GuardClauses](https://github.com/ardalis/GuardClauses) | Manual null/arg checks | Defensive programming, guard clauses |
| [Mapster](https://github.com/MapsterMapper/Mapster) | Manual mapping in controllers | High-performance object mapping |

### Data access

| Project | What to learn |
|---------|--------------|
| [EF Core](https://github.com/dotnet/efcore) | The unused `ModeloDualContext` is EF6 — EF Core is the modern path | LINQ provider, migrations, DbContext lifetime |
| [Dapper](https://github.com/DapperLib/Dapper) | Your raw SQL with `MySqlConnector` is close to Dapper's style | Micro-ORM that wraps raw SQL elegantly — consider for refactor |

### Study plan for this codebase

1. **Start with [CleanArchitecture](https://github.com/jasontaylordev/CleanArchitecture)** — shows how your MVC + controllers should evolve into Clean Architecture with MediatR.
2. **Then [MediatR](https://github.com/jbogard/MediatR)** — small codebase, shows CQRS pattern you can apply to split `AlumnoController` into `CreateAlumnoCommand`, `UpdateAlumnoCommand`, etc.
3. **Then [FluentValidation](https://github.com/FluentValidation/FluentValidation)** — replace `AlumnoValidator` static methods with fluent validators that compose better.
4. **Then [Polly](https://github.com/App-vNext/Polly)** — wrap your `MySQLConnection.GetConnection()` calls with retry policies for transient failures.
5. **Then [Dapper](https://github.com/DapperLib/Dapper)** — consider replacing raw `MySqlCommand` + `reader` loops with Dapper extensions for less boilerplate.

---

## Known Issues & Cleanup Items

These are documented inconsistencies. Do not "fix" them unless explicitly assigned:

1. **Framework version drift**: `csproj` targets `net481` but `App.config` `<supportedRuntime>` says `v4.7.2`. `openspec/config.yaml` also says 4.7.2. The csproj is authoritative — the others are stale.
2. **Package version drift**: `SistemaTests` uses older `MySqlConnector 2.3.1` and `BCrypt.Net-Next 4.0.3`. Align with main project's `2.6.0` / `4.2.0`.
3. **`SistemaTests` not in `.slnx`**: `dotnet build Laboratorio_del_Tema_5.2.slnx` silently skips tests. Add it to the solution.
4. **`ServiceLocator` is dead**: `Utils/ServiceLocator.cs` exists but views do `new XController()` directly. Do not imply DI is the convention.
5. **EF6 `ModeloDualContext` is scaffolded but unused**: raw `MySqlConnector` is the live data path. EF6 is reserved for a future SQL Server migration.
6. **No unit test framework**: consider adding xUnit + FluentAssertions for real unit tests before the codebase grows further.

---

## Quick Reference — When You Need To

| Task | Start here |
|------|-----------|
| Add a new entity CRUD | Read `AlumnoController.cs` + `AlumnoValidator.cs` + `FormAlumnos.cs` — mirror the pattern |
| Add a new auth flow | Read `AuthController.cs` (1467 LOC) + `Sesion.cs` + `FormLogin.cs` |
| Add a DB migration | Read `MigracionHelper.cs` (967 LOC) + `Database/Auth_Migration.sql` |
| Change a form | Read the matching `Form<Entity>.cs` + its `.Designer.cs` |
| Run tests | `dotnet run --project SistemaTests/SistemaTests.csproj` (NOT `dotnet test`) |
| Check SDD status | `/sdd-status` inside an agent, or read `openspec/changes/` |
| Refresh skills | `gentle-ai skill-registry refresh` |

---

## Do Not

- ❌ Commit `App.config`, `appsettings.json`, or any file containing credentials
- ❌ Use EF6 `ModeloDualContext` in new code unless explicitly told to migrate
- ❌ Use `ServiceLocator` — views use `new XController()` directly
- ❌ Use magic strings for status — use `Constantes.cs`
- ❌ Run `SistemaTests` against a shared/staging DB — it deletes rows
- ❌ Skip the `bitacora` audit row in CRUD operations
- ❌ Skip the `ValidarTransicionStatus` check when changing entity status
