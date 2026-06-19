using System.Web;

namespace ManagerRounds.Account
{
    public class MarcarLeida : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string idParam = context.Request.QueryString["id"];
            if (!string.IsNullOrEmpty(idParam))
            {
                int id = int.Parse(idParam);
                Control.Control.MarcarLeida(id);
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write("ok");
        }

        public bool IsReusable { get { return false; } }
    }
}