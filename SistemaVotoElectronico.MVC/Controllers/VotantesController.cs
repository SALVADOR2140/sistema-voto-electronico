using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // Para convertir la lista a JSON
using System.Text;
using ClosedXML.Excel; // La librería que instalaste
using SistemaVotoElectronico.Modelos.Votacion; // Tu modelo Votante

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotantesController : Controller
    {
        private readonly HttpClient _httpClient;

      
        // Cuando ejecutas la API, fíjate en la barra de direcciones
        private readonly string _apiUrl = "https://sistema-voto-electronico-z3q0.onrender.com/api/Usuarios";

        public VotantesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // GET: Muestra la página con la tabla de votantes
        public async Task<IActionResult> Index()
        {
            var votantes = new List<Votante>();
            try
            {
                var response = await _httpClient.GetAsync(_apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    votantes = JsonConvert.DeserializeObject<List<Votante>>(content) ?? new List<Votante>();
                }
            }
            catch
            {
                TempData["Error"] = "No se pudo conectar con la API.";
            }
            return View(votantes);
        }

        // POST: Recibe el Excel y lo procesa
        [HttpPost]
        public async Task<IActionResult> ImportarExcel(IFormFile archivoExcel)
        {
            if (archivoExcel == null || archivoExcel.Length == 0) return RedirectToAction("Index", "Usuarios");

            // Usamos el modelo Usuario que es el que muestra tu lista
            var listaUsuarios = new List<SistemaVoto.Modelos.Usuario>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await archivoExcel.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var hoja = workbook.Worksheet(1);
                        var filas = hoja.RangeUsed().RowsUsed().Skip(1);

                        foreach (var fila in filas)
                        {
                            // 1. Obtener el valor de la cédula y limpiar espacios
                            string cedulaExcel = fila.Cell(1).GetString().Trim();

                            // 2. FILTRO DE SEGURIDAD: Si la cédula está vacía, saltar a la siguiente fila
                            if (string.IsNullOrEmpty(cedulaExcel)) continue;

                            // 3. FILTRO DE SEGURIDAD: No intentar cargar al administrador Salvador Fonte
                            // Si la cédula es la del admin ya registrado, la ignoramos para evitar el choque
                            if (cedulaExcel == "1005182272") continue;

                            // Solo si pasa los filtros anteriores, agregamos al votante a la lista de envío
                            listaUsuarios.Add(new SistemaVoto.Modelos.Usuario
                            {
                                Cedula = cedulaExcel,
                                Nombres = fila.Cell(2).GetString().Trim(),
                                Correo = fila.Cell(3).GetString().Trim(),
                                YaVoto = false,
                                TokenVotacion = null, // Se generará en la Junta Receptora
                                RolUsuarioId = 2      // Rol de Votante
                            });
                        }
                    }
                }

                var json = JsonConvert.SerializeObject(listaUsuarios);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Enviamos a un nuevo endpoint en la API que guarde Usuarios
                var response = await _httpClient.PostAsync($"{_apiUrl}/Masivo", content);

                if (response.IsSuccessStatusCode)
                    TempData["Mensaje"] = $"¡Éxito! {listaUsuarios.Count} votantes cargados al padrón.";
                else
                    TempData["Error"] = "La API rechazó los datos (posible cédula repetida).";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction("Index", "Usuarios"); // Regresamos a la pantalla de la lista
        }
    }
}