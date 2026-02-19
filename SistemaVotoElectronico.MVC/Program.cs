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
            Crud<EventoElectoral>.UrlBase = "https://sistema-voto-electronico-z3q0.onrender.com/api/EventosElectorales";
            Crud<Candidato>.UrlBase = "https://sistema-voto-electronico-z3q0.onrender.com/api/Candidatos";
            Crud<Voto>.UrlBase = "https://sistema-voto-electronico-z3q0.onrender.com/api/Votos";
            Crud<Usuario>.UrlBase = "https://sistema-voto-electronico-z3q0.onrender.com/api/Usuarios";
            Crud<ListaPolitica>.UrlBase = "https://sistema-voto-electronico-z3q0.onrender.com/api/ListasPoliticas";

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
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
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