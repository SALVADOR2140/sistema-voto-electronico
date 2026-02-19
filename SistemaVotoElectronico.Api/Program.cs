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

            var connectionString = builder.Configuration.GetConnectionString("CadenaPostgres");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("No se encontró la CadenaPostgres en appsettings.json");
            }

            builder.Services.AddDbContext<SistemaVotoElectronicoApiContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            );

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            builder.Services.AddScoped<SistemaVotoElectronico.Api.Servicios.IEmailService, SistemaVotoElectronico.Api.Servicios.EmailService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirTodo", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Registrar automáticamente las peticiones/respuestas en Serilog
            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
                // Muestra detalles de excepciones cuando estás en Development
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseCors("PermitirTodo");
            app.UseSwaggerUI();

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}