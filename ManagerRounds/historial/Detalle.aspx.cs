using System;
using System.Linq;
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

            int hallazgosCount = Control.Control.GetRespuestasConHallazgoAbierto(revisionId).Count;
            lblHallazgosPendientes.Text = hallazgosCount > 0
                ? $"⚠ {hallazgosCount} hallazgo(s) abierto(s)"
                : "";

            string estatus = revision.EstatusRevision.Estatus;
            lblEstatus.Text = $"<span class='{GetBadgeEstatus(estatus)}'>{estatus}</span>";

            var respuestas = Control.Control.GetRespuestas(revisionId);
            gvRespuestas.DataSource = respuestas;
            gvRespuestas.DataBind();

            // Panel revisor
            if ((Session["rol"].ToString() == "Revisor" || Session["rol"].ToString() == "Admin") &&
                (estatus == "Pendiente" || estatus == "Corregido"))
            {
                panelRevisor.Visible = true;
                if (!string.IsNullOrEmpty(revision.ComentarioRevisor))
                    txtComentario.Text = revision.ComentarioRevisor;
            }

            // Panel corregir
            if (Session["rol"].ToString() == "Manager" && estatus == "Rechazado")
            {
                panelCorregir.Visible = true;
                btnCorregir.OnClientClick = $"window.location='/formulario/Default.aspx?corregir={revision.Check_id}'; return false;";
            }

            // Panel cerrar hallazgos
            if (Session["rol"].ToString() == "Manager")
            {
                var hallazgosAbiertos = Control.Control.GetRespuestasConHallazgoAbierto(revisionId);
                if (hallazgosAbiertos.Count > 0)
                {
                    panelCerrarHallazgos.Visible = true;
                    rptHallazgosAbiertos.DataSource = hallazgosAbiertos;
                    rptHallazgosAbiertos.DataBind();
                }
                else
                {
                    panelCerrarHallazgos.Visible = false;
                }
            }

            ViewState["revisionId"] = revisionId;
        }

        protected void btnAprobar_Click(object sender, EventArgs e)
        {
            int revisionId = (int)ViewState["revisionId"];
            Control.Control.CambiarEstatusRevision(revisionId, 2, txtComentario.Text.Trim());
            var rev = Control.Control.GetRevision(revisionId);
            Control.Control.NotificarManager(rev.Usuario_id, rev.Check_id, "Revisado", "");
            Control.Control.CrearNotificacion(rev.Usuario_id, $"Tu formulario {rev.Check_id} fue aprobado", "/historial/Default.aspx");
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
            var rev = Control.Control.GetRevision(revisionId);
            Control.Control.NotificarManager(rev.Usuario_id, rev.Check_id, "Rechazado", txtComentario.Text.Trim());
            Control.Control.CrearNotificacion(rev.Usuario_id, $"Tu formulario {rev.Check_id} fue rechazado — {txtComentario.Text.Trim()}", $"/formulario/Default.aspx?corregir={rev.Check_id}");
            MostrarMensaje("Revisión rechazada.", "alert-warning");
            CargarDetalle(revisionId);
        }

        protected void rptHallazgosAbiertos_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "CerrarHallazgo")
            {
                int respuestaId = int.Parse(e.CommandArgument.ToString());
                int revisionId = (int)ViewState["revisionId"];

                var fuCierre = (System.Web.UI.WebControls.FileUpload)e.Item.FindControl("fuCierreHallazgo");

                if (fuCierre != null && fuCierre.HasFile)
                {
                    // Obtener preguntaId correcto de la respuesta
                    var respuesta = Control.Control.GetRespuestas(revisionId)
                        .FirstOrDefault(r => r.id == respuestaId);

                    if (respuesta != null)
                    {
                        string serverPath = Server.MapPath("~/");
                        string rutaCierre = Control.Control.SubirFoto(
                            fuCierre.PostedFile, revisionId, respuesta.Pregunta_id, "cierre", serverPath);
                        Control.Control.CerrarHallazgo(respuestaId, rutaCierre);
                        MostrarMensaje("Hallazgo cerrado correctamente.", "alert-success");
                        CargarDetalle(revisionId);
                    }
                }
                else
                {
                    MostrarMensaje("Debes subir una foto de cierre.", "alert-danger");
                }
            }
        }

        public string GetBadgeRespuesta(string respuesta)
        {
            switch (respuesta)
            {
                case "Aprobado": return "badge badge-cumple";
                case "Falla": return "badge badge-nocumple";
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

        public string RenderHallazgo(string fotoProblema, string fotoCierre, bool cerrado, string respuesta)
        {
            if (respuesta != "Falla") return "";
            if (string.IsNullOrEmpty(fotoProblema))
                return "<span style='font-size:11px; color:#F0A500;'><i class='fas fa-exclamation-triangle'></i> Sin foto</span>";

            string html = "<div style='display:flex; gap:8px; align-items:center;'>";
            html += "<div style='text-align:center;'>";
            html += $"<p style='font-size:10px; color:#888; margin:0 0 3px;'>Problema</p>";
            html += $"<img src='{ResolveUrl(fotoProblema)}' class='foto-hallazgo' onclick=\"verFoto('{ResolveUrl(fotoProblema)}')\" />";
            html += "</div>";

            if (!string.IsNullOrEmpty(fotoCierre))
            {
                html += "<div style='text-align:center;'>";
                html += "<p style='font-size:10px; color:#3B6D11; margin:0 0 3px;'>Cierre</p>";
                html += $"<img src='{ResolveUrl(fotoCierre)}' class='foto-hallazgo' onclick=\"verFoto('{ResolveUrl(fotoCierre)}')\" />";
                html += "</div>";
            }
            else
            {
                html += "<span style='font-size:11px; color:#F0A500; margin-left:4px;'><i class='fas fa-clock'></i> Pendiente cierre</span>";
            }

            html += "</div>";
            return html;
        }
    }
}