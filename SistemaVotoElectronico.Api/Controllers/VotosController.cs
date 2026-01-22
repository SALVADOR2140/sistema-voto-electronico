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
            public string Token { get; set; }
            public int EventoId { get; set; }
            public int? ListaId { get; set; }
        }

        [HttpPost("Emitir")]
        public async Task<IActionResult> EmitirVoto([FromBody] IntencionVoto datos)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TokenVotacion == datos.Token);

            if (usuario == null) return Unauthorized("Token no válido.");
            if (usuario.YaVoto) return BadRequest("El usuario ya registró su voto.");

            var evento = await _context.EventosElectorales.FindAsync(datos.EventoId);
            if (evento == null || !evento.Activo) return BadRequest("El evento electoral no está activo.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var voto = new Voto
                {
                    Fecha = DateTime.UtcNow,
                    EventoElectoralId = datos.EventoId,
                    ListaPoliticaId = datos.ListaId,
                    HashSeguridad = Guid.NewGuid().ToString()
                };
                _context.Votos.Add(voto);

                var certificado = new Certificado
                {
                    UsuarioId = usuario.Id,
                    EventoElectoralId = datos.EventoId,
                    FechaEmision = DateTime.UtcNow,
                    CodigoQR = Guid.NewGuid().ToString()
                };
                _context.Certificados.Add(certificado);

                usuario.YaVoto = true;
                usuario.TokenVotacion = null;
                _context.Entry(usuario).State = EntityState.Modified;

                if (datos.ListaId.HasValue)
                {
                    var resultado = await _context.ResultadosElecciones
                        .FirstOrDefaultAsync(r => r.EventoElectoralId == datos.EventoId && r.ListaPoliticaId == datos.ListaId);

                    if (resultado == null)
                    {
                        resultado = new ResultadoEleccion
                        {
                            EventoElectoralId = datos.EventoId,
                            ListaPoliticaId = datos.ListaId.Value,
                            TotalVotos = 1
                        };
                        _context.ResultadosElecciones.Add(resultado);
                    }
                    else
                    {
                        resultado.TotalVotos++;
                        _context.Entry(resultado).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Voto registrado exitosamente." });
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al procesar la solicitud.");
            }
        }
    }
}