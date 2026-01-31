using Microsoft.AspNetCore.Mvc;
using SistemaVoto.Modelos;
using SistemaVotoElectronico.ApiConsumer;
using SistemaVotoElectronico.MVC.Filtros;

namespace SistemaVotoElectronico.MVC.Controllers
{
    [VerificarSesion] 
    public class CandidatosController : Controller
    {
        // GET: Candidatos
        public ActionResult Index()
        {
            var data = Crud<Candidato>.ReadAll();
            return View(data.Data ?? new List<Candidato>());
        }

        // GET: Candidatos/Details/5
        public ActionResult Details(int id)
        {
            var data = Crud<Candidato>.ReadBy(id.ToString());
            return View(data.Data);
        }

        // GET: Candidatos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Candidatos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Candidato data)
        {
            try
            {
                data.ListaPolitica = null;
                Crud<Candidato>.Create(data);

                TempData["MensajeExito"] = $"¡El candidato {data.Nombres} se registró correctamente!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(data);
            }
        }

        // GET: Candidatos/Edit/5
        public ActionResult Edit(int id)
        {
            var data = Crud<Candidato>.ReadBy(id.ToString());
            return View(data.Data);
        }

        // POST: Candidatos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Candidato data)
        {
            try
            {
                Crud<Candidato>.Update(id.ToString(), data);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Candidatos/Delete/5
        public ActionResult Delete(int id)
        {
            var data = Crud<Candidato>.ReadBy(id.ToString());
            return View(data.Data);
        }

        // POST: Candidatos/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Candidato data)
        {
            try
            {
                Crud<Candidato>.Delete(id.ToString());
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}