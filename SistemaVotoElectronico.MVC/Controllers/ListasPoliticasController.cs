using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVoto.Modelos;
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

        // 1. LISTADO
        public async Task<IActionResult> Index()
        {
            var listas = await ObtenerListaDesdeApi<ListaPolitica>(_apiUrl);
            return View(listas);
        }

        // 2. CREAR (VISTA)
        public async Task<IActionResult> Create()
        {
            var eventos = await ObtenerListaDesdeApi<EventoElectoral>(_apiEventos);

            ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre");

            return View();
        }

        // 3. CREAR (ACCIÓN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListaPolitica lista)
        {
            ModelState.Remove("EventoElectoral");

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(lista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(_apiUrl, content);
                    if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión: " + ex.Message);
                }
            }

            var eventos = await ObtenerListaDesdeApi<EventoElectoral>(_apiEventos);
            ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre");

            return View(lista);
        }
        // GET: ListasPoliticas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var token = JToken.Parse(content);
                    ListaPolitica lista = null;

                    if (token is JObject obj)
                    {
                        if (obj.ContainsKey("result")) lista = obj["result"].ToObject<ListaPolitica>();
                        else if (obj.ContainsKey("data")) lista = obj["data"].ToObject<ListaPolitica>();
                        else lista = JsonConvert.DeserializeObject<ListaPolitica>(content); // Viene directo
                    }

                    if (lista != null) return View(lista);
                }
            }
            catch
            {
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. EDITAR (VISTA)
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<ListaPolitica>(content);

            var respEventos = await _httpClient.GetAsync(_apiEventos);
            if (respEventos.IsSuccessStatusCode)
            {
                var contentEventos = await respEventos.Content.ReadAsStringAsync();
                if (contentEventos.Trim().StartsWith("["))
                {
                    var eventos = JsonConvert.DeserializeObject<IEnumerable<EventoElectoral>>(contentEventos);
                    ViewBag.Eventos = new SelectList(eventos, "Id", "Nombre", lista.EventoElectoralId);
                }
            }

            return View(lista);
        }

        // 4. EDITAR 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListaPolitica lista)
        {
            if (id != lista.Id) return BadRequest();

            ModelState.Remove("EventoElectoral"); 

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(lista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);

                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            }

            return await Edit(id);
        }

        // 5. ELIMINAR (VISTA DE CONFIRMACIÓN)
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<ListaPolitica>(content);

            return View(lista);
        }

        // 5. ELIMINAR (ACCIÓN CONFIRMADA)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            return View();
        }

        private async Task<IEnumerable<T>> ObtenerListaDesdeApi<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return new List<T>();

                var json = await response.Content.ReadAsStringAsync();
                var token = JToken.Parse(json);

                if (token is JArray)
                {
                    return token.ToObject<IEnumerable<T>>();
                }

                if (token is JObject)
                {
                    var propArray = ((JObject)token).Properties()
                        .FirstOrDefault(p => p.Value.Type == JTokenType.Array);

                    if (propArray != null)
                    {
                        return propArray.Value.ToObject<IEnumerable<T>>();
                    }
                }
            }
            catch
            {
            }
            return new List<T>();
        }
    }
}