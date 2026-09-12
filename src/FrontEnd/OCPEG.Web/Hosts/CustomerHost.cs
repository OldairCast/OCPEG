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
    public class CustomerHost: ICustomerHost
    {
        private readonly HttpClient _client;
        public IConfiguration _configuration { get; }
        private readonly string uri = "";

        public CustomerHost(HttpClient client, IConfiguration Configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _configuration = Configuration;

            uri = _configuration["ServiceUrls:CustomerAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:CustomerAPI não configurado.");
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

        #region Cliente
        /// <summary>
        /// Obtém Lista de Clientes
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <returns>Lista de Clientes</returns>
        public async Task<ResponseResult<List<CustomerDto>>> GetCustomerAll(string token)
        {
            string url = uri + ConstantsWeb.BasePathCustomer + "GetAll";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<List<CustomerDto>>>();

            return objResponse!;
        }

        /// <summary>
        /// Obtém um cliente pelo Id
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <returns>Objeto de Negocio de Cliente</returns>
        public async Task<ResponseResult<CustomerDto>> GetCustomerById(string token, string customerId)
        {
            string url = uri + ConstantsWeb.BasePathCustomer + "GetById/" + customerId;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<CustomerDto>>();

            return objResponse!;
        }


        /// <summary>
        /// Obtém um cliente pelo Nome
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <param name="name">Nome do Cliente</param>
        /// <returns>Objeto de Negocio de Cliente</returns>
        public async Task<ResponseResult<List<ComboOption>>> GetCustomerByName(string token, string name)
        {
            string url = uri + ConstantsWeb.BasePathCustomer + "GetByName/" + name;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<List<ComboOption>>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>  
        /// <param name="customer">Objeto de Negócio de Cliente</param>
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<long>> CreateCustomer(string token, CustomerDto customer)
        {
            string url = uri + ConstantsWeb.BasePathCustomer + "Create";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, customer);
            var objResponse = await response.ReadContentAs<ResponseResult<long>>();

            return objResponse!;
        }


        /// <summary>
        /// Atualiza um Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <param name="product">Objeto de Negocio Cliente</param>  
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> UpdateCustomer(string token, CustomerDto customer)
        {
            string url = uri + ConstantsWeb.BasePathCustomer + "Update";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PutAsJson(url, customer);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        /// <summary>
        /// Exclui um Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="id">Id do Cliente</param> 
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> DeleteCustomerById(string token, string customerId)
        {
            string url = uri + ConstantsWeb.BasePathCustomer + "Delete/" + customerId;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.DeleteAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        #endregion

        #region Tipo de Cliente
        /// <summary>
        /// Obtém Lista de Tipo de Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <returns>Lista de Tipo de Cliente</returns>
        public async Task<ResponseResult<List<CustomerType>>> GetCustomerTypeAll(string token)
        {
            string url = uri + ConstantsWeb.BasePathCustomerType + "GetAll";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<List<CustomerType>>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um Tipo de Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>  
        /// <param name="product">Objeto de Negócio de Tipo de Cliente</param>
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<int>> CreateCustomerType(string token, CustomerType customerType)
        {
            string url = uri + ConstantsWeb.BasePathCustomerType + "Create";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, customerType);
            var objResponse = await response.ReadContentAs<ResponseResult<int>>();

            return objResponse!;
        }

        /// <summary>
        /// Obtém um Tipo de Cliente pelo Id
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <returns>Objeto de Negocio de Tipo de Cliente</returns>
        public async Task<ResponseResult<CustomerType>> GetCustomerTypeById(string token, int id)
        {
            string url = uri + ConstantsWeb.BasePathCustomerType + "GetById/" + id;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<CustomerType>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um Tipo de Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <param name="product">Objeto de Negocio Tipo de Cliente</param>  
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> UpdateCustomerType(string token, CustomerType customerType)
        {
            string url = uri + ConstantsWeb.BasePathCustomerType + "Update";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PutAsJson(url, customerType);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        /// <summary>
        /// Exclui um Tipo de Cliente
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="id">Id do Tipo de Cliente</param> 
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> DeleteCustomerTypeById(string token, int id)
        {
            string url = uri + ConstantsWeb.BasePathCustomerType + "Delete/" + id;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.DeleteAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        #endregion

    }
}
