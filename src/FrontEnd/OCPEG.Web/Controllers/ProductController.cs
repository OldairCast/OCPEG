using Microsoft.AspNetCore.Mvc;
using OCPEG.Domain.BusinessObject;
using OCPEG.Web.Services.IServices;
using static OCPEG.Framework.Enums;


namespace OCPEG.Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly ICallHost _callService;
        private const string urlIndex = "~/Views/Product/Index.cshtml";

        public ProductController(ICallHost callService)
        {
            _callService = callService ?? throw new ArgumentNullException(nameof(callService));
        }

        [AcceptVerbs("GET")]
        public async Task<IActionResult> Index()
        {
            string token = GetToken();

            var objJsonResponse = await _callService.GetProductAll(token);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlIndex, new List<Product>());
            }

            List<Product> lstProduct = objJsonResponse.Exchange;

            return View(urlIndex, lstProduct);
        }

        public ActionResult Create()
        {
            return View("~/Views/Product/Create.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateJson(Product model)
        {
            string token = GetToken();

            var objJsonResponse = await _callService.CreateProduct(token, model);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                return Json(objJsonResponse.Message);
            }

            return Json(objJsonResponse);
        }

        public async Task<IActionResult> Edit(int id)
        {
            string token = GetToken();

            var objJsonResponse = await _callService.GetProductById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlIndex, new List<Product>());
            }

            Product product = objJsonResponse.Exchange;
            return View("~/Views/Product/Edit.cshtml", product);
        }

        [HttpPut, ActionName("EditJson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model)
        {
            string token = GetToken();

            var objJsonResponse = await _callService.UpdateProduct(token, model);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                return Json(objJsonResponse.Message);
            }

            bool ret = objJsonResponse.Exchange;

            return Json(ret);
        }

        public async Task<IActionResult> Delete(int id)
        {
            string token = GetToken();

            var objJsonResponse = await _callService.GetProductById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlIndex, new List<Product>());
            }

            Product product = objJsonResponse.Exchange;

            return View("~/Views/Product/Delete.cshtml", product);
        }

        [HttpDelete, ActionName("DeleteJson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            string token = GetToken();

            var objJsonResponse = await _callService.DeleteProductById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                return Json(objJsonResponse.Message);
            }

            bool ret = objJsonResponse.Exchange;

            return Json(ret);
        }

    }
}
