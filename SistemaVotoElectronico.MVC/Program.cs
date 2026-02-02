using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;

namespace SistemaVotoElectronico.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Crud<EventoElectoral>.UrlBase = "http://localhost:5111/api/EventosElectorales";
            Crud<Candidato>.UrlBase = "http://localhost:5111/api/Candidatos";
            Crud<Voto>.UrlBase = "http://localhost:5111/api/Votos"; 
            Crud<Usuario>.UrlBase = "http://localhost:5111/api/Usuarios"; 

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); 
            });

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); 

            app.UseRouting();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Inicio}/{action=Index}/{id?}"); // Ahora arranca en la portada

            app.Run();
        }
    }
}