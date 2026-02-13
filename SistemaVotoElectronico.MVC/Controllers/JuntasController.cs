using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVoto.Modelos;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JuntasController : Controller
    {
        private bool EsAdministrador() => HttpContext.Session.GetInt32("RolUsuarioId") == 1;

        // GET: Pantalla de Mesa de Control
        public IActionResult Index()
        {
            if (!EsAdministrador()) return RedirectToAction("Index", "Login");
            return View();
        }

        // POST: Generar Token llamando a la API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarToken(string cedula)
        {
            if (!EsAdministrador()) return RedirectToAction("Index", "Login");

            if (string.IsNullOrEmpty(cedula))
            {
                TempData["Error"] = "Ingrese la cédula.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using (var client = new HttpClient())
                {
                    string urlApi = "http://localhost:5111/api/Junta/GenerarToken";
                    var jsonContent = JsonConvert.SerializeObject(cedula);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlApi, content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic resultado = JsonConvert.DeserializeObject<dynamic>(responseString);

 
                        var usuarioVista = new Usuario
                        {
                            Cedula = cedula,
                            Nombres = resultado.nombre,
                            Correo = resultado.correo
                        };

                        ViewBag.Mensaje = resultado.mensaje;
                        return View("TokenGenerado", usuarioVista);
                    }
                    else
                    {
                        // Manejo de errores de la API
                        dynamic errorObj = JsonConvert.DeserializeObject<dynamic>(responseString);
                        TempData["Error"] = errorObj.mensaje ?? "Error en la operación.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error de conexión con la API.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}