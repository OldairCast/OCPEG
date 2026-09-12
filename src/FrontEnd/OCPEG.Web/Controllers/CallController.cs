using Microsoft.AspNetCore.Mvc;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.ViewModels;
using static OCPEG.Domain.Enum.Enums;
using static OCPEG.Framework.Enums;
using OCPEG.Application.Communication.Requests;
using OCPEG.Framework.Exception;

namespace OCPEG.Web.Controllers
{
    public class CallController : BaseController
    {
        private readonly ICallHost _callClient;
        private readonly CallViewModel model = new CallViewModel();
        private const string urlIndex = "~/Views/Call/Index.cshtml";

        public CallController(ICallHost callClient)
        {
            _callClient = callClient ?? throw new ArgumentNullException(nameof(callClient));
        }


        [AcceptVerbs("GET")]
        public async Task<IActionResult> Index()
        {
            string token = GetToken();

            var objJsonResponse = await _callClient.GetCallAll(token);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                var lstCallNumber = new List<CallViewModel>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlIndex, lstCallNumber);
            }

            var modelLst = new List<CallViewModel>();

            if (objJsonResponse.Exchange != null)
            {
                foreach (var item in objJsonResponse.Exchange)
                {
                    var vmodel = new CallViewModel
                    {
                        Id = item.Id,
                        CallNumber = item.CallNumber,
                        Customer = new ComboOption
                        {
                            Id = item.Customer!.Id,
                            Name = item.Customer.Name
                        },
                        PrioritySel = new ComboOption
                        {
                            Id = item.Priority.ToString(),
                            Name = Enum.GetName(typeof(PriorityEn), item.Priority) ?? StatusEn.None.ToString()
                        },
                        ProductSel = new Product
                        {
                            Id = item.Product!.Id,
                            Name = item.Product.Name
                        },
                        StartDate = item.StartDate.ToString(),
                        StatusSel = new ComboOption
                        {
                            Id = Convert.ToByte(item.Status).ToString(),
                            Name = Enum.GetName(typeof(StatusEn), item.Status) ?? StatusEn.None.ToString()
                        },
                        FinishDate = item.FinishDate == null ? "" : item.FinishDate.ToString()
                    };

                    modelLst.Add(vmodel);
                }
            }

            return View(urlIndex, modelLst);
        }


        public async Task<IActionResult> Create()
        {
            model.Priority = GetPriority();
            model.Product = await GetProduct();

            return View("~/Views/Call/Create.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CallViewModel model)
        {
            string token = GetToken();

            if (model != null)
            {
                string customerId = (model.Customer != null) ? model.Customer.Id : "0";

                CallDto callDto = new CallDto
                {
                    CallNumber = "0",
                    Customer = new ComboOption
                    {
                        Id = customerId
                    },

                    Product = new Product
                    {
                        Id = model.ProductId ?? 0
                    },

                    Priority = model.PriorityId != null ? (PriorityEn)model.PriorityId : PriorityEn.None,

                    Comment = model.Comment != null ? model.Comment : ""
                };

                CallRequest callRequest = new CallRequest
                {
                    Call = callDto
                };

                var objJsonResponse = await _callClient.CreateCall(token, callRequest);

                if (objJsonResponse.Status != ResponseResultStatus.Success)
                {
                    model.Priority = GetPriority();
                    model.Product = await GetProduct();
                    ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                    return View(model);
                }
            }

            return RedirectToAction("Index", new { opt = "5" });
        }

        public async Task<IActionResult> Edit(string id)
        {
            string token = GetToken();

            CallDto callParams = new CallDto
            {
                Id = id,
                StartDate = null,
                FinishDate = null,
                Customer = new ComboOption
                {
                    Id = "0"
                },
                Status = (byte)StatusEn.None
            };

            CallRequest callRequest = new CallRequest
            {
                Call = callParams
            };

            //Obtém o atendimento
            var objCall = await _callClient.GetCallbyParams(token, callRequest);

            if (objCall.Status != ResponseResultStatus.Success)
            {
                var lstCallNumber = new List<CallViewModel>();
                ModelState.AddModelError(string.Empty, objCall.Message);
                return View(urlIndex, lstCallNumber);
            }

            if (objCall.Exchange.Count == 0)
            {
                return View("~/Views/Call/Details.cshtml", new CallViewModel());
            }

            CallDto callDto = objCall.Exchange[0];
            var vmodel = new CallViewModel
            {
                Id = callDto.Id,
                CallNumber = callDto.CallNumber,
                Customer = new ComboOption
                {
                    Id = callDto.Customer != null ? callDto.Customer.Id : "0",
                    Name = callDto.Customer != null ? callDto.Customer.Name : ""
                },
                PrioritySel = new ComboOption
                {
                    Id = callDto.Priority.ToString(),
                    Name = Enum.GetName(typeof(PriorityEn), callDto.Priority) ?? StatusEn.None.ToString()
                },
                ProductSel = new Product
                {
                    Id = callDto.Product != null ? callDto.Product.Id : 0,
                    Name = callDto.Product != null ? callDto.Product.Name : ""
                },
                StartDate = callDto.StartDate.ToString(),
                StatusSel = new ComboOption
                {
                    Id = callDto.Status.ToString(),
                    Name = Enum.GetName(typeof(StatusEn), callDto.Status) ?? StatusEn.None.ToString()
                },
                FinishDate = callDto.FinishDate == null ? "" : callDto.FinishDate.ToString(),
                Comment = callDto.Comment
            };


            return View("~/Views/Call/Details.cshtml", vmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CallDto model)
        {
            string token = GetToken();

            CallRequest callRequest = new CallRequest
            {
                Call = model
            };

            var objJsonResponse = await _callClient.UpdateCall(token, callRequest);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                var lstCallNumber = new List<CallViewModel>();
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(urlIndex, lstCallNumber);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Product>> GetProduct()
        {
            string token = GetToken();

            var objJsonResponse = await _callClient.GetProductAll(token);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                throw new OcException(objJsonResponse.Message);
            }

            List<Product> comboOptionList = objJsonResponse.Exchange;

            return comboOptionList;
        }


        private List<ComboOption> GetPriority()
        {
            List<ComboOption> comboOptionList = new List<ComboOption>();
            foreach (byte i in Enum.GetValues(typeof(PriorityEn)))
            {
                ComboOption function = new ComboOption
                {
                    Id = i.ToString(),
                    Name = Enum.GetName(typeof(PriorityEn), i) ?? StatusEn.None.ToString()
                };

                if (function.Id != "0")
                {
                    comboOptionList.Add(function);
                }
            }

            return comboOptionList;
        }

    }
}
