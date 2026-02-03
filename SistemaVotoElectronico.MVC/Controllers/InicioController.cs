using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            // Verificamos si hay alguien logueado revisando su Rol
            var rol = HttpContext.Session.GetInt32("RolUsuarioId");

            if (rol != null)
            {
                // Si es Administrador (Rol 1), lo mandamos al Dashboard
                if (rol == 1)
                {
                    return RedirectToAction("Index", "Home");
                }

                // Si es Jefe de Junta (Rol 3), lo mandamos directo a su buscador centrado
                if (rol == 3)
                {
                    return RedirectToAction("Index", "Juntas");
                }
            }

            // Si no hay nadie logueado o es un Votante, muestra la portada general (Botón "IR A VOTAR")
            return View();
        }
    }
}