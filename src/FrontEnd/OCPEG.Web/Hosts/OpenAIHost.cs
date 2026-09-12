using OCPEG.Application.Communication.Responses;
using OCPEG.Framework;
using OCPEG.Web.Extensions;
using OCPEG.Web.Hosts.IHosts;
using OCPEG.Web.Util;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OCPEG.Web.Services
{
    public class OpenAIHost : IOpenAIHost
    {
        private readonly HttpClient _client;

        public IConfiguration _configuration { get; }
        private readonly string uri = "";

        public OpenAIHost(HttpClient client, IConfiguration Configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _configuration = Configuration;

            uri = _configuration["ServiceUrls:OPenAIAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:OpenAI não configurado.");
        }

        /// <summary>
        /// Realiza operações iniciais no objeto client
        /// </summary>
        private void InitializeClient(string token)
        {
            //limpa o header
            _client.DefaultRequestHeaders.Accept.Clear();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }


        /// <summary>
        /// Obtém Resposta conforme requisição
        /// </summary>
        /// <param name="requestJson">token de Requisição</param>        
        /// <returns>Resposta</returns>
        public async Task<ResponseResult<string>> ExecuteMain(string token, string requestJson)
        {
            string url = uri + ConstantsWeb.BasePathOpenAI + "GenerateRecipe";

            //Inicializa o client
            InitializeClient(token);

            var obj = JsonNode.Parse(requestJson);
            var response = await _client.PostAsJson(url, obj);
            var objResponse = await response.ReadContentAs<ResponseResult<GenerateRecipeResponse>>();

            string jsonResult = JsonSerializer.Serialize(objResponse!.Exchange);

            ResponseResult<string> result = new ResponseResult<string>
            {
                Exchange = jsonResult,
                Status = objResponse.Status,
                Message = objResponse.Message
            };

            return result;
        }

    }
}
