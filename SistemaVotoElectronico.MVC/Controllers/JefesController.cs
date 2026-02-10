using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class JefesController : Controller
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public JefesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Seguridad: Solo Admin entra
            if (HttpContext.Session.GetInt32("RolUsuarioId") != 1) return RedirectToAction("Index", "Login");

            var jefes = await _context.Usuarios
                .Where(u => u.RolUsuarioId == 2) 
                .Include(u => u.RolUsuario)
                .ToListAsync();
            return View(jefes);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario usuario)
        {

            usuario.RolUsuarioId = 2;
            usuario.YaVoto = false;
            usuario.TokenVotacion = null;


            ModelState.Remove("RolUsuario");
            ModelState.Remove("TokenVotacion");

            if (ModelState.IsValid || true) 
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.Id) return NotFound();


            usuario.RolUsuarioId = 2;

            if (ModelState.IsValid || true)
            {
                _context.Update(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Jefes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.RolUsuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Jefes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}