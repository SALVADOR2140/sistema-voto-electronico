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
    public class ListasPoliticasController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public ListasPoliticasController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/ListasPoliticas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListaPolitica>>> GetListaPolitica()
        {
            return await _context.ListasPoliticas.ToListAsync();
        }

        // GET: api/ListasPoliticas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ListaPolitica>> GetListaPolitica(int id)
        {
            var listaPolitica = await _context.ListasPoliticas.FindAsync(id);

            if (listaPolitica == null)
            {
                return NotFound();
            }

            return listaPolitica;
        }

        // PUT: api/ListasPoliticas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListaPolitica(int id, ListaPolitica listaPolitica)
        {
            if (id != listaPolitica.Id)
            {
                return BadRequest();
            }

            _context.Entry(listaPolitica).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ListaPoliticaExists(id))
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

        // POST: api/ListasPoliticas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ListaPolitica>> PostListaPolitica(ListaPolitica listaPolitica)
        {
            _context.ListasPoliticas.Add(listaPolitica);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetListaPolitica", new { id = listaPolitica.Id }, listaPolitica);
        }

        // DELETE: api/ListasPoliticas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListaPolitica(int id)
        {
            var listaPolitica = await _context.ListasPoliticas.FindAsync(id);
            if (listaPolitica == null)
            {
                return NotFound();
            }

            _context.ListasPoliticas.Remove(listaPolitica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ListaPoliticaExists(int id)
        {
            return _context.ListasPoliticas.Any(e => e.Id == id);
        }
    }
}
