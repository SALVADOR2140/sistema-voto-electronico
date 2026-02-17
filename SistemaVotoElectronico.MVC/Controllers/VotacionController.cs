using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.Api.Servicios;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotacionController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IEmailService _emailService;
        private readonly string _apiBase = "http://localhost:5111/api";

        public VotacionController(IHttpClientFactory httpClientFactory, IEmailService emailService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            string idUsuario = HttpContext.Session.GetString("IdUsuarioLogueado");
            if (string.IsNullOrEmpty(idUsuario)) return RedirectToAction("Login", "AccesoVotante");

            var eventos = await ObtenerDatosApi<EventoElectoral>($"{_apiBase}/EventosElectorales");

            var eventosDisponibles = eventos.Where(e =>
                e.Activo == true &&
                e.FechaInicio <= DateTime.Now &&
                e.FechaFin > DateTime.Now        
            ).ToList();

            return View(eventosDisponibles);
        }

        public async Task<IActionResult> Papeleta(int idEvento)
        {
            string idUsuario = HttpContext.Session.GetString("IdUsuarioLogueado");
            if (string.IsNullOrEmpty(idUsuario)) return RedirectToAction("Login", "AccesoVotante");

            var evento = await ObtenerUnico<EventoElectoral>($"{_apiBase}/EventosElectorales/{idEvento}");

                       if (evento == null || evento.FechaFin < DateTime.Now)
            {
                TempData["Error"] = "El tiempo de votación ha finalizado.";
                return RedirectToAction("Index");
            }

            var todasListas = await ObtenerDatosApi<ListaPolitica>($"{_apiBase}/ListasPoliticas");
            var todosCandidatos = await ObtenerDatosApi<Candidato>($"{_apiBase}/Candidatos");

            if (evento != null)
            {
                var idsListas = todasListas.Where(l => l.EventoElectoralId == idEvento).Select(l => l.Id).ToList();
                evento.Candidatos = todosCandidatos.Where(c => idsListas.Contains(c.ListaPoliticaId)).ToList();
            }
            return View(evento);
        }

        [HttpPost]
        public async Task<IActionResult> Votar(int idCandidato, int idEvento)
        {
            string idUsuarioString = HttpContext.Session.GetString("IdUsuarioLogueado");
            string emailUsuario = HttpContext.Session.GetString("EmailUsuario");

            // Validación de sesión
            if (string.IsNullOrEmpty(idUsuarioString)) return RedirectToAction("Login", "AccesoVotante");

            if (idEvento == 0)
            {
                TempData["Error"] = "Error: No se identificó el evento electoral.";
                return RedirectToAction("Index");
            }

            var eventoCheck = await ObtenerUnico<EventoElectoral>($"{_apiBase}/EventosElectorales/{idEvento}");
            if (eventoCheck != null && eventoCheck.FechaFin < DateTime.Now)
            {
                TempData["Error"] = "⚠️ Lo sentimos, el tiempo de votación terminó justo ahora.";
                return RedirectToAction("Index");
            }

            var votoData = new
            {
                UsuarioId = int.Parse(idUsuarioString),
                EventoId = idEvento,
                CandidatoId = idCandidato
            };

            var content = new StringContent(JsonConvert.SerializeObject(votoData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiBase}/Votos", content);

            if (response.IsSuccessStatusCode)
            {
                // 1. Enviar Correo
                if (!string.IsNullOrEmpty(emailUsuario))
                {
                    _ = Task.Run(() => _emailService.EnviarCertificado(emailUsuario, "Estudiante", "Elecciones 2026"));
                }

                // 2. CERRAR SESIÓN 
                HttpContext.Session.Clear();

                // 3. Redirigir al Inicio con mensaje de éxito
                TempData["VotoExitoso"] = "true";
                return RedirectToAction("Index", "Inicio");
            }
            else
            {
                var errorApi = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo registrar el voto: {errorApi}";
                return RedirectToAction("Papeleta", new { idEvento = idEvento });
            }
        }

        private async Task<List<T>> ObtenerDatosApi<T>(string url)
        {
            try
            {
                var json = await _httpClient.GetStringAsync(url);
                var token = JToken.Parse(json);
                if (token is JArray) return token.ToObject<List<T>>();
                if (token is JObject && token["result"] != null) return token["result"].ToObject<List<T>>();
                if (token is JObject && token["data"] != null) return token["data"].ToObject<List<T>>();
                if (token is JObject) return new List<T> { token.ToObject<T>() };
            }
            catch { }
            return new List<T>();
        }

        private async Task<T> ObtenerUnico<T>(string url)
        {
            try
            {
                var json = await _httpClient.GetStringAsync(url);
                var token = JToken.Parse(json);

                if (token is JObject && token["result"] != null)
                    return token["result"].ToObject<T>();

                if (token is JObject && token["data"] != null)
                    return token["data"].ToObject<T>();

                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al leer API: {ex.Message}");
            }
            return default(T);
        }
    }
}