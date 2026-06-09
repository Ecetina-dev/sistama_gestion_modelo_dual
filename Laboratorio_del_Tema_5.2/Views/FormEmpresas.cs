#pragma warning disable CS0414
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Laboratorio_del_Tema_5_2.Controllers;
using Laboratorio_del_Tema_5_2.Models;
using Laboratorio_del_Tema_5_2.Utils;

namespace Laboratorio_del_Tema_5_2.Views
{
    public partial class FormEmpresas : Form
    {
        private EmpresaController controller = new EmpresaController();
        private bool isEditing = false;
        private bool isNewRecord = false;
        private bool isLoading = false;
        private List<Empresa> _cache = new List<Empresa>();

        // Limites EXACTOS del modelo y BD
        private const int MAX_NOMBRE_COMERCIAL = 200;
        private const int MAX_RAZON_SOCIAL = 200;
        private const int MAX_RFC = 13;
        private const int EXT_MIN_RFC = 12;
        private const int MAX_DIRECCION = 300;
        private const int MAX_CIUDAD = 100;
        private const int MAX_ESTADO = 100;
        private const int MAX_CP = 5;
        private const int EXT_MIN_CP = 5;
        private const int MAX_TELEFONO = 10;
        private const int EXT_MIN_TELEFONO = 10;
        private const int MAX_NOMBRE_CONTACTO = 100;
        private const int EXT_MIN_NOMBRE_CONTACTO = 3;
        private const int MAX_PUESTO_CONTACTO = 100;
        private const int EXT_MIN_NOMBRE_COMERCIAL = 3;

        public FormEmpresas()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CargarEmpresas();
        }

        private void ConfigurarFormulario()
        {
            // Limitar longitud de campos segun modelo
            txtNombreComercial.MaxLength = MAX_NOMBRE_COMERCIAL;
            txtRazonSocial.MaxLength = MAX_RAZON_SOCIAL;
            txtRFC.MaxLength = MAX_RFC;
            txtDireccion.MaxLength = MAX_DIRECCION;
            txtCiudad.MaxLength = MAX_CIUDAD;
            txtEstado.MaxLength = MAX_ESTADO;
            txtCP.MaxLength = MAX_CP;
            txtTelefono.MaxLength = MAX_TELEFONO;
            txtNombreContacto.MaxLength = MAX_NOMBRE_CONTACTO;
            txtPuestoContacto.MaxLength = MAX_PUESTO_CONTACTO;

            // Enter y Escape a nivel form
            this.KeyPreview = true;
            this.KeyDown += FormEmpresas_KeyDown;

            // Doble click en grid para editar
            dgvEmpresas.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) btnEditar_Click(s, e);
            };

            // Restaurar color al escribir
            EventHandler restaurar = (s, e) => {
                if (s is TextBox txt && txt.BackColor == Color.FromArgb(255, 235, 235))
                    txt.BackColor = Color.FromArgb(248, 250, 252);
            };
            txtNombreComercial.TextChanged += restaurar;
            txtRazonSocial.TextChanged += restaurar;
            txtRFC.TextChanged += restaurar;
            txtDireccion.TextChanged += restaurar;
            txtCiudad.TextChanged += restaurar;
            txtEstado.TextChanged += restaurar;
            txtCP.TextChanged += restaurar;
            txtTelefono.TextChanged += restaurar;
            txtNombreContacto.TextChanged += restaurar;
            txtPuestoContacto.TextChanged += restaurar;

            // Enter navega al siguiente campo
            ConfigurarEnterSgteCampo(txtNombreComercial, txtRazonSocial);
            ConfigurarEnterSgteCampo(txtRazonSocial, txtRFC);
            ConfigurarEnterSgteCampo(txtRFC, txtDireccion);
            ConfigurarEnterSgteCampo(txtDireccion, txtCiudad);
            ConfigurarEnterSgteCampo(txtCiudad, txtEstado);
            ConfigurarEnterSgteCampo(txtEstado, txtCP);
            ConfigurarEnterSgteCampo(txtCP, txtTelefono);
            ConfigurarEnterSgteCampo(txtTelefono, txtNombreContacto);
            ConfigurarEnterSgteCampo(txtNombreContacto, txtPuestoContacto);
            ConfigurarEnterSgteCampo(txtPuestoContacto, btnGuardar);
            // Desde btnGuardar con Enter tambien guarda
            btnGuardar.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !isLoading)
                {
                    e.SuppressKeyPress = true;
                    btnGuardar_Click(s, e);
                }
            };

            // Tab index
            txtNombreComercial.TabIndex = 0;
            txtRazonSocial.TabIndex = 1;
            txtRFC.TabIndex = 2;
            txtDireccion.TabIndex = 3;
            txtCiudad.TabIndex = 4;
            txtEstado.TabIndex = 5;
            txtCP.TabIndex = 6;
            txtTelefono.TabIndex = 7;
            txtNombreContacto.TabIndex = 8;
            txtPuestoContacto.TabIndex = 9;
            btnGuardar.TabIndex = 10;
            btnCancelar.TabIndex = 11;
        }

        // Enter sgte campo, Escape = cancelar
        private void FormEmpresas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && panelCardDatos.Visible)
            {
                e.SuppressKeyPress = true;
                btnCancelar_Click(sender, e);
            }
        }

        private void ConfigurarEnterSgteCampo(TextBox actual, Control siguiente)
        {
            actual.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    siguiente.Focus();
                }
            };
        }

        // ==================== CARGA DE DATOS ====================

        private void CargarEmpresas()
        {
            try
            {
                _cache = controller.Read() ?? new List<Empresa>();
                AplicarFiltroBusqueda();
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text.Trim().ToLower();

            var fuente = string.IsNullOrEmpty(filtro)
                ? _cache
                : _cache.Where(e =>
                    (e.Nombre_Comercial?.ToLower().Contains(filtro) ?? false) ||
                    (e.Razon_Social?.ToLower().Contains(filtro) ?? false) ||
                    (e.RFC?.ToLower().Contains(filtro) ?? false) ||
                    (e.Ciudad?.ToLower().Contains(filtro) ?? false) ||
                    (e.Estado?.ToLower().Contains(filtro) ?? false) ||
                    (e.Nombre_Contacto?.ToLower().Contains(filtro) ?? false)
                ).ToList();

            dgvEmpresas.DataSource = null;
            dgvEmpresas.DataSource = fuente;
            ConfigurarGrid();
        }

        private void ConfigurarGrid()
        {
            if (dgvEmpresas.Columns.Count == 0) return;

            // Ocultar columnas tecnicas
            var ocultar = new[] { "Id_Empresa", "Created_At", "Updated_At", "Status_Empresa" };
            foreach (var col in ocultar)
                if (dgvEmpresas.Columns.Contains(col))
                    dgvEmpresas.Columns[col].Visible = false;

            // Headers legibles
            var headers = new Dictionary<string, string>
            {
                ["Nombre_Comercial"] = "Nombre Comercial",
                ["Razon_Social"] = "Razon Social",
                ["RFC"] = "RFC",
                ["Direccion"] = "Direccion",
                ["Ciudad"] = "Ciudad",
                ["Estado"] = "Estado",
                ["CP"] = "C.P.",
                ["Telefono_Empresa"] = "Telefono",
                ["Email_Empresa"] = "Email",
                ["Nombre_Contacto"] = "Contacto",
                ["Puesto_Contacto"] = "Puesto"
            };
            foreach (var kv in headers)
                if (dgvEmpresas.Columns.Contains(kv.Key))
                    dgvEmpresas.Columns[kv.Key].HeaderText = kv.Value;

            // Reordenar
            string[] orden = { "Nombre_Comercial", "Razon_Social", "RFC", "Direccion",
                               "Ciudad", "Estado", "CP", "Telefono_Empresa",
                               "Email_Empresa", "Nombre_Contacto", "Puesto_Contacto" };
            int idx = 0;
            foreach (var col in orden)
                if (dgvEmpresas.Columns.Contains(col))
                    dgvEmpresas.Columns[col].DisplayIndex = idx++;
        }

        // ==================== NAVEGACION ====================

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltroBusqueda();
        }

        // ==================== CRUD ====================

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            isNewRecord = true;
            isEditing = true;
            LimpiarFormulario();
            panelCardDatos.Visible = true;
            txtNombreComercial.Focus();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvEmpresas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una empresa de la lista.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isNewRecord = false;
            isEditing = true;
            CargarDatosFormulario();
            panelCardDatos.Visible = true;
            txtNombreComercial.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvEmpresas.SelectedRows.Count == 0)
            {
                MessageBox.Show("Selecciona una empresa.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var nombre = dgvEmpresas.SelectedRows[0].Cells["Nombre_Comercial"]?.Value?.ToString() ?? "esta empresa";

            if (MessageBox.Show($"Eliminar a '{nombre}'?\n\nEsto no se puede deshacer.",
                "Confirmar eliminacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;

            int id = (int)dgvEmpresas.SelectedRows[0].Cells["Id_Empresa"].Value;
            try
            {
                if (controller.Delete(id))
                {
                    MostrarExito("Empresa eliminada.");
                    CargarEmpresas();
                }
                else
                    MostrarError("No se pudo eliminar. Puede tener registros asociados.");
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarEmpresas();
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
        }

        // ==================== GUARDAR ====================

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (isLoading) return;

            // 1. Validar
            string error = ValidarFormulario();
            if (error != null)
            {
                MostrarAdvertencia(error);
                return;
            }

            // 2. Duplicado RFC
            int? idExcluir = isNewRecord ? null : (int?)Convert.ToInt32(txtIdEmpresa.Text);
            if (controller.ExisteRFC(txtRFC.Text.Trim(), idExcluir))
            {
                MarcarError(txtRFC);
                MostrarAdvertencia("El RFC '" + txtRFC.Text.Trim() + "' ya existe.");
                txtRFC.Focus();
                return;
            }

            // 3. Construir objeto
            var empresa = new Empresa
            {
                Nombre_Comercial = txtNombreComercial.Text.Trim(),
                Razon_Social = NullIfEmpty(txtRazonSocial.Text),
                RFC = txtRFC.Text.Trim(),
                Direccion = NullIfEmpty(txtDireccion.Text),
                Ciudad = NullIfEmpty(txtCiudad.Text),
                Estado = NullIfEmpty(txtEstado.Text),
                CP = NullIfEmpty(txtCP.Text),
                Telefono_Empresa = NullIfEmpty(txtTelefono.Text),
                Email_Empresa = NullIfEmpty(txtEmail.Text),
                Nombre_Contacto = txtNombreContacto.Text.Trim(),
                Puesto_Contacto = NullIfEmpty(txtPuestoContacto.Text),
                Status_Empresa = Estatus.EmpresaActiva,
            };

            // 4. Loading state
            isLoading = true;
            btnGuardar.Enabled = false;
            btnGuardar.Text = "Guardando...";
            Cursor = Cursors.WaitCursor;

            // 5. Guardar
            bool success;
            try
            {
                if (isNewRecord)
                    success = await System.Threading.Tasks.Task.Run(() => controller.Create(empresa));
                else
                {
                    empresa.Id_Empresa = Convert.ToInt32(txtIdEmpresa.Text);
                    success = await System.Threading.Tasks.Task.Run(() => controller.Update(empresa));
                }
            }
            catch (Exception ex)
            {
                MostrarError("Error BD: " + ex.Message);
                return;
            }
            finally
            {
                isLoading = false;
                btnGuardar.Enabled = true;
                btnGuardar.Text = "\u2714\uFE0F  Guardar";
                Cursor = Cursors.Default;
            }

            if (success)
            {
                MostrarExito("Empresa guardada.");
                isEditing = false;
                isNewRecord = false;
                panelCardDatos.Visible = false;
                CargarEmpresas();
            }
            else
                MostrarError("Error al guardar. Verifica los datos.");
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (HayCambiosSinGuardar())
            {
                var r = MessageBox.Show("Hay cambios sin guardar.\n\n\u00bfDescartarlos?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (r != DialogResult.Yes) return;
            }
            isEditing = false;
            isNewRecord = false;
            panelCardDatos.Visible = false;
            LimpiarFormulario();
            RestaurarColores();
        }

        // ==================== VALIDACION ====================

        private string ValidarFormulario()
        {
            RestaurarColores();

            // RFC
            string rfc = txtRFC.Text.Trim();
            if (string.IsNullOrEmpty(rfc))
            {
                MarcarError(txtRFC);
                return "El RFC es obligatorio.";
            }
            if (rfc.Length < EXT_MIN_RFC || rfc.Length > MAX_RFC)
            {
                MarcarError(txtRFC);
                return $"El RFC debe tener entre {EXT_MIN_RFC} y {MAX_RFC} caracteres.";
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(rfc, @"^[A-Za-z0-9]{12,13}$"))
            {
                MarcarError(txtRFC);
                return "El RFC debe contener solo letras y numeros (ej: ABCD123456XX).";
            }

            // Nombre Comercial
            if (string.IsNullOrWhiteSpace(txtNombreComercial.Text))
            {
                MarcarError(txtNombreComercial);
                return "El nombre comercial es obligatorio.";
            }
            if (txtNombreComercial.Text.Trim().Length < EXT_MIN_NOMBRE_COMERCIAL)
            {
                MarcarError(txtNombreComercial);
                return "El nombre comercial debe tener al menos 3 caracteres.";
            }

            // Nombre Contacto
            if (string.IsNullOrWhiteSpace(txtNombreContacto.Text))
            {
                MarcarError(txtNombreContacto);
                return "El nombre de contacto es obligatorio.";
            }
            if (txtNombreContacto.Text.Trim().Length < EXT_MIN_NOMBRE_CONTACTO)
            {
                MarcarError(txtNombreContacto);
                return "El nombre de contacto debe tener al menos 3 caracteres.";
            }

            // Email
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                string email = txtEmail.Text.Trim();
                if (!System.Text.RegularExpressions.Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                {
                    MarcarError(txtEmail);
                    return "Formato de email invalido (ej: usuario@dominio.com).";
                }
            }

            // Telefono
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                string tel = txtTelefono.Text.Trim();
                string soloDigitos = new string(tel.Where(char.IsDigit).ToArray());
                if (soloDigitos.Length != EXT_MIN_TELEFONO)
                {
                    MarcarError(txtTelefono);
                    return $"El telefono debe tener {EXT_MIN_TELEFONO} digitos (ej: 5551234567).";
                }
            }

            // CP
            if (!string.IsNullOrWhiteSpace(txtCP.Text))
            {
                string cp = txtCP.Text.Trim();
                string soloDigitos = new string(cp.Where(char.IsDigit).ToArray());
                if (soloDigitos.Length != EXT_MIN_CP)
                {
                    MarcarError(txtCP);
                    return $"El CP debe tener {EXT_MIN_CP} digitos (ej: 12345).";
                }
            }

            return null;
        }

        private bool HayCambiosSinGuardar()
        {
            return !string.IsNullOrWhiteSpace(txtNombreComercial.Text) ||
                   !string.IsNullOrWhiteSpace(txtRFC.Text) ||
                   !string.IsNullOrWhiteSpace(txtNombreContacto.Text);
        }

        private void MarcarError(TextBox txt)
        {
            txt.BackColor = Color.FromArgb(255, 235, 235);
            txt.Focus();
        }

        private void RestaurarColores()
        {
            foreach (var txt in new[] { txtNombreComercial, txtRazonSocial, txtRFC,
                txtDireccion, txtCiudad, txtEstado, txtCP, txtTelefono, txtEmail,
                txtNombreContacto, txtPuestoContacto })
                txt.BackColor = Color.FromArgb(248, 250, 252);
        }

        // ==================== CARGA AL FORMULARIO ====================

        private void CargarDatosFormulario()
        {
            if (dgvEmpresas.SelectedRows.Count == 0) return;

            int idEmpresa = (int)dgvEmpresas.SelectedRows[0].Cells["Id_Empresa"].Value;

            // Buscar en cache (tiene TODOS los datos)
            var e = _cache.FirstOrDefault(x => x.Id_Empresa == idEmpresa);
            if (e == null)
            {
                MostrarAdvertencia("No se pudieron cargar los datos completos de la empresa.");
                return;
            }

            txtIdEmpresa.Text = e.Id_Empresa.ToString();
            txtNombreComercial.Text = e.Nombre_Comercial ?? "";
            txtRazonSocial.Text = e.Razon_Social ?? "";
            txtRFC.Text = e.RFC ?? "";
            txtDireccion.Text = e.Direccion ?? "";
            txtCiudad.Text = e.Ciudad ?? "";
            txtEstado.Text = e.Estado ?? "";
            txtCP.Text = e.CP ?? "";
            txtTelefono.Text = e.Telefono_Empresa ?? "";
            txtEmail.Text = e.Email_Empresa ?? "";
            txtNombreContacto.Text = e.Nombre_Contacto ?? "";
            txtPuestoContacto.Text = e.Puesto_Contacto ?? "";
        }

        private void LimpiarFormulario()
        {
            foreach (var txt in new[] { txtIdEmpresa, txtNombreComercial, txtRazonSocial,
                txtRFC, txtDireccion, txtCiudad, txtEstado, txtCP, txtTelefono, txtEmail,
                txtNombreContacto, txtPuestoContacto })
                txt.Clear();
            RestaurarColores();
        }

        // ==================== HELPERS ====================

        private string NullIfEmpty(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private void MostrarError(string msg) =>
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void MostrarAdvertencia(string msg) =>
            MessageBox.Show(msg, "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void MostrarExito(string msg) =>
            MessageBox.Show(msg, "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
