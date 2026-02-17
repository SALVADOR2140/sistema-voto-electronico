using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AccesoVotanteController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            HttpContext.Session.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Ingresar(string tokenIngresado)
        {
            if (string.IsNullOrEmpty(tokenIngresado))
            {
                ViewBag.Error = "Por favor, escriba su token.";
                return View("Login");
            }

            try
            {
                using (var client = new HttpClient())
                {
                    string urlApi = "http://localhost:5111/api/Usuarios";
                    var response = await client.GetAsync(urlApi);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(json);

                        // Buscamos el token
                        var votante = usuarios.FirstOrDefault(u => u.TokenVotacion != null && u.TokenVotacion.Trim() == tokenIngresado.Trim());

                        if (votante != null)
                        {
                            // 1. VALIDACIÓN: ¿Ya votó?
                            if (votante.YaVoto)
                            {
                                ViewBag.Error = "⛔ Este token YA fue utilizado. No puede volver a votar.";
                                return View("Login");
                            }

                            // 2. VALIDACIÓN: ¿Tiene ID válido?
                            if (votante.Id <= 0)
                            {
                                ViewBag.Error = "⚠️ Error de datos: El usuario tiene ID 0.";
                                return View("Login");
                            }

                            // 3. ÉXITO: GUARDAMOS EN SESIÓN
                            HttpContext.Session.SetString("IdUsuarioLogueado", votante.Id.ToString());
                            HttpContext.Session.SetString("EmailUsuario", votante.Correo ?? "");

                            return RedirectToAction("Index", "Votacion");
                        }
                    }
                }

                ViewBag.Error = "❌ Token no encontrado. Verifique que esté bien escrito.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error de conexión: {ex.Message}";
                return View("Login");
            }
        }
    }
}