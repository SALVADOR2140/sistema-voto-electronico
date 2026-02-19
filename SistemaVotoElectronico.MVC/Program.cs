using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using Microsoft.EntityFrameworkCore;
using SistemaVotoElectronico.Api.Servicios;
using Microsoft.AspNetCore.Authentication.Cookies; 

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
            Crud<ListaPolitica>.UrlBase = "http://localhost:5111/api/ListasPoliticas";

            var builder = WebApplication.CreateBuilder(args);

            // 1. REGISTRAR EL CONTEXTO DE LA BASE DE DATOS
            builder.Services.AddDbContext<SistemaVotoElectronicoApiContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("CadenaPostgres")));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = "/AccesoVotante/Login"; 
                    option.ExpireTimeSpan = TimeSpan.FromMinutes(20); // Tiempo de sesión
                    option.AccessDeniedPath = "/Inicio/Index";
                });

            // 2. CONFIGURACIÓN DE SESIONES
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            // Registro del servicio de Correo
            builder.Services.AddTransient<IEmailService, EmailService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication(); 

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Inicio}/{action=Index}/{id?}");

            app.Run();
        }
    }
}