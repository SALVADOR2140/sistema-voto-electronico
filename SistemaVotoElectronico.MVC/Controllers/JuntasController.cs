using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JuntasController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly string _apiBase = "http://localhost:5111/api";

        public JuntasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1. PANTALLA PRINCIPAL
        public IActionResult Index()
        {
            return View();
        }

        // 2. ACCIÓN: GENERAR TOKEN
        [HttpPost]
        public async Task<IActionResult> GenerarToken(string cedula)
        {
            if (string.IsNullOrEmpty(cedula))
            {
                TempData["Error"] = "Por favor ingrese un número de cédula.";
                return RedirectToAction("Index");
            }

            try
            {
                // 1. LLAMADA A LA API
                var response = await _httpClient.PostAsync($"{_apiBase}/Usuarios/GenerarTokenManual?cedula={cedula}", null);
                var jsonRespuesta = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var tokenObj = JToken.Parse(jsonRespuesta);
                    string tokenGenerado = tokenObj["token"]?.ToString() ?? "TOKEN-OK";
                    string nombreUsuario = tokenObj["nombre"]?.ToString() ?? "Ciudadano";

                    TempData["TokenGenerado"] = tokenGenerado;
                    TempData["MensajeExito"] = $"Token generado correctamente para: {nombreUsuario}";
                }
                else
                {

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = "🔍 Cédula no encontrada en el padrón electoral.";
                    }
                    else
                    {
                        TempData["Error"] = "⚠️ Votante no encontrado. Verifique el número de cédula.";
                    }
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "❌ No hay conexión con el servidor. Intente más tarde.";
            }

            return RedirectToAction("Index");
        }

    }
}