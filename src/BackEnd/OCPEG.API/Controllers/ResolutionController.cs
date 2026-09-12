using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCPEG.API.Filters;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using static OCPEG.Framework.Enums;

namespace OCPEG.Api.Controllers
{
    /// <summary>
    /// Resolução
    /// </summary>    
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class ResolutionController : ControllerBaseLocal
    {
        private readonly IResolutionService _resolutionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolutionService"></param>
        /// <param name="mapper"></param>
        public ResolutionController(IResolutionService resolutionService, IMapper mapper) : base(mapper)
        {
            _resolutionService = resolutionService;
        }

        /// <summary>
        /// Obtém dados da resolução por numero do protocolo
        /// </summary>
        /// <param name="callNumber">Numero do Protocolo</param>
        /// <returns>Objeto de negocio ResolutionDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetByCallNumber/{callNumber}")]
        public async Task<IActionResult> GetByCallNumber(string callNumber)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _resolutionService.GetByCallNumber(callNumber);

                ResponseResult<Resolution> objResult = new ResponseResult<Resolution>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<Resolution> objResult = new ResponseResult<Resolution>
                {
                    Exchange = new Resolution(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<Resolution> objResult = new ResponseResult<Resolution>
                {
                    Exchange = new Resolution(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<Resolution> objResult = new ResponseResult<Resolution>
                {
                    Exchange = new Resolution(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }


        /// <summary>
        /// Realiza a resolução de um atendimento
        /// </summary>
        /// <param name="resolution">Dados da Resolução</param> 
        /// <returns>Id do protocolo criado</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] Resolution resolution)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _resolutionService.Insert(resolution);

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
        /// Envia as informações ao cliente com os dados da resolução
        /// </summary>
        ///<param name="message">Dados da Resolução</param>
        /// <returns>Flag Sucesso/erro</returns>
        [HttpPost]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] Message message)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _resolutionService.SendMessage(message);

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
        /// Lê e executa as mensagens do RabbitMQ
        /// </summary>
        /// <returns>Flag de sucesso/erro</returns>
        [AcceptVerbs("GET")]
        [Route("ReadMessage")]
        public async Task<IActionResult> ReadMessage()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _resolutionService.ReadMessage();

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
