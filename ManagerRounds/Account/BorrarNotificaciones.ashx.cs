using System.Web;

namespace ManagerRounds.Account
{
    public class BorrarNotificaciones : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["idUsuario"] != null)
            {
                int usuarioId = (int)context.Session["idUsuario"];
                Control.Control.BorrarNotificaciones(usuarioId);
            }
            context.Response.Redirect(context.Request.UrlReferrer?.ToString() ?? "/historial/Default.aspx");
        }

        public bool IsReusable { get { return false; } }
    }
}