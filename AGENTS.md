# AGENTS.md - C# Windows Forms Project

## Project Overview
- **Type**: C# Windows Forms application (.NET Framework 4.7.2)
- **Architecture**: MVC pattern (Models, Controllers, Views)
- **Database**: MySQL (localhost:3307, database: ModeloDualDB)
- **Entry Point**: `Laboratorio_del_Tema_5.2/Views/FormMenuPrincipal.cs`

## Build Commands
```bash
# Build (from solution root)
dotnet build Laboratorio_del_Tema_5.2.slnx

# Run (Debug build output)
dotnet run --project Laboratorio_del_Tema_5.2/Laboratorio_del_Tema_5.2.csproj
```

## Key Conventions

### Database Access
- Use `MySQLConnection.GetConnection()` from `Laboratorio_del_Tema_5_2.Data`
- Always use `using` blocks for connections and commands
- All queries use parameterized queries (`@param` syntax)

### Status Constants
Defined in `Utils/Constantes.cs` under static classes:
- `Estatus.AlumnoActivo`, `Estatus.AlumnoInactivo`, etc.
- Use these instead of magic strings

### Controllers
Each entity has a controller with `Create`, `Read`, `ReadById`, `Update`, `Delete` methods.
- Errors logged via `Logger.Error()` and thrown as `CrudOperationException`

### Models
- Located in `Models/` folder
- Use `DataAnnotations` for validation ([Required], [StringLength], [EmailAddress])
- Nullable fields use `reader.IsDBNull()` check pattern

## Database Connection
- Connection string in `App.config` (excluded from git)
- MySQL on port 3307
- User: root, Password: Creeper77xd

## SQL Scripts
- `Database/Fix_Errores_Ortografia.sql` - corrections for misspelled column names
- `Database/Auth_Migration.sql` - **MUST RUN FIRST** for auth system (creates Usuario, Rol, Privilegio, Sesion tables)
- Run manually via MySQL Workbench or CLI before first run

## Authentication System

### Password Storage
- Uses **BCrypt** with auto-salt and adaptive cost factor (11)
- Password hash stored in `Usuario.password_hash`
- Package: `BCrypt.Net-Next` (NuGet)

### Auth Flow
1. `FormLogin` -> `AuthController.ValidarCredenciales()`
2. On success: `SesionActiva.Instance.IniciarSesion()` stores session
3. `FormMenuPrincipal` checks `SesionActiva.Instance` for permissions
4. Logout: `AuthController.CerrarSesion()` clears session

### Roles & Privileges
| Role | Privileges |
|------|-----------|
| admin | All privileges (admin.crud_todo) |
| alumno | alumno.ver_propio, alumno.editar_propio, alumno.ver_proyectos |
| profesor | profesor.crud_alumno, profesor.crud_materia, profesor.crud_tema, etc. |
| empresa | empresa.ver_asignaciones, empresa.editar_perfil, empresa.crud_proyecto |

### Session Singleton
- `SesionActiva.Instance` holds current user data
- Check `SesionActiva.Instance.TienePrivilegio("nombre_privilegio")` for authorization
- Properties: `Id_Usuario`, `Username`, `Nombre_Rol`, `EsAdmin`, `EsAlumno`, `EsProfesor`, `EsEmpresa`

## New Auth Files
```
Models/
  Usuario.cs       - Usuario, Rol, Privilegio, Sesion models
  Sesion.cs        - SesionActiva singleton

Controllers/
  AuthController.cs - Login, logout, create user, change password

Views/
  FormLogin.cs         - Login form with logo placeholder
  FormCrearCuenta.cs   - User registration with entity linking
  FormCambiarPassword.cs - Change password dialog
```

## Project Structure
```
Laboratorio_del_Tema_5.2/
├── Controllers/     # Business logic (AlumnoController, etc.)
├── Data/            # MySQLConnection helper
├── Database/       # SQL scripts (Auth_Migration.sql MUST run first)
├── Models/          # Entity classes + Usuario, Sesion
├── Utils/           # Constantes, Validacion, Logger, etc.
└── Views/           # Windows Forms (FormLogin, FormMenuPrincipal, etc.)
```

## .gitignore Notes
- `App.config` and `appsettings.json` are excluded (contain credentials)
- Build outputs: `bin/`, `obj/`
- IDE files: `.vs/`, `*.suo`, `*.user`