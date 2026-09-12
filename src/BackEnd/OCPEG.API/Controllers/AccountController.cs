using AutoMapper;
using Azure.Core;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp;
using Microsoft.Azure.Amqp.Framing;
using OCPEG.API.Filters;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using System.Security.Claims;
using static OCPEG.Framework.Enums;

namespace OCPEG.Api.Controllers
{
    /// <summary>
    /// Conta do Usuário
    /// </summary>    
    [ServiceFilter(typeof(LogFilter))]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class AccountController : ControllerBaseLocal
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public AccountController(IAccountService accountService, ILogger<AccountController> logger,
            IMapper mapper) : base(mapper)
        {
            _accountService = accountService;
            _logger = logger;
        }

        /// <summary>
        /// Registra um Novo Usuario
        /// </summary>
        /// <returns>Dados do Usuário</returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserDto userReg)
        {
            try
            {
                userReg.Admin = false;

                User user = _mapper.Map<User>(userReg);

                // ***** Mapster substituto gratuito do AutoMapper
                //Ele faz o mapper comparando o o nome dos campos de cada lado
                user = userReg.Adapt<User>();

                var result = await _accountService.Register(user);

                ResponseResult<int> objResult = new ResponseResult<int>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<int> objResult = new ResponseResult<int>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<int> objResult = new ResponseResult<int>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<Customer> objResult = new ResponseResult<Customer>
                {
                    Exchange = new Customer(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Login do Usuario
        /// </summary>
        /// <returns>Informações de Login</returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);
                _logger.LogInformation(Constants.Inicio);

                User user = _mapper.Map<User>(userLogin);

                TokenOut objToken = await _accountService.Login(user);

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = objToken,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException ex)
            {
                NLogManager.LogError($"{ex}");
                _logger.LogError($"{ex}");

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                _logger.LogError($"{ex}");

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Login do Usuario com Google
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoginGoogle")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginGoogle(string returnUrl)
        {
            try
            {
                var authenticate = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!authenticate.Succeeded || authenticate.Principal is null
                    || !authenticate.Principal.Identities.Any(id => id.IsAuthenticated))
                {
                    return Challenge(GoogleDefaults.AuthenticationScheme);
                }
                else
                {
                    var claims = authenticate.Principal!.Identities.First().Claims;

                    var name = claims.First(c => c.Type == ClaimTypes.Name).Value;
                    var email = claims.First(c => c.Type == ClaimTypes.Email).Value;

                    User userLogin = new User
                    { Email = email,
                      Name = name
                    };

                    TokenOut objToken = await _accountService.LoginGoogle(userLogin);

                    ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                    {
                        Exchange = objToken,
                        Status = ResponseResultStatus.Success,
                        Message = Constants.OperacaoRealizadaSucesso
                    };

                    return Ok(objResult);

                }
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Login do Usuario com Google
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LoginInternalGoogle")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginInternalGoogle(UserDto userLogin)
        {
            try
            {
                var authenticate = await Request.HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (!authenticate.Succeeded || authenticate.Principal is null
                    || !authenticate.Principal.Identities.Any(id => id.IsAuthenticated))
                {
                    throw new BusinessLogicCustomException("Falha na autenticação com o Google.");
                }
                else
                {
                    User user = new User
                    {
                        Email = userLogin.Email,
                        Name = userLogin.Name
                    };

                    TokenOut objToken = await _accountService.LoginGoogle(user);

                    ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                    {
                        Exchange = objToken,
                        Status = ResponseResultStatus.Success,
                        Message = Constants.OperacaoRealizadaSucesso
                    };

                    return Ok(objResult);

                }
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Login do Usuario Json
        /// </summary>
        /// <returns>Informações de Login</returns>
        [HttpPost]
        [Route("LoginJson")]
        [AllowAnonymous]
        public async Task<JsonResult> LoginJson([FromBody] UserLoginDto userLogin)
        {
            try
            {
                User user = _mapper.Map<User>(userLogin);

                TokenOut objToken = await _accountService.Login(user);

                Response.StatusCode = StatusCodes.Status200OK;
                return new JsonResult(objToken);
            }
            catch (OcException ex)
            {
                NLogManager.LogError($"{ex}");

                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult("Erro ao tentar realizar o revoke do token do usuario. Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém dados do Usuário logado
        /// </summary>
        /// <param name="userIdentifier">Identificador do usuário</param>
        /// <returns>Objeto de negocio UserDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetUserIdentifierLogged/{userIdentifier}")]
        public async Task<IActionResult> GetUserIdentifierLogged(string userIdentifier)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                User user = await _accountService.GetUserLogged();

                UserDto userDto = new UserDto();

                if (! string.IsNullOrEmpty(userIdentifier) && user != null)
                {
                    userDto = _mapper.Map<UserDto>(user);
                }

                ResponseResult<UserDto> objResult = new ResponseResult<UserDto>
                {
                    Exchange = userDto,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<UserDto> objResult = new ResponseResult<UserDto>
                {
                    Exchange = new UserDto(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<UserDto> objResult = new ResponseResult<UserDto>
                {
                    Exchange = new UserDto(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<UserDto> objResult = new ResponseResult<UserDto>
                {
                    Exchange = new UserDto(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }


        /// <summary>
        /// Realiza o Refresh no token de Acesso
        /// </summary>
        /// <returns>Informações do Token de Acesso</returns>
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken(TokenOut tokenModel)
        {
            try
            {
                var result = await _accountService.RefreshToken(tokenModel);

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<TokenOut> objResult = new ResponseResult<TokenOut>
                {
                    Exchange = new TokenOut(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Revoga o Token de um usuario
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("Revoke/{userId}")]
        public async Task<IActionResult> Revoke(int userId)
        {
            try
            {
                var result = await _accountService.Revoke(userId);

                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = false,
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = false,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = false,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Revoga o Token de Todos os Usuarios
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("RevokeAll")]
        public async Task<IActionResult> RevokeAll()
        {
            try
            {
                var result = await _accountService.RevokeAll();

                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = false,
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = false,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = false,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Obtém dados do Usuario logado
        /// </summary>
        /// <returns>Dados do Usuário</returns>
        [HttpGet("GetUserLogged")]
        public async Task<IActionResult> GetUserLogged()
        {
            try
            {
                var result = await _accountService.GetUserLogged();

                ResponseResult<User> objResult = new ResponseResult<User>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<User> objResult = new ResponseResult<User>
                {
                    Exchange = new User(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<User> objResult = new ResponseResult<User>
                {
                    Exchange = new User(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<User> objResult = new ResponseResult<User>
                {
                    Exchange = new User(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }
    }
}
