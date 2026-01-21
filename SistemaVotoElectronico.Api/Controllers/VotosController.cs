using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.Api.Controllers;


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

        // DTO: Clase auxiliar para recibir solo lo necesario
        public class IntencionVoto
        {
            public int UsuarioId { get; set; }
            public int EventoId { get; set; }
            public int? ListaId { get; set; }
        }

        // POST: api/Votos/Emitir
        [HttpPost("Emitir")]
        public async Task<IActionResult> EmitirVoto([FromBody] IntencionVoto datos)
        {
            var usuario = await _context.Usuarios.FindAsync(datos.UsuarioId);
            if (usuario == null) return BadRequest("Usuario no encontrado.");
            if (usuario.YaVoto) return BadRequest("Este usuario ya sufragó.");

            var evento = await _context.EventosElectorales.FindAsync(datos.EventoId);
            if (evento == null || !evento.Activo) return BadRequest("La elección no está activa.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var voto = new Voto
                {
                    Fecha = DateTime.Now,

                    EventoElectoralId = datos.EventoId,

                    ListaPoliticaId = datos.ListaId,

                    HashSeguridad = Guid.NewGuid().ToString()
                };

                var certificado = new Certificado
                {
                    UsuarioId = datos.UsuarioId,
                    EventoElectoralId = datos.EventoId,
                    FechaEmision = DateTime.Now,
                    CodigoQR = Guid.NewGuid().ToString() 
                };
                _context.Certificados.Add(certificado);

                usuario.YaVoto = true;
                _context.Entry(usuario).State = EntityState.Modified;

 
                if (datos.ListaId.HasValue)
                {
                    var resultado = await _context.ResultadosElecciones
                        .FirstOrDefaultAsync(r => r.EventoElectoralId == datos.EventoId
                                               && r.ListaPoliticaId == datos.ListaId.Value);

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

                return Ok(new
                {
                    mensaje = "Voto registrado con éxito",
                    codigoCertificado = certificado.CodigoQR
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, "ERROR CRÍTICO: " + errorReal);
            }
        }
    }
}