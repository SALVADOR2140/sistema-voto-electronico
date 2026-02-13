using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class LoginController : Controller
    {
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
                    string urlApi = "http://localhost:5111/api/Auth/LoginWeb";
                    var loginDto = new { Correo = usuario, Clave = clave };
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
            catch
            {
                ViewBag.Error = "Error de conexión con el servidor.";
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