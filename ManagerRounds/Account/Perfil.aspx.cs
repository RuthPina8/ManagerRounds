using System;

namespace ManagerRounds.Account
{
    public partial class Perfil : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null)
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
                CargarPerfil();
        }

        private void CargarPerfil()
        {
            int id = (int)Session["idUsuario"];
            var u = Control.Control.GetUsuario(id);
            if (u == null) return;

            lblNombre.Text = u.Nombre;
            lblNomina.Text = u.Nomina;
            lblRol.Text = u.Roles.Rol;

            if (u.Rol_id == 1)
            {
                var seccion = Control.Control.GetSeccionManager(id);
                if (seccion != null)
                {
                    panelSeccion.Visible = true;
                    lblSeccion.Text = seccion.Seccion;
                    lblTipoArea.Text = seccion.TiposArea.Area;
                }
            }
        }

        protected void btnCambiarPassword_Click(object sender, EventArgs e)
        {
            int id = (int)Session["idUsuario"];
            var u = Control.Control.GetUsuario(id);

            string actual = txtPasswordActual.Text.Trim();
            string nueva = txtPasswordNueva.Text.Trim();
            string confirmar = txtPasswordConfirm.Text.Trim();

            if (u.Password != actual)
            {
                MostrarMensaje("La contraseña actual es incorrecta.", "alert-danger");
                return;
            }

            if (string.IsNullOrEmpty(nueva))
            {
                MostrarMensaje("La nueva contraseña no puede estar vacía.", "alert-danger");
                return;
            }

            if (nueva != confirmar)
            {
                MostrarMensaje("Las contraseñas no coinciden.", "alert-danger");
                return;
            }

            Control.Control.EditarUsuario(id, u.Nombre, u.Nomina, nueva, u.Rol_id);
            MostrarMensaje("Contraseña actualizada correctamente.", "alert-success");

            txtPasswordActual.Text = "";
            txtPasswordNueva.Text = "";
            txtPasswordConfirm.Text = "";
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}