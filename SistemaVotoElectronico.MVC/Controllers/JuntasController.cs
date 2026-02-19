using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JuntasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "https://sistema-voto-electronico-z3q0.onrender.com/api";

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
            cedula = cedula?.Trim();

            if (string.IsNullOrEmpty(cedula))
            {
                TempData["Error"] = "Ingrese la cédula para validar.";
                return RedirectToAction("Index");
            }

            try
            {
             
                var response = await _httpClient.PostAsync($"{_apiBase}/Junta/GenerarToken?cedula={cedula}", null);
                var jsonRespuesta = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonObj = JObject.Parse(jsonRespuesta);
                    // Usamos el operador ? para evitar que el programa se rompa si el campo no existe
                    TempData["TokenGenerado"] = jsonObj["token"]?.ToString();
                    TempData["MensajeExito"] = jsonObj["nombres"]?.ToString() ?? "Votante Habilitado";
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest || response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        TempData["Error"] = "🚫 Acceso denegado: El sistema central no permite el voto para este perfil o el formato es incorrecto.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        TempData["Error"] = "🔍 Cédula no encontrada en el padrón electoral.";
                    }
                    else
                    {
                        TempData["Error"] = "⚠️ Error de validación. Verifique los datos en el Padrón.";
                    }
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "❌ Error de conexión con el servidor central.";
            }

            return RedirectToAction("Index");
        }
    }
}