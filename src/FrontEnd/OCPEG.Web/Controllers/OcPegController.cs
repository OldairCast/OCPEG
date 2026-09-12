using Microsoft.AspNetCore.Mvc;

namespace OCPEG.Web.Controllers
{
    public class OcPegController : Controller
    {
        private readonly IConfiguration _config;

        public OcPegController(IConfiguration config)
        {
            _config = config;
        }


        public IActionResult Index()
        {
            ViewBag.Url = _config.GetValue<string>("urlWeb")!; 
            return View();
        }
    }
}
