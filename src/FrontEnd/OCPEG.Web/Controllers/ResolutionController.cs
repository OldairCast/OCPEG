using Microsoft.AspNetCore.Mvc;
using OCPEG.Application.Communication.Requests;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.ViewModels;
using static OCPEG.Domain.Enum.Enums;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class ResolutionController : BaseController
    {
        private readonly ICallHost _callClient;

        public ResolutionController(ICallHost callClient, ICustomerHost customerClient)
        {
            _callClient = callClient ?? throw new ArgumentNullException(nameof(callClient));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResolutionViewModel model)
        {
            string token = GetToken();

            Resolution resolution = new Resolution
            {
                CallNumber = model.CallNumber,
                Comment = model.Comment != null ? model.Comment : ""
            };


            var objJsonResponse = await _callClient.CreateResolution(token, resolution);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                ModelState.AddModelError(string.Empty, objJsonResponse.Message);
                return View(model);
            }

            //Envia Mensagem ao cliente assincrona
            var objSend = await SendMessageAsync(model.CallNumber, token);


            if (objSend.Status != ResponseResultStatus.Success)
            {
                return RedirectToAction("Index", "Error");
            }

            return RedirectToAction("Index", "Call");
        }

        public async Task<IActionResult> Edit(string callId)
        {
            string token = GetToken();

            CallDto callParams = new CallDto
            {
                Id = callId,
                StartDate = null,
                FinishDate = null,
                Customer = new ComboOption
                {
                    Id = "0"
                },
                Status = StatusEn.None
            };

            CallRequest callRequest = new CallRequest
            {
                Call = callParams
            };

            //Obtém o cliente
            var objJsonResponse = await _callClient.GetCallbyParams(token, callRequest);
            var item = objJsonResponse.Exchange[0];

            if (objJsonResponse.Status != ResponseResultStatus.Success || item == null)
            {
                var lstCallNumber = new List<CallViewModel>();
                ModelState.AddModelError(string.Empty, "Protocolo não encontrado");
                return View("~/Views/Call/Index.cshtml", lstCallNumber);
            }

            ResolutionViewModel model = new ResolutionViewModel();

            if (item.Status == StatusEn.Fechado)
            {
                var objJsonResponseR = await _callClient.GetResolutionbyParams(token, Convert.ToString(item.CallNumber));

                if (objJsonResponseR.Status != ResponseResultStatus.Success)
                {
                    var lstCallNumber = new List<CallViewModel>();
                    ModelState.AddModelError(string.Empty, objJsonResponseR.Message);
                    return View("~/Views/Call/Index.cshtml", lstCallNumber);
                }

                var resolution = objJsonResponseR.Exchange;
                model.CallNumber = resolution.CallNumber;
                model.User = resolution.User!.Name;
                model.Comment = resolution.Comment;
                model.Date = resolution.Date;

                return View("~/Views/Resolution/Details.cshtml", model);

            }
            else
            {
                model.CallNumber = Convert.ToInt32(item.CallNumber);
                return View("~/Views/Resolution/Create.cshtml", model);
            }
        }

        private async Task<ResponseResult<bool>> SendMessageAsync(int callNumber, string token)
        {
            //Envia a mensagem para o serviço assincrono
            Message message = new Message
            {
                CallNumber = callNumber,
                Comment = "Texto da Resolução"
            };

            var objMessage = await _callClient.SendMessageAsync(token, message);

            return objMessage;

        }
    }
}
