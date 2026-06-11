using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManagerRounds.admin
{
    public partial class Usuarios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null || Session["rol"].ToString() != "Revisor")
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
                CargarUsuarios();
        }

        private void CargarUsuarios()
        {
            gvUsuarios.DataSource = Control.Control.GetUsuarios();
            gvUsuarios.DataBind();
        }

        protected void gvUsuarios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Toggle")
            {
                Control.Control.ToggleUsuario(id);
                CargarUsuarios();
            }
            else if (e.CommandName == "Editar")
            {
                var u = Control.Control.GetUsuario(id);
                if (u == null) return;

                hfIdUsuario.Value = u.id.ToString();
                lblTituloModal.Text = "Editar Usuario";
                txtNombre.Text = u.Nombre;
                txtNomina.Text = u.Nomina;
                txtPassword.Text = "";
                ddlRol.SelectedValue = u.Rol_id.ToString();

                if (u.Rol_id == 1)
                {
                    var seccion = Control.Control.GetSeccionManager(u.id);
                    if (seccion != null)
                    {
                        txtSeccion.Text = seccion.Seccion;
                        ddlTipoArea.SelectedValue = seccion.TipoArea_id.ToString();
                    }
                }

                CargarUsuarios();

                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalUsuario').modal('show'); toggleSeccion('" + u.Rol_id + "'); };", true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string nomina = txtNomina.Text.Trim();
            string password = txtPassword.Text.Trim();
            int rolId = int.Parse(ddlRol.SelectedValue);
            int id = int.Parse(hfIdUsuario.Value);

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

            if (rolId == 1 && string.IsNullOrEmpty(txtSeccion.Text.Trim()))
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

                if (rolId == 1)
                {
                    var nuevoUsuario = Control.Control.GetUsuarios()
                        .FirstOrDefault(u => u.Nomina == nomina);
                    if (nuevoUsuario != null)
                        Control.Control.CrearSeccionManager(
                            nuevoUsuario.id,
                            txtSeccion.Text.Trim(),
                            int.Parse(ddlTipoArea.SelectedValue),
                            1
                        );
                }

                MostrarMensaje("Usuario creado correctamente.", "alert-success");
            }
            else
            {
                Control.Control.EditarUsuario(id, nombre, nomina, password, rolId);

                if (rolId == 1)
                    Control.Control.EditarSeccionManager(
                        id,
                        txtSeccion.Text.Trim(),
                        int.Parse(ddlTipoArea.SelectedValue),
                        1
                    );

                MostrarMensaje("Usuario actualizado correctamente.", "alert-success");
            }

            CargarUsuarios();
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}