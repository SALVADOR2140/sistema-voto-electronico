using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using Microsoft.EntityFrameworkCore; 

namespace SistemaVotoElectronico.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configuración de URLs de la API
            Crud<EventoElectoral>.UrlBase = "http://localhost:5111/api/EventosElectorales";
            Crud<Candidato>.UrlBase = "http://localhost:5111/api/Candidatos";
            Crud<Voto>.UrlBase = "http://localhost:5111/api/Votos";
            Crud<Usuario>.UrlBase = "http://localhost:5111/api/Usuarios";

            var builder = WebApplication.CreateBuilder(args);


            // 1. REGISTRAR EL CONTEXTO DE LA BASE DE DATOS (Solución al Error)

            builder.Services.AddDbContext<SistemaVotoElectronicoApiContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("CadenaPostgres")));


            // 2. CONFIGURACIÓN DE SESIONES

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2); // 2 horas para mayor estabilidad en la defensa
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            // 3. MIDDLEWARES (Orden importante)

            app.UseSession(); 
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Inicio}/{action=Index}/{id?}");

            app.Run();
        }
    }
}