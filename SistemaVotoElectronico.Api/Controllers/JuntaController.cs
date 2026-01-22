using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuntaController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public JuntaController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // POST: api/Junta/GenerarToken
        [HttpPost("GenerarToken")]
        public async Task<IActionResult> AutorizarVotante([FromBody] string cedula)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Cedula == cedula);

            if (usuario == null) return NotFound("Estudiante no encontrado.");
            if (usuario.YaVoto) return BadRequest("Este estudiante YA votó.");

            // Generar Token
            string token = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();

            // Guardar Token
            usuario.TokenVotacion = token;
            _context.Entry(usuario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Usuario autorizado",
                nombre = usuario.Nombres,
                tokenParaVotar = token
            });
        }
    }
}
