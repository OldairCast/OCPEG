using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Web.Extensions;
using OCPEG.Web.Services.IServices;
using OCPEG.Web.Util;
using System.Net.Http.Headers;

namespace OCPEG.Web.Services
{
    public class AccessControlHost : IAccessControlHost
    {
        private readonly HttpClient _client;
        public IConfiguration _configuration { get; }
        private readonly string uri = "";
        

        public AccessControlHost(HttpClient client, IConfiguration Configuration)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _configuration = Configuration;

            uri = _configuration["ServiceUrls:AccessControlAPI"]
                ?? throw new InvalidOperationException("ServiceUrls:AccessControlAPI não configurado.");
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

        #region Login

        /// <summary>
        /// Realiza o Login do Usuário
        /// </summary>
        /// <param name="userLogin">Dados de Login</param>         
        /// <returns>Objeto de Negocio de Login</returns>
        public async Task<ResponseResult<TokenOut>> Login(UserLoginDto userLogin)
        {
            string url = uri + ConstantsWeb.BasePathAccount + "Login";

            //limpa o header
            _client.DefaultRequestHeaders.Accept.Clear();

            var response = await _client.PostAsJson(url, userLogin);
            var objResponse = await response.Content.ReadFromJsonAsync<ResponseResult<TokenOut>>();

            return objResponse!;
        }


        /// <summary>
        /// Realiza o Login com o Google
        /// </summary>
        /// <param name="userLogin">Dados de Login</param>         
        /// <returns>Objeto de Negocio de Login</returns>
        public async Task<ResponseResult<TokenOut>> LoginGoogle(UserDto user)
        {
            string url = uri + ConstantsWeb.BasePathAccount + "LoginInternalGoogle";

            //limpa o header
            _client.DefaultRequestHeaders.Accept.Clear();

            var response = await _client.PostAsJson(url, url);
            var objResponse = await response.Content.ReadFromJsonAsync<ResponseResult<TokenOut>>();

            return objResponse!;
        }


        /// <summary>
        /// Registra um Usuário
        /// </summary>
        /// <param name="userRegister">Dados de Usuário de Registro</param>         
        /// <returns>Flag de Sucesso/Erro</returns>
        public async Task<ResponseResult<bool>> Register(UserDto userRegister)
        {
            string url = uri + ConstantsWeb.BasePathAccount + "Register";

            //limpa o header
            _client.DefaultRequestHeaders.Accept.Clear();

            var response = await _client.PostAsJson(url, userRegister);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }


        #endregion


        #region Usuário
        /// <summary>
        /// Obtém Lista de Usuário
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <returns>Lista de Usuário</returns>
        public async Task<ResponseResult<List<UserDto>>> GetUserAll(string token)
        {
            string url = uri + ConstantsWeb.BasePathUser + "GetAll";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<List<UserDto>>>();

            return objResponse!;
        }

        /// <summary>
        /// Obtém Usuário logado
        /// </summary>
        /// <param name="userIdentifier">Identificação do Usuário</param>        
        /// <param name="token">token de Autenticação</param>
        // <returns>Dados do Usuário</returns>
        public async Task<ResponseResult<UserDto>> GetUserIdentifierLogged(string token, string userIdentifier)
        {
            string url = uri + ConstantsWeb.BasePathAccount + "GetUserIdentifierLogged/" + userIdentifier;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<UserDto>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um Usuário
        /// </summary>
        /// <param name="token">token de Autenticação</param>  
        /// <param name="product">Objeto de Negócio de Usuário</param>
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<int>> CreateUser(string token, UserDto user)
        {
            string url = uri + ConstantsWeb.BasePathUser + "Create";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PostAsJson(url, user);
            var objResponse = await response.ReadContentAs<ResponseResult<int>>();

            return objResponse!;

        }

        /// <summary>
        /// Obtém um Usuário pelo Id
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <returns>Objeto de Negocio de Usuário</returns>
        public async Task<ResponseResult<UserDto>> GetUserById(string token, string id)
        {
            string url = uri + ConstantsWeb.BasePathUser + "GetUserById/" + id;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<UserDto>>();

            return objResponse!;
        }


        /// <summary>
        /// Obtém um Usuário pelo Nome
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <returns>Objeto de Negocio de Usuário</returns>
        public async Task<ResponseResult<UserDto>> GetUserByName(string token, string name)
        {
            string url = uri + ConstantsWeb.BasePathUser + "GetUserByName/" + name;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.GetAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<UserDto>>();

            return objResponse!;
        }

        /// <summary>
        /// Atualiza um Usuário
        /// </summary>
        /// <param name="token">token de Autenticação</param>         
        /// <param name="product">Objeto de Negocio Usuário</param>  
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> UpdateUser(string token, UserDto user)
        {
            string url = uri + ConstantsWeb.BasePathUser + "Update";

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.PutAsJson(url, user);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        /// <summary>
        /// Exclui um Usuário
        /// </summary>
        /// <param name="token">token de Autenticação</param>        
        /// <param name="id">Id do Usuário</param> 
        /// <returns>Flag de sucesso/erro</returns>
        public async Task<ResponseResult<bool>> DeleteUserById(string token, string id)
        {
            string url = uri + ConstantsWeb.BasePathUser + "Delete/" + id;

            //Inicializa o client
            InitializeClient(token);

            var response = await _client.DeleteAsync(url);
            var objResponse = await response.ReadContentAs<ResponseResult<bool>>();

            return objResponse!;
        }

        #endregion
    }
}
