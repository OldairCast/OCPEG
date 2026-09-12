using OCPEG.Application.Communication.Requests;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Web.Extensions;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.Util;
using System.Net.Http.Headers;

namespace OCPEG.Web.Services
{
    public class CallHost: ICallHost
    {
        private readonly HttpClient _client;

        public IConfiguration _configuration { get; }
        private readonly string uri = "";

        public CallHost(HttpClient client,IConfiguration Configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _configuration = Configuration;

            uri = _configuration["ServiceUrls:CallAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:CallAPI não configurado.");
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


        #region Call

        /// <summary>
        /// Obtém Lista de Atendimentos
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <returns>Lista de Atendimentos</returns>
        public async Task<ResponseResult<List<CallDto>>> GetCallAll(string token)
        {
            string url = uri + ConstantsWeb.BasePathCall + "GetAll";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<List<CallDto>>>();

            return objResponse!;
        }

        /// <summary>
        /// Obtém um atendimento por parametros
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="id">id do atendimento</param> 
        /// <returns>Objeto de negocio com lista de atendimentos</returns>
        public async Task<ResponseResult<List<CallDto>>> GetCallbyParams(string token, CallRequest request)
        {
            string url = uri + ConstantsWeb.BasePathCall + "GetbyParams";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, request);
            var objResponse = await response.ReadContentAs<ResponseResult<List<CallDto>>>();

            return objResponse!;
        }

        /// <summary>
        /// Cria um atendimento
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="model">Dados do atendimento</param> 
        /// <returns>Objeto de negocio com dados de atendimento</returns>
        public async Task<ResponseResult<Call>> CreateCall(string token, CallRequest callRequest)
        {
            string url = uri + ConstantsWeb.BasePathCall + "Create";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, callRequest);
            var objResponse = await response.ReadContentAs<ResponseResult<Call>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um atendimento aberto
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="model">Dados do atendimento</param> 
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> UpdateCall(string token, CallRequest callRequest)
        {
            string url = uri + ConstantsWeb.BasePathCall + "Update";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PutAsJson(url, callRequest);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        #endregion


        #region Resolução

        /// <summary>
        /// Cria um atendimento
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="resolutionDto">Dados da Resolução</param> 
        /// <returns>Objeto de negocio com dados de atendimento</returns>
        public async Task<ResponseResult<bool>> CreateResolution(string token, Resolution resolution)
        {
            string url = uri + ConstantsWeb.BasePathResolution + "Create";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, resolution);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        /// <summary>
        /// Obtém lista de resolução por parâmetros
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="callNumber">Numero do Protocolo</param> 
        /// <returns>Objeto de Negocio com informações de Resolução</returns>
        public async Task<ResponseResult<Resolution>> GetResolutionbyParams(string token, string callNumber)
        {
            //Inicializa o client
            InitializeClient(token);

            string url = uri + ConstantsWeb.BasePathResolution + "GetByCallNumber/" + callNumber;

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<Resolution>>();

            return objResponse!;
        }

        /// <summary>
        /// Envia uma mensagem assincrona ao RabbitMQ
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="message">Dados da Mensagem</param> 
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> SendMessageAsync(string token, Message message)
        {
            string url = uri + ConstantsWeb.BasePathResolution + "SendMessage";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, message);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        /// <summary>
        /// Lê as mensagens do RabbitMQ
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> ReadMessageAsync(string token)
        {
            string url = uri + ConstantsWeb.BasePathResolution + "ReadMessage";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        #endregion


        #region Assunto
        /// <summary>
        /// Obtém Lista de Assuntos
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <returns>Lista de Assuntos</returns>
        public async Task<ResponseResult<List<Product>>> GetProductAll(string token)
        {
            string url = uri + ConstantsWeb.BasePathProduct + "GetAll";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<List<Product>>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um assunto
        /// </summary>
        /// <param name="token">token de Autenticação</param>  
        /// <param name="product">Objeto de Negócio de Assunto</param>
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<int>> CreateProduct(string token, Product product)
        {
            string url = uri + ConstantsWeb.BasePathProduct + "Create";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, product);
            var objResponse = await response.ReadContentAs<ResponseResult<int>>();

            return objResponse!;
        }

        /// <summary>
        /// Obtém um assunto pelo Id
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <returns>Objeto de Negocio de Assunto</returns>
        public async Task<ResponseResult<Product>> GetProductById(string token, int id)
        {
            string url = uri + ConstantsWeb.BasePathProduct + "GetById/" + id;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<Product>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um assunto
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <param name="product">Objeto de Negocio Assunto</param>  
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> UpdateProduct(string token, Product product)
        {
            string url = uri + ConstantsWeb.BasePathProduct + "Update";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PutAsJson(url, product);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        /// <summary>
        /// Exclui um assunto
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="id">Id do Assunto</param> 
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> DeleteProductById(string token, int id)
        {
            string url = uri + ConstantsWeb.BasePathProduct + "Delete/" + id;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.DeleteAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        #endregion
    }
}
