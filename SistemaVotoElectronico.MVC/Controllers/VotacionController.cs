using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // Necesitarás este using arriba. Si sale rojo, avísame.
using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.Modelos;
using System.Text;

namespace SistemaVotoElectronico.MVC.Controllers
{
    public class VotacionController : Controller
    {
        public async Task<ActionResult> Papeleta(int idEvento)
        {
            List<Candidato> listaCandidatos = new List<Candidato>();

            try
            {
                // 1. Conectamos DIRECTO a la API (Sin intermediarios)
                using (var client = new HttpClient())
                {
                    // Ajusta esta URL si tu puerto es diferente
                    string urlApi = "http://127.0.0.1:5111/api/Candidatos";

                    var response = await client.GetAsync(urlApi);

                    if (response.IsSuccessStatusCode)
                    {
                        // Leemos la respuesta "cruda" como texto
                        var jsonString = await response.Content.ReadAsStringAsync();

                        // Intentamos convertir ese texto en una lista de candidatos
                        listaCandidatos = JsonConvert.DeserializeObject<List<Candidato>>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                // Si falla, no explotamos, solo seguimos con la lista vacía
                Console.WriteLine("Error leyendo API: " + ex.Message);
            }

            ViewBag.EventoId = idEvento;
            return View(listaCandidatos ?? new List<Candidato>());
        }
        // 1. PANTALLA DE BIENVENIDA AL VOTO (Elige la elección)
        public ActionResult Index()
        {
            // 1. Traemos la lista cruda de la API
            var respuesta = Crud<EventoElectoral>.ReadAll();

            // 2. Si la API devolvió algo, lo usamos. Si no, lista vacía.
            var listaEventos = respuesta.Data ?? new List<EventoElectoral>();

            // --- COMENTAMOS O BORRAMOS EL FILTRO DE FECHAS PARA PROBAR ---
            // var activos = listaEventos.Where(e => e.Activo && ...).ToList(); 

            // 3. Mandamos TODO a la vista
            return View(listaEventos);
        }


        [HttpPost]
        public async Task<ActionResult> Votar(int idLista, int idEvento)
        {
            try
            {
                // 1. Recuperamos el Token de la sesión
                string tokenUsuario = HttpContext.Session.GetString("TokenVotante");

                if (string.IsNullOrEmpty(tokenUsuario))
                {
                    TempData["Error"] = "Error de seguridad: No hay token en la sesión.";
                    return RedirectToAction("Index");
                }

                // 2. Creamos el objeto EXACTO que pide tu API (IntencionVoto)
                var datosVoto = new
                {
                    Token = tokenUsuario,
                    EventoId = idEvento,
                    ListaId = idLista
                };

                // 3. Enviamos a la API
                using (var client = new HttpClient())
                {
                    string urlApi = "http://localhost:5111/api/Votos/Emitir";

                    var json = JsonConvert.SerializeObject(datosVoto);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(urlApi, content);

                    if (response.IsSuccessStatusCode)
                    {
                        HttpContext.Session.Clear();
                        TempData["MensajeVoto"] = "¡Voto Exitoso! Se ha generado su certificado.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorMsg = await response.Content.ReadAsStringAsync();
                        TempData["Error"] = $"Error: {errorMsg}";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error crítico: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}