using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVoto.Modelos; 
namespace SistemaVotoElectronico.MVC.Controllers
{
    public class AccesoVotanteController : Controller
    {
        // 1. PANTALLA PARA ESCRIBIR EL TOKEN
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. RECIBE EL TOKEN Y LO VALIDA CON LA API
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
                    // Ajusta el puerto si tu API no está en el 5111
                    string urlApi = "http://localhost:5111/api/Usuarios";

                    var response = await client.GetAsync(urlApi);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var listaUsuarios = JsonConvert.DeserializeObject<List<Usuario>>(json);

                        // BUSCAMOS AL DUEÑO DEL TOKEN
                        var votante = listaUsuarios.FirstOrDefault(u => u.TokenVotacion == tokenIngresado.Trim());

                        if (votante != null)
                        {
                            // VALIDACIÓN: ¿Ya votó?
                            if (votante.YaVoto)
                            {
                                ViewBag.Error = "⛔ Este token ya fue utilizado. No puede volver a votar.";
                                return View("Login");
                            }

                            // ¡GUARDAMOS EL TOKEN EN LA MEMORIA (SESIÓN)
                            HttpContext.Session.SetString("TokenVotante", votante.TokenVotacion);

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