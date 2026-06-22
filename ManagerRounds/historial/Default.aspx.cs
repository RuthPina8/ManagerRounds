using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;

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

                string filtro = Request.QueryString["filtro"];
                if (!string.IsNullOrEmpty(filtro))
                    AplicarFiltroDashboard(filtro, lunes);
                else
                    CargarRevisiones(lunes);

                if (Session["rol"]?.ToString() == "Admin")
                {
                    pnlReporte.Visible = true;
                    ddlMes.SelectedValue = DateTime.Today.Month.ToString();
                    ddlAnio.SelectedValue = DateTime.Today.Year.ToString();
                }
            }
        }

        private void AplicarFiltroDashboard(string filtro, DateTime lunes)
        {
            var db = new Datos.DataClasses1DataContext();
            DateTime viernes = lunes.AddDays(4);
            int usuarioId = Convert.ToInt32(Session["idUsuario"]);
            string rol = Session["rol"]?.ToString();

            switch (filtro)
            {
                case "completados":
                    ddlFiltroEstatus.SelectedValue = "2";
                    CargarRevisiones(lunes);
                    break;
                case "pendientes":
                    ddlFiltroEstatus.SelectedValue = "1";
                    CargarRevisiones(lunes);
                    break;
                case "hallazgos_abiertos":
                    var revHallazgosAbiertos = db.RespuestasRevision
                        .Where(r => r.TiposRespuesta.Respuesta == "Falla"
                            && r.HallazgoCerrado == false
                            && (rol == "Admin" || r.Revisiones.Usuario_id == usuarioId))
                        .Select(r => r.Revisiones)
                        .Distinct()
                        .OrderByDescending(r => r.FechaInicio)
                        .ToList();
                    gvRevisiones.DataSource = revHallazgosAbiertos;
                    gvRevisiones.DataBind();
                    break;
                case "hallazgos_cerrados":
                    var revHallazgosCerrados = db.RespuestasRevision
                        .Where(r => r.HallazgoCerrado == true
                            && r.FechaCierre >= lunes
                            && r.FechaCierre <= viernes
                            && (rol == "Admin" || r.Revisiones.Usuario_id == usuarioId))
                        .Select(r => r.Revisiones)
                        .Distinct()
                        .OrderByDescending(r => r.FechaInicio)
                        .ToList();
                    gvRevisiones.DataSource = revHallazgosCerrados;
                    gvRevisiones.DataBind();
                    break;
                default:
                    CargarRevisiones(lunes);
                    break;
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
            int semana = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                lunes, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            lblSemana.Text = "Semana " + semana;
            lblRangoSemana.Text = lunes.ToString("dd MMM") + " – " + viernes.ToString("dd MMM yyyy");
        }

        private void CargarRevisiones(DateTime lunes)
        {
            int usuarioId = Convert.ToInt32(Session["idUsuario"]);
            string rol = Session["rol"]?.ToString();

            string checkId = ddlFiltroCheck.SelectedValue;
            int? estatusId = string.IsNullOrEmpty(ddlFiltroEstatus.SelectedValue) ? (int?)null : int.Parse(ddlFiltroEstatus.SelectedValue);

            var revisiones = Control.Control.GetRevisiones(lunes, null,
                string.IsNullOrEmpty(checkId) ? null : checkId,
                estatusId);

            if (rol == "Manager")
                revisiones = revisiones.Where(r => r.Usuario_id == usuarioId).ToList();

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
            DateTime inicioMes = new DateTime(lunes.Year, lunes.Month, 1);
            DateTime finMes = inicioMes.AddMonths(1).AddDays(-1);
            var revisiones = Control.Control.GetRevisiones(inicioMes, finMes);
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

        protected void btnDescargarExcel_Click(object sender, EventArgs e)
        {
            int mes = int.Parse(ddlMes.SelectedValue);
            int anio = int.Parse(ddlAnio.SelectedValue);
            DateTime inicio = new DateTime(anio, mes, 1);
            DateTime fin = inicio.AddMonths(1).AddDays(-1);

            var db = new Datos.DataClasses1DataContext();
            var managers = db.Usuarios
                .Where(u => u.Activo == true && u.Roles.Rol == "Manager")
                .ToList();

            using (var pck = new ExcelPackage())
            {
                var ws = pck.Workbook.Worksheets.Add("Reporte");

                ws.Cells[1, 1].Value = "Reporte Manager Rounds — Astemo MXQRP1";
                ws.Cells[1, 1].Style.Font.Bold = true;
                ws.Cells[1, 1].Style.Font.Size = 14;
                ws.Cells[2, 1].Value = inicio.ToString("MMMM yyyy");
                ws.Cells[3, 1].Value = $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}";

                string[] headers = { "Manager", "Sección", "Formularios completados", "Entregados tarde", "Calificación promedio", "Hallazgos abiertos", "Hallazgos cerrados" };
                for (int i = 0; i < headers.Length; i++)
                {
                    ws.Cells[5, i + 1].Value = headers[i];
                    ws.Cells[5, i + 1].Style.Font.Bold = true;
                    ws.Cells[5, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[5, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(204, 0, 0));
                    ws.Cells[5, i + 1].Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                int row = 6;
                foreach (var m in managers)
                {
                    var seccion = db.ManagerSecciones.FirstOrDefault(s => s.Usuario_id == m.id && s.Activo == true);
                    var revisiones = db.Revisiones
                        .Where(r => r.Usuario_id == m.id && r.Fecha >= inicio && r.Fecha <= fin)
                        .ToList();

                    int completados = revisiones.Count(r => r.Estatus_id != 3);
                    int tarde = revisiones.Count(r => r.EntregadoTarde == true);
                    decimal? promedio = revisiones.Any(r => r.Calificacion != null)
                        ? Math.Round(revisiones.Where(r => r.Calificacion != null).Average(r => r.Calificacion.Value), 1)
                        : (decimal?)null;

                    var respuestas = db.RespuestasRevision
                        .Where(r => r.Revisiones.Usuario_id == m.id
                            && r.TiposRespuesta.Respuesta == "Falla"
                            && r.Revisiones.Fecha >= inicio
                            && r.Revisiones.Fecha <= fin)
                        .ToList();

                    ws.Cells[row, 1].Value = m.Nombre;
                    ws.Cells[row, 2].Value = seccion?.Seccion ?? "—";
                    ws.Cells[row, 3].Value = completados;
                    ws.Cells[row, 4].Value = tarde;
                    ws.Cells[row, 5].Value = promedio.HasValue ? promedio + "%" : "—";
                    ws.Cells[row, 6].Value = respuestas.Count(r => r.HallazgoCerrado == false);
                    ws.Cells[row, 7].Value = respuestas.Count(r => r.HallazgoCerrado == true);
                    row++;
                }

                ws.Cells[ws.Dimension.Address].AutoFitColumns();

                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", $"attachment;filename=ManagerRounds_{inicio:MMMM_yyyy}.xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Session[$"reporteDescargado_{inicio.Year}_{inicio.Month}"] = true;
                Response.End();
            }
        }

        protected void btnDescargarPdf_Click(object sender, EventArgs e)
        {
            int mes = int.Parse(ddlMes.SelectedValue);
            int anio = int.Parse(ddlAnio.SelectedValue);
            DateTime inicio = new DateTime(anio, mes, 1);
            DateTime fin = inicio.AddMonths(1).AddDays(-1);

            var db = new Datos.DataClasses1DataContext();
            var managers = db.Usuarios
                .Where(u => u.Activo == true && u.Roles.Rol == "Manager")
                .ToList();

            var sb = new System.Text.StringBuilder();
            sb.Append("<html><head><style>body{font-family:Arial;font-size:11px;}h1{color:#CC0000;font-size:16px;}h2{font-size:12px;color:#666;}table{width:100%;border-collapse:collapse;margin-top:16px;}th{background:#CC0000;color:white;padding:6px;text-align:left;}td{padding:5px 6px;border-bottom:1px solid #eee;}tr:nth-child(even){background:#f9f9f9;}</style></head><body>");
            sb.Append($"<h1>Reporte Manager Rounds — Astemo MXQRP1</h1>");
            sb.Append($"<h2>{inicio:MMMM yyyy} &nbsp;·&nbsp; Generado: {DateTime.Now:dd/MM/yyyy HH:mm}</h2>");
            sb.Append("<table><tr><th>Manager</th><th>Sección</th><th>Completados</th><th>Tarde</th><th>Cal. promedio</th><th>Hallazgos abiertos</th><th>Hallazgos cerrados</th></tr>");

            foreach (var m in managers)
            {
                var seccion = db.ManagerSecciones.FirstOrDefault(s => s.Usuario_id == m.id && s.Activo == true);
                var revisiones = db.Revisiones
                    .Where(r => r.Usuario_id == m.id && r.Fecha >= inicio && r.Fecha <= fin)
                    .ToList();

                int completados = revisiones.Count(r => r.Estatus_id != 3);
                int tarde = revisiones.Count(r => r.EntregadoTarde == true);
                decimal? promedio = revisiones.Any(r => r.Calificacion != null)
                    ? Math.Round(revisiones.Where(r => r.Calificacion != null).Average(r => r.Calificacion.Value), 1)
                    : (decimal?)null;

                var respuestas = db.RespuestasRevision
                    .Where(r => r.Revisiones.Usuario_id == m.id
                        && r.TiposRespuesta.Respuesta == "Falla"
                        && r.Revisiones.Fecha >= inicio
                        && r.Revisiones.Fecha <= fin)
                    .ToList();

                sb.Append($"<tr><td>{m.Nombre}</td><td>{seccion?.Seccion ?? "—"}</td><td>{completados}</td><td>{tarde}</td><td>{(promedio.HasValue ? promedio + "%" : "—")}</td><td>{respuestas.Count(r => r.HallazgoCerrado == false)}</td><td>{respuestas.Count(r => r.HallazgoCerrado == true)}</td></tr>");
            }

            sb.Append("</table></body></html>");

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", $"attachment;filename=ManagerRounds_{inicio:MMMM_yyyy}.pdf");

            using (var ms = new System.IO.MemoryStream())
            {
                var doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();
                var html = HTMLWorker.ParseToList(new System.IO.StringReader(sb.ToString()), null);
                foreach (var element in html)
                    doc.Add(element);
                doc.Close();
                Response.BinaryWrite(ms.ToArray());
            }
            Response.End();
        }
    }
}