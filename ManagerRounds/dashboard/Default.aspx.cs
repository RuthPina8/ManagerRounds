using System;
using System.Web.UI;

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
                lblManagersActivos.Text = control.TotalManagersActivos().ToString();
                lblCompletadosHoy.Text = control.CompletadosHoy().ToString();
                lblPendientesHoy.Text = control.PendientesHoy().ToString();
                lblCumplimiento.Text = control.CumplimientoSemana().ToString() + "%";

                DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
                gvManagers.DataSource = control.TablaManagersSemana(lunes);
                gvManagers.DataBind();
            }
        }
    }
}