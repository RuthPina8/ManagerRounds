using System.Web;
namespace ManagerRounds.Account
{
    public class MarcarTodasLeidas : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Session["idUsuario"] != null)
            {
                int usuarioId = (int)context.Session["idUsuario"];
                Control.Control.MarcarTodasLeidas(usuarioId);
            }
            context.Response.Redirect(context.Request.UrlReferrer?.ToString() ?? "/historial/Default.aspx");
        }
        public bool IsReusable { get { return false; } }
    }
}