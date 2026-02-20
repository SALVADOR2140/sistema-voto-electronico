using Microsoft.EntityFrameworkCore;
using Serilog;
using Newtonsoft.Json;
using Resend; // 1. Agregamos el using de Resend

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


            // CONFIGURACIÓN DE RESEND (API DE CORREOS)

            builder.Services.AddOptions();
            builder.Services.AddHttpClient<IResend, ResendClient>();
            builder.Services.Configure<ResendClientOptions>(options =>
            {
                // Aplicamos tu llave maestra de VOTO_SEGURO_UTN
                options.ApiToken = "re_Dg3nFKtP_7phjtsEFowsyxg1EpG5sGzUF";
            });

            // Registrar el servicio de Email
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

            app.UseSerilogRequestLogging();

            if (app.Environment.IsDevelopment())
            {
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