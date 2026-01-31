using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using System;

namespace SistemaVotoElectronico.ApiTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Crud<EventoElectoral>.UrlBase = "http://127.0.0.1:5111/api/EventosElectorales";

            // CREATE (Insertar)
            var nuevoEvento = new EventoElectoral
            {
                Id = 0,
                Nombre = "Eleccion Test Final",
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(1),
                Activo = true
            };

            var apiResult = Crud<EventoElectoral>.Create(nuevoEvento);

            // READ ALL (Leer todo)
            var eventos = Crud<EventoElectoral>.ReadAll();

            if (apiResult != null && apiResult.Data != null)
            {
                nuevoEvento = apiResult.Data;

                // UPDATE (Modificar)
                nuevoEvento.Nombre = "Eleccion Test MODIFICADA";
                Crud<EventoElectoral>.Update(nuevoEvento.Id.ToString(), nuevoEvento);

                // READ BY (Leer por ID)
                var unEvento = Crud<EventoElectoral>.ReadBy(nuevoEvento.Id.ToString());

                // DELETE (Eliminar)
                Crud<EventoElectoral>.Delete(nuevoEvento.Id.ToString());
            }

            Console.WriteLine(apiResult);
            Console.ReadLine();
        }
    }
}