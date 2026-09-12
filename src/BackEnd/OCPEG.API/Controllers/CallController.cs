using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCPEG.API.Filters;
using OCPEG.Application.Communication.Requests;
using OCPEG.Application.Communication.Responses;
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
    /// Classe de Atendimento
    /// </summary>
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class CallController : ControllerBaseLocal
    {
        private readonly ICallService _callService;
        private readonly SqidsEncoder<int> _idEnconder;
        private readonly ILogger<CallController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callService"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="idEnconder"></param>
        public CallController(ICallService callService, ILogger<CallController> logger, IMapper mapper, SqidsEncoder<int> idEnconder) : base(mapper)
        {
            _callService = callService;
            _logger = logger;
            _idEnconder = idEnconder;
        }

        /// <summary>
        /// Retorna lista de Protocolos
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);
                _logger.LogInformation(Constants.Inicio);

                var calls = await _callService.GetAll();

                List<CallDto> callDto = new List<CallDto>();

                if (calls != null && calls.Count > 0)
                {
                    foreach (var item in calls)
                    {
                        CallDto userDto = _mapper.Map<CallDto>(item);
                        callDto.Add(userDto);
                    }
                }

                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = callDto,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = new List<CallDto>(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = new List<CallDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                _logger.LogError($"{ex}");

                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = new List<CallDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Retorna lista de atendimentos conforme parãmetros
        /// </summary>
        /// <param name="paramSearch">Lista de Parãmetros de pesquisa</param>
        /// <returns>Lista de Objeto de negocio CallDto</returns>
        [HttpPost]
        [Route("GetbyParams")]
        public async Task<IActionResult> GetbyParams([FromBody] CallRequest paramSearch)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                string callNumber = "";

                if (string.IsNullOrEmpty(paramSearch.Call!.Id))
                {
                    callNumber = paramSearch.Call.CallNumber;
                }
                else
                {
                    var arrCall = _idEnconder.Decode(paramSearch.Call!.Id);
                    callNumber = arrCall.Count > 0 ? arrCall[0].ToString() : "";
                }

                paramSearch.Call.CallNumber = callNumber;
                var calls = await _callService.GetbyParams(paramSearch);

                List<CallDto> callDto = new List<CallDto>();

                if (calls != null && calls.Count > 0)
                {
                    foreach (var item in calls)
                    {
                        CallDto userDto = _mapper.Map<CallDto>(item);
                        callDto.Add(userDto);
                    }
                }

                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = callDto,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = new List<CallDto>(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = new List<CallDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<List<CallDto>> objResult = new ResponseResult<List<CallDto>>
                {
                    Exchange = new List<CallDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Insere um novo atendimento
        /// </summary>
        /// <returns>Id do protocolo criado</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CallRequest callRequest)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (callRequest == null) return BadRequest();

                Call call = _mapper.Map<Call>(callRequest.Call);

                var result = await _callService.Create(call);

                ResponseResult<CallResponse> objResult = new ResponseResult<CallResponse>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<CallResponse> objResult = new ResponseResult<CallResponse>
                {
                    Exchange = new CallResponse(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<CallResponse> objResult = new ResponseResult<CallResponse>
                {
                    Exchange = new CallResponse(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<CallResponse> objResult = new ResponseResult<CallResponse>
                {
                    Exchange = new CallResponse(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Atualiza um atendimento aberto
        /// </summary>
        /// <param name="callRequest">Dados do cliente</param> 
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] CallRequest callRequest)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (callRequest == null) return BadRequest();

                Call call = _mapper.Map<Call>(callRequest.Call);

                var result = await _callService.Update(call);

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
                    Exchange = new bool(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<bool> objResult = new ResponseResult<bool>
                {
                    Exchange = new bool(),
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
                    Exchange = new bool(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }
    }

}
