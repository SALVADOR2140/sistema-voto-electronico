using Microsoft.AspNetCore.Mvc;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class InicioController : Controller
    {
        public IActionResult Index()
        {
            return View(); // Esto mostrará tu nueva portada
        }
    }
}