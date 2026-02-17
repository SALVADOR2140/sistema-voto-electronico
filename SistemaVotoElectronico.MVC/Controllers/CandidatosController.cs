using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVoto.Modelos;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly string _apiCandidatos = "http://localhost:5111/api/Candidatos";
        private readonly string _apiListas = "http://localhost:5111/api/ListasPoliticas";

        public CandidatosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1. LISTADO DE CANDIDATOS
        public async Task<IActionResult> Index()
        {
            var candidatos = await ObtenerListaDesdeApi<Candidato>(_apiCandidatos);
            return View(candidatos);
        }

        // 2. CREAR (VISTA)
        public async Task<IActionResult> Create()
        {
            var listas = await ObtenerListaDesdeApi<ListaPolitica>(_apiListas);

            ViewBag.Listas = new SelectList(listas, "Id", "Nombre");

            return View();
        }

        // 3. CREAR 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidato candidato)
        {
            ModelState.Remove("ListaPolitica");

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(candidato);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await _httpClient.PostAsync(_apiCandidatos, content);
                    if (response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión: " + ex.Message);
                }
            }

            var listas = await ObtenerListaDesdeApi<ListaPolitica>(_apiListas);
            ViewBag.Listas = new SelectList(listas, "Id", "Nombre");

            return View(candidato);
        }

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
    }
}