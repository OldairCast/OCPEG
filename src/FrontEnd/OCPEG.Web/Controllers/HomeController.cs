using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OCPEG.Framework;
using OCPEG.Web.Models;
using System.Diagnostics;

namespace OCPEG.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return RedirectToActionPermanent("Index", "Login");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            {
                var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

                var exception = exceptionFeature?.Error; // 🔥 aqui está a exceção

                var mensagem = exception?.Message;
                var stackTrace = exception?.StackTrace;

                NLogManager.LogError($"{mensagem}\n{stackTrace}");

                return View(new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }
}
