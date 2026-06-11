using System;
using System.Collections.Generic;
using System.Linq;
using Datos;

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
            var revision = new Revisiones
            {
                Usuario_id = usuarioId,
                ManagerSeccion_id = seccionId,
                Fecha = DateTime.Today,
                Check_id = checkId,
                FechaInicio = DateTime.Now,
                Estatus_id = 1
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
            int cumplen = respuestas.Count(r => r.TiposRespuesta.Respuesta == "Cumple");
            int na = respuestas.Count(r => r.TiposRespuesta.Respuesta == "N/A");

            int evaluables = total - na;
            decimal calificacion = evaluables > 0 ? Math.Round((decimal)cumplen / evaluables * 100, 1) : 0;

            revision.Calificacion = calificacion;
            revision.FechaFin = DateTime.Now;
            revision.Estatus_id = 2;
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
        // Obtener todas las preguntas con filtros opcionales
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

        // Activar / desactivar una pregunta
        public static void TogglePregunta(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var p = db.Preguntas.FirstOrDefault(x => x.id == id);
            if (p == null) return;
            p.Activo = !p.Activo;
            db.SubmitChanges();
        }

        // Activar / desactivar todas las preguntas de un check
        public static void ToggleCheck(string checkId, bool activo)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var preguntas = db.Preguntas.Where(p => p.Check_id == checkId).ToList();
            foreach (var p in preguntas)
                p.Activo = activo;
            db.SubmitChanges();
        }

        // Crear pregunta
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

        // Editar pregunta
        public static void EditarPregunta(int id, string texto)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            var p = db.Preguntas.FirstOrDefault(x => x.id == id);
            if (p == null) return;
            p.Pregunta = texto;
            db.SubmitChanges();
        }

        // Verificar si un check tiene todas sus preguntas activas
        public static bool CheckActivo(string checkId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Preguntas.Any(p => p.Check_id == checkId && p.Activo == true);
        }
        // Obtener revisiones por semana con filtros opcionales
        public static List<Datos.Revisiones> GetRevisiones(DateTime lunes, string checkId = null, int? estatusId = null)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            DateTime viernes = lunes.AddDays(4);
            var query = db.Revisiones.Where(r => r.Fecha >= lunes && r.Fecha <= viernes);
            if (!string.IsNullOrEmpty(checkId))
                query = query.Where(r => r.Check_id == checkId);
            if (estatusId.HasValue)
                query = query.Where(r => r.Estatus_id == estatusId.Value);
            return query.OrderByDescending(r => r.FechaInicio).ToList();
        }

        // Obtener una revisión por id
        public static Datos.Revisiones GetRevision(int id)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.Revisiones.FirstOrDefault(r => r.id == id);
        }
        // Obtener respuestas de una revisión
        public static List<Datos.RespuestasRevision> GetRespuestas(int revisionId)
        {
            Datos.DataClasses1DataContext db = new Datos.DataClasses1DataContext();
            return db.RespuestasRevision
                .Where(r => r.Revision_id == revisionId)
                .OrderBy(r => r.Pregunta_id)
                .ToList();
        }

        // Cambiar estatus de una revisión (aprobar o rechazar)
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
    }
}
