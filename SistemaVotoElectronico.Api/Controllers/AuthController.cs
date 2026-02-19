using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(SistemaVotoElectronicoApiContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public class LoginDto
        {
            public string Correo { get; set; }
            public string Clave { get; set; }
        }

        [HttpPost("LoginWeb")]
        public async Task<IActionResult> LoginAdministrativo([FromBody] LoginDto login)
        {
            try
            {
                if (login == null) return BadRequest("Payload inválido.");

                var identifier = (login.Correo ?? string.Empty).Trim();
                var password = (login.Clave ?? string.Empty).Trim();

                _logger.LogInformation("LoginWeb intento: identifier='{identifier}', passwordLength={len}", identifier, password.Length);

                var usuario = await _context.Usuarios
                    .Include(u => u.RolUsuario)
                    .FirstOrDefaultAsync(u =>
                        (u.Cedula != null && u.Cedula.Trim() == identifier) ||
                        (u.Correo != null && u.Correo.Trim().ToLower() == identifier.ToLower()));

                if (usuario == null && identifier.All(char.IsDigit))
                {
                    usuario = _context.Usuarios
                        .Include(u => u.RolUsuario)
                        .AsEnumerable()
                        .FirstOrDefault(u =>
                            !string.IsNullOrEmpty(u.Cedula) &&
                            new string(u.Cedula.Where(char.IsDigit).ToArray()) == identifier);
                }

                if (usuario == null)
                {
                    _logger.LogInformation("LoginWeb: usuario no encontrado para identifier='{identifier}'", identifier);
                    return Unauthorized("Usuario no encontrado.");
                }

                if (string.IsNullOrEmpty(usuario.Clave) || usuario.Clave.Trim() != password)
                {
                    _logger.LogWarning("LoginWeb: contraseña incorrecta para usuario Id={id}", usuario.Id);
                    return Unauthorized("Contraseña incorrecta.");
                }

                if (usuario.RolUsuario?.NombreRol?.Trim()?.Equals("Votante", System.StringComparison.OrdinalIgnoreCase) == true)
                {
                    _logger.LogWarning("LoginWeb: acceso denegado (Votante) usuario Id={id}", usuario.Id);
                    return StatusCode(403, "ACCESO DENEGADO: Los votantes ingresan por la Urna.");
                }

                _logger.LogInformation("LoginWeb: login exitoso usuario Id={id}", usuario.Id);

                return Ok(new
                {
                    usuarioId = usuario.Id,
                    nombre = usuario.Nombres,
                    rol = usuario.RolUsuario?.NombreRol,
                    tokenSesion = System.Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                // Log detallado para diagnosticar el 500
                _logger.LogError(ex, "Excepción en LoginWeb");
                // En producción evita devolver stacktrace; aquí devolvemos mensaje genérico
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        [HttpPost("LoginUrna")]
        public async Task<IActionResult> LoginUrna([FromBody] string token)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.RolUsuario)
                .FirstOrDefaultAsync(u => u.TokenVotacion == token);

            if (usuario == null) return Unauthorized("Token inválido.");

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