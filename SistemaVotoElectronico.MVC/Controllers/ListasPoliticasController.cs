using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class ListasPoliticasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "http://localhost:5111/api/ListasPoliticas";
        private readonly string _apiEventos = "http://localhost:5111/api/EventosElectorales";

        public ListasPoliticasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1. LISTADO (INDEX)
        public async Task<IActionResult> Index()
        {
            var listas = await ObtenerListaDesdeApi<ListaPolitica>(_apiUrl);
            return View(listas);
        }

        // 2. CREAR (VISTA - GET)
        public async Task<IActionResult> Create()
        {
            var eventos = await ObtenerListaDesdeApi<EventoElectoral>(_apiEventos);
            ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre");
            return View();
        }

        // 3. CREAR (ACCIÓN - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListaPolitica lista, IFormFile? logoArchivo, string? logoUrlTexto)
        {
            ModelState.Remove("EventoElectoral");
            ModelState.Remove("logoArchivo");
            ModelState.Remove("logoUrlTexto");
            ModelState.Remove("LogoUrl");

            try
            {
                if (logoArchivo != null && logoArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await logoArchivo.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        lista.LogoUrl = $"data:image/png;base64,{base64String}";
                    }
                }
                else if (!string.IsNullOrEmpty(logoUrlTexto))
                {
                    lista.LogoUrl = logoUrlTexto.Trim();
                }
                else
                {
                    lista.LogoUrl = null;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error procesando imagen: " + ex.Message);
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(lista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(_apiUrl, content);
                    if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error API: {response.StatusCode} - {errorMsg}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión: " + ex.Message);
                }
            }

            var eventos = await ObtenerListaDesdeApi<EventoElectoral>(_apiEventos);
            ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre", lista.EventoElectoralId);
            return View(lista);
        }

        // 4. DETALLES
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // A. Buscamos la Lista
                var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var lista = ParsearRespuestaUnica(content);

                    if (lista != null)
                    {
                        // B. BUSCAMOS EL NOMBRE DEL EVENTO
                        ViewBag.NombreEvento = "Evento no disponible";
                        ViewBag.FechaEvento = null;

                        if (lista.EventoElectoralId > 0)
                        {
                            var respEvento = await _httpClient.GetAsync($"{_apiEventos}/{lista.EventoElectoralId}");
                            if (respEvento.IsSuccessStatusCode)
                            {
                                var contentEvento = await respEvento.Content.ReadAsStringAsync();

                                EventoElectoral evento = null;
                                var token = JToken.Parse(contentEvento);
                                if (token is JObject obj && obj.ContainsKey("result"))
                                    evento = obj["result"].ToObject<EventoElectoral>();
                                else if (token is JObject obj2 && obj2.ContainsKey("data"))
                                    evento = obj2["data"].ToObject<EventoElectoral>();
                                else
                                    evento = JsonConvert.DeserializeObject<EventoElectoral>(contentEvento);

                                if (evento != null)
                                {
                                    ViewBag.NombreEvento = evento.Nombre;
                                    ViewBag.FechaEvento = evento.FechaInicio;
                                }
                            }
                        }

                        return View(lista);
                    }
                }
            }
            catch { }
            return RedirectToAction(nameof(Index));
        }

        // 5. EDITAR (VISTA - GET)
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var lista = ParsearRespuestaUnica(content);

            var eventos = await ObtenerListaDesdeApi<EventoElectoral>(_apiEventos);
            ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre", lista.EventoElectoralId);

            return View(lista);
        }

        // 6. EDITAR (ACCIÓN - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListaPolitica lista, IFormFile? logoArchivo, string? logoUrlTexto)
        {
            ModelState.Remove("EventoElectoral");
            ModelState.Remove("logoArchivo");
            ModelState.Remove("logoUrlTexto");

            try
            {
                if (logoArchivo != null && logoArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await logoArchivo.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        lista.LogoUrl = $"data:image/png;base64,{base64String}";
                    }
                }
                else if (!string.IsNullOrEmpty(logoUrlTexto))
                {
                    lista.LogoUrl = logoUrlTexto.Trim();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error procesando imagen: " + ex.Message);
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(lista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);

                    if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"Error API: {response.StatusCode} - {errorMsg}");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión: " + ex.Message);
                }
            }

            var eventos = await ObtenerListaDesdeApi<EventoElectoral>(_apiEventos);
            ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre", lista.EventoElectoralId);
            return View(lista);
        }

        // 7. ELIMINAR (VISTA)
        public async Task<IActionResult> Delete(int id)
        {
            return View();
        }

        // 8. ELIMINAR (CONFIRMADO)
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }

        // --- HELPERS ---

        private async Task<IEnumerable<T>> ObtenerListaDesdeApi<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<T>();

                var json = await response.Content.ReadAsStringAsync();
                var token = JToken.Parse(json);

                if (token is JArray) return token.ToObject<IEnumerable<T>>();

                if (token is JObject)
                {
                    var propArray = ((JObject)token).Properties()
                        .FirstOrDefault(p => p.Value.Type == JTokenType.Array);
                    if (propArray != null) return propArray.Value.ToObject<IEnumerable<T>>();
                }
            }
            catch { }
            return new List<T>();
        }

        private ListaPolitica ParsearRespuestaUnica(string jsonContent)
        {
            try
            {
                var token = JToken.Parse(jsonContent);
                if (token is JObject obj)
                {
                    if (obj.ContainsKey("result")) return obj["result"].ToObject<ListaPolitica>();
                    if (obj.ContainsKey("data")) return obj["data"].ToObject<ListaPolitica>();
                }
                return JsonConvert.DeserializeObject<ListaPolitica>(jsonContent);
            }
            catch { return null; }
        }
    }
}