using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

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

                string checkNumero = control.GetCheckDelDia();
                if (checkNumero == null)
                {
                    lblEstatus.Text = "No hay formulario hoy";
                    return;
                }

                Session["checkNumero"] = checkNumero;
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
            string checkNumero = Session["checkNumero"].ToString();
            string checkId = checkNumero + tipoCheck;

            Session["checkId"] = checkId;
            Session["tipoCheck"] = tipoCheck;

            if (control.YaCompletadoHoy(usuarioId, checkId))
            {
                pnlFormulario.Visible = false;
                pnlYaCompletado.Visible = true;
                return;
            }

            var preguntas = control.GetPreguntas(checkNumero, tipoCheck);
            rptPreguntas.DataSource = preguntas;
            rptPreguntas.DataBind();

            lblRespondidas.Text = "0 / " + preguntas.Count;
            pnlFormulario.Visible = true;
            pnlYaCompletado.Visible = false;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int usuarioId = Convert.ToInt32(Session["idUsuario"]);
            string checkId = Session["checkId"].ToString();

            // Obtener la sección del manager
            var db = new Datos.DataClasses1DataContext();
            var seccion = db.ManagerSecciones
                .FirstOrDefault(s => s.Usuario_id == usuarioId && s.Activo == true);

            if (seccion == null)
            {
                lblEstatus.Text = "No tienes sección asignada";
                return;
            }

            // Crear la revisión
            int revisionId = control.CrearRevision(usuarioId, seccion.id, checkId);

            // Guardar cada respuesta
            foreach (RepeaterItem item in rptPreguntas.Items)
            {
                var hfPreguntaId = (HiddenField)item.FindControl("hfPreguntaId");
                var rblRespuesta = (RadioButtonList)item.FindControl("rblRespuesta");
                var chkSinComentario = (CheckBox)item.FindControl("chkSinComentario");
                var txtComentario = (TextBox)item.FindControl("txtComentario");

                if (rblRespuesta.SelectedValue == "") continue;

                int preguntaId = Convert.ToInt32(hfPreguntaId.Value);
                int respuestaId = Convert.ToInt32(rblRespuesta.SelectedValue);
                string comentario = txtComentario.Text.Trim();
                bool sinComentario = chkSinComentario.Checked;

                control.GuardarRespuesta(revisionId, preguntaId, respuestaId, comentario, sinComentario);
            }

            // Cerrar la revisión y calcular calificación
            control.CerrarRevision(revisionId);

            pnlFormulario.Visible = false;
            pnlYaCompletado.Visible = true;
            lblEstatus.Text = "Completado";
        }
    }
}