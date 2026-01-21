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
    public class ResultadosEleccionesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public ResultadosEleccionesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/ResultadosElecciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ResultadoEleccion>>> GetResultadoEleccion()
        {
            return await _context.ResultadoEleccion.ToListAsync();
        }

        // GET: api/ResultadosElecciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultadoEleccion>> GetResultadoEleccion(int id)
        {
            var resultadoEleccion = await _context.ResultadoEleccion.FindAsync(id);

            if (resultadoEleccion == null)
            {
                return NotFound();
            }

            return resultadoEleccion;
        }

        // PUT: api/ResultadosElecciones/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResultadoEleccion(int id, ResultadoEleccion resultadoEleccion)
        {
            if (id != resultadoEleccion.Id)
            {
                return BadRequest();
            }

            _context.Entry(resultadoEleccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultadoEleccionExists(id))
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

        // POST: api/ResultadosElecciones
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ResultadoEleccion>> PostResultadoEleccion(ResultadoEleccion resultadoEleccion)
        {
            _context.ResultadoEleccion.Add(resultadoEleccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResultadoEleccion", new { id = resultadoEleccion.Id }, resultadoEleccion);
        }

        // DELETE: api/ResultadosElecciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResultadoEleccion(int id)
        {
            var resultadoEleccion = await _context.ResultadoEleccion.FindAsync(id);
            if (resultadoEleccion == null)
            {
                return NotFound();
            }

            _context.ResultadoEleccion.Remove(resultadoEleccion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultadoEleccionExists(int id)
        {
            return _context.ResultadoEleccion.Any(e => e.Id == id);
        }
    }
}
