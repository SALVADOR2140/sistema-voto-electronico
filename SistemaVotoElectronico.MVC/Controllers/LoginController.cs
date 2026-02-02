using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class LoginController : Controller
    {

        //AGREGAR ESTO: Declaración del contexto
        private readonly SistemaVotoElectronicoApiContext _context;

        //AGREGAR ESTO: Constructor que recibe el contexto
        public LoginController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: Muestra la pantalla de login
        public IActionResult Index()
        {
            return View();
        }

        // POST: Recibe los datos y valida
        [HttpPost]
        public IActionResult Entrar(string usuario, string clave)
        {
            // Buscamos en la base de datos usando el contexto
            var user = _context.Usuarios.FirstOrDefault(u => u.Cedula == usuario && u.Clave == clave);

            if (user != null)
            {
                HttpContext.Session.SetString("UsuarioLogueado", user.Nombres);
                return RedirectToAction("Index", "Home");
            }
            else
            {
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