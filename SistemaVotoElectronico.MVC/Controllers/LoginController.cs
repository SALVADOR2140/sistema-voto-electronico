using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class LoginController : Controller
    {
        // 1. Inyectamos IConfiguration para leer las variables de Render
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index(string tipo = "")
        {
            ViewBag.TipoEsperado = tipo;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Entrar(string usuario, string clave, string tipoEsperado)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // 2. LEEMOS LA URL DESDE LA VARIABLE DE ENTORNO
                    // Render inyecta esto como "ApiSettings:BaseUrl"
                    string baseUrl = _configuration["ApiSettings:BaseUrl"];

                    // Si por alguna razón no la encuentra, usamos un valor por defecto (pero en Render la encontrará)
                    if (string.IsNullOrEmpty(baseUrl))
                    {
                        ViewBag.Error = "⚠️ Error Crítico: No se configuró la URL de la API.";
                        return View("Index");
                    }
                    
                    // Aseguramos que la URL no tenga doble barra al final
                    baseUrl = baseUrl.TrimEnd('/');
                    
                    // Armamos la URL final
                    string urlApi = $"{baseUrl}/api/Auth/LoginWeb";

                    
                    // Por esto (usando un Diccionario para asegurar los nombres):
                    var loginDto = new Dictionary<string, string>
                    {
                        { "Correo", usuario }, // Asegúrate que en la API el DTO diga "Correo"
                        { "Clave", clave }
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlApi, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

                        string rolNombre = ((string)data.rol)?.Trim().ToLower() ?? "";
                        int rolId = 3; 

                        if (rolNombre.Contains("admin")) rolId = 1;
                        else if (rolNombre.Contains("candidato")) rolId = 2;

                        if (tipoEsperado == "admin" && rolId != 1)
                        {
                            ViewBag.Error = "⛔ Acceso Denegado: Esta cuenta no es de Administrador.";
                            ViewBag.TipoEsperado = tipoEsperado;
                            return View("Index");
                        }

                        if (tipoEsperado == "candidato" && rolId != 2)
                        {
                            ViewBag.Error = "⛔ Acceso Denegado: Esta cuenta no es de Candidato.";
                            ViewBag.TipoEsperado = tipoEsperado;
                            return View("Index");
                        }
  
                        HttpContext.Session.SetString("UsuarioLogueado", (string)data.nombre);
                        HttpContext.Session.SetInt32("RolUsuarioId", rolId);

                        if (rolId == 1) return RedirectToAction("Index", "Home"); 
                        if (rolId == 2) return RedirectToAction("Index", "Home"); 

                        return RedirectToAction("Index", "Votacion");
                    }
                    else
                    {
                        ViewBag.Error = "Usuario o contraseña incorrectos.";
                        ViewBag.TipoEsperado = tipoEsperado; 
                        return View("Index");
                    }
                }
            }
            catch (Exception ex) // Capturamos la excepción para ver el mensaje real si falla
            {
                // Mostramos el mensaje real del error para depurar
                ViewBag.Error = $"Error de conexión: {ex.Message}";
                ViewBag.TipoEsperado = tipoEsperado;
                return View("Index");
            }
        }

        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Inicio"); 
        }
    }
}