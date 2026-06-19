using System;
using System.Linq;
using System.Web.UI;

namespace ManagerRounds.Account
{
    public partial class Login : Page
    {
        Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["idUsuario"] != null)
                Response.Redirect("/dashboard/Default.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string nomina = txtNomina.Text.Trim();
            string password = txtPassword.Text.Trim();

            var usuario = db.Usuarios
                .Where(u => u.Nomina == nomina && u.Password == password && u.Activo == true)
                .FirstOrDefault();

            if (usuario != null)
            {
                Session.Clear();
                Session["idUsuario"] = usuario.id;
                Session["nombreUsuario"] = usuario.Nombre;
                Session["rol"] = usuario.Roles.Rol;

                // Si no tiene pregunta configurada, redirigir a configurar
                if (string.IsNullOrEmpty(usuario.PreguntaRecuperacion))
                    Response.Redirect("/Account/Configurar.aspx");
                else
                    Response.Redirect("/dashboard/Default.aspx");
            }
            else
            {
                errorMsg.Style["display"] = "block";
            }
        }
    }
}