using System;
using System.Web.UI.WebControls;

namespace ManagerRounds.historial
{
    public partial class Detalle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null)
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
            {
                string idParam = Request.QueryString["id"];
                if (string.IsNullOrEmpty(idParam))
                    Response.Redirect("~/historial/Default.aspx");

                int revisionId = int.Parse(idParam);
                CargarDetalle(revisionId);
            }
        }

        private void CargarDetalle(int revisionId)
        {
            var revision = Control.Control.GetRevision(revisionId);
            if (revision == null)
                Response.Redirect("~/historial/Default.aspx");

            lblManager.Text = revision.Usuarios.Nombre;
            lblSeccion.Text = revision.ManagerSecciones?.Seccion ?? "—";
            lblCheck.Text = revision.Check_id;
            lblFecha.Text = revision.FechaInicio?.ToString("ddd dd MMM yyyy · hh:mm tt");
            lblCalificacion.Text = revision.Calificacion != null ? revision.Calificacion + "%" : "—";

            string estatus = revision.EstatusRevision.Estatus;
            lblEstatus.Text = $"<span class='{GetBadgeEstatus(estatus)}'>{estatus}</span>";

            var respuestas = Control.Control.GetRespuestas(revisionId);
            gvRespuestas.DataSource = respuestas;
            gvRespuestas.DataBind();

            // Mostrar panel revisor solo si es Revisor y la revisión está Pendiente o Rechazado
            if (Session["rol"].ToString() == "Revisor" &&
                (estatus == "Pendiente" || estatus == "Rechazado"))
            {
                panelRevisor.Visible = true;
                if (!string.IsNullOrEmpty(revision.ComentarioRevisor))
                    txtComentario.Text = revision.ComentarioRevisor;
            }

            ViewState["revisionId"] = revisionId;
        }

        protected void btnAprobar_Click(object sender, EventArgs e)
        {
            int revisionId = (int)ViewState["revisionId"];
            Control.Control.CambiarEstatusRevision(revisionId, 2, txtComentario.Text.Trim());
            MostrarMensaje("Revisión aprobada correctamente.", "alert-success");
            panelRevisor.Visible = false;
            CargarDetalle(revisionId);
        }

        protected void btnRechazar_Click(object sender, EventArgs e)
        {
            int revisionId = (int)ViewState["revisionId"];

            if (string.IsNullOrEmpty(txtComentario.Text.Trim()))
            {
                MostrarMensaje("El comentario es obligatorio para rechazar.", "alert-danger");
                return;
            }

            Control.Control.CambiarEstatusRevision(revisionId, 3, txtComentario.Text.Trim());
            MostrarMensaje("Revisión rechazada.", "alert-warning");
            CargarDetalle(revisionId);
        }

        public string GetBadgeRespuesta(string respuesta)
        {
            switch (respuesta)
            {
                case "Cumple": return "badge badge-cumple";
                case "No Cumple": return "badge badge-nocumple";
                case "N/A": return "badge badge-na";
                default: return "badge badge-secondary";
            }
        }

        public string GetBadgeEstatus(string estatus)
        {
            switch (estatus)
            {
                case "Pendiente": return "badge badge-pendiente";
                case "Revisado": return "badge badge-revisado";
                case "Rechazado": return "badge badge-rechazado";
                case "Corregido": return "badge badge-corregido";
                default: return "badge badge-secondary";
            }
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}
