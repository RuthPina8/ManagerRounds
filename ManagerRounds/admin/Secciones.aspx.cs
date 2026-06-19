using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManagerRounds.admin
{
    public partial class Secciones : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null || Session["rol"].ToString() != "Admin")
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
                CargarSecciones();
        }

        private void CargarSecciones()
        {
            gvSecciones.DataSource = Control.Control.GetSecciones();
            gvSecciones.DataBind();
        }

        protected void gvSecciones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Toggle")
            {
                Control.Control.ToggleSeccion(id);
                CargarSecciones();
            }
            else if (e.CommandName == "Editar")
            {
                var s = Control.Control.GetSecciones().FirstOrDefault(x => x.id == id);
                if (s == null) return;

                hfIdSeccion.Value = s.id.ToString();
                lblTituloModal.Text = "Editar Sección";
                txtNombre.Text = s.Nombre;

                CargarSecciones();

                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalSeccion').modal('show'); };", true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            int id = int.Parse(hfIdSeccion.Value);

            if (string.IsNullOrEmpty(nombre))
            {
                MostrarMensaje("El nombre es obligatorio.", "alert-danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalSeccion').modal('show'); };", true);
                return;
            }

            if (id == 0)
            {
                Control.Control.CrearSeccion(nombre);
                MostrarMensaje("Sección creada correctamente.", "alert-success");
            }
            else
            {
                Control.Control.EditarSeccion(id, nombre);
                MostrarMensaje("Sección actualizada correctamente.", "alert-success");
            }

            CargarSecciones();
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}