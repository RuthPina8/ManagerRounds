using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManagerRounds.admin
{
    public partial class Usuarios : System.Web.UI.Page
    {
        private const int PageSize = 15;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null || Session["rol"].ToString() != "Admin")
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
            {
                CargarUsuarios();
                CargarSeccionesDdl();
            }
        }

        private void CargarUsuarios(int pageIndex = 0)
        {
            var usuarios = Control.Control.GetUsuarios();

            int totalRegistros = usuarios.Count;
            int totalPaginas = (int)Math.Ceiling((double)totalRegistros / PageSize);

            if (pageIndex < 0) pageIndex = 0;
            if (pageIndex >= totalPaginas && totalPaginas > 0) pageIndex = totalPaginas - 1;

            ViewState["PageIndex"] = pageIndex;
            ViewState["TotalPaginas"] = totalPaginas;

            var paginado = usuarios.Skip(pageIndex * PageSize).Take(PageSize).ToList();

            gvUsuarios.DataSource = paginado;
            gvUsuarios.DataBind();

            lblPaginacion.Text = $"Página {pageIndex + 1} de {totalPaginas} · {totalRegistros} usuarios";
            btnPagAnterior.Enabled = pageIndex > 0;
            btnPagSiguiente.Enabled = pageIndex < totalPaginas - 1;
        }

        private void CargarSeccionesDdl()
        {
            ddlSeccion.DataSource = Control.Control.GetSecciones(true);
            ddlSeccion.DataTextField = "Nombre";
            ddlSeccion.DataValueField = "Nombre";
            ddlSeccion.DataBind();
            ddlSeccion.Items.Insert(0, new ListItem("-- Selecciona una sección --", ""));
        }

        protected void btnPagAnterior_Click(object sender, EventArgs e)
        {
            int pageIndex = (int)(ViewState["PageIndex"] ?? 0);
            CargarUsuarios(pageIndex - 1);
        }

        protected void btnPagSiguiente_Click(object sender, EventArgs e)
        {
            int pageIndex = (int)(ViewState["PageIndex"] ?? 0);
            CargarUsuarios(pageIndex + 1);
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            int pageIndex = (int)(ViewState["PageIndex"] ?? 0);

            if (e.CommandName == "Toggle")
            {
                Control.Control.ToggleUsuario(id);
                CargarUsuarios(pageIndex);
            }
            else if (e.CommandName == "Editar")
            {
                var u = Control.Control.GetUsuario(id);
                if (u == null) return;

                hfIdUsuario.Value = u.id.ToString();
                lblTituloModal.Text = "Editar Usuario";
                txtNombre.Text = u.Nombre;
                txtNomina.Text = u.Nomina;
                txtEmail.Text = u.Email ?? "";
                txtPassword.Text = "";
                ddlRol.SelectedValue = u.Rol_id.ToString();

                if (u.Rol_id == 1)
                {
                    CargarSeccionesDdl();
                    var seccion = Control.Control.GetSeccionManager(u.id);
                    if (seccion != null)
                    {
                        if (ddlSeccion.Items.FindByValue(seccion.Seccion) != null)
                            ddlSeccion.SelectedValue = seccion.Seccion;
                        ddlTipoArea.SelectedValue = seccion.TipoArea_id.ToString();
                    }
                }

                CargarUsuarios(pageIndex);

                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalUsuario').modal('show'); toggleSeccion('" + u.Rol_id + "'); };", true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string nomina = txtNomina.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            int rolId = int.Parse(ddlRol.SelectedValue);
            int id = int.Parse(hfIdUsuario.Value);
            int pageIndex = (int)(ViewState["PageIndex"] ?? 0);

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(nomina))
            {
                MostrarMensaje("Nombre y nómina son obligatorios.", "alert-danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalUsuario').modal('show'); };", true);
                return;
            }

            if (Control.Control.NominaExiste(nomina, id == 0 ? (int?)null : id))
            {
                MostrarMensaje("Esa nómina ya está registrada.", "alert-danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalUsuario').modal('show'); };", true);
                return;
            }

            if (rolId == 1 && string.IsNullOrEmpty(ddlSeccion.SelectedValue))
            {
                MostrarMensaje("La sección es obligatoria para managers.", "alert-danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalUsuario').modal('show'); toggleSeccion('1'); };", true);
                return;
            }

            if (id == 0)
            {
                if (string.IsNullOrEmpty(password))
                {
                    MostrarMensaje("La contraseña es obligatoria para usuarios nuevos.", "alert-danger");
                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                        "window.onload = function(){ $('#modalUsuario').modal('show'); };", true);
                    return;
                }

                Control.Control.CrearUsuario(nombre, nomina, password, rolId);

                var nuevoUsuario = Control.Control.GetUsuarios()
                    .FirstOrDefault(u => u.Nomina == nomina);

                if (nuevoUsuario != null)
                {
                    if (rolId == 1)
                        Control.Control.CrearSeccionManager(
                            nuevoUsuario.id,
                            ddlSeccion.SelectedValue,
                            int.Parse(ddlTipoArea.SelectedValue),
                            1
                        );

                    if (!string.IsNullOrEmpty(email))
                        Control.Control.GuardarEmail(nuevoUsuario.id, email);
                }

                MostrarMensaje("Usuario creado correctamente.", "alert-success");
            }
            else
            {
                Control.Control.EditarUsuario(id, nombre, nomina, password, rolId);

                if (rolId == 1)
                    Control.Control.EditarSeccionManager(
                        id,
                        ddlSeccion.SelectedValue,
                        int.Parse(ddlTipoArea.SelectedValue),
                        1
                    );

                if (!string.IsNullOrEmpty(email))
                    Control.Control.GuardarEmail(id, email);

                MostrarMensaje("Usuario actualizado correctamente.", "alert-success");
            }

            CargarUsuarios(pageIndex);
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
        public string GetBadgeRol(string rol)
        {
            switch (rol)
            {
                case "Manager": return "badge-rol-manager";
                case "Revisor": return "badge-rol-revisor";
                case "Admin": return "badge-rol-admin";
                default: return "badge badge-secondary";
            }
        }
    }
}