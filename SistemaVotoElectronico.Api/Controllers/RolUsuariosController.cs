using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolUsuariosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public RolUsuariosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/RolUsuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolUsuario>>> GetRolUsuario()
        {
            return await _context.RolUsuarios.ToListAsync();
        }

        // GET: api/RolUsuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RolUsuario>> GetRolUsuario(int id)
        {
            var rolUsuario = await _context.RolUsuarios.FindAsync(id);

            if (rolUsuario == null)
            {
                return NotFound();
            }

            return rolUsuario;
        }

        // PUT: api/RolUsuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRolUsuario(int id, RolUsuario rolUsuario)
        {
            if (id != rolUsuario.Id)
            {
                return BadRequest();
            }

            _context.Entry(rolUsuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolUsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/RolUsuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RolUsuario>> PostRolUsuario(RolUsuario rolUsuario)
        {
            _context.RolUsuarios.Add(rolUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRolUsuario", new { id = rolUsuario.Id }, rolUsuario);
        }

        // DELETE: api/RolUsuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRolUsuario(int id)
        {
            var rolUsuario = await _context.RolUsuarios.FindAsync(id);
            if (rolUsuario == null)
            {
                return NotFound();
            }

            _context.RolUsuarios.Remove(rolUsuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RolUsuarioExists(int id)
        {
            return _context.RolUsuarios.Any(e => e.Id == id);
        }
    }
}
