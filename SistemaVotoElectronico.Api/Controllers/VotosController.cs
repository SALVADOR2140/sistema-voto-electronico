using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public VotosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        public class IntencionVoto
        {
            public int UsuarioId { get; set; }
            public int EventoId { get; set; }
            public int CandidatoId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> EmitirVoto([FromBody] IntencionVoto datos)
        {
            // LOG PARA DEBUG API
            Console.WriteLine($"[API DEBUG] Recibiendo voto -> UsuarioID: {datos.UsuarioId}, Evento: {datos.EventoId}");

            if (datos.UsuarioId <= 0)
                return BadRequest("ERROR: El ID del usuario es 0 o inválido.");

            // 1. BUSCAR USUARIO
            var usuario = await _context.Usuarios.FindAsync(datos.UsuarioId);

            if (usuario == null)
            {
                Console.WriteLine("[API ERROR] Usuario no encontrado en BD.");
                return NotFound("Usuario no existe en la base de datos.");
            }

            // 2. VERIFICAR SI YA VOTÓ
            if (usuario.YaVoto)
            {
                Console.WriteLine($"[API ALERTA] El usuario {usuario.Id} intentó votar de nuevo.");
                return BadRequest("El usuario YA TIENE un voto registrado.");
            }

            // 3. BUSCAR CANDIDATO
            var candidato = await _context.Candidatos.FindAsync(datos.CandidatoId);
            if (candidato == null) return BadRequest("Candidato no existe.");

            try
            {
                // A. CREAR EL VOTO
                var voto = new Voto
                {
                    Fecha = DateTime.Now,
                    EventoElectoralId = datos.EventoId,
                    ListaPoliticaId = candidato.ListaPoliticaId,
                    HashSeguridad = Guid.NewGuid().ToString()
                };
                _context.Votos.Add(voto);

                // B. ACTUALIZAR RESULTADOS (Contador)
                var resultado = await _context.ResultadosElecciones
                    .FirstOrDefaultAsync(r => r.EventoElectoralId == datos.EventoId && r.ListaPoliticaId == candidato.ListaPoliticaId);

                if (resultado == null)
                {
                    _context.ResultadosElecciones.Add(new ResultadoEleccion
                    {
                        EventoElectoralId = datos.EventoId,
                        ListaPoliticaId = candidato.ListaPoliticaId,
                        TotalVotos = 1
                    });
                }
                else
                {
                    resultado.TotalVotos++;
                }

                // C. QUEMAR TOKEN
                _context.Usuarios.Attach(usuario);

                usuario.YaVoto = true;
                usuario.TokenVotacion = null;
                _context.Entry(usuario).Property(u => u.YaVoto).IsModified = true;
                _context.Entry(usuario).Property(u => u.TokenVotacion).IsModified = true;

                // D. GUARDAR TODO
                await _context.SaveChangesAsync();

                Console.WriteLine("[API SUCCESS] Voto guardado y Token quemado.");
                return Ok(new { mensaje = "Voto Exitoso" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API CRASH] {ex.Message}");
                return StatusCode(500, $"Error Interno: {ex.Message} - {ex.InnerException?.Message}");
            }
        }
    }
}