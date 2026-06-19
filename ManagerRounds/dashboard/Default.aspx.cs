using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ManagerRounds.dashboard
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
                DateTime lunes = GetLunesActual();
                hfLunes.Value = lunes.ToString("yyyy-MM-dd");
                CargarDashboard(lunes);
            }
        }

        private DateTime GetLunesActual()
        {
            DateTime hoy = DateTime.Today;
            int diff = (int)hoy.DayOfWeek - 1;
            if (diff < 0) diff = 6;
            return hoy.AddDays(-diff);
        }

        private void CargarDashboard(DateTime lunes)
        {
            DateTime viernes = lunes.AddDays(4);
            int semana = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                lunes, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            lblSemana.Text = "Semana " + semana;
            lblRango.Text = lunes.ToString("dd MMM") + " – " + viernes.ToString("dd MMM yyyy");

            lblManagersActivos.Text = control.TotalManagersActivos().ToString();
            lblCompletadosHoy.Text = control.CompletadosHoy().ToString();
            lblPendientesHoy.Text = control.PendientesHoy().ToString();
            lblCumplimiento.Text = control.CumplimientoSemana().ToString() + "%";

            // Hallazgos
            int usuarioId = Convert.ToInt32(Session["idUsuario"]);
            string rol = Session["rol"]?.ToString();

            if (rol == "Admin")
            {
                lblHallazgosAbiertos.Text = Control.Control.HallazgosAbiertosTotal().ToString();
                lblHallazgosCerrados.Text = Control.Control.HallazgosCerradosSemanaTotal(lunes).ToString();
            }
            else
            {
                lblHallazgosAbiertos.Text = Control.Control.HallazgosAbiertos(usuarioId).ToString();
                lblHallazgosCerrados.Text = Control.Control.HallazgosCerradosSemana(usuarioId).ToString();
            }

            gvManagers.DataSource = control.TablaManagersSemana(lunes);
            gvManagers.DataBind();

            CargarGrafica(lunes);
        }

        private void CargarGrafica(DateTime lunes)
        {
            var db = new Datos.DataClasses1DataContext();
            var dias = new[] {
                new { Dia = "Lun", Fecha = lunes },
                new { Dia = "Mar", Fecha = lunes.AddDays(1) },
                new { Dia = "Mié", Fecha = lunes.AddDays(2) },
                new { Dia = "Jue", Fecha = lunes.AddDays(3) },
                new { Dia = "Vie", Fecha = lunes.AddDays(4) }
            };

            var datos = new List<object>();
            foreach (var d in dias)
            {
                var revs = db.Revisiones
                    .Where(r => r.Fecha == d.Fecha && r.Calificacion != null)
                    .ToList();

                decimal? promedio = revs.Any() ? Math.Round(revs.Average(r => r.Calificacion.Value), 1) : (decimal?)null;

                string etiqueta = promedio.HasValue ? promedio + "%" : "—";
                string color = promedio == null ? "#ccc" : promedio >= 90 ? "#639922" : promedio >= 70 ? "#F0A500" : "#E24B4A";
                int altura = promedio.HasValue ? (int)(promedio.Value * 1.2m) : 4;

                datos.Add(new { Dia = d.Dia, Etiqueta = etiqueta, Color = color, Altura = altura });
            }

            rptGrafica.DataSource = datos;
            rptGrafica.DataBind();
        }

        protected void btnAnterior_Click(object sender, EventArgs e)
        {
            DateTime lunes = DateTime.Parse(hfLunes.Value).AddDays(-7);
            hfLunes.Value = lunes.ToString("yyyy-MM-dd");
            CargarDashboard(lunes);
        }

        protected void btnSiguiente_Click(object sender, EventArgs e)
        {
            DateTime lunes = DateTime.Parse(hfLunes.Value).AddDays(7);
            hfLunes.Value = lunes.ToString("yyyy-MM-dd");
            CargarDashboard(lunes);
        }

        public string FormatCalificacion(object valor)
        {
            if (valor == null || valor == DBNull.Value)
                return "<span style='color:#ccc;'>—</span>";

            decimal cal = Convert.ToDecimal(valor);
            string color = cal >= 90 ? "#639922" : cal >= 70 ? "#F0A500" : "#E24B4A";
            string bg = cal >= 90 ? "#EAF3DE" : cal >= 70 ? "#FFF3CD" : "#FCEBEB";
            return $"<span style='background:{bg}; color:{color}; padding:3px 8px; border-radius:20px; font-size:12px; font-weight:500;'>{cal:0}%</span>";
        }

        public string FormatTotalSemana(object lunes, object martes, object miercoles, object jueves, object viernes)
        {
            var valores = new[] { lunes, martes, miercoles, jueves, viernes }
                .Where(v => v != null && v != DBNull.Value)
                .Select(v => Convert.ToDecimal(v))
                .ToList();

            if (!valores.Any())
                return "<span style='color:#ccc;'>—</span>";

            decimal total = Math.Round(valores.Sum() / 4, 1);
            string color = total >= 90 ? "#639922" : total >= 70 ? "#F0A500" : "#E24B4A";
            string bg = total >= 90 ? "#EAF3DE" : total >= 70 ? "#FFF3CD" : "#FCEBEB";
            return $"<span style='background:{bg}; color:{color}; padding:3px 8px; border-radius:20px; font-size:12px; font-weight:600;'>{total:0}%</span>";
        }
    }
}