using System;

namespace ManagerRounds.Account
{
    public partial class Configurar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
                Response.Redirect("/Account/Login.aspx");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string pregunta = ddlPregunta.SelectedValue;
            string respuesta = txtRespuesta.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmar = txtConfirmar.Text.Trim();

            if (string.IsNullOrEmpty(pregunta))
            {
                MostrarMensaje("Selecciona una pregunta de seguridad.", "alert-danger");
                return;
            }

            if (string.IsNullOrEmpty(respuesta))
            {
                MostrarMensaje("Ingresa tu respuesta.", "alert-danger");
                return;
            }

            if (!string.IsNullOrEmpty(password) && password != confirmar)
            {
                MostrarMensaje("Las contraseñas no coinciden.", "alert-danger");
                return;
            }

            int id = (int)Session["idUsuario"];
            Control.Control.GuardarRecuperacion(id, pregunta, respuesta);

            if (!string.IsNullOrEmpty(password))
            {
                var u = Control.Control.GetUsuario(id);
                Control.Control.EditarUsuario(id, u.Nombre, u.Nomina, password, u.Rol_id);
            }

            Response.Redirect("/dashboard/Default.aspx");
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}