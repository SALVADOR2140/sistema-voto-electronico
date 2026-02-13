using System.Net;
using System.Net.Mail;

namespace SistemaVotoElectronico.Api.Servicios
{
    public interface IEmailService
    {
        Task<bool> EnviarToken(string correoDestino, string nombre, string token);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> EnviarToken(string correoDestino, string nombre, string token)
        {
            try
            {
                var emailOrigen = _config["ConfiguracionCorreo:EmailOrigen"];
                var password = _config["ConfiguracionCorreo:PasswordAplicacion"];
                var host = _config["ConfiguracionCorreo:SmtpHost"];
                var port = int.Parse(_config["ConfiguracionCorreo:SmtpPort"]);

                var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(emailOrigen, password),
                    EnableSsl = true,
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(emailOrigen, "Sistema Voto U"),
                    Subject = "🔐 Tu Token de Votación Electrónica",
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd;'>
                            <h2 style='color: #0d6efd;'>Hola, {nombre}</h2>
                            <p>Has sido habilitado para votar en las elecciones universitarias.</p>
                            <p>Tu token de acceso único es:</p>
                            <h1 style='background: #f8f9fa; padding: 10px; display: inline-block; letter-spacing: 5px; border-radius: 5px;'>{token}</h1>
                            <p>Ingresa este código en la urna electrónica.</p>
                            <hr>
                            <small>Junta Electoral Universitaria</small>
                        </div>",
                    IsBodyHtml = true,
                };

                mensaje.To.Add(correoDestino);

                await smtpClient.SendMailAsync(mensaje);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR EMAIL] {ex.Message}");
                return false;
            }
        }
    }
}