using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ManagerRounds.historial
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"] == null)
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
            {
                DateTime lunes = GetLunesActual();
                hfLunes.Value = lunes.ToString("yyyy-MM-dd");
                ActualizarNavegador(lunes);
                CargarRevisiones(lunes);
            }
        }

        private DateTime GetLunesActual()
        {
            DateTime hoy = DateTime.Today;
            int diff = (int)hoy.DayOfWeek - 1;
            if (diff < 0) diff = 6;
            return hoy.AddDays(-diff);
        }

        private void ActualizarNavegador(DateTime lunes)
        {
            DateTime viernes = lunes.AddDays(4);
            int semana = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear( lunes, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            lblSemana.Text = "Semana " + semana;
            lblRangoSemana.Text = lunes.ToString("dd MMM") + " – " + viernes.ToString("dd MMM yyyy");
        }

        private void CargarRevisiones(DateTime lunes)
        {
            string checkId = ddlFiltroCheck.SelectedValue;
            int? estatusId = string.IsNullOrEmpty(ddlFiltroEstatus.SelectedValue) ? (int?)null : int.Parse(ddlFiltroEstatus.SelectedValue);

            var revisiones = Control.Control.GetRevisiones(
                lunes,
                string.IsNullOrEmpty(checkId) ? null : checkId,
                estatusId
            );

            string buscar = txtBuscar.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(buscar))
                revisiones = revisiones.Where(r =>
                    r.Usuarios.Nombre.ToLower().Contains(buscar) ||
                    (r.ManagerSecciones != null && r.ManagerSecciones.Seccion.ToLower().Contains(buscar))
                ).ToList();

            gvRevisiones.DataSource = revisiones;
            gvRevisiones.DataBind();
        }

        private void CargarBitacora(DateTime lunes)
        {
            var revisiones = Control.Control.GetRevisiones(lunes);

            var entradas = new List<object>();

            foreach (var r in revisiones)
            {
                string color = "#639922";
                string desc = $"<strong>{r.Usuarios.Nombre}</strong> guardó el formulario {r.Check_id}";

                if (r.EstatusRevision.Estatus == "Rechazado")
                {
                    color = "#E24B4A";
                    desc = $"<strong>Revisor</strong> rechazó el formulario {r.Check_id} de {r.Usuarios.Nombre}";
                    if (!string.IsNullOrEmpty(r.ComentarioRevisor))
                        desc += $" · \"{r.ComentarioRevisor}\"";
                }
                else if (r.EstatusRevision.Estatus == "Corregido")
                {
                    color = "#BA7517";
                    desc = $"<strong>{r.Usuarios.Nombre}</strong> corrigió el formulario {r.Check_id}";
                }
                else if (r.EstatusRevision.Estatus == "Revisado")
                {
                    color = "#378ADD";
                    desc = $"<strong>Revisor</strong> revisó el formulario {r.Check_id} de {r.Usuarios.Nombre}";
                }

                entradas.Add(new
                {
                    Color = color,
                    Descripcion = desc,
                    Fecha = r.FechaInicio?.ToString("ddd dd MMM yyyy · hh:mm:ss tt")
                });
            }

            rptBitacora.DataSource = entradas;
            rptBitacora.DataBind();
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            DateTime lunes = DateTime.Parse(hfLunes.Value).AddDays(-7);
            hfLunes.Value = lunes.ToString("yyyy-MM-dd");
            ActualizarNavegador(lunes);
            if (hfTab.Value == "bitacora")
                CargarBitacora(lunes);
            else
                CargarRevisiones(lunes);
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            DateTime lunes = DateTime.Parse(hfLunes.Value).AddDays(7);
            hfLunes.Value = lunes.ToString("yyyy-MM-dd");
            ActualizarNavegador(lunes);
            if (hfTab.Value == "bitacora")
                CargarBitacora(lunes);
            else
                CargarRevisiones(lunes);
        }

        protected void tabRevisiones_Click(object sender, EventArgs e)
        {
            hfTab.Value = "revisiones";
            tabRevisiones.CssClass = "nav-link active";
            tabBitacora.CssClass = "nav-link";
            panelRevisiones.Visible = true;
            panelBitacora.Visible = false;
            CargarRevisiones(DateTime.Parse(hfLunes.Value));
        }

        protected void tabBitacora_Click(object sender, EventArgs e)
        {
            hfTab.Value = "bitacora";
            tabRevisiones.CssClass = "nav-link";
            tabBitacora.CssClass = "nav-link active";
            panelRevisiones.Visible = false;
            panelBitacora.Visible = true;
            CargarBitacora(DateTime.Parse(hfLunes.Value));
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CargarRevisiones(DateTime.Parse(hfLunes.Value));
        }

        protected void gvRevisiones_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int id = int.Parse(e.CommandArgument.ToString());
                Response.Redirect("~/historial/Detalle.aspx?id=" + id);
            }
        }

        public string GetBadgeCalificacion(decimal? cal)
        {
            if (cal == null) return "badge badge-secondary";
            if (cal >= 90) return "badge badge-cumple";
            if (cal >= 70) return "badge badge-rechazado";
            return "badge badge-nocumple";
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
    }
}