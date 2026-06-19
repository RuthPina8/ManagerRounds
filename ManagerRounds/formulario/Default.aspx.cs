using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManagerRounds.formulario
{
    public partial class WebForm1 : Page
    {
        Control.Control control = new Control.Control();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] == null)
                Response.Redirect("/Account/Login.aspx");

            if (!IsPostBack)
            {
                int usuarioId = Convert.ToInt32(Session["idUsuario"]);
                lblFecha.Text = DateTime.Today.ToString("dddd dd 'de' MMMM yyyy", new System.Globalization.CultureInfo("es-MX"));
                lblNombreManager.Text = Session["nombreUsuario"].ToString();
                lblEstatus.Text = "En progreso";

                string corregirCheckId = Request.QueryString["corregir"];
                if (!string.IsNullOrEmpty(corregirCheckId))
                {
                    string checkNumero = corregirCheckId.Substring(0, 1);
                    string tipoCheck = corregirCheckId.Substring(1, 1);
                    Session["checkNumero"] = checkNumero;
                    pnlRecuperacion.Visible = false;
                    CargarFormulario(tipoCheck);
                    return;
                }

                string checkNumero2 = control.GetCheckDelDia();

                if (checkNumero2 == null)
                {
                    lblEstatus.Text = "No hay formulario hoy";
                    return;
                }

                if (checkNumero2 == "R")
                {
                    pnlSelectorCheck.Visible = false;
                    CargarChecksPendientes(usuarioId);
                }
                else
                {
                    Session["checkNumero"] = checkNumero2;
                    pnlRecuperacion.Visible = false;
                    CargarFormulario("A");
                }
            }
        }

        private void CargarChecksPendientes(int usuarioId)
        {
            var checksSemanales = new List<string> { "1A", "1B", "2A", "2B", "3A", "3B", "4A", "4B" };
            var db = new Datos.DataClasses1DataContext();

            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);

            var completados = db.Revisiones
                .Where(r => r.Usuario_id == usuarioId && r.Fecha >= lunes && r.Fecha <= DateTime.Today)
                .Select(r => r.Check_id)
                .Distinct()
                .ToList();

            var todos = checksSemanales
                .Select(c => new { CheckId = c, Completado = completados.Contains(c) })
                .ToList();

            bool todoCompleto = todos.All(c => c.Completado);

            if (todoCompleto)
            {
                pnlRecuperacion.Visible = false;
                pnlAlCorriente.Visible = true;
                lblEstatus.Text = "Al corriente";
            }
            else
            {
                pnlRecuperacion.Visible = true;
                rptChecksPendientes.DataSource = todos;
                rptChecksPendientes.DataBind();
            }
        }

        protected void rptChecksPendientes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SeleccionarCheck")
            {
                string checkId = e.CommandArgument.ToString();
                string checkNumero = checkId.Substring(0, 1);
                string tipoCheck = checkId.Substring(1, 1);
                Session["checkNumero"] = checkNumero;
                CargarFormulario(tipoCheck);
            }
        }

        protected void btnCheckA_Click(object sender, EventArgs e)
        {
            CargarFormulario("A");
        }

        protected void btnCheckB_Click(object sender, EventArgs e)
        {
            CargarFormulario("B");
        }

        private void CargarFormulario(string tipoCheck)
        {
            int usuarioId = Convert.ToInt32(Session["idUsuario"]);
            string checkNumero = Session["checkNumero"]?.ToString();

            if (string.IsNullOrEmpty(checkNumero)) return;

            string checkId = checkNumero + tipoCheck;
            Session["checkId"] = checkId;
            Session["tipoCheck"] = tipoCheck;

            var rechazado = Control.Control.GetRevisionRechazada(usuarioId, checkId);
            if (rechazado != null)
            {
                hfRevisionId.Value = rechazado.id.ToString();
                lblEstatus.Text = "Corrigiendo";

                var preguntas = control.GetPreguntas(checkNumero, tipoCheck);
                var respuestasAnteriores = Control.Control.GetRespuestas(rechazado.id);

                rptPreguntas.DataSource = preguntas;
                rptPreguntas.DataBind();

                foreach (RepeaterItem item in rptPreguntas.Items)
                {
                    var fu1 = (System.Web.UI.WebControls.FileUpload)item.FindControl("fuFotoProblema");
                    var fu2 = (System.Web.UI.WebControls.FileUpload)item.FindControl("fuFotoCierre");
                    if (fu1 != null) fu1.Attributes["capture"] = "environment";
                    if (fu2 != null) fu2.Attributes["capture"] = "environment";
                }

                foreach (RepeaterItem item in rptPreguntas.Items)
                {
                    var hfPreguntaId = (HiddenField)item.FindControl("hfPreguntaId");
                    var rblRespuesta = (RadioButtonList)item.FindControl("rblRespuesta");
                    var chkSinComentario = (CheckBox)item.FindControl("chkSinComentario");
                    var txtComentario = (TextBox)item.FindControl("txtComentario");

                    int preguntaId = Convert.ToInt32(hfPreguntaId.Value);
                    var respAnterior = respuestasAnteriores.FirstOrDefault(r => r.Pregunta_id == preguntaId);

                    if (respAnterior != null)
                    {
                        rblRespuesta.SelectedValue = respAnterior.Respuesta_id.ToString();
                        if (respAnterior.Comentario == "Sin comentario")
                        {
                            chkSinComentario.Checked = true;
                            txtComentario.Text = "";
                        }
                        else
                        {
                            txtComentario.Text = respAnterior.Comentario ?? "";
                        }
                    }
                }

                lblRespondidas.Text = preguntas.Count + " / " + preguntas.Count;
                pnlFormulario.Visible = true;
                pnlYaCompletado.Visible = false;
                pnlCorrigiendo.Visible = true;
            }
            else if (control.YaCompletadoAprobadoHoy(usuarioId, checkId))
            {
                pnlFormulario.Visible = false;
                pnlYaCompletado.Visible = true;
                pnlCorrigiendo.Visible = false;
                return;
            }
            else
            {
                hfRevisionId.Value = "0";
                var preguntas = control.GetPreguntas(checkNumero, tipoCheck);
                rptPreguntas.DataSource = preguntas;
                rptPreguntas.DataBind();

                foreach (RepeaterItem item in rptPreguntas.Items)
                {
                    var fu1 = (System.Web.UI.WebControls.FileUpload)item.FindControl("fuFotoProblema");
                    var fu2 = (System.Web.UI.WebControls.FileUpload)item.FindControl("fuFotoCierre");
                    if (fu1 != null) fu1.Attributes["capture"] = "environment";
                    if (fu2 != null) fu2.Attributes["capture"] = "environment";
                }

                lblRespondidas.Text = "0 / " + preguntas.Count;
                pnlFormulario.Visible = true;
                pnlYaCompletado.Visible = false;
                pnlCorrigiendo.Visible = false;
            }

            if (tipoCheck == "A")
            {
                btnCheckA.CssClass = "btn-check-selector btn-check-active";
                btnCheckB.CssClass = "btn-check-selector";
            }
            else
            {
                btnCheckA.CssClass = "btn-check-selector";
                btnCheckB.CssClass = "btn-check-selector btn-check-active";
            }
        }






        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int usuarioId = Convert.ToInt32(Session["idUsuario"]);
            string checkId = Session["checkId"]?.ToString();

            if (string.IsNullOrEmpty(checkId)) return;

            var db = new Datos.DataClasses1DataContext();
            var seccion = db.ManagerSecciones
                .FirstOrDefault(s => s.Usuario_id == usuarioId && s.Activo == true);

            if (seccion == null)
            {
                lblEstatus.Text = "No tienes sección asignada";
                return;
            }

            int revisionId;
            bool esCorreccion = hfRevisionId.Value != "0";

            if (!esCorreccion)
            {
                var rechazadoEnBD = Control.Control.GetRevisionRechazada(usuarioId, checkId);
                if (rechazadoEnBD != null)
                {
                    esCorreccion = true;
                    revisionId = rechazadoEnBD.id;
                }
                else
                {
                    revisionId = control.CrearRevision(usuarioId, seccion.id, checkId);
                }
            }
            else
            {
                revisionId = int.Parse(hfRevisionId.Value);
            }

            string serverPath = Server.MapPath("~/");

            foreach (RepeaterItem item in rptPreguntas.Items)
            {
                var hfPreguntaId = (HiddenField)item.FindControl("hfPreguntaId");
                var rblRespuesta = (RadioButtonList)item.FindControl("rblRespuesta");
                var chkSinComentario = (CheckBox)item.FindControl("chkSinComentario");
                var txtComentario = (TextBox)item.FindControl("txtComentario");
                var fuFotoProblema = (System.Web.UI.WebControls.FileUpload)item.FindControl("fuFotoProblema");
                var fuFotoCierre = (System.Web.UI.WebControls.FileUpload)item.FindControl("fuFotoCierre");

                if (rblRespuesta.SelectedValue == "") continue;

                int preguntaId = Convert.ToInt32(hfPreguntaId.Value);
                int respuestaId = Convert.ToInt32(rblRespuesta.SelectedValue);
                string comentario = txtComentario.Text.Trim();
                bool sinComentario = chkSinComentario.Checked;

                control.GuardarRespuesta(revisionId, preguntaId, respuestaId, comentario, sinComentario);

                // Obtener el id de la respuesta recién guardada
                var respGuardada = Control.Control.GetRespuestas(revisionId)
                    .FirstOrDefault(r => r.Pregunta_id == preguntaId);

                if (respGuardada != null)
                {
                    // Guardar foto del problema si es Falla
                    if (respuestaId == 2 && fuFotoProblema.HasFile)
                    {
                        string rutaProblema = Control.Control.SubirFoto(
                            fuFotoProblema.PostedFile, revisionId, preguntaId, "problema", serverPath);
                        Control.Control.GuardarFotoProblema(respGuardada.id, rutaProblema);
                    }

                    // Guardar foto de cierre si se subió
                    if (fuFotoCierre.HasFile)
                    {
                        string rutaCierre = Control.Control.SubirFoto(
                            fuFotoCierre.PostedFile, revisionId, preguntaId, "cierre", serverPath);
                        Control.Control.CerrarHallazgo(respGuardada.id, rutaCierre);
                    }
                }
            }

            if (esCorreccion)
            {
                Control.Control.CambiarEstatusRevision(revisionId, 4, "");
                control.RecalcularCalificacion(revisionId);
            }
            else
            {
                control.CerrarRevision(revisionId);
            }

            var revision = Control.Control.GetRevision(revisionId);
            if (revision != null && revision.Calificacion.HasValue)
            {
                Control.Control.NotificarRevisor(
                    Session["nombreUsuario"].ToString(),
                    checkId,
                    revision.Calificacion.Value
                );
                Control.Control.NotificarRevisores(
                    $"{Session["nombreUsuario"]} {(esCorreccion ? "corrigió" : "envió")} el formulario {checkId}",
                    "/historial/Default.aspx"
                );
            }

            hfRevisionId.Value = "0";
            pnlFormulario.Visible = false;
            pnlYaCompletado.Visible = true;
            lblEstatus.Text = esCorreccion ? "Corregido" : "Completado";
        }
    }
}
