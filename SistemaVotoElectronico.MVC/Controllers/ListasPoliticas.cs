using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVoto.Modelos;
using System.Text;

namespace SistemaVoto.Web.Controllers
{
    public class ListasPoliticasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:5111/api/ListasPoliticas"; // Reemplaza XXXX con tu puerto de API

        public ListasPoliticasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: ListasPoliticas (Index)
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    var listas = JsonConvert.DeserializeObject<IEnumerable<ListaPolitica>>(contenido);
                    return View(listas);
                }
            }
            catch (Exception) { /* Log error */ }

            // CORREGIDO: Retorna una lista vacía si la API falla o no hay datos
            return View(new List<ListaPolitica>());
        }

        // GET: ListasPoliticas/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var contenido = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<ListaPolitica>(contenido);
            return View(lista);
        }

        // GET: ListasPoliticas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ListasPoliticas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListaPolitica lista)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(lista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_apiUrl, content);

                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            }
            return View(lista);
        }

        // GET: ListasPoliticas/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var contenido = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<ListaPolitica>(contenido);
            return View(lista);
        }

        // POST: ListasPoliticas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListaPolitica lista)
        {
            if (id != lista.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(lista);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);

                if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
            }
            return View(lista);
        }

        // GET: ListasPoliticas/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var contenido = await response.Content.ReadAsStringAsync();
            var lista = JsonConvert.DeserializeObject<ListaPolitica>(contenido);
            return View(lista);
        }

        // POST: ListasPoliticas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Delete), new { id = id });
        }
    }
}