using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public AuthController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        public class LoginDto
        {
            public string Correo { get; set; }
            public string Clave { get; set; }
        }

        [HttpPost("LoginWeb")]
        public async Task<IActionResult> LoginAdministrativo([FromBody] LoginDto login)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.RolUsuario)
                .FirstOrDefaultAsync(u => u.Correo == login.Correo);

            if (usuario == null) return Unauthorized("Correo incorrecto.");
            if (usuario.Clave != login.Clave) return Unauthorized("Contraseña incorrecta.");

            if (usuario.RolUsuario.NombreRol == "Votante")
            {
                return StatusCode(403, "ACCESO DENEGADO: Los votantes solo pueden ingresar en la Urna Electrónica con su Token.");
            }
      
            return Ok(new
            {
                usuarioId = usuario.Id,
                nombre = usuario.Nombres,
                rol = usuario.RolUsuario.NombreRol,
                tokenSesion = Guid.NewGuid().ToString()
            });
        }


        [HttpPost("LoginUrna")]
        public async Task<IActionResult> LoginUrna([FromBody] string token)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.RolUsuario)
                .FirstOrDefaultAsync(u => u.TokenVotacion == token);

            if (usuario == null) return Unauthorized("Token inválido.");

            if (usuario.RolUsuario.NombreRol != "Votante")
            {
                return BadRequest("El personal administrativo no puede votar en esta urna.");
            }

            if (usuario.YaVoto) return BadRequest("Este token ya fue utilizado.");

            return Ok(new
            {
                usuarioId = usuario.Id,
                nombre = usuario.Nombres,
                mensaje = "Bienvenido a la Urna. Puede proceder."
            });
        }
    }
}