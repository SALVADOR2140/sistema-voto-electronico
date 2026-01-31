using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.MVC.Filtros;  

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class EventosElectoralesController : Controller
    {
        [VerificarSesion]
        // GET: EventosElectorales
        public ActionResult Index()
        {
            // 1. Traemos la lista de la API
            var data = Crud<EventoElectoral>.ReadAll();

            // 2. Calcular las estadísticas
            if (data.Data != null && data.Data.Count > 0)
            {
                var total = data.Data.Count;
                ViewBag.Total = total;
                ViewData["Total"] = total;

                ViewBag.Promedio = data.Data.Average(e => e.Id);
                ViewBag.Minimo = data.Data.Min(e => e.Id);
                ViewBag.Maximo = data.Data.Max(e => e.Id);
            }
            else
            {
                ViewBag.Total = 0;
                ViewBag.Promedio = 0;
                ViewBag.Minimo = 0;
                ViewBag.Maximo = 0;
            }

            return View(data.Data ?? new List<EventoElectoral>());
        }

        // GET: EventosElectorales/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<EventoElectoral>.ReadBy(id.ToString());
            return View(data.Data);
        }

        // GET: EventosElectorales/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EventosElectorales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventoElectoral data)
        {
            try
            {
                if (data.FechaInicio == DateTime.MinValue) data.FechaInicio = DateTime.Now;
                if (data.FechaFin == DateTime.MinValue) data.FechaFin = DateTime.Now.AddDays(1);

                Crud<EventoElectoral>.Create(data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EventosElectorales/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<EventoElectoral>.ReadBy(id.ToString());
            return View(data.Data);
        }

        // POST: EventosElectorales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EventoElectoral data)
        {
            try
            {
                Crud<EventoElectoral>.Update(id.ToString(), data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EventosElectorales/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<EventoElectoral>.ReadBy(id.ToString());
            return View(data.Data);
        }

        // POST: EventosElectorales/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, EventoElectoral data)
        {
            try
            {
                Crud<EventoElectoral>.Delete(id.ToString());
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}