using Microsoft.AspNetCore.Mvc;
using OCPEG.Web.Services.IServices;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class ActionsController : BaseController
    {

        private readonly ICallHost _callService;

        public ActionsController(ICallHost callService, ICustomerHost customerService)
        {
            _callService = callService ?? throw new ArgumentNullException(nameof(callService));
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ReadMessage()
        {
            string token = GetToken();

            var objCustomerType = await _callService.ReadMessageAsync(token);

            if (objCustomerType.Status != ResponseResultStatus.Success)
            {
                return View("Index", "Error");
            }

            return View("RabbitReturn");

        }


        public async Task<IActionResult> ChatGpt()
        {
            return RedirectToAction("Index", "ChatGpt");
        }
    }
}
