using System;
using System.Collections.Generic;
using System.Linq;
using Datos;
using System.Web;

namespace Control
{
    public class Control
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        public int TotalManagersActivos()
        {
            return db.Usuarios
                .Where(u => u.Activo == true && u.Roles.Rol == "Manager")
                .Count();
        }

        public int CompletadosHoy()
        {
            DateTime hoy = DateTime.Today;
            return db.Revisiones
                .Where(r => r.Fecha == hoy)
                .Select(r => r.Usuario_id)
                .Distinct()
                .Count();
        }

        public int PendientesHoy()
        {
            return TotalManagersActivos() - CompletadosHoy();
        }

        public decimal CumplimientoSemana()
        {
            DateTime hoy = DateTime.Today;
            DateTime lunes = hoy.AddDays(-(int)hoy.DayOfWeek + 1);
            DateTime viernes = lunes.AddDays(4);

            var revisiones = db.Revisiones
                .Where(r => r.Fecha >= lunes && r.Fecha <= viernes && r.Calificacion != null)
                .ToList();

            if (!revisiones.Any()) return 0;

            return Math.Round(revisiones.Average(r => r.Calificacion.Value), 1);
        }

        public List<object> TablaManagersSemana(DateTime lunes)
        {
            DateTime viernes = lunes.AddDays(4);

            var managers = db.Usuarios
                .Where(u => u.Activo == true && u.Roles.Rol == "Manager")
                .ToList();

            var revisiones = db.Revisiones
                .Where(r => r.Fecha >= lunes && r.Fecha <= viernes)
                .ToList();

            var resultado = new List<object>();

            foreach (var m in managers)
            {
                var seccion = db.ManagerSecciones
                    .Where(s => s.Usuario_id == m.id && s.Activo == true)
                    .FirstOrDefault();

                var revManager = revisiones.Where(r => r.Usuario_id == m.id).ToList();

                resultado.Add(new
                {
                    Nombre = m.Nombre,
                    Seccion = seccion != null ? seccion.Seccion : "—",
                    Lunes = revManager.FirstOrDefault(r => r.Fecha == lunes)?.Calificacion,
                    Martes = revManager.FirstOrDefault(r => r.Fecha == lunes.AddDays(1))?.Calificacion,
                    Miercoles = revManager.FirstOrDefault(r => r.Fecha == lunes.AddDays(2))?.Calificacion,
                    Jueves = revManager.FirstOrDefault(r => r.Fecha == lunes.AddDays(3))?.Calificacion,
                    Viernes = revManager.FirstOrDefault(r => r.Fecha == lunes.AddDays(4))?.Calificacion
                });
            }

            return resultado;
        }

        public string GetCheckDelDia()
        {
            switch (DateTime.Today.DayOfWeek)
            {
                case DayOfWeek.Monday: return "1";
                case DayOfWeek.Tuesday: return "2";
                case DayOfWeek.Wednesday: return "3";
                case DayOfWeek.Thursday: return "4";
                case DayOfWeek.Friday: return "R";
                default: return null;
            }
        }

        public List<Preguntas> GetPreguntas(string checkNumero, string tipoCheck)
        {
            string checkId = checkNumero + tipoCheck;
            return db.Preguntas
                .Where(p => p.Check_id == checkId && p.Activo == true)
                .ToList();
        }

        public bool YaCompletadoHoy(int usuarioId, string checkId)
        {
            DateTime hoy = DateTime.Today;
            return db.Revisiones
                .Any(r => r.Usuario_id == usuarioId && r.Fecha == hoy && r.Check_id == checkId);
        }

        public int CrearRevision(int usuarioId, int seccionId, string checkId)
        {
            // Calcular fecha correcta según el número de check
            string checkNumero = checkId.Substring(0, 1);
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime fechaCheck;

            switch (checkNumero)
            {
                case "1": fechaCheck = lunes; break;
                case "2": fechaCheck = lunes.AddDays(1); break;
                case "3": fechaCheck = lunes.AddDays(2); break;
                case "4": fechaCheck = lunes.AddDays(3); break;
                default: fechaCheck = DateTime.Today; break;
            }

            bool entregadoTarde = DateTime.Today.DayOfWeek == DayOfWeek.Friday && fechaCheck != DateTime.Today;

            var revision = new Revisiones
            {
                Usuario_id = usuarioId,
                ManagerSeccion_id = seccionId,
                Fecha = fechaCheck,
                Check_id = checkId,
                FechaInicio = DateTime.Now,
                Estatus_id = 1,
                EntregadoTarde = entregadoTarde
            };
            db.Revisiones.InsertOnSubmit(revision);
            db.SubmitChanges();
            return revision.id;
        }

        public void GuardarRespuesta(int revisionId, int preguntaId, int respuestaId, string comentario, bool sinComentario)
        {
            var existente = db.RespuestasRevision
                .FirstOrDefault(r => r.Revision_id == revisionId && r.Pregunta_id == preguntaId);

            if (existente != null)
            {
                existente.Respuesta_id = respuestaId;
                existente.Comentario = sinComentario ? "Sin comentario" : comentario;
            }
            else
            {
                var respuesta = new RespuestasRevision
                {
                    Revision_id = revisionId,
                    Pregunta_id = preguntaId,
                    Respuesta_id = respuestaId,
                    Comentario = sinComentario ? "Sin comentario" : comentario
                };
                db.RespuestasRevision.InsertOnSubmit(respuesta);
            }
            db.SubmitChanges();
        }

        public void CerrarRevision(int revisionId)
        {
            var revision = db.Revisiones.FirstOrDefault(r => r.id == revisionId);
            if (revision == null) return;

            var respuestas = db.RespuestasRevision
                .Where(r => r.Revision_id == revisionId)
                .ToList();

            int total = respuestas.Count;
            int cumplen = respuestas.Count(r => r.TiposRespuesta.Respuesta == "Aprobado");
            int na = respuestas.Count(r => r.TiposRespuesta.Respuesta == "N/A");

            int evaluables = total - na;
            decimal calificacion = evaluables > 0 ? Math.Round((decimal)cumplen / evaluables * 100, 1) : 0;

            revision.Calificacion = calificacion;
            revision.FechaFin = DateTime.Now;
            revision.Estatus_id = 1;
            db.SubmitChanges();
        }
        public void RecalcularCalificacion(int revisionId)
        {
            var revision = db.Revisiones.FirstOrDefault(r => r.id == revisionId);
            if (revision == null) return;

            var respuestas = db.RespuestasRevision
                .Where(r => r.Revision_id == revisionId)
                .ToList();

            int total = respuestas.Count;
            int cumplen = respuestas.Count(r => r.TiposRespuesta.Respuesta == "Aprobado");
            int na = respuestas.Count(r => r.TiposRespuesta.Respuesta == "N/A");

            int evaluables = total - na;
            decimal calificacion = evaluables > 0 ? Math.Round((decimal)cumplen / evaluables * 100, 1) : 0;

            revision.Calificacion = calificacion;
            revision.FechaFin = DateTime.Now;
            db.SubmitChanges();
        }

        public static List<Datos.Usuarios> GetUsuarios()
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Usuarios.OrderBy(u => u.Nomina).ToList();
        }

        public static Datos.Usuarios GetUsuario(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Usuarios.FirstOrDefault(u => u.id == id);
        }

        public static void CrearUsuario(string nombre, string nomina, string password, int rolId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = new Datos.Usuarios
            {
                Nombre = nombre,
                Nomina = nomina,
                Password = password,
                Rol_id = rolId,
                Activo = true
            };
            db.Usuarios.InsertOnSubmit(u);
            db.SubmitChanges();
        }

        public static void EditarUsuario(int id, string nombre, string nomina, string password, int rolId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.id == id);
            if (u == null) return;
            u.Nombre = nombre;
            u.Nomina = nomina;
            if (!string.IsNullOrEmpty(password))
                u.Password = password;
            u.Rol_id = rolId;
            db.SubmitChanges();
        }

        public static void ToggleUsuario(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.id == id);
            if (u == null) return;
            u.Activo = !u.Activo;
            db.SubmitChanges();
        }

        public static bool NominaExiste(string nomina, int? excluirId = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Usuarios.Any(u => u.Nomina == nomina && (excluirId == null || u.id != excluirId));
        }

        public static List<Datos.Preguntas> GetPreguntas(string checkId = null, int? clasificacionId = null, bool? activo = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var query = db.Preguntas.AsQueryable();
            if (!string.IsNullOrEmpty(checkId))
                query = query.Where(p => p.Check_id == checkId);
            if (clasificacionId.HasValue)
                query = query.Where(p => p.Clasificacion_id == clasificacionId.Value);
            if (activo.HasValue)
                query = query.Where(p => p.Activo == activo.Value);
            return query.OrderBy(p => p.Check_id).ThenBy(p => p.Clasificacion_id).ToList();
        }

        public static void TogglePregunta(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var p = db.Preguntas.FirstOrDefault(x => x.id == id);
            if (p == null) return;
            p.Activo = !p.Activo;
            db.SubmitChanges();
        }

        public static void ToggleCheck(string checkId, bool activo)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var preguntas = db.Preguntas.Where(p => p.Check_id == checkId).ToList();
            foreach (var p in preguntas)
                p.Activo = activo;
            db.SubmitChanges();
        }

        public static void CrearPregunta(string checkId, int clasificacionId, int tipoAreaId, string texto)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var p = new Datos.Preguntas
            {
                Check_id = checkId,
                Clasificacion_id = clasificacionId,
                TipoArea_id = tipoAreaId,
                Pregunta = texto,
                Activo = true
            };
            db.Preguntas.InsertOnSubmit(p);
            db.SubmitChanges();
        }

        public static void EditarPregunta(int id, string texto)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var p = db.Preguntas.FirstOrDefault(x => x.id == id);
            if (p == null) return;
            p.Pregunta = texto;
            db.SubmitChanges();
        }

        public static bool CheckActivo(string checkId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Preguntas.Any(p => p.Check_id == checkId && p.Activo == true);
        }

        public static List<Datos.Revisiones> GetRevisiones(DateTime inicio, DateTime? fin = null, string checkId = null, int? estatusId = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            DateTime fechaFin = fin ?? inicio.AddDays(4);
            var query = db.Revisiones.Where(r => r.Fecha >= inicio && r.Fecha <= fechaFin);
            if (!string.IsNullOrEmpty(checkId))
                query = query.Where(r => r.Check_id == checkId);
            if (estatusId.HasValue)
                query = query.Where(r => r.Estatus_id == estatusId.Value);
            return query.OrderByDescending(r => r.FechaInicio).ToList();
        }

        public static Datos.Revisiones GetRevision(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Revisiones.FirstOrDefault(r => r.id == id);
        }

        public static List<Datos.RespuestasRevision> GetRespuestas(int revisionId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.RespuestasRevision
                .Where(r => r.Revision_id == revisionId)
                .OrderBy(r => r.Pregunta_id)
                .ToList();
        }

        public static void CambiarEstatusRevision(int revisionId, int estatusId, string comentario)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var r = db.Revisiones.FirstOrDefault(x => x.id == revisionId);
            if (r == null) return;
            r.Estatus_id = estatusId;
            r.ComentarioRevisor = comentario;
            db.SubmitChanges();
        }

        public static void CrearSeccionManager(int usuarioId, string seccion, int tipoAreaId, int tipoCheckId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var s = new Datos.ManagerSecciones
            {
                Usuario_id = usuarioId,
                Seccion = seccion,
                TipoArea_id = tipoAreaId,
                TipoCheck_id = tipoCheckId,
                Activo = true
            };
            db.ManagerSecciones.InsertOnSubmit(s);
            db.SubmitChanges();
        }

        public static Datos.ManagerSecciones GetSeccionManager(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.ManagerSecciones.FirstOrDefault(s => s.Usuario_id == usuarioId && s.Activo == true);
        }

        public static void EditarSeccionManager(int usuarioId, string seccion, int tipoAreaId, int tipoCheckId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var s = db.ManagerSecciones.FirstOrDefault(x => x.Usuario_id == usuarioId && x.Activo == true);
            if (s == null)
            {
                CrearSeccionManager(usuarioId, seccion, tipoAreaId, tipoCheckId);
                return;
            }
            s.Seccion = seccion;
            s.TipoArea_id = tipoAreaId;
            s.TipoCheck_id = tipoCheckId;
            db.SubmitChanges();
        }

        public static List<Datos.Secciones> GetSecciones(bool? activo = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var query = db.Secciones.AsQueryable();
            if (activo.HasValue)
                query = query.Where(s => s.Activo == activo.Value);
            return query.OrderBy(s => s.Nombre).ToList();
        }

        public static void CrearSeccion(string nombre)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var s = new Datos.Secciones { Nombre = nombre, Activo = true };
            db.Secciones.InsertOnSubmit(s);
            db.SubmitChanges();
        }

        public static void EditarSeccion(int id, string nombre)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var s = db.Secciones.FirstOrDefault(x => x.id == id);
            if (s == null) return;
            s.Nombre = nombre;
            db.SubmitChanges();
        }

        public static void ToggleSeccion(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var s = db.Secciones.FirstOrDefault(x => x.id == id);
            if (s == null) return;
            s.Activo = !s.Activo;
            db.SubmitChanges();
        }

        public static void GuardarRecuperacion(int id, string pregunta, string respuesta)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.id == id);
            if (u == null) return;
            u.PreguntaRecuperacion = pregunta;
            u.RespuestaRecuperacion = respuesta;
            db.SubmitChanges();
        }

        public static string GetPreguntaRecuperacion(string nomina)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.Nomina == nomina && x.Activo == true);
            return u?.PreguntaRecuperacion;
        }

        public static bool ValidarRespuestaRecuperacion(string nomina, string respuesta)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.Nomina == nomina && x.Activo == true);
            if (u == null) return false;
            return u.RespuestaRecuperacion == respuesta;
        }

        public static void ResetearPassword(string nomina, string nuevaPassword)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.Nomina == nomina && x.Activo == true);
            if (u == null) return;
            u.Password = nuevaPassword;
            db.SubmitChanges();
        }

        public static void EnviarCorreo(string destinatario, string asunto, string cuerpo)
        {
            try
            {
                var mensaje = new System.Net.Mail.MailMessage();
                mensaje.To.Add(destinatario);
                mensaje.Subject = asunto;
                mensaje.Body = cuerpo;
                mensaje.IsBodyHtml = true;
                var cliente = new System.Net.Mail.SmtpClient();
                cliente.Send(mensaje);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al enviar correo: " + ex.Message);
            }
        }

        public static void NotificarRevisor(string nombreManager, string checkId, decimal calificacion)
        {
            var db = new Datos.DataClasses1DataContext();
            var revisores = db.Usuarios
                .Where(u => u.Roles.Rol == "Revisor" && u.Activo == true && u.Email != null)
                .ToList();

            string asunto = $"Manager Rounds — Nuevo formulario enviado: {checkId}";
            string cuerpo = $@"
        <div style='font-family:Segoe UI,sans-serif; font-size:14px;'>
            <div style='background:#1a1a1a; padding:16px 20px; border-bottom:3px solid #CC0000;'>
                <span style='color:#fff; font-weight:600;'>MANAGER <span style='color:#CC0000;'>ROUNDS</span></span>
            </div>
            <div style='padding:24px;'>
                <p><strong>{nombreManager}</strong> ha enviado el formulario <strong>{checkId}</strong>.</p>
                <p>Calificación obtenida: <strong>{calificacion}%</strong></p>
                <p>Ingresa al sistema para revisar y aprobar o rechazar.</p>
                <a href='https://localhost:44324/historial/Default.aspx' 
                   style='background:#CC0000; color:#fff; padding:10px 20px; border-radius:6px; text-decoration:none; display:inline-block; margin-top:8px;'>
                    Ver en el sistema
                </a>
            </div>
            <div style='padding:12px 24px; background:#f4f6f8; font-size:12px; color:#888;'>
                Astemo · MXQRP1 · Manager Rounds
            </div>
        </div>";

            foreach (var revisor in revisores)
                EnviarCorreo(revisor.Email, asunto, cuerpo);
        }

        public static void NotificarManager(int usuarioId, string checkId, string estatus, string comentario)
        {
            var db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.id == usuarioId);
            if (u == null || string.IsNullOrEmpty(u.Email)) return;

            string color = estatus == "Revisado" ? "#3B6D11" : "#A32D2D";
            string asunto = $"Manager Rounds — Tu formulario {checkId} fue {estatus.ToLower()}";
            string cuerpo = $@"
        <div style='font-family:Segoe UI,sans-serif; font-size:14px;'>
            <div style='background:#1a1a1a; padding:16px 20px; border-bottom:3px solid #CC0000;'>
                <span style='color:#fff; font-weight:600;'>MANAGER <span style='color:#CC0000;'>ROUNDS</span></span>
            </div>
            <div style='padding:24px;'>
                <p>Hola <strong>{u.Nombre}</strong>,</p>
                <p>Tu formulario <strong>{checkId}</strong> ha sido 
                   <strong style='color:{color};'>{estatus.ToLower()}</strong>.</p>
                {(string.IsNullOrEmpty(comentario) ? "" : $"<p>Comentario del revisor: <em>{comentario}</em></p>")}
            </div>
            <div style='padding:12px 24px; background:#f4f6f8; font-size:12px; color:#888;'>
                Astemo · MXQRP1 · Manager Rounds
            </div>
        </div>";

            EnviarCorreo(u.Email, asunto, cuerpo);
        }

        public static void GuardarEmail(int id, string email)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.id == id);
            if (u == null) return;
            u.Email = email;
            db.SubmitChanges();
        }

        public static void CrearNotificacion(int usuarioId, string mensaje, string url = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var n = new Datos.Notificaciones
            {
                Usuario_id = usuarioId,
                Mensaje = mensaje,
                Leida = false,
                FechaCreacion = DateTime.Now,
                Url = url
            };
            db.Notificaciones.InsertOnSubmit(n);
            db.SubmitChanges();
        }

        public static List<Datos.Notificaciones> GetNotificaciones(int usuarioId, bool soloNoLeidas = false)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var query = db.Notificaciones.Where(n => n.Usuario_id == usuarioId);
            if (soloNoLeidas)
                query = query.Where(n => n.Leida == false);
            return query.OrderByDescending(n => n.FechaCreacion).Take(20).ToList();
        }

        public static int ContarNoLeidas(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Notificaciones.Count(n => n.Usuario_id == usuarioId && n.Leida == false);
        }

        public static void MarcarLeida(int notificacionId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var n = db.Notificaciones.FirstOrDefault(x => x.id == notificacionId);
            if (n == null) return;
            n.Leida = true;
            db.SubmitChanges();
        }

        public static void MarcarTodasLeidas(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var notifs = db.Notificaciones.Where(n => n.Usuario_id == usuarioId && n.Leida == false).ToList();
            foreach (var n in notifs)
                n.Leida = true;
            db.SubmitChanges();
        }

        public static void NotificarRevisores(string mensaje, string url = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var revisores = db.Usuarios
                .Where(u => u.Roles.Rol == "Revisor" && u.Activo == true)
                .ToList();
            foreach (var r in revisores)
                CrearNotificacion(r.id, mensaje, url);
        }

        public static Datos.Revisiones GetRevisionRechazada(int usuarioId, string checkId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            return db.Revisiones
                .Where(r => r.Usuario_id == usuarioId
                    && r.Check_id == checkId
                    && r.Fecha >= lunes
                    && r.Estatus_id == 3)
                .OrderByDescending(r => r.Fecha)
                .FirstOrDefault();
        }
        public bool YaCompletadoAprobadoHoy(int usuarioId, string checkId)
        {
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            return db.Revisiones
                .Any(r => r.Usuario_id == usuarioId
                    && r.Fecha >= lunes
                    && r.Check_id == checkId
                    && r.Estatus_id != 3);
        }
        public static void BorrarNotificaciones(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var notifs = db.Notificaciones.Where(n => n.Usuario_id == usuarioId).ToList();
            db.Notificaciones.DeleteAllOnSubmit(notifs);
            db.SubmitChanges();
        }
        public static void GuardarFotoProblema(int respuestaId, string rutaFoto)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var r = db.RespuestasRevision.FirstOrDefault(x => x.id == respuestaId);
            if (r == null) return;
            r.FotoProblema = rutaFoto;
            db.SubmitChanges();
        }

        public static void CerrarHallazgo(int respuestaId, string rutaFoto)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var r = db.RespuestasRevision.FirstOrDefault(x => x.id == respuestaId);
            if (r == null) return;
            r.FotoCierre = rutaFoto;
            r.FechaCierre = DateTime.Now;
            r.HallazgoCerrado = true;
            db.SubmitChanges();
        }

        public static int HallazgosAbiertos(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            return db.RespuestasRevision
                .Where(r => r.Revisiones.Usuario_id == usuarioId
                    && r.TiposRespuesta.Respuesta == "Falla"
                    && r.HallazgoCerrado == false)
                .Count();
        }

        public static int HallazgosCerradosSemana(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime viernes = lunes.AddDays(4);
            return db.RespuestasRevision
                .Where(r => r.Revisiones.Usuario_id == usuarioId
                    && r.HallazgoCerrado == true
                    && r.FechaCierre >= lunes
                    && r.FechaCierre <= viernes)
                .Count();
        }

        public static string SubirFoto(System.Web.HttpPostedFile foto, int revisionId, int preguntaId, string tipo, string serverPath)
        {
            string carpeta = System.IO.Path.Combine(serverPath, "Uploads", "Hallazgos",
                revisionId.ToString(), preguntaId.ToString());
            if (!System.IO.Directory.Exists(carpeta))
                System.IO.Directory.CreateDirectory(carpeta);
            string extension = System.IO.Path.GetExtension(foto.FileName);
            string nombreArchivo = tipo + extension;
            string rutaCompleta = System.IO.Path.Combine(carpeta, nombreArchivo);
            foto.SaveAs(rutaCompleta);
            return $"~/Uploads/Hallazgos/{revisionId}/{preguntaId}/{nombreArchivo}";
        }
        public static int HallazgosAbiertosTotal()
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.RespuestasRevision
                .Where(r => r.TiposRespuesta.Respuesta == "Falla" && r.HallazgoCerrado == false)
                .Count();
        }

        public static int HallazgosCerradosSemanaTotal(DateTime lunes)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            DateTime viernes = lunes.AddDays(4);
            return db.RespuestasRevision
                .Where(r => r.HallazgoCerrado == true
                    && r.FechaCierre >= lunes
                    && r.FechaCierre <= viernes)
                .Count();
        }

        public static void EnviarRecordatorioHallazgos(int usuarioId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var u = db.Usuarios.FirstOrDefault(x => x.id == usuarioId);
            if (u == null || string.IsNullOrEmpty(u.Email)) return;

            if (u.UltimoRecordatorio.HasValue &&
                (DateTime.Now - u.UltimoRecordatorio.Value).TotalHours < 3) return;

            int abiertos = HallazgosAbiertos(usuarioId);
            if (abiertos == 0) return;

            string asunto = $"Manager Rounds — Tienes {abiertos} hallazgo(s) pendiente(s) de cerrar";
            string cuerpo = $@"
        <div style='font-family:Segoe UI,sans-serif; font-size:14px;'>
            <div style='background:#1a1a1a; padding:16px 20px; border-bottom:3px solid #CC0000;'>
                <span style='color:#fff; font-weight:600;'>MANAGER <span style='color:#CC0000;'>ROUNDS</span></span>
            </div>
            <div style='padding:24px;'>
                <p>Hola <strong>{u.Nombre}</strong>,</p>
                <p>Tienes <strong>{abiertos} hallazgo(s)</strong> pendiente(s) de cerrar. 
                Recuerda subir la foto de cierre para cada hallazgo resuelto.</p>
                <a href='https://localhost:44324/historial/Default.aspx' 
                   style='background:#CC0000; color:#fff; padding:10px 20px; border-radius:6px; text-decoration:none; display:inline-block; margin-top:8px;'>
                    Ver hallazgos
                </a>
            </div>
            <div style='padding:12px 24px; background:#f4f6f8; font-size:12px; color:#888;'>
                Astemo · MXQRP1 · Manager Rounds
            </div>
        </div>";

            EnviarCorreo(u.Email, asunto, cuerpo);
            CrearNotificacion(usuarioId, $"Tienes {abiertos} hallazgo(s) pendiente(s) de cerrar", "/historial/Default.aspx");

            u.UltimoRecordatorio = DateTime.Now;
            db.SubmitChanges();
        }
        public static List<Datos.RespuestasRevision> GetRespuestasConHallazgoAbierto(int revisionId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.RespuestasRevision
                .Where(r => r.Revision_id == revisionId
                    && r.TiposRespuesta.Respuesta == "Falla"
                    && r.HallazgoCerrado == false
                    && r.FotoProblema != null)
                .ToList();
        }
        public int CompletadosSemanaManager(int usuarioId)
        {
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime viernes = lunes.AddDays(4);
            var db = new Datos.DataClasses1DataContext();
            return db.Revisiones
                .Where(r => r.Usuario_id == usuarioId
                    && r.Fecha >= lunes
                    && r.Fecha <= viernes
                    && r.Estatus_id != 3)
                .Select(r => r.Check_id)
                .Distinct()
                .Count();
        }

        public decimal CumplimientoSemanaManager(int usuarioId)
        {
            DateTime lunes = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            DateTime viernes = lunes.AddDays(4);
            var db = new Datos.DataClasses1DataContext();
            var revisiones = db.Revisiones
                .Where(r => r.Usuario_id == usuarioId
                    && r.Fecha >= lunes
                    && r.Fecha <= viernes
                    && r.Calificacion != null)
                .ToList();
            if (!revisiones.Any()) return 0;
            return Math.Round(revisiones.Sum(r => r.Calificacion.Value) / 4, 1);
        }
    }
}