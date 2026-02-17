using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.MVC.Models;
using System.Diagnostics;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly string _apiEventos = "http://localhost:5111/api/EventosElectorales";
        private readonly string _apiCandidatos = "http://localhost:5111/api/Candidatos";
        private readonly string _apiVotantes = "http://localhost:5111/api/Usuarios";
        // private readonly string _apiVotos = "http://localhost:5111/api/Votos"; 

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var modelo = new DashboardViewModel();

            try
            {
                var eventos = await ObtenerConteo(_apiEventos);
                var candidatos = await ObtenerConteo(_apiCandidatos);
                var votantes = await ObtenerConteo(_apiVotantes);

                modelo.TotalEventos = eventos;
                modelo.TotalCandidatos = candidatos;
                modelo.TotalVotantes = votantes;
                modelo.TotalVotos = 0;
            }
            catch
            {
            }

            return View(modelo);
        }

        private async Task<int> ObtenerConteo(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var token = JToken.Parse(json);

                    if (token is JArray arr) return arr.Count;

                    if (token is JObject obj && obj.ContainsKey("result") && obj["result"] is JArray arrResult)
                        return arrResult.Count;
                }
            }
            catch { }
            return 0;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}