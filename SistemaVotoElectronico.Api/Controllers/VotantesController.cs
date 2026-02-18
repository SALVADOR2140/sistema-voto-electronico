using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Modelos.Votacion;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotantesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VotantesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // Recibe una lista de votantes y los guarda todos de una vez
        [HttpPost("Masivo")]
        public async Task<IActionResult> PostMasivo(List<Votante> votantes)
        {
            if (votantes == null || votantes.Count == 0)
            {
                return BadRequest("La lista de votantes está vacía.");
            }

            try
            {
                // Usamos "Votantes" (plural) como está en tu Contexto
                await _context.Votantes.AddRangeAsync(votantes);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = $"Se registraron {votantes.Count} votantes correctamente." });
            }
            catch (Exception ex)
            {
                // Este error suele salir si hay cédulas repetidas
                return StatusCode(500, $"Error interno: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        // GET: api/Votantes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Votante>>> GetVotante()
        {
            return await _context.Votantes.ToListAsync();
        }

        // GET: api/Votantes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Votante>> GetVotante(int id)
        {
            var votante = await _context.Votantes.FindAsync(id);

            if (votante == null)
            {
                return NotFound();
            }

            return votante;
        }

        // PUT: api/Votantes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVotante(int id, Votante votante)
        {
            if (id != votante.Id)
            {
                return BadRequest();
            }

            _context.Entry(votante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotanteExists(id))
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

        // POST: api/Votantes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Votante>> PostVotante(Votante votante)
        {
            _context.Votantes.Add(votante);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVotante", new { id = votante.Id }, votante);
        }

        // DELETE: api/Votantes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVotante(int id)
        {
            var votante = await _context.Votantes.FindAsync(id);
            if (votante == null)
            {
                return NotFound();
            }

            _context.Votantes.Remove(votante);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VotanteExists(int id)
        {
            return _context.Votantes.Any(e => e.Id == id);
        }
    }
}
