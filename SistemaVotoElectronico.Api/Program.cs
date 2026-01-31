using Microsoft.EntityFrameworkCore;
using Serilog;
using Newtonsoft.Json;

namespace SistemaVotoElectronico.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            var builder = WebApplication.CreateBuilder(args);

            // 1. Configurar Serilog (Simple)
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            builder.Host.UseSerilog();

            // 2. CONEXIÓN A BASE DE DATOS (Usando el nombre exacto del JSON)
            var connectionString = builder.Configuration.GetConnectionString("CadenaPostgres");

            builder.Services.AddDbContext<SistemaVotoElectronicoApiContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            // 3. Configuración básica de controladores
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Licencia PDF (Opcional, para que no de error si lo tienes)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var app = builder.Build();

            // 4. Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}