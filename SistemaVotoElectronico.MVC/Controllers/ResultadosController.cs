using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class ResultadosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBase = "http://localhost:5111/api";
        private const int NUMERO_ESCAÑOS = 5;

        public ResultadosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var resultados = new List<ResultadoLista>();

            // 1. Obtenemos las listas de tu base de datos
            var listas = await ObtenerDatosApi<ListaPolitica>($"{_apiBase}/ListasPoliticas");

            // 2. Buscamos el evento activo
            int eventoId = 1;
            try
            {
                var eventos = await ObtenerDatosApi<EventoElectoral>($"{_apiBase}/EventosElectorales");
                var activo = eventos.FirstOrDefault(e => e.Activo == true);
                if (activo != null) eventoId = activo.Id;
            }
            catch { }

            // 3. Pedimos los resultados EN VIVO
            string urlConteo = $"{_apiBase}/Resultados/EnVivo/{eventoId}";
            string jsonRespuesta = "";
            try { jsonRespuesta = await _httpClient.GetStringAsync(urlConteo); } catch { }

            var conteos = ProcesarConteo(jsonRespuesta);

            string[] paleta = { "#4e73df", "#1cc88a", "#36b9cc", "#f6c23e", "#e74a3b", "#858796", "#6610f2" };
            int i = 0;

            foreach (var lista in listas)
            {
                var dato = conteos.FirstOrDefault(c => c.ListaId == lista.Id);

                if (dato == null)
                {
                    dato = conteos.FirstOrDefault(c =>
                        string.Equals(c.NombreLista, lista.Nombre, StringComparison.OrdinalIgnoreCase));
                }

                int votosReales = dato != null ? dato.CantidadVotos : 0;

                resultados.Add(new ResultadoLista
                {
                    NombreLista = lista.Nombre,
                    LogoUrl = lista.LogoUrl,
                    TotalVotos = votosReales,
                    EscañosGanados = 0,
                    Color = paleta[i % paleta.Length]
                });
                i++;
            }

            // 4. APLICAR MÉTODO WEBSTER
            var calculo = resultados.Select(r => new CalculadoraWebster { Original = r, Votos = r.TotalVotos, Divisor = 1.0 }).ToList();
            if (calculo.Sum(c => c.Votos) > 0)
            {
                for (int k = 0; k < NUMERO_ESCAÑOS; k++)
                {
                    var ganador = calculo.OrderByDescending(t => t.Votos).First();
                    if (ganador.Original.TotalVotos == 0) break;
                    ganador.Original.EscañosGanados++;
                    ganador.Divisor += 2;
                    ganador.Votos = ganador.Original.TotalVotos / ganador.Divisor;
                }
            }

            ViewBag.DebugJson = null;

            return View(resultados.OrderByDescending(r => r.EscañosGanados).ThenByDescending(r => r.TotalVotos).ToList());
        }

        private List<DtoConteo> ProcesarConteo(string json)
        {
            var lista = new List<DtoConteo>();
            try
            {
                if (string.IsNullOrEmpty(json)) return lista;

                var token = JToken.Parse(json);
                IEnumerable<JToken> items = null;

                if (token is JObject && token["detalle"] != null)
                {
                    items = token["detalle"].Children();
                }
                else if (token is JArray) items = token.Children();
                else if (token["result"] != null) items = token["result"].Children();

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        string nombre = (string)item["partido"] ?? (string)item["nombre"] ?? "";

                        int votos = (int?)item["votos"] ?? (int?)item["cantidad"] ?? (int?)item["total"] ?? (int?)item["cantidadVotos"] ?? 0;

                        int id = (int?)item["listaId"] ?? (int?)item["id"] ?? 0;

                        lista.Add(new DtoConteo
                        {
                            ListaId = id,
                            NombreLista = nombre,
                            CantidadVotos = votos
                        });
                    }
                }
            }
            catch { }
            return lista;
        }

        private async Task<IEnumerable<T>> ObtenerDatosApi<T>(string url)
        {
            try
            {
                var json = await _httpClient.GetStringAsync(url);
                var token = JToken.Parse(json);
                if (token is JArray) return token.ToObject<IEnumerable<T>>();
                if (token is JObject && token["result"] != null) return token["result"].ToObject<IEnumerable<T>>();
            }
            catch { }
            return new List<T>();
        }
    }

    // DTO Actualizado para soportar Nombre
    public class DtoConteo
    {
        public int ListaId { get; set; }
        public string NombreLista { get; set; }
        public int CantidadVotos { get; set; }
    }

    public class CalculadoraWebster
    {
        public ResultadoLista Original { get; set; }
        public double Votos { get; set; }
        public double Divisor { get; set; }
    }
}