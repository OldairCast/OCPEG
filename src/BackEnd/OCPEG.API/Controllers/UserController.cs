using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCPEG.API.Filters;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using Sqids;
using static OCPEG.Framework.Enums;

namespace OCPEG.Api.Controllers
{
    /// <summary>
    /// Usuário
    /// </summary>    
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class UserController : ControllerBaseLocal
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly SqidsEncoder<int> _idEnconder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountService"></param>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        /// <param name="idEnconder"></param>
        public UserController(IAccountService accountService, IUserService userService,
            IMapper mapper, SqidsEncoder<int> idEnconder) : base(mapper)
        {
            _userService = userService;
            _accountService = accountService;
            _idEnconder = idEnconder; 
        }

        /// <summary>
        /// Obtém dados do Usuário pelo Id
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        /// <returns>Objeto de negocio UserDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetUserById/{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(string userId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrUserId = _idEnconder.Decode(userId);
                int iUserId = arrUserId.Count > 0 ? arrUserId[0] : 0;

                var result = await _userService.GetById(iUserId);
                UserDto userDto = _mapper.Map<UserDto>(result);

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
        /// Obtém Todos os Usuários
        /// </summary>
        /// <returns>Lista de Objeto de negocio UserDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetAll")]
        public async Task<IActionResult> GetUsersAsync()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _userService.GetAll();

                ResponseResult<List<UserDto>> objResult = new ResponseResult<List<UserDto>>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);
            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<List<UserDto>> objResult = new ResponseResult<List<UserDto>>
                {
                    Exchange = new List<UserDto>(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<List<UserDto>> objResult = new ResponseResult<List<UserDto>>
                {
                    Exchange = new List<UserDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<List<UserDto>> objResult = new ResponseResult<List<UserDto>>
                {
                    Exchange = new List<UserDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }


        /// <summary>
        /// Obtém Primeiro usuário como principal
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [Route("GetUserMain")]
        public async Task<IActionResult> GetUserMain()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _accountService.GetUserMain();

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

        /// <summary>
        /// Cria novo Usuário
        /// </summary>
        /// <returns>Dados do usuario</returns>
        [HttpPost("Create")]
        [AllowAnonymous]
        public async Task<IActionResult> Create(UserDto userDto)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                User user = _mapper.Map<User>(userDto);
                var result = await _accountService.Register(user);

                ResponseResult<int> objResult = new ResponseResult<int>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return new JsonResult(objResult);

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
                ResponseResult<Customer> objResult = new ResponseResult<Customer>
                {
                    Exchange = new Customer(),
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
        /// Altera os dados do Usuário
        /// </summary>
        /// <returns></returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update(UserDto userDto)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                User user = _mapper.Map<User>(userDto);
                
                await _userService.Update(user);

                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = true,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return new JsonResult(objResult);

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
        /// Exclui um Usuário
        /// </summary>
        /// <param name="userId">Id do usuário</param>
        [HttpDelete]
        [Route("Delete/{userId}")]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrUserId = _idEnconder.Decode(userId);
                int iUserId = arrUserId.Count > 0 ? arrUserId[0] : 0;

                var result = await _userService.Delete(iUserId);

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
    }
}
