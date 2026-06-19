using System;

namespace ManagerRounds.Account
{
    public partial class Recuperar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string nomina = txtNomina.Text.Trim();

            if (string.IsNullOrEmpty(nomina))
            {
                MostrarMensaje("Ingresa tu nómina.", "alert-danger");
                return;
            }

            string pregunta = Control.Control.GetPreguntaRecuperacion(nomina);

            if (string.IsNullOrEmpty(pregunta))
            {
                MostrarMensaje("No se encontró ningún usuario con esa nómina o no tiene pregunta de recuperación configurada.", "alert-danger");
                return;
            }

            Session["nominaRecuperacion"] = nomina;
            lblPregunta.Text = pregunta;
            pnlPaso1.Visible = false;
            pnlPaso2.Visible = true;
            lblMensaje.Visible = false;
        }

        protected void btnValidar_Click(object sender, EventArgs e)
        {
            string nomina = Session["nominaRecuperacion"]?.ToString();
            string respuesta = txtRespuesta.Text.Trim();

            if (string.IsNullOrEmpty(respuesta))
            {
                MostrarMensaje("Ingresa tu respuesta.", "alert-danger");
                return;
            }

            if (!Control.Control.ValidarRespuestaRecuperacion(nomina, respuesta))
            {
                MostrarMensaje("Respuesta incorrecta.", "alert-danger");
                return;
            }

            pnlPaso2.Visible = false;
            pnlPaso3.Visible = true;
            lblMensaje.Visible = false;
        }

        protected void btnResetear_Click(object sender, EventArgs e)
        {
            string nomina = Session["nominaRecuperacion"]?.ToString();
            string nueva = txtNueva.Text.Trim();
            string confirmar = txtConfirmar.Text.Trim();

            if (string.IsNullOrEmpty(nueva))
            {
                MostrarMensaje("Ingresa una nueva contraseña.", "alert-danger");
                return;
            }

            if (nueva != confirmar)
            {
                MostrarMensaje("Las contraseñas no coinciden.", "alert-danger");
                return;
            }

            Control.Control.ResetearPassword(nomina, nueva);
            Session.Remove("nominaRecuperacion");

            MostrarMensaje("Contraseña actualizada correctamente. Ya puedes iniciar sesión.", "alert-success");
            pnlPaso3.Visible = false;
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}