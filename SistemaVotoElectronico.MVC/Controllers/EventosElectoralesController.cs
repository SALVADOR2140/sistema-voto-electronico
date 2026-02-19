using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.MVC.Filtros;


namespace SistemaVotoElectronico.MVC.Controllers
{
    public class EventosElectoralesController : Controller
    {
        // 1. INYECCIÓN DE HTTP
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "https://sistema-voto-electronico-z3q0.onrender.com/api";

        public EventosElectoralesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [VerificarSesion]
        public ActionResult Index()
        {
            // 1. Traer todos los eventos
            var respuesta = Crud<EventoElectoral>.ReadAll();
            var eventos = respuesta.Data ?? new List<EventoElectoral>();

            bool huboCambios = false;
            DateTime ahora = DateTime.Now;

            foreach (var evento in eventos)
            {
                if (evento.Activo && evento.FechaFin < ahora)
                {
                    evento.Activo = false;
                    try
                    {
                        Crud<EventoElectoral>.Update(evento.Id.ToString(), evento);
                        huboCambios = true;
                    }
                    catch { /* Silenciar errores de red temporales */ }
                }
            }

            if (huboCambios)
            {
                respuesta = Crud<EventoElectoral>.ReadAll();
                eventos = respuesta.Data ?? new List<EventoElectoral>();
            }

            // 2. Estadísticas para los indicadores de la vista
            ViewBag.Total = eventos.Count;
            ViewBag.Activos = eventos.Count(e => e.Activo);

            return View(eventos);
        }

        // GET: EventosElectorales/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                var response = Crud<EventoElectoral>.ReadBy(id.ToString());
                if (response.Data == null) return RedirectToAction(nameof(Index));

                return View(response.Data);
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }

        public ActionResult Create() => View();

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
            catch { return View(); }
        }

        public ActionResult Edit(int id) => View(Crud<EventoElectoral>.ReadBy(id.ToString()).Data);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, EventoElectoral data)
        {
            try
            {
                Crud<EventoElectoral>.Update(id.ToString(), data);
                return RedirectToAction(nameof(Index));
            }
            catch { return View(); }
        }

        [HttpPost]
        public async Task<ActionResult> EliminarConfirmado(int id, string password)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBase}/Usuarios");
                bool autorizado = false;

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    List<Usuario> usuarios = null;

                    try
                    {
                        usuarios = JsonConvert.DeserializeObject<List<Usuario>>(json);
                    }
                    catch
                    {
                        var wrapper = JsonConvert.DeserializeObject<dynamic>(json);
                        try { usuarios = wrapper.result.ToObject<List<Usuario>>(); } catch { }
                        if (usuarios == null) try { usuarios = wrapper.data.ToObject<List<Usuario>>(); } catch { }
                    }

                    if (usuarios != null)
                    {
                        var admin = usuarios.FirstOrDefault(u => u.Clave == password);
                        if (admin != null) autorizado = true;
                    }
                }

                if (!autorizado)
                {
                    for (int i = 1; i <= 200; i++)
                    {
                        var res = Crud<Usuario>.ReadBy(i.ToString());
                        if (res.Data != null && res.Data.Clave == password)
                        {
                            autorizado = true;
                            break;
                        }
                    }
                }

                if (!autorizado)
                {
                    return Json(new { success = false, message = "⛔ Contraseña incorrecta o nivel de acceso insuficiente." });
                }

                Crud<EventoElectoral>.Delete(id.ToString());
                return Json(new { success = true, message = "Evento eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error interno del servidor: " + ex.Message });
            }
        }
    }
}