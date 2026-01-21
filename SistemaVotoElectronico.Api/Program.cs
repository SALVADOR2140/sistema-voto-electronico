using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaVotoElectronico.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SistemaVotoElectronicoApiContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("SistemaVotoElectronicoApiContext") ?? throw new InvalidOperationException("Connection string 'SistemaVotoElectronicoApiContext' not found.")));
            options.UseNpgsql(builder.Configuration.GetConnectionString("SistemaVotoElectronicoApiContext.postgresql") ?? throw new InvalidOperationException("Connection string 'SistemaVotoElectronicoApiContext' not found.")));


            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
