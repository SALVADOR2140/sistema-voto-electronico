using System.Net;
using System.Net.Mail;

namespace SistemaVotoElectronico.Api.Servicios
{
    public interface IEmailService
    {
        Task<bool> EnviarToken(string correoDestino, string nombre, string token);
        Task<bool> EnviarCertificado(string correoDestino, string nombreVotante, string nombreEvento);
    }

    public class EmailService : IEmailService
    {
        // Credenciales oficiales de tu cuenta de Google
        private readonly string _remitente = "nahininba@gmail.com";
        private readonly string _passwordApp = "ogngmjfwiowuajhu";

        // MÉTODO 1: ENVIAR TOKEN CON GMAIL SMTP
        public async Task<bool> EnviarToken(string correoDestino, string nombre, string token)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // Puerto seguro habilitado en Render
                    Credentials = new NetworkCredential(_remitente, _passwordApp),
                    EnableSsl = true,
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_remitente, "VOTO SEGURO UTN"),
                    Subject = "🔐 Tu Token de Seguridad - UTN",
                    Body = $@"
                        <div style='font-family: sans-serif; border-top: 4px solid #0d6efd; padding: 20px;'>
                            <h2 style='color: #0d6efd;'>VOTO SEGURO UTN</h2>
                            <p>Hola <b>{nombre}</b>, tu token de acceso único es:</p>
                            <h1 style='background: #f8f9fa; border: 1px solid #ddd; padding: 15px; text-align: center; letter-spacing: 5px;'>{token}</h1>
                            <p>Usa este código en la urna electrónica.</p>
                            <small style='color: gray;'>Este es un proceso automático, por favor no responda este correo.</small>
                        </div>",
                    IsBodyHtml = true,
                };

                mensaje.To.Add(correoDestino);
                await smtpClient.SendMailAsync(mensaje);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GMAIL TOKEN]: {ex.Message}");
                return false;
            }
        }

        // MÉTODO 2: ENVIAR CERTIFICADO CON GMAIL SMTP
        public async Task<bool> EnviarCertificado(string correoDestino, string nombreVotante, string nombreEvento)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_remitente, _passwordApp),
                    EnableSsl = true,
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_remitente, "VOTO SEGURO UTN"),
                    Subject = "🗳️ Certificado de Votación - UTN",
                    Body = $@"
                        <div style='font-family: sans-serif; border: 2px solid #0d6efd; padding: 30px; text-align: center;'>
                            <h2 style='color: #0d6efd;'>CERTIFICADO DE SUFRAGIO</h2>
                            <p>El sistema <b>VOTO SEGURO UTN</b> certifica que:</p>
                            <h3>{nombreVotante}</h3>
                            <p>Ha votado con éxito en el evento: <b>{nombreEvento}</b></p>
                            <hr>
                            <small>Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}</small>
                        </div>",
                    IsBodyHtml = true,
                };

                mensaje.To.Add(correoDestino);
                await smtpClient.SendMailAsync(mensaje);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR GMAIL CERTIFICADO]: {ex.Message}");
                return false;
            }
        }
    }
}