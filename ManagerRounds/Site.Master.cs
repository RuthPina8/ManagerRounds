using System;
using System.Collections.Generic;

namespace ManagerRounds
{
    public partial class Site : System.Web.UI.MasterPage
    {
        public int NotificacionesNoLeidas { get; set; }
        public List<Datos.Notificaciones> Notificaciones { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] != null)
            {
                int userId = (int)Session["idUsuario"];
                NotificacionesNoLeidas = Control.Control.ContarNoLeidas(userId);
                Notificaciones = Control.Control.GetNotificaciones(userId);

                if (Session["rol"]?.ToString() == "Manager")
                    Control.Control.EnviarRecordatorioHallazgos(userId);
            }
            else
            {
                Notificaciones = new List<Datos.Notificaciones>();
            }

            rptNotificaciones.DataSource = Notificaciones;
            rptNotificaciones.DataBind();
        }
    }
}