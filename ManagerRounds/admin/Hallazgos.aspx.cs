using System;
using System.Linq;

namespace ManagerRounds.admin
{
    public partial class Hallazgos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rol"]?.ToString() != "Admin")
                Response.Redirect("~/Account/Login.aspx");

            if (!IsPostBack)
                CargarHallazgos();
        }

        private void CargarHallazgos()
        {
            var db = new Datos.DataClasses1DataContext();
            DateTime hoy = DateTime.Today;
            DateTime inicio, fin;

            if (ddlFiltro.SelectedValue == "mes")
            {
                inicio = new DateTime(hoy.Year, hoy.Month, 1);
                fin = inicio.AddMonths(1).AddDays(-1);
            }
            else
            {
                inicio = hoy.AddDays(-(int)hoy.DayOfWeek + 1);
                fin = inicio.AddDays(4);
            }

            var managers = db.Usuarios
                .Where(u => u.Activo == true && u.Roles.Rol == "Manager")
                .ToList();

            var resultado = managers.Select(m =>
            {
                var seccion = db.ManagerSecciones
                    .FirstOrDefault(s => s.Usuario_id == m.id && s.Activo == true);

                var respuestas = db.RespuestasRevision
                    .Where(r => r.Revisiones.Usuario_id == m.id
                        && r.TiposRespuesta.Respuesta == "Falla"
                        && r.Revisiones.Fecha >= inicio
                        && r.Revisiones.Fecha <= fin)
                    .ToList();

                int abiertos = respuestas.Count(r => r.HallazgoCerrado == false);
                int cerrados = respuestas.Count(r => r.HallazgoCerrado == true);

                return new
                {
                    Nombre = m.Nombre,
                    Seccion = seccion?.Seccion ?? "—",
                    Abiertos = abiertos,
                    Cerrados = cerrados,
                    Total = abiertos + cerrados
                };
            }).ToList();

            gvHallazgos.DataSource = resultado;
            gvHallazgos.DataBind();
        }

        protected void ddlFiltro_Changed(object sender, EventArgs e)
        {
            CargarHallazgos();
        }
    }
}