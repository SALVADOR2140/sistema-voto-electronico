using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Api.Servicios; 

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuntaController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;
        private readonly IEmailService _emailService;

        public JuntaController(SistemaVotoElectronicoApiContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService; 
        }

        [HttpPost("GenerarToken")]
        public async Task<IActionResult> GenerarToken([FromBody] string cedula)
        {
            // 1. Buscar al usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Cedula == cedula);

            if (usuario == null) return NotFound(new { mensaje = "Usuario no encontrado en el padrón." });

            // 2. Validar si ya votó
            if (usuario.YaVoto)
                return BadRequest(new { mensaje = "⛔ Este usuario YA ejerció su voto. No se puede generar token." });

            // 3. LÓGICA DE TOKEN ÚNICO (Aquí está el cambio)
            string tokenParaEnviar;
            bool esNuevoToken = false;

            if (!string.IsNullOrEmpty(usuario.TokenVotacion))
            {
                // CASO A: Ya tiene token -> Reutilizamos el existente
                tokenParaEnviar = usuario.TokenVotacion;
            }
            else
            {
                // CASO B: No tiene token -> Generamos uno nuevo
                tokenParaEnviar = Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
                usuario.TokenVotacion = tokenParaEnviar;
                esNuevoToken = true;
            }

            // 4. Guardamos en BD solo si hubo cambios (si es nuevo)
            if (esNuevoToken)
            {
                _context.Entry(usuario).State = EntityState.Modified;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    return StatusCode(500, new { mensaje = "Error al guardar el token. Intente de nuevo." });
                }
            }

            // 5. Enviamos el correo 
            bool enviado = await _emailService.EnviarToken(usuario.Correo, usuario.Nombres, tokenParaEnviar);

            if (!enviado)
                return StatusCode(500, new { mensaje = "El token existe, pero falló el envío del correo." });

            // 6. Respondemos al Cliente
            string mensajeRespuesta = esNuevoToken
                ? "✅ Token generado y enviado correctamente."
                : "🔄 El usuario ya tenía un token activo. Se ha REENVIADO el mismo código.";

            return Ok(new
            {
                mensaje = mensajeRespuesta,
                nombre = usuario.Nombres,
                correo = usuario.Correo,
                token = tokenParaEnviar 
            });
        }
    }
}