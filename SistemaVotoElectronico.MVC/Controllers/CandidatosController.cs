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
        private readonly string _apiUrl = "http://localhost:5111/api/Candidatos";
        private readonly string _apiListas = "http://localhost:5111/api/ListasPoliticas";

        public CandidatosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> Index()
        {
            var candidatos = await ObtenerListaGenerica<Candidato>(_apiUrl);
            return View(candidatos);
        }


        public async Task<IActionResult> Create()
        {
            var listas = await ObtenerListaGenerica<ListaPolitica>(_apiListas);
            ViewBag.Listas = new SelectList(listas, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Candidato candidato, IFormFile? fotoArchivo, string? fotoUrlTexto)
        {
            ModelState.Remove("ListaPolitica");
            ModelState.Remove("fotoArchivo");
            ModelState.Remove("fotoUrlTexto");
            ModelState.Remove("FotoUrl");

            try
            {
                if (fotoArchivo != null && fotoArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fotoArchivo.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        candidato.FotoUrl = $"data:image/png;base64,{base64String}";
                    }
                }
                else if (!string.IsNullOrEmpty(fotoUrlTexto))
                {
                    candidato.FotoUrl = fotoUrlTexto.Trim();
                }
                else
                {
                    candidato.FotoUrl = null;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error imagen: " + ex.Message);
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(candidato);
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
                    ModelState.AddModelError("", "Error conexión: " + ex.Message);
                }
            }

            var listas = await ObtenerListaGenerica<ListaPolitica>(_apiListas);
            ViewBag.Listas = new SelectList(listas, "Id", "Nombre", candidato.ListaPoliticaId);
            return View(candidato);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var candidato = ParsearObjetoUnico<Candidato>(content);

                    if (candidato != null)
                    {
                        ViewBag.NombreLista = "Candidato Independiente";
                        if (candidato.ListaPoliticaId > 0)
                        {
                            var respLista = await _httpClient.GetAsync($"{_apiListas}/{candidato.ListaPoliticaId}");
                            if (respLista.IsSuccessStatusCode)
                            {
                                var contentLista = await respLista.Content.ReadAsStringAsync();
                                var listaObj = ParsearObjetoUnico<ListaPolitica>(contentLista);
                                if (listaObj != null) ViewBag.NombreLista = listaObj.Nombre;
                            }
                        }
                        return View(candidato);
                    }
                }
            }
            catch { }
            return RedirectToAction(nameof(Index));
        }

  
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var candidato = ParsearObjetoUnico<Candidato>(content);

            var listas = await ObtenerListaGenerica<ListaPolitica>(_apiListas);
            ViewBag.Listas = new SelectList(listas, "Id", "Nombre", candidato.ListaPoliticaId);

            return View(candidato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Candidato candidato, IFormFile? fotoArchivo, string? fotoUrlTexto)
        {
            ModelState.Remove("ListaPolitica");
            ModelState.Remove("fotoArchivo");
            ModelState.Remove("fotoUrlTexto");

            try
            {
                if (fotoArchivo != null && fotoArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fotoArchivo.CopyToAsync(memoryStream);
                        byte[] imageBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(imageBytes);
                        candidato.FotoUrl = $"data:image/png;base64,{base64String}";
                    }
                }
                else if (!string.IsNullOrEmpty(fotoUrlTexto))
                {
                    candidato.FotoUrl = fotoUrlTexto.Trim();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error imagen: " + ex.Message);
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(candidato);
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
                    ModelState.AddModelError("", "Error conexión: " + ex.Message);
                }
            }

            var listas = await ObtenerListaGenerica<ListaPolitica>(_apiListas);
            ViewBag.Listas = new SelectList(listas, "Id", "Nombre", candidato.ListaPoliticaId);
            return View(candidato);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var candidato = ParsearObjetoUnico<Candidato>(content);

            return View(candidato);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return RedirectToAction(nameof(Index));
        }


        private async Task<IEnumerable<T>> ObtenerListaGenerica<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var token = JToken.Parse(json);

                    if (token is JArray) return token.ToObject<IEnumerable<T>>();

                    if (token is JObject obj)
                    {
                        if (obj["result"] is JArray arr1) return arr1.ToObject<IEnumerable<T>>();
                        if (obj["data"] is JArray arr2) return arr2.ToObject<IEnumerable<T>>();
                        if (obj["value"] is JArray arr3) return arr3.ToObject<IEnumerable<T>>();
                    }
                }
            }
            catch { }
            return new List<T>();
        }

        private T ParsearObjetoUnico<T>(string json)
        {
            try
            {
                var token = JToken.Parse(json);
                if (token is JObject obj)
                {
                    if (obj.ContainsKey("result")) return obj["result"].ToObject<T>();
                    if (obj.ContainsKey("data")) return obj["data"].ToObject<T>();
                }
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { return default(T); }
        }
    }
}