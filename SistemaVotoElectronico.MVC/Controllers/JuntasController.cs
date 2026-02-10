using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JuntasController : Controller
    {
        private readonly SistemaVotoElectronicoApiContext _context;


        public JuntasController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        private bool EsJefeDeJunta() => HttpContext.Session.GetInt32("RolUsuarioId") == 2;

        // GET: Juntas
        public IActionResult Index()
        {
            if (!EsJefeDeJunta()) return RedirectToAction("Index", "Login"); // Seguridad
            return View();
        }


        // POST: Juntas/Create Tokens 
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerarToken(string cedula)
        {
            // Agregamos seguridad también al procesar el formulario
            if (!EsJefeDeJunta()) return RedirectToAction("Index", "Login");

            if (string.IsNullOrEmpty(cedula))
            {
                TempData["Error"] = "Por favor, ingrese un número de cédula.";
                return RedirectToAction(nameof(Index));
            }

            // Buscamos solo usuarios que sean Votantes (Rol 2)
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Cedula == cedula && u.RolUsuarioId == 3);

            if (usuario == null)
            {
                TempData["Error"] = "Votante no encontrado en el padrón electoral.";
                return RedirectToAction(nameof(Index));
            }

            if (usuario.YaVoto)
            {
                TempData["Error"] = "¡Atención! Este estudiante ya ejerció su derecho al voto.";
                return RedirectToAction(nameof(Index));
            }

            // Generamos el Token de 6 caracteres único
            usuario.TokenVotacion = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return View("TokenGenerado", usuario);
        }

    }
}
