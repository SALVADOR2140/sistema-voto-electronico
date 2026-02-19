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
        private readonly string _apiUrl = "https://sistema-voto-electronico-z3q0.onrender.com/api/Candidatos";
        private readonly string _apiListas = "https://sistema-voto-electronico-z3q0.onrender.com/api/ListasPoliticas";

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
            // 1. LIMPIEZA TOTAL: Eliminamos las validaciones que estorban
            ModelState.Remove("ListaPolitica");
            ModelState.Remove("fotoArchivo");
            ModelState.Remove("fotoUrlTexto");
            ModelState.Remove("FotoUrl");

            // Lógica de procesamiento de imagen
            try
            {
                if (fotoArchivo != null && fotoArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fotoArchivo.CopyToAsync(memoryStream);
                        candidato.FotoUrl = $"data:image/png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
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

            // 2. ENVÍO LIMPIO (Clave para evitar el BadRequest)
            // Creamos un objeto anónimo para enviar SOLO lo que la base de datos necesita
            var datosLimpios = new
            {
                nombres = candidato.Nombres,
                cargo = candidato.Cargo,
                fotoUrl = candidato.FotoUrl,
                planGobiernoUrl = candidato.PlanGobiernoUrl,
                listaPoliticaId = candidato.ListaPoliticaId // Asegúrate que este valor no sea 0
            };

            var json = JsonConvert.SerializeObject(datosLimpios);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Error API: {errorMsg}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error conexión: " + ex.Message);
            }

            // Si falló, recargar el combo para la vista
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

            candidato.Id = id;

            ModelState.Remove("ListaPolitica");
            ModelState.Remove("fotoArchivo");
            ModelState.Remove("fotoUrlTexto");
            ModelState.Remove("FotoUrl");
            ModelState.Remove("PlanGobiernoUrl");

            // Lógica de procesamiento de imagen
            try
            {
                if (fotoArchivo != null && fotoArchivo.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await fotoArchivo.CopyToAsync(memoryStream);
                        candidato.FotoUrl = $"data:image/png;base64,{Convert.ToBase64String(memoryStream.ToArray())}";
                    }
                }
                else if (!string.IsNullOrEmpty(fotoUrlTexto))
                {
                    candidato.FotoUrl = fotoUrlTexto.Trim();
                }
            }
            catch { }


            var datosLimpios = new
            {
                id = candidato.Id,
                nombres = candidato.Nombres,
                cargo = candidato.Cargo,
                fotoUrl = candidato.FotoUrl,
                planGobiernoUrl = candidato.PlanGobiernoUrl,
                listaPoliticaId = candidato.ListaPoliticaId 
            };

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(datosLimpios);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
      
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Error en API: " + error);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error de conexión: " + ex.Message);
            }


            var listas = await ObtenerListaGenerica<ListaPolitica>(_apiListas);
            ViewBag.Listas = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(listas, "Id", "Nombre", candidato.ListaPoliticaId);
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