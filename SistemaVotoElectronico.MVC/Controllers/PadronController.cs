using Microsoft.AspNetCore.Mvc;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using SistemaVotoElectronico.MVC.Filtros; 

namespace SistemaVotoElectronico.MVC.Controllers
{
    [VerificarSesion] 
    public class PadronController : Controller
    {
        // 1. Ver lista de ciudadanos registrados
        public IActionResult Index()
        {
            var usuarios = Crud<Usuario>.ReadAll();
            var votantes = usuarios.Data.Where(u => !string.IsNullOrEmpty(u.TokenVotacion)).ToList();
            return View(votantes);
        }

        // 2. Pantalla de Registro
        public IActionResult Create()
        {
            return View();
        }

        // 3. Guardar y Generar Token
        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            usuario.RolUsuarioId = 2; 
            usuario.YaVoto = false;
            usuario.Clave = "VOTANTE-NO-USA-CLAVE"; 

            // Generar Token Único 
            usuario.TokenVotacion = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

            Crud<Usuario>.Create(usuario);

            TempData["Mensaje"] = $"¡Usuario Registrado! ENTREGA ESTE TOKEN: {usuario.TokenVotacion}";

            return RedirectToAction("Index");
        }
    }
}