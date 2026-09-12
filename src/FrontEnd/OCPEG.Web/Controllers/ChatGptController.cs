using Microsoft.AspNetCore.Mvc;
using OCPEG.Framework.Exception;
using OCPEG.Web.Hosts.IHosts;
using static OCPEG.Framework.Enums;

namespace OCPEG.Web.Controllers
{
    public class ChatGptController : BaseController
    {
        private readonly IOpenAIHost _openAiClient;   

        public ChatGptController(IOpenAIHost openAiClient)
        {
            _openAiClient = openAiClient ?? throw new ArgumentNullException(nameof(openAiClient));
        }


        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Executa a pesquisa no Chat GPT
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IActionResult> Execute(string request)
        {
            string token = GetToken();

            //Obtém o cliente
            var objJsonResponse = await _openAiClient.ExecuteMain(token, request);

            if (objJsonResponse.Status != ResponseResultStatus.Success)
            {
                throw new OcException(objJsonResponse.Message);
            }

            return Json(objJsonResponse);

        }

    }
}
