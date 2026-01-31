using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class LoginController : Controller
    {
        // GET: Muestra la pantalla de login
        public IActionResult Index()
        {
            return View();
        }

        // POST: Recibe los datos y valida
        [HttpPost]
        public IActionResult Entrar(string usuario, string clave)
        {
            if (usuario == "admin" && clave == "1234")
            {
                HttpContext.Session.SetString("UsuarioLogueado", usuario);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Error
                ViewBag.Error = "Usuario o contraseña incorrectos.";
                return View("Index");
            }
        }

        // Para cerrar sesión
        public IActionResult Salir()
        {
            HttpContext.Session.Clear(); // Borramos la memoria
            return RedirectToAction("Index");
        }
    }
}