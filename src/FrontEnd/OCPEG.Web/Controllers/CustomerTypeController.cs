using Microsoft.AspNetCore.Mvc;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework.Exception;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.ViewModels;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class CustomerTypeController : BaseController
    {
        private readonly ICustomerHost _customerService;
        private readonly CustomerTypeViewModel model = new CustomerTypeViewModel();

        public CustomerTypeController(ICustomerHost customerService)
        {
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [AcceptVerbs("GET")]
        public async Task<IActionResult> Index()
        {
            string token = GetToken();

            var objJsonResponse = await _customerService.GetCustomerTypeAll(token);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                model.CustomerTypes = new List<CustomerType>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View("~/Views/CustomerType/Index.cshtml", model);
            }

            model.CustomerTypes = objJsonResponse.Exchange;

            return View("~/Views/CustomerType/Index.cshtml", model);
        }

        public ActionResult Create()
        {
            return View("~/Views/CustomerType/Create.cshtml");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerTypeViewModel model)
        {
            string token = GetToken();

            var objJsonResponse = await _customerService.CreateCustomerType(token, model.CustomerType!);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            string token = GetToken();

            //Obtém o cliente
            var objJsonResponse = await _customerService.GetCustomerTypeById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                CustomerType CustomerType = new CustomerType();
                model.CustomerType = CustomerType;
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View("~/Views/CustomerType/Edit.cshtml", model);
            }

            model.CustomerType = objJsonResponse.Exchange;

            return View("~/Views/CustomerType/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerType customerType)
        {
            string token = GetToken();

            var objJsonResponse = await _customerService.UpdateCustomerType(token, customerType);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                model.CustomerType = customerType;
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            string token = GetToken();

            var objJsonResponse = await _customerService.GetCustomerTypeById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                CustomerType CustomerType = new CustomerType();
                model.CustomerType = CustomerType;
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            CustomerType customerType = objJsonResponse.Exchange;

            return View("~/Views/CustomerType/Delete.cshtml", customerType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            string token = GetToken();

            var objJsonResponse = await _customerService.DeleteCustomerTypeById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                CustomerType CustomerType = new CustomerType();
                model.CustomerType = CustomerType;
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
