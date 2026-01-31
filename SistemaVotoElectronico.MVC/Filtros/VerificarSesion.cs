using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaVotoElectronico.MVC.Filtros
{
    public class VerificarSesion : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Verificamos si existe la variable de sesión que creamos en el Login
            var usuario = context.HttpContext.Session.GetString("UsuarioLogueado");

            if (usuario == null)
            {
                // Si no hay usuario, lo pateamos al Login
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}