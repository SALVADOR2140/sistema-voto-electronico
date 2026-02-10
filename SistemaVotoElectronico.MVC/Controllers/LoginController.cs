using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class LoginController : Controller
    {

      
        private readonly SistemaVotoElectronicoApiContext _context;

       
        public LoginController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: Muestra la pantalla de login
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Entrar(string usuario, string clave)
        {
      
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Cedula == usuario && u.Clave == clave);

            if (user != null)
            {
                // 2. Guardamos datos en sesión (para que el sistema sepa quién es)
                HttpContext.Session.SetString("UsuarioLogueado", user.Nombres);
                HttpContext.Session.SetInt32("RolUsuarioId", user.RolUsuarioId);

                // 3. SEMÁFORO DE REDIRECCIÓN (Usando los nombres de TU captura)
                switch (user.RolUsuarioId)
                {
                    case 1: // Administrador
                            // Lo enviamos al Home (que parece ser tu panel principal)
                        return RedirectToAction("Index", "Home");

                    case 2: // Jefe de Junta
                            // CORREGIDO: Usamos "Juntas" porque tu archivo es JuntasController.cs
                        return RedirectToAction("Index", "Juntas");

                    case 3: // Votante
                            // Lo enviamos a votar. (Asumo que VotacionController tiene un Index o Papeleta)
                        return RedirectToAction("Index", "Votacion");

                    default:
                        // Si algo falla, al inicio público
                        return RedirectToAction("Index", "Inicio");
                }
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