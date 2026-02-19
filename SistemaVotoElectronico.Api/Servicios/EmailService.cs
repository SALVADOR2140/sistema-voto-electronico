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
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        // MÉTODO 1: ENVIAR TOKEN (MODO DEMO RENDER)
   
        public async Task<bool> EnviarToken(string correoDestino, string nombre, string token)
        {
            try
            {
                // SIMULAMOS EL ENVÍO PARA EVITAR EL BLOQUEO DEL FIREWALL DE RENDER
                Console.WriteLine($"[MODO DEMO] Simulando envío de Token a: {correoDestino}. Token: {token}");

                // Esperamos medio segundo para simular el proceso de red
                await Task.Delay(500);

                // Retornamos true inmediatamente para que el MVC reciba el OK y muestre la pantalla verde
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR EMAIL TOKEN] {ex.Message}");
                return false;
            }
        }


        // MÉTODO 2: ENVIAR CERTIFICADO (MODO DEMO RENDER)

        public async Task<bool> EnviarCertificado(string correoDestino, string nombreVotante, string nombreEvento)
        {
            try
            {
                // SIMULAMOS EL ENVÍO PARA EVITAR EL BLOQUEO DEL FIREWALL DE RENDER
                Console.WriteLine($"[MODO DEMO] Simulando envío de Certificado a: {correoDestino}. Evento: {nombreEvento}");

                await Task.Delay(500);

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