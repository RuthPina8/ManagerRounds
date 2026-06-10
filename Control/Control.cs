using System;
using System.Collections.Generic;
using System.Linq;
using Datos;

namespace Control
{
    public class Control
    {
        DataClasses1DataContext db = new DataClasses1DataContext();

        // Obtener total de managers activos
        public int TotalManagersActivos()
        {
            return db.Usuarios
                .Where(u => u.Activo == true && u.Roles.Rol == "Manager")
                .Count();
        }

        // Obtener cuántos managers completaron su formulario hoy
        public int CompletadosHoy()
        {
            DateTime hoy = DateTime.Today;
            return db.Revisiones
                .Where(r => r.Fecha == hoy)
                .Select(r => r.Usuario_id)
                .Distinct()
                .Count();
        }

        // Obtener cuántos managers faltan por completar hoy
        public int PendientesHoy()
        {
            return TotalManagersActivos() - CompletadosHoy();
        }

        // Obtener cumplimiento promedio de la semana actual
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

        // Obtener tabla de managers con su estatus por día en la semana
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

        // Obtener el check que corresponde hoy según el día de la semana
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

        // Obtener preguntas del formulario según check y tipo de área
        public List<Preguntas> GetPreguntas(string checkNumero, string tipoCheck)
        {
            string checkId = checkNumero + tipoCheck;
            return db.Preguntas
                .Where(p => p.Check_id == checkId && p.Activo == true)
                .ToList();
        }

        // Verificar si el manager ya hizo su formulario hoy
        public bool YaCompletadoHoy(int usuarioId, string checkId)
        {
            DateTime hoy = DateTime.Today;
            return db.Revisiones
                .Any(r => r.Usuario_id == usuarioId && r.Fecha == hoy && r.Check_id == checkId);
        }

        // Crear una nueva revisión
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

        // Guardar respuesta de una pregunta
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

        // Calcular y guardar la calificación final de la revisión
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
    }
}