using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManagerRounds.admin
{
    public partial class Preguntas : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null || Session["rol"].ToString() != "Revisor")
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
            {
                CargarPreguntas();
                CargarChecks();
            }
        }

        private void CargarPreguntas()
        {
            string checkId = ddlFiltroCheck.SelectedValue;
            int? clasificacionId = string.IsNullOrEmpty(ddlFiltroClasificacion.SelectedValue) ? (int?)null : int.Parse(ddlFiltroClasificacion.SelectedValue);
            bool? activo = string.IsNullOrEmpty(ddlFiltroEstatus.SelectedValue) ? (bool?)null : ddlFiltroEstatus.SelectedValue == "1";

            var preguntas = Control.Control.GetPreguntas(
                string.IsNullOrEmpty(checkId) ? null : checkId,
                clasificacionId,
                activo
            );

            string buscar = txtBuscar.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(buscar))
                preguntas = preguntas.Where(p => p.Pregunta.ToLower().Contains(buscar)).ToList();

            gvPreguntas.DataSource = preguntas;
            gvPreguntas.DataBind();
        }

        private void CargarChecks()
        {
            var checks = new List<object>
            {
                new { CheckId = "1A", Descripcion = "5S + Seg. · Productiva",    Activo = Control.Control.CheckActivo("1A") },
                new { CheckId = "1B", Descripcion = "5S + Seg. · Almacenes",     Activo = Control.Control.CheckActivo("1B") },
                new { CheckId = "2A", Descripcion = "Puesta a punto · Prod.",    Activo = Control.Control.CheckActivo("2A") },
                new { CheckId = "2B", Descripcion = "Puesta a punto · Alm.",     Activo = Control.Control.CheckActivo("2B") },
                new { CheckId = "3A", Descripcion = "Prod. no conf. · Prod.",    Activo = Control.Control.CheckActivo("3A") },
                new { CheckId = "3B", Descripcion = "Prod. no conf. · Alm.",     Activo = Control.Control.CheckActivo("3B") },
                new { CheckId = "4A", Descripcion = "4M / P-Y · Productiva",     Activo = Control.Control.CheckActivo("4A") },
                new { CheckId = "4B", Descripcion = "Doctos. · Almacenes",       Activo = Control.Control.CheckActivo("4B") },
            };

            rptChecks.DataSource = checks;
            rptChecks.DataBind();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarPreguntas();
        }

        protected void gvPreguntas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());

            if (e.CommandName == "Toggle")
            {
                Control.Control.TogglePregunta(id);
                CargarPreguntas();
                CargarChecks();
            }
            else if (e.CommandName == "Editar")
            {
                var p = Control.Control.GetPreguntas().FirstOrDefault(x => x.id == id);
                if (p == null) return;

                hfIdPregunta.Value = p.id.ToString();
                lblTituloModal.Text = "Editar Pregunta";
                txtPregunta.Text = p.Pregunta;
                ddlCheck.SelectedValue = p.Check_id;
                ddlClasificacion.SelectedValue = p.Clasificacion_id.ToString();

                CargarPreguntas();
                CargarChecks();

                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalPregunta').modal('show'); };", true);
            }
        }

        protected void rptChecks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string checkId = e.CommandArgument.ToString();

            if (e.CommandName == "ActivarCheck")
                Control.Control.ToggleCheck(checkId, true);
            else if (e.CommandName == "DesactivarCheck")
                Control.Control.ToggleCheck(checkId, false);

            CargarPreguntas();
            CargarChecks();
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            string texto = txtPregunta.Text.Trim();
            string checkId = ddlCheck.SelectedValue;
            int clasificacionId = int.Parse(ddlClasificacion.SelectedValue);
            int id = int.Parse(hfIdPregunta.Value);

            if (string.IsNullOrEmpty(texto))
            {
                MostrarMensaje("El texto de la pregunta es obligatorio.", "alert-danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "window.onload = function(){ $('#modalPregunta').modal('show'); };", true);
                return;
            }

            // TipoArea: A = Productiva (1), B = Almacenes (2)
            int tipoAreaId = checkId.EndsWith("A") ? 1 : 2;

            if (id == 0)
            {
                Control.Control.CrearPregunta(checkId, clasificacionId, tipoAreaId, texto);
                MostrarMensaje("Pregunta creada correctamente.", "alert-success");
            }
            else
            {
                Control.Control.EditarPregunta(id, texto);
                MostrarMensaje("Pregunta actualizada correctamente.", "alert-success");
            }

            CargarPreguntas();
            CargarChecks();
        }

        private void MostrarMensaje(string texto, string clase)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = "alert " + clase + " d-block mb-3";
            lblMensaje.Visible = true;
        }
    }
}