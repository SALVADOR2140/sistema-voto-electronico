using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SistemaVoto.Modelos;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://sistema-voto-electronico-z3q0.onrender.com/api/Usuarios";

        public UsuariosController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // 1. LISTADO (INDEX)
        public async Task<IActionResult> Index()
        {
            var usuarios = await ObtenerListaGenerica<Usuario>(_apiUrl);
            return View(usuarios);
        }

        // 2. DETALLES (DETAILS)
        public async Task<IActionResult> Details(int id)
        {
            var usuario = await ObtenerEntidad<Usuario>(id);
            if (usuario == null) return RedirectToAction(nameof(Index));
            return View(usuario);
        }

        // 3. CREAR (VISTA - GET)
        public IActionResult Create()
        {
            return View();
        }

        // 4. CREAR (ACCIÓN - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            ModelState.Remove("Clave");
            ModelState.Remove("RolUsuario");
            ModelState.Remove("TokenVotacion");
            ModelState.Remove("YaVoto");

            try
            {
                usuario.Cedula = usuario.Cedula?.Trim();
                usuario.YaVoto = false;
                usuario.TokenVotacion = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

                if (string.IsNullOrEmpty(usuario.Clave))
                {
                    usuario.Clave = usuario.Cedula;
                }

                if (usuario.RolUsuarioId == 0)
                {
                    usuario.RolUsuarioId = 2;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error en datos por defecto: " + ex.Message);
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(usuario);
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
                        if (response.StatusCode == System.Net.HttpStatusCode.Conflict || errorMsg.Contains("Cédula"))
                        {
                            ModelState.AddModelError("Cedula", "Esta cédula ya está registrada.");
                        }
                        else
                        {
                            ModelState.AddModelError("", $"Error API: {response.StatusCode} - {errorMsg}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error de conexión: " + ex.Message);
                }
            }

            return View(usuario);
        }

        // 5. EDITAR (VISTA - GET)
        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await ObtenerEntidad<Usuario>(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // 6. EDITAR (ACCIÓN - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            ModelState.Remove("Clave");
            ModelState.Remove("RolUsuario");
            ModelState.Remove("TokenVotacion");

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", "No se pudo actualizar el votante.");
            }
            return View(usuario);
        }

        // 7. ELIMINAR (VISTA - GET)
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await ObtenerEntidad<Usuario>(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // 8. ELIMINAR (CONFIRMACIÓN - POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index), new { error = "No se pudo eliminar el registro" });
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
                        if (obj["result"] is JArray arr) return arr.ToObject<IEnumerable<T>>();
                        if (obj["data"] is JArray arr2) return arr2.ToObject<IEnumerable<T>>();
                    }
                }
            }
            catch { }
            return new List<T>();
        }

        private async Task<T> ObtenerEntidad<T>(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiUrl}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var token = JToken.Parse(json);
                    if (token is JObject obj)
                    {
                        if (obj.ContainsKey("result")) return obj["result"].ToObject<T>();
                        if (obj.ContainsKey("data")) return obj["data"].ToObject<T>();
                    }
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            catch { }
            return default(T);
        }
    }
}