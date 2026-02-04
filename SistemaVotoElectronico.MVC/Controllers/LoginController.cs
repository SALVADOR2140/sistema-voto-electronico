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
            // Buscamos al usuario en la base de datos
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Cedula == usuario && u.Clave == clave);

            if (user != null)
            {
                // Guardamos los datos esenciales en la sesión
                HttpContext.Session.SetString("UsuarioLogueado", user.Nombres);
                HttpContext.Session.SetInt32("RolUsuarioId", user.RolUsuarioId);

                // REDIRECCIÓN POR ROL
                if (user.RolUsuarioId == 1) 
                {
                    return RedirectToAction("Index", "Home"); 
                }
                else if (user.RolUsuarioId == 3) 
                {
                    return RedirectToAction("Index", "Juntas"); 
                }

                return RedirectToAction("Index", "Inicio");
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
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "Login");
        }
    }
}