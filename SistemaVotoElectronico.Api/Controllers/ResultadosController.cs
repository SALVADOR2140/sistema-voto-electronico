using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public ResultadosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Resultados/EnVivo/{eventoId}
        [HttpGet("EnVivo/{eventoId}")]
        public async Task<IActionResult> ObtenerResultados(int eventoId)
        {
            var evento = await _context.EventosElectorales.FindAsync(eventoId);
            if (evento == null) return NotFound("Elección no encontrada.");

            var resultados = await _context.ResultadosElecciones
                .Include(r => r.ListaPolitica) 
                .Where(r => r.EventoElectoralId == eventoId)
                .Select(r => new
                {
                    Partido = r.ListaPolitica.Nombre,
                    Color = "#FF5733",
                    Logo = r.ListaPolitica.LogoUrl,
                    Votos = r.TotalVotos
                })
                .OrderByDescending(r => r.Votos) 
                .ToListAsync();

            int totalVotos = resultados.Sum(r => r.Votos);

            return Ok(new
            {
                Evento = evento.Nombre,
                TotalVotosEmitidos = totalVotos,
                Detalle = resultados
            });
        }
    }
}