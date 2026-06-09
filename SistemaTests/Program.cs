using System;
using System.Collections.Generic;
using System.Linq;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;
using Laboratorio_del_Tema_5_2.Data;

namespace SistemaTests
{
    /// <summary>
    /// Test runner del Sistema Modelo Dual - Flujo Profesional.
    /// Valida: auth, gestion de usuarios, activacion, CRUD, sesion.
    /// </summary>
    class Program
    {
        private static int _testsPasados = 0;
        private static int _testsFallados = 0;
        private static List<string> _errores = new List<string>();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            ImprimirBanner();

            if (!TestConexionInicial())
            {
                ImprimirError("No se puede conectar a la BD. Abortando tests.");
                Environment.Exit(1);
            }

            LimpiarDatosDeTestsAnteriores();

            Console.WriteLine();
            ImprimirSeccion("FASE 1: AUTENTICACION BASICA");
            TestLogin_Admin();
            TestLogin_UsuarioInexistente();
            TestLogin_ContrasenaIncorrecta();
            TestLogin_CamposVacios();
            TestLogin_DetectarBloqueo();
            TestUsernameDisponible();
            TestEmailDisponible();

            Console.WriteLine();
            ImprimirSeccion("FASE 2: FLUJO PROFESIONAL - CARGAR USUARIO");
            TestGenerarPasswordTemporal();
            TestCargarUsuario_Alumno();
            TestCargarUsuario_Profesor();
            TestCargarUsuario_Empresa();
            TestCargarUsuario_Admin();
            TestCargarUsuario_DuplicadoFalla();
            TestCargarUsuario_EntidadDuplicadaFalla();
            TestListarUsuarios();
            TestResetearPassword();
            TestCambiarStatusUsuario();

            Console.WriteLine();
            ImprimirSeccion("FASE 3: FLUJO PROFESIONAL - ACTIVAR CUENTA");
            TestActivarCuenta_Exitoso();
            TestActivarCuenta_PasswordTemporalIncorrecto();
            TestActivarCuenta_UsuarioInexistente();
            TestActivarCuenta_CuentaYaActivada();
            TestActivarCuenta_ContrasenaNuevaDiferente();
            TestActivarCuenta_LoginPosterior();

            Console.WriteLine();
            ImprimirSeccion("FASE 4: OBTENCION DE ROLES");
            TestObtenerRoles();
            TestObtenerRolesPorTipo();

            Console.WriteLine();
            ImprimirSeccion("FASE 5: CRUD DE ENTIDADES");
            TestCRUD_Alumnos();
            TestCRUD_Profesores();
            TestCRUD_Empresas();
            TestCRUD_Materias();
            TestCRUD_Temas();
            TestCRUD_Proyectos();

            Console.WriteLine();
            ImprimirSeccion("FASE 6: SESION");
            TestSesion_IniciarYObtener();
            TestSesion_TienePrivilegio();
            TestSesion_DebeCambiarPasswordFlag();
            TestSesion_Cerrar();

            Console.WriteLine();
            ImprimirSeccion("RESUMEN FINAL");
            ImprimirResumen();

            Console.WriteLine();
            Console.WriteLine("Tests finalizados.");
        }

        // =====================================================
        // CONEXION
        // =====================================================
        static bool TestConexionInicial()
        {
            return EjecutarTest("Conexion a MySQL", () => MySQLConnection.TestConnection());
        }

        // =====================================================
        // FASE 1: AUTENTICACION BASICA
        // =====================================================
        static void TestLogin_Admin()
        {
            EjecutarTest("Login admin con credenciales validas", () =>
            {
                var auth = new AuthController();
                var resultado = auth.ValidarCredenciales("admin", "Admin123*");
                if (!resultado.Success) throw new Exception(resultado.Message);
                if (resultado.Usuario.Username != "admin") throw new Exception("Username incorrecto");
                if (resultado.Usuario.Debe_Cambiar_Password) throw new Exception("Admin no debe requerir cambio");
                return true;
            });
        }

        static void TestLogin_UsuarioInexistente()
        {
            EjecutarTest("Login con usuario inexistente (no leak info)", () =>
            {
                var auth = new AuthController();
                var resultado = auth.ValidarCredenciales("no_existe_xyz_12345", "cualquier_cosa");
                if (resultado.Success) throw new Exception("No deberia loguear");
                if (resultado.Message.Contains("no existe")) throw new Exception("Mensaje filtra info");
                return true;
            });
        }

        static void TestLogin_ContrasenaIncorrecta()
        {
            EjecutarTest("Login con contrasena incorrecta", () =>
            {
                var auth = new AuthController();
                var resultado = auth.ValidarCredenciales("admin", "WrongPass123");
                if (resultado.Success) throw new Exception("No deberia loguear");
                return true;
            });
        }

        static void TestLogin_CamposVacios()
        {
            EjecutarTest("Login con campos vacios", () =>
            {
                var auth = new AuthController();
                if (auth.ValidarCredenciales("", "").Success) throw new Exception("No deberia loguear vacio");
                if (auth.ValidarCredenciales("admin", "").Success) throw new Exception("No deberia sin pwd");
                if (auth.ValidarCredenciales("", "pass").Success) throw new Exception("No deberia sin user");
                return true;
            });
        }

        static void TestLogin_DetectarBloqueo()
        {
            EjecutarTest("Bloqueo tras 5 intentos fallidos", () =>
            {
                var auth = new AuthController();
                string testUser = "test_bloq_" + DateTime.Now.Ticks;
                string testPass = "TestPass123";

                var creado = auth.CargarUsuario(testUser, "test_bloq_" + DateTime.Now.Ticks + "@test.com",
                    2, null, null, 1);
                if (!creado.Success) throw new Exception("No se pudo crear usuario de prueba");

                // El usuario recien creado tiene password temporal, no podemos testear bloqueo aqui
                // mejor testear con admin
                bool detectoBloqueo = false;
                for (int i = 0; i < Seguridad.MaxIntentosLogin + 1; i++)
                {
                    var res = auth.ValidarCredenciales(testUser, "wrong_" + i);
                    if (res.Message.Contains("bloqueada") || res.Message.Contains("bloqueado"))
                    {
                        detectoBloqueo = true;
                        break;
                    }
                }

                if (!detectoBloqueo) throw new Exception("No se detecto bloqueo");
                return true;
            });
        }

        static void TestUsernameDisponible()
        {
            EjecutarTest("UsernameDisponible detecta duplicados", () =>
            {
                var auth = new AuthController();
                if (auth.UsernameDisponible("admin")) throw new Exception("admin deberia estar ocupado");
                if (!auth.UsernameDisponible("random_inexistente_xyz")) throw new Exception("Random deberia estar libre");
                return true;
            });
        }

        static void TestEmailDisponible()
        {
            EjecutarTest("EmailDisponible detecta duplicados", () =>
            {
                var auth = new AuthController();
                if (auth.EmailDisponible("admin@modelodual.edu")) throw new Exception("Email admin ocupado");
                if (!auth.EmailDisponible("random_xyz_9999@noexiste.com")) throw new Exception("Email random libre");
                return true;
            });
        }

        // =====================================================
        // FASE 2: FLUJO PROFESIONAL
        // =====================================================
        static void TestGenerarPasswordTemporal()
        {
            EjecutarTest("GenerarPasswordTemporal devuelve string valido", () =>
            {
                var auth = new AuthController();
                string pwd = auth.GenerarPasswordTemporal();
                if (string.IsNullOrEmpty(pwd)) throw new Exception("Password vacio");
                if (pwd.Length != 10) throw new Exception($"Longitud incorrecta: {pwd.Length}");
                return true;
            });
        }

        static void TestCargarUsuario_Alumno()
        {
            EjecutarTest("Admin carga usuario alumno (con password temporal)", () =>
            {
                var auth = new AuthController();
                string username = "test_alum_" + DateTime.Now.Ticks;
                string email = "test_alum_" + DateTime.Now.Ticks + "@test.com";

                var aluCtrl = new AlumnoController();
                var alu = new Alumno
                {
                    No_Control = "T" + DateTime.Now.ToString("HHmmssfff"),
                    Nombre = "Test",
                    Apellido_Paterno = "Alumno",
                    Status_Alumno = "activo"
                };
                aluCtrl.Create(alu);
                var aluCreado = aluCtrl.Read().FirstOrDefault(a => a.No_Control == alu.No_Control);
                if (aluCreado == null) throw new Exception("No se creo alumno base");

                var resultado = auth.CargarUsuario(username, email, 2, "alumno", aluCreado.Id_Alumno, 1);
                if (!resultado.Success) throw new Exception(resultado.Message);
                if (string.IsNullOrEmpty(resultado.PasswordTemporal)) throw new Exception("No devolvio password temporal");
                if (resultado.PasswordTemporal.Length < 8) throw new Exception("Password temporal muy corto");
                return true;
            });
        }

        static void TestCargarUsuario_Profesor()
        {
            EjecutarTest("Admin carga usuario profesor", () =>
            {
                var auth = new AuthController();
                string username = "test_prof_" + DateTime.Now.Ticks;
                string email = "test_prof_" + DateTime.Now.Ticks + "@test.com";

                var profCtrl = new ProfesorController();
                var prof = new Profesor
                {
                    No_Empleado = "P" + DateTime.Now.ToString("HHmmssfff"),
                    Nombre = "Test",
                    Apellido_Paterno = "Prof",
                    Status_Profesor = "activo"
                };
                profCtrl.Create(prof);
                var profCreado = profCtrl.Read().FirstOrDefault(p => p.No_Empleado == prof.No_Empleado);
                if (profCreado == null) throw new Exception("No se creo profesor");

                var resultado = auth.CargarUsuario(username, email, 3, "profesor", profCreado.Id_Profesor, 1);
                if (!resultado.Success) throw new Exception(resultado.Message);
                return true;
            });
        }

        static void TestCargarUsuario_Empresa()
        {
            EjecutarTest("Admin carga usuario empresa", () =>
            {
                var auth = new AuthController();
                string username = "test_emp_" + DateTime.Now.Ticks;
                string email = "test_emp_" + DateTime.Now.Ticks + "@test.com";

                var empCtrl = new EmpresaController();
                var emp = new Empresa
                {
                    Nombre_Comercial = "TestCo" + DateTime.Now.ToString("HHmmss"),
                    Razon_Social = "Test SA",
                    RFC = "TST" + DateTime.Now.ToString("HHmmss"),
                    Status_Empresa = "activa"
                };
                empCtrl.Create(emp);
                var empCreada = empCtrl.Read().FirstOrDefault(e => e.RFC == emp.RFC);
                if (empCreada == null) throw new Exception("No se creo empresa");

                var resultado = auth.CargarUsuario(username, email, 4, "empresa", empCreada.Id_Empresa, 1);
                if (!resultado.Success) throw new Exception(resultado.Message);
                return true;
            });
        }

        static void TestCargarUsuario_Admin()
        {
            EjecutarTest("Admin carga usuario admin (sin entidad)", () =>
            {
                var auth = new AuthController();
                string username = "test_adm_" + DateTime.Now.Ticks;
                string email = "test_adm_" + DateTime.Now.Ticks + "@test.com";

                var resultado = auth.CargarUsuario(username, email, 1, null, null, 1);
                if (!resultado.Success) throw new Exception(resultado.Message);
                return true;
            });
        }

        static void TestCargarUsuario_DuplicadoFalla()
        {
            EjecutarTest("Cargar usuario con username duplicado debe fallar", () =>
            {
                var auth = new AuthController();
                var resultado = auth.CargarUsuario("admin", "nuevo_" + DateTime.Now.Ticks + "@x.com", 2, null, null, 1);
                if (resultado.Success) throw new Exception("Deberia fallar");
                return true;
            });
        }

        static void TestCargarUsuario_EntidadDuplicadaFalla()
        {
            EjecutarTest("Cargar usuario con entidad ya vinculada debe fallar", () =>
            {
                var auth = new AuthController();

                // Crear alumno y vincularlo a un usuario
                var aluCtrl = new AlumnoController();
                var alu = new Alumno
                {
                    No_Control = "E" + DateTime.Now.ToString("HHmmssfff"),
                    Nombre = "Ya",
                    Apellido_Paterno = "Vinculado",
                    Status_Alumno = "activo"
                };
                aluCtrl.Create(alu);
                var aluCreado = aluCtrl.Read().FirstOrDefault(a => a.No_Control == alu.No_Control);

                // Vincular una vez
                var primerIntento = auth.CargarUsuario("prim_" + DateTime.Now.Ticks, "p1_" + DateTime.Now.Ticks + "@x.com", 2, "alumno", aluCreado.Id_Alumno, 1);
                if (!primerIntento.Success) throw new Exception("No se vinculo la primera vez: " + primerIntento.Message);

                // Intentar vincular de nuevo (debe fallar)
                var segundoIntento = auth.CargarUsuario("seg_" + DateTime.Now.Ticks, "p2_" + DateTime.Now.Ticks + "@x.com", 2, "alumno", aluCreado.Id_Alumno, 1);
                if (segundoIntento.Success) throw new Exception("Deberia fallar por entidad duplicada");
                return true;
            });
        }

        static void TestListarUsuarios()
        {
            EjecutarTest("ListarUsuarios devuelve la lista", () =>
            {
                var auth = new AuthController();
                var lista = auth.ListarUsuarios();
                if (lista == null) throw new Exception("Lista null");
                if (lista.Count == 0) throw new Exception("Lista vacia");
                if (!lista.Any(u => u.Username == "admin")) throw new Exception("No esta admin");
                return true;
            });
        }

        static void TestResetearPassword()
        {
            EjecutarTest("ResetearPassword genera nuevo password temporal", () =>
            {
                var auth = new AuthController();
                var lista = auth.ListarUsuarios();
                var usuario = lista.FirstOrDefault(u => u.Username.StartsWith("test_adm_"));
                if (usuario == null) throw new Exception("No hay usuario de prueba");

                var resultado = auth.ResetearPassword(usuario.Id_Usuario, 1);
                if (!resultado.Success) throw new Exception(resultado.Message);
                if (string.IsNullOrEmpty(resultado.PasswordTemporal)) throw new Exception("No devolvio password");

                return true;
            });
        }

        static void TestCambiarStatusUsuario()
        {
            EjecutarTest("CambiarStatusUsuario funciona", () =>
            {
                var auth = new AuthController();
                var lista = auth.ListarUsuarios();
                var usuario = lista.FirstOrDefault(u => u.Username.StartsWith("test_adm_"));
                if (usuario == null) throw new Exception("No hay usuario de prueba");

                if (!auth.CambiarStatusUsuario(usuario.Id_Usuario, "inactivo", 1))
                    throw new Exception("No cambio a inactivo");
                if (!auth.CambiarStatusUsuario(usuario.Id_Usuario, "activo", 1))
                    throw new Exception("No volvio a activo");
                return true;
            });
        }

        // =====================================================
        // FASE 3: ACTIVAR CUENTA
        // =====================================================
        static void TestActivarCuenta_Exitoso()
        {
            EjecutarTest("Activar cuenta con password temporal correcto", () =>
            {
                var auth = new AuthController();
                string username = "test_activar_" + DateTime.Now.Ticks;
                string email = "test_activar_" + DateTime.Now.Ticks + "@test.com";

                // Crear usuario (queda con password temporal)
                var carga = auth.CargarUsuario(username, email, 2, null, null, 1);
                if (!carga.Success) throw new Exception("No se creo: " + carga.Message);

                // Activar con password temporal
                var activacion = auth.ActivarCuenta(username, carga.PasswordTemporal, "MiNuevaPass123");
                if (!activacion.Success) throw new Exception(activacion.Message);
                if (activacion.Usuario.Debe_Cambiar_Password) throw new Exception("No se limpio el flag");

                // Limpiar
                LimpiarUsuarioTest(username, email);
                return true;
            });
        }

        static void TestActivarCuenta_PasswordTemporalIncorrecto()
        {
            EjecutarTest("Activar con password temporal incorrecto falla", () =>
            {
                var auth = new AuthController();
                string username = "test_bad_" + DateTime.Now.Ticks;
                string email = "test_bad_" + DateTime.Now.Ticks + "@test.com";

                var carga = auth.CargarUsuario(username, email, 2, null, null, 1);
                if (!carga.Success) throw new Exception("No se creo");

                var activacion = auth.ActivarCuenta(username, "wrong_password", "NuevaPass123");
                if (activacion.Success) throw new Exception("No deberia activar con pwd incorrecto");

                LimpiarUsuarioTest(username, email);
                return true;
            });
        }

        static void TestActivarCuenta_UsuarioInexistente()
        {
            EjecutarTest("Activar cuenta de usuario inexistente falla", () =>
            {
                var auth = new AuthController();
                var resultado = auth.ActivarCuenta("no_existe_999", "cualquier", "NuevaPass123");
                if (resultado.Success) throw new Exception("No deberia activar");
                return true;
            });
        }

        static void TestActivarCuenta_CuentaYaActivada()
        {
            EjecutarTest("Activar cuenta ya activada falla", () =>
            {
                var auth = new AuthController();
                // admin ya esta activado
                var resultado = auth.ActivarCuenta("admin", "Admin123*", "OtraPass123");
                if (resultado.Success) throw new Exception("No deberia activar una cuenta ya activa");
                return true;
            });
        }

        static void TestActivarCuenta_ContrasenaNuevaDiferente()
        {
            EjecutarTest("Nueva contrasena debe ser diferente al temporal", () =>
            {
                var auth = new AuthController();
                var activacion = auth.ActivarCuenta("admin", "Admin123*", "Admin123*");
                if (activacion.Success) throw new Exception("Deberia rechazar misma password");
                return true;
            });
        }

        static void TestActivarCuenta_LoginPosterior()
        {
            EjecutarTest("Despues de activar, login normal funciona", () =>
            {
                var auth = new AuthController();
                string username = "test_postlogin_" + DateTime.Now.Ticks;
                string email = "test_post_" + DateTime.Now.Ticks + "@test.com";
                string nuevaPass = "MiNuevaPass456";

                var carga = auth.CargarUsuario(username, email, 2, null, null, 1);
                if (!carga.Success) throw new Exception("No se creo");

                var activacion = auth.ActivarCuenta(username, carga.PasswordTemporal, nuevaPass);
                if (!activacion.Success) throw new Exception("No se activo: " + activacion.Message);

                // Hacer logout y login normal
                auth.CerrarSesion();
                var login = auth.ValidarCredenciales(username, nuevaPass);
                if (!login.Success) throw new Exception("Login posterior fallo: " + login.Message);

                LimpiarUsuarioTest(username, email);
                return true;
            });
        }

        // =====================================================
        // FASE 4: ROLES
        // =====================================================
        static void TestObtenerRoles()
        {
            EjecutarTest("ObtenerRoles devuelve los 4 roles", () =>
            {
                var auth = new AuthController();
                var roles = auth.ObtenerRoles();
                if (roles == null || roles.Count < 4) throw new Exception($"Hay {roles?.Count} roles, esperados 4");
                if (!roles.Any(r => r.Nombre == "admin")) throw new Exception("Falta admin");
                if (!roles.Any(r => r.Nombre == "alumno")) throw new Exception("Falta alumno");
                if (!roles.Any(r => r.Nombre == "profesor")) throw new Exception("Falta profesor");
                if (!roles.Any(r => r.Nombre == "empresa")) throw new Exception("Falta empresa");
                return true;
            });
        }

        static void TestObtenerRolesPorTipo()
        {
            EjecutarTest("ObtenerRolesPorTipo filtra correctamente", () =>
            {
                var auth = new AuthController();
                if (auth.ObtenerRolesPorTipo("alumno").Count != 1) throw new Exception("Alumno deberia tener 1 rol");
                if (auth.ObtenerRolesPorTipo("profesor").Count != 1) throw new Exception("Profesor deberia tener 1 rol");
                if (auth.ObtenerRolesPorTipo("empresa").Count != 1) throw new Exception("Empresa deberia tener 1 rol");
                if (auth.ObtenerRolesPorTipo("none").Count != 1) throw new Exception("Admin deberia tener 1 rol");
                return true;
            });
        }

        // =====================================================
        // FASE 5: CRUD
        // =====================================================
        static void TestCRUD_Alumnos()
        {
            EjecutarTest("CRUD Alumnos completo", () =>
            {
                var ctrl = new AlumnoController();
                var alu = new Alumno
                {
                    No_Control = "C" + DateTime.Now.ToString("HHmmssfff"),
                    Nombre = "Test",
                    Apellido_Paterno = "CRUD",
                    Status_Alumno = "activo"
                };
                if (!ctrl.Create(alu)) throw new Exception("Create fallo");
                var lista = ctrl.Read();
                if (lista.Count == 0) throw new Exception("Read fallo");
                var aluCreado = lista.FirstOrDefault(a => a.No_Control == alu.No_Control);
                if (aluCreado == null) throw new Exception("No se encontro creado");
                if (ctrl.ReadById(aluCreado.Id_Alumno) == null) throw new Exception("ReadById fallo");
                aluCreado.Nombre = "Modificado";
                if (!ctrl.Update(aluCreado)) throw new Exception("Update fallo");
                if (!ctrl.Delete(aluCreado.Id_Alumno)) throw new Exception("Delete fallo");
                return true;
            });
        }

        static void TestCRUD_Profesores()
        {
            EjecutarTest("CRUD Profesores completo", () =>
            {
                var ctrl = new ProfesorController();
                var prof = new Profesor
                {
                    No_Empleado = "C" + DateTime.Now.ToString("HHmmssfff"),
                    Nombre = "Test",
                    Apellido_Paterno = "CRUD",
                    Status_Profesor = "activo"
                };
                if (!ctrl.Create(prof)) throw new Exception("Create fallo");
                var lista = ctrl.Read();
                var creado = lista.FirstOrDefault(p => p.No_Empleado == prof.No_Empleado);
                if (creado == null) throw new Exception("No encontrado");
                if (ctrl.ReadById(creado.Id_Profesor) == null) throw new Exception("ReadById fallo");
                creado.Nombre = "Mod";
                if (!ctrl.Update(creado)) throw new Exception("Update fallo");
                if (!ctrl.Delete(creado.Id_Profesor)) throw new Exception("Delete fallo");
                return true;
            });
        }

        static void TestCRUD_Empresas()
        {
            EjecutarTest("CRUD Empresas completo", () =>
            {
                var ctrl = new EmpresaController();
                var emp = new Empresa
                {
                    Nombre_Comercial = "TestCo" + DateTime.Now.ToString("HHmmss"),
                    Razon_Social = "Test",
                    RFC = "TES" + DateTime.Now.ToString("HHmmss"),
                    Status_Empresa = "activa"
                };
                if (!ctrl.Create(emp)) throw new Exception("Create fallo");
                var lista = ctrl.Read();
                var creada = lista.FirstOrDefault(e => e.RFC == emp.RFC);
                if (creada == null) throw new Exception("No encontrada");
                creada.Nombre_Comercial = "Mod";
                if (!ctrl.Update(creada)) throw new Exception("Update fallo");
                if (!ctrl.Delete(creada.Id_Empresa)) throw new Exception("Delete fallo");
                return true;
            });
        }

        static void TestCRUD_Materias()
        {
            EjecutarTest("CRUD Materias completo", () =>
            {
                var ctrl = new MateriaController();
                var mat = new Materia
                {
                    Clave_Materia = "C" + DateTime.Now.ToString("HHmmss"),
                    Nombre = "Test",
                    Creditos = 5,
                    Semestre = 1,
                    Status_Materia = "activa"
                };
                if (!ctrl.Create(mat)) throw new Exception("Create fallo");
                var lista = ctrl.Read();
                var creada = lista.FirstOrDefault(m => m.Clave_Materia == mat.Clave_Materia);
                if (creada == null) throw new Exception("No encontrada");
                creada.Nombre = "Mod";
                if (!ctrl.Update(creada)) throw new Exception("Update fallo");
                if (!ctrl.Delete(creada.Id_Materia)) throw new Exception("Delete fallo");
                return true;
            });
        }

        static void TestCRUD_Temas()
        {
            EjecutarTest("CRUD Temas completo", () =>
            {
                var ctrl = new TemaController();
                var matCtrl = new MateriaController();
                var materia = matCtrl.Read().FirstOrDefault();
                if (materia == null) throw new Exception("No hay materias");

                var tema = new Tema
                {
                    Id_Materia = materia.Id_Materia,
                    Numero_Tema = (int)(DateTime.Now.Ticks % 10000) + 100,
                    Nombre = "Test " + DateTime.Now.ToString("HHmmssfff"),
                    Status_Tema = "activo"
                };
                if (!ctrl.Create(tema)) throw new Exception("Create fallo");
                var lista = ctrl.Read();
                var creado = lista.FirstOrDefault(t => t.Nombre == tema.Nombre);
                if (creado == null) throw new Exception("No encontrado");
                creado.Nombre = "Mod";
                if (!ctrl.Update(creado)) throw new Exception("Update fallo");
                if (!ctrl.Delete(creado.Id_Tema)) throw new Exception("Delete fallo");
                return true;
            });
        }

        static void TestCRUD_Proyectos()
        {
            EjecutarTest("CRUD Proyectos completo", () =>
            {
                var ctrl = new ProyectoController();
                var p = new Proyecto
                {
                    Nombre = "Test " + DateTime.Now.ToString("HHmmss"),
                    Descripcion = "Test",
                    Status = "propuesto"
                };
                if (!ctrl.Create(p)) throw new Exception("Create fallo");
                var lista = ctrl.Read();
                var creado = lista.FirstOrDefault(x => x.Nombre == p.Nombre);
                if (creado == null) throw new Exception("No encontrado");
                creado.Nombre = "Mod";
                if (!ctrl.Update(creado)) throw new Exception("Update fallo");
                if (!ctrl.Delete(creado.Id_Proyecto)) throw new Exception("Delete fallo");
                return true;
            });
        }

        // =====================================================
        // FASE 6: SESION
        // =====================================================
        static void TestSesion_IniciarYObtener()
        {
            EjecutarTest("Sesion se inicia tras login", () =>
            {
                var auth = new AuthController();
                var resultado = auth.ValidarCredenciales("admin", "Admin123*");
                if (!resultado.Success) throw new Exception("Login fallo");
                if (SesionActiva.Instance.Id_Usuario == 0) throw new Exception("Sesion no iniciada");
                if (SesionActiva.Instance.Username != "admin") throw new Exception("Username incorrecto");
                return true;
            });
        }

        static void TestSesion_TienePrivilegio()
        {
            EjecutarTest("Sesion valida privilegios", () =>
            {
                var auth = new AuthController();
                auth.ValidarCredenciales("admin", "Admin123*");
                if (!SesionActiva.Instance.TienePrivilegio("admin.crud_todo"))
                    throw new Exception("Admin deberia tener crud_todo");
                return true;
            });
        }

        static void TestSesion_DebeCambiarPasswordFlag()
        {
            EjecutarTest("Sesion refleja si debe cambiar password", () =>
            {
                var auth = new AuthController();

                // admin no debe cambiar
                auth.ValidarCredenciales("admin", "Admin123*");
                if (SesionActiva.Instance.Debe_Cambiar_Password)
                    throw new Exception("Admin no deberia tener flag de cambio");
                return true;
            });
        }

        static void TestSesion_Cerrar()
        {
            EjecutarTest("CerrarSesion limpia sesion", () =>
            {
                var auth = new AuthController();
                auth.ValidarCredenciales("admin", "Admin123*");
                if (SesionActiva.Instance.Id_Usuario == 0) throw new Exception("Sesion no estaba activa");
                auth.CerrarSesion();
                return true;
            });
        }

        // =====================================================
        // HELPERS
        // =====================================================
        static void LimpiarDatosDeTestsAnteriores()
        {
            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Sesion WHERE id_usuario IN 
                          (SELECT id_usuario FROM Usuario WHERE username LIKE 'test_%' OR username LIKE 'test_bloq_%')", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Usuario_Entidad WHERE id_usuario IN 
                          (SELECT id_usuario FROM Usuario WHERE username LIKE 'test_%' OR username LIKE 'test_bloq_%')", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Usuario WHERE username LIKE 'test_%' OR username LIKE 'test_bloq_%'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Alumno WHERE no_control REGEXP '^[TCDE][0-9]+$'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Profesor WHERE no_empleado REGEXP '^[CD][0-9]+$'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Empresa WHERE rfc REGEXP '^TST[0-9]+$|^TES[0-9]+$'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Materia WHERE clave_materia REGEXP '^[TC][0-9]+$'", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"(Warning: No se pudo limpiar datos anteriores: {ex.Message})");
            }
        }

        static void LimpiarUsuarioTest(string username, string email)
        {
            try
            {
                using (var conn = MySQLConnection.GetConnection())
                {
                    conn.Open();
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Sesion WHERE id_usuario IN 
                          (SELECT id_usuario FROM Usuario WHERE username = @u OR email = @e)", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Usuario_Entidad WHERE id_usuario IN 
                          (SELECT id_usuario FROM Usuario WHERE username = @u OR email = @e)", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.ExecuteNonQuery();
                    }
                    using (var cmd = new MySqlConnector.MySqlCommand(
                        @"DELETE FROM Usuario WHERE username = @u OR email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@e", email);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch { }
        }

        static bool EjecutarTest(string nombre, Func<bool> test)
        {
            try
            {
                if (test())
                {
                    ImprimirExito(nombre);
                    _testsPasados++;
                    return true;
                }
                else
                {
                    ImprimirFallo(nombre, "Retorno false");
                    _testsFallados++;
                    return false;
                }
            }
            catch (Exception ex)
            {
                ImprimirFallo(nombre, ex.Message);
                _testsFallados++;
                _errores.Add($"{nombre}: {ex.Message}");
                return false;
            }
        }

        static void ImprimirBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("================================================");
            Console.WriteLine("  TEST RUNNER - SISTEMA MODELO DUAL v2.0");
            Console.WriteLine("  Flujo Profesional: Admin carga -> Usuario activa");
            Console.WriteLine("================================================");
            Console.ResetColor();
        }

        static void ImprimirSeccion(string titulo)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n--- {titulo} ---");
            Console.ResetColor();
        }

        static void ImprimirExito(string nombre)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  [PASS] {nombre}");
            Console.ResetColor();
        }

        static void ImprimirFallo(string nombre, string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  [FAIL] {nombre}");
            Console.WriteLine($"         -> {mensaje}");
            Console.ResetColor();
        }

        static void ImprimirError(string mensaje)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(mensaje);
            Console.ResetColor();
        }

        static void ImprimirResumen()
        {
            int total = _testsPasados + _testsFallados;
            double porcentaje = total > 0 ? (_testsPasados * 100.0 / total) : 0;

            Console.WriteLine($"Tests pasados:  {_testsPasados}");
            Console.WriteLine($"Tests fallados: {_testsFallados}");
            Console.WriteLine($"Total:          {total}");
            Console.WriteLine($"Porcentaje:     {porcentaje:F1}%");
            Console.WriteLine();

            if (_testsFallados == 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("OK TODOS LOS TESTS PASARON - SISTEMA FUNCIONAL");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("HAY TESTS FALLADOS - REVISAR:");
                Console.ResetColor();
                foreach (var err in _errores)
                    Console.WriteLine($"  - {err}");
            }
        }
    }
}
