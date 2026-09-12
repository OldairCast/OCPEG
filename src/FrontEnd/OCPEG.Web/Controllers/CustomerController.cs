using Microsoft.AspNetCore.Mvc;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.ViewModels;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ICustomerHost _customerClient;
        private readonly CustomerViewModel model = new CustomerViewModel();

        public CustomerController(ICustomerHost customerService)
        {
            _customerClient = customerService ?? throw new ArgumentNullException(nameof(customerService));
        }

        [AcceptVerbs("GET")]
        public async Task<IActionResult> Index()
        {
            string token = GetToken();

            var objJsonResponse = await _customerClient.GetCustomerAll(token);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                model.Customers = new List<CustomerDto>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View("~/Views/Customer/Index.cshtml", model);
            }

            List<CustomerDto> lstCustomer = objJsonResponse.Exchange;

            model.Customers = lstCustomer;

            return View("~/Views/Customer/Index.cshtml", model);
        }


        [HttpPost, ActionName("GetByName")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetByName(string name)
        {
            string token = GetToken();

            var objJsonResponse = await _customerClient.GetCustomerByName(token, name);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                return Json(new List<ComboOption>(), objJsonResponse.Message);
            }

            return Json(objJsonResponse.Exchange);
        }

        public async Task<IActionResult> Create()
        {
            string token = GetToken();

            //Obtém o Tipo de Cliente
            var objCustomerType = await _customerClient.GetCustomerTypeAll(token);

            if (objCustomerType.Status != ResponseResultStatus.Success)
            {
                model.Type = new List<CustomerType>();
                ModelState.AddModelError(string.Empty, objCustomerType.Message);
                return View(model);
            }

            model.Type = objCustomerType.Exchange;

            return View("~/Views/Customer/Create.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerDto customer)
        {
            string token = GetToken();

            var objJsonResponse = await _customerClient.CreateCustomer(token, customer);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                model.Type = new List<CustomerType>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            return RedirectToAction("Index", new { opt = "5" });
        }

        public async Task<IActionResult> Edit(string id)
        {
            string token = GetToken();

            //Obtém o cliente
            var objJsonResponse = await _customerClient.GetCustomerById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                return Json(objJsonResponse.Message);
            }


            //Obtém o Tipo de Cliente
            var objCustomerType = await _customerClient.GetCustomerTypeAll(token);

            if (objCustomerType.Status != ResponseResultStatus.Success)
            {
                model.Type = new List<CustomerType>();
                ModelState.AddModelError(string.Empty, objCustomerType.Message);
                return View("~/Views/Customer/Edit.cshtml", model);
            }

            model.Type = objCustomerType.Exchange;

            model.Customer = objJsonResponse.Exchange;

            return View("~/Views/Customer/Edit.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerDto customer)
        {
            string token = GetToken();

            var objJsonResponse = await _customerClient.UpdateCustomer(token, customer);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                model.Type = new List<CustomerType>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            string token = GetToken();

            //Obtém o cliente
            var objJsonResponse = await _customerClient.GetCustomerById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View("~/Views/Customer/Index.cshtml", model);
            }

            CustomerDto customer = objJsonResponse.Exchange;

            return View("~/Views/Customer/Delete.cshtml", customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, IFormCollection collection)
        {
            string token = GetToken();

            var objJsonResponse = await _customerClient.DeleteCustomerById(token, id);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                Customer customer = new Customer();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(customer);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
