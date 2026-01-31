using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventosElectoralesController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public EventosElectoralesController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/EventosElectorales
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<EventoElectoral>>>> GetEventosElectorales()
        {
            var lista = await _context.EventosElectorales.ToListAsync();

            return Ok(ApiResult<List<EventoElectoral>>.Ok(lista));
        }

        // GET: api/EventosElectorales/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<EventoElectoral>>> GetEventoElectoral(int id)
        {
            var eventoElectoral = await _context.EventosElectorales.FindAsync(id);

            if (eventoElectoral == null)
            {
                return NotFound(ApiResult<EventoElectoral>.Fail("No encontrado"));
            }

            return Ok(ApiResult<EventoElectoral>.Ok(eventoElectoral));
        }

        // PUT: api/EventosElectorales/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEventoElectoral(int id, EventoElectoral eventoElectoral)
        {
            if (id != eventoElectoral.Id)
            {
                return BadRequest();
            }

            _context.Entry(eventoElectoral).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoElectoralExists(id))
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

        // POST: api/EventosElectorales
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApiResult<EventoElectoral>>> PostEventoElectoral(EventoElectoral eventoElectoral)
        {
            _context.EventosElectorales.Add(eventoElectoral);
            await _context.SaveChangesAsync();

            return Ok(ApiResult<EventoElectoral>.Ok(eventoElectoral));
        }

        // DELETE: api/EventosElectorales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventoElectoral(int id)
        {
            var eventoElectoral = await _context.EventosElectorales.FindAsync(id);
            if (eventoElectoral == null)
            {
                return NotFound();
            }

            _context.EventosElectorales.Remove(eventoElectoral);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventoElectoralExists(int id)
        {
            return _context.EventosElectorales.Any(e => e.Id == id);
        }
    }
}
