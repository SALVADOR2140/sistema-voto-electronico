using System.Net;
using System.Net.Mail;

namespace SistemaVotoElectronico.Api.Servicios
{
    // 1. ACTUALIZAMOS LA INTERFAZ
    public interface IEmailService
    {
        Task<bool> EnviarToken(string correoDestino, string nombre, string token);
        Task<bool> EnviarCertificado(string correoDestino, string nombreVotante, string nombreEvento);
    }

    // 2. ACTUALIZAMOS LA CLASE
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // (TOKEN)
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
                Console.WriteLine($"[ERROR EMAIL TOKEN] {ex.Message}");
                return false;
            }
        }

        // (CERTIFICADO)
        public async Task<bool> EnviarCertificado(string correoDestino, string nombreVotante, string nombreEvento)
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

                string fecha = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
                string codigoHash = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // Diseño del Certificado
                string htmlBody = $@"
                <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
                    <div style='max-width: 600px; margin: 0 auto; background: white; border: 10px solid #0d6efd; padding: 40px; text-align: center;'>
                        
                        <h2 style='color: #0d6efd; margin-bottom: 5px;'>UNIVERSIDAD TÉCNICA DEL NORTE</h2>
                        <h4 style='color: #555; margin-top: 0;'>SISTEMA DE VOTO ELECTRÓNICO</h4>
                        <hr style='border: 1px solid #ddd; margin: 20px 0;'>

                        <h1 style='color: #333; text-transform: uppercase; font-size: 24px; letter-spacing: 2px;'>Certificado de Votación</h1>
                        
                        <p style='font-size: 16px; color: #666; margin-top: 30px;'>Por medio del presente se certifica que el/la ciudadano/a:</p>
                        
                        <h2 style='font-size: 28px; color: #000; text-decoration: underline; margin: 10px 0;'>{nombreVotante}</h2>
                        
                        <p style='font-size: 16px; color: #666;'>Ha ejercido su derecho al voto en el proceso electoral:</p>
                        <h3 style='color: #0d6efd;'>{nombreEvento}</h3>

                        <div style='background-color: #f8f9fa; padding: 15px; margin-top: 30px; border-radius: 5px; text-align: left;'>
                            <p style='margin: 5px 0;'><strong>📅 Fecha y Hora:</strong> {fecha}</p>
                            <p style='margin: 5px 0;'><strong>🔑 Código de Verificación:</strong> {codigoHash}</p>
                            <p style='margin: 5px 0;'><strong>✅ Estado:</strong> SUFRAGIO COMPLETADO</p>
                        </div>

                        <div style='margin-top: 40px;'>
                            <p style='font-size: 12px; color: #999; margin-top: 10px;'>Este es un documento generado electrónicamente.<br>No requiere firma física.</p>
                        </div>
                    </div>
                </div>";

                var mensaje = new MailMessage
                {
                    From = new MailAddress(emailOrigen, "Sistema Voto U"),
                    Subject = $"🗳️ Certificado de Votación - {nombreEvento}",
                    Body = htmlBody,
                    IsBodyHtml = true,
                };

                mensaje.To.Add(correoDestino);

                await smtpClient.SendMailAsync(mensaje);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR EMAIL CERTIFICADO] {ex.Message}");
                return false;
            }
        }
    }
}