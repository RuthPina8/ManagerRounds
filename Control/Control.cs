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
    }
}