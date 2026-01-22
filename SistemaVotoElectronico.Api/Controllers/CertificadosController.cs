using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace SistemaVotoElectronico.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificadosController : ControllerBase
    {
        private readonly SistemaVotoElectronicoApiContext _context;

        public CertificadosController(SistemaVotoElectronicoApiContext context)
        {
            _context = context;
        }

        // GET: api/Certificados/Descargar/1002003001
        [HttpGet("Descargar/{cedula}")]
        public async Task<IActionResult> DescargarPorCedula(string cedula)
        {
            var certificado = await _context.Certificados
                .Include(c => c.Usuario)
                .Include(c => c.EventoElectoral)
                .FirstOrDefaultAsync(c => c.Usuario.Cedula == cedula);

            if (certificado == null)
                return NotFound("No se encontró un certificado para esta cédula. Asegúrese de haber votado.");

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text($"Certificado de Votación - {certificado.EventoElectoral.Nombre}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Item().Text("Certificamos que el estudiante:");
                            x.Item().Text(certificado.Usuario.Nombres).Bold().FontSize(18).AlignCenter();
                            x.Item().PaddingTop(10).Text($"Cédula: {certificado.Usuario.Cedula}");
                            x.Item().Text($"Ha ejercido su derecho al voto.");
                            x.Item().Text($"Fecha: {certificado.FechaEmision.ToString("dd/MM/yyyy HH:mm")}");


                            x.Item().PaddingTop(40).AlignCenter().Text("___________________________");
                            x.Item().AlignCenter().Text("Junta Electoral Universitaria");
                        });

                    page.Footer().AlignCenter().Text(x => { x.CurrentPageNumber(); });
                });
            });

            byte[] archivoBytes = documento.GeneratePdf();
            return File(archivoBytes, "application/pdf", $"Certificado_{cedula}.pdf");
        }
    }
}