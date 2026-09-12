using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCPEG.Api.Controllers;
using OCPEG.API.Filters;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using Sqids;
using static OCPEG.Framework.Enums;


/*No swagger
 * Selecione multipart/form-data

Escolha a imagem e envie
 * 
 * 
 * 
 * 
 * 
 */
namespace OCPEG.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo upload de imagens
    /// </summary>    
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class CloudController : ControllerBaseLocal
    {
        private readonly ICustomerImageService _imageService;
        private readonly ICloudImageService _cloudimageService;
        private readonly ICloudMessageService _cloudmessageService;
        private readonly SqidsEncoder<int> _idEnconder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageService"></param>
        /// <param name="cloudimageService"></param>
        /// <param name="cloudmessageService"></param>
        /// <param name="mapper"></param>
        /// <param name="idEnconder"></param>
        public CloudController(ICustomerImageService imageService, 
            ICloudImageService cloudimageService, ICloudMessageService cloudmessageService,
            IMapper mapper, SqidsEncoder<int> idEnconder) : base(mapper)
        {
            _imageService = imageService;
            _cloudimageService = cloudimageService;
            _cloudmessageService = cloudmessageService;
            _idEnconder = idEnconder;
        }

        #region Image
        /// <summary>
        /// Faz o upload de uma imagem de cliente para o Banco de Dados
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost("upload/{customerId}")]
        public async Task<IActionResult> Upload(IFormFile file, string customerId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (file == null || file.Length == 0)
                    return BadRequest("Nenhuma imagem enviada.");

                // Validação de tipo
                var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!tiposPermitidos.Contains(file.ContentType))
                    return BadRequest("Formato de imagem inválido.");

                // Limite de tamanho (ex: 2MB)
                if (file.Length > 2 * 1024 * 1024)
                    return BadRequest("Imagem maior que 2MB.");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                var arrCustomerId = _idEnconder.Decode(customerId);
                int iCustomerId = arrCustomerId.Count > 0 ? arrCustomerId[0] : 0;

                var customerImage = new CustomerImage
                {
                    CustomerId = iCustomerId,
                    Nome = file.FileName,
                    ContentType = file.ContentType,
                    Dados = ms.ToArray(),
                    DataUpload = DateTime.UtcNow
                };

                var result = await _imageService.Upload(customerImage);

                ResponseResult<long> objResult = new ResponseResult<long>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<long> objResult = new ResponseResult<long>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<long> objResult = new ResponseResult<long>
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

                ResponseResult<long> objResult = new ResponseResult<long>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Faz o upload de uma imagem de cliente para a Nuvem
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost("uploadCloud/{customerId}")]
        public async Task<IActionResult> UploadCloud(IFormFile file, string customerId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (file == null || file.Length == 0)
                    return BadRequest("Nenhuma imagem enviada.");

                // Validação de tipo
                var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!tiposPermitidos.Contains(file.ContentType))
                    return BadRequest("Formato de imagem inválido.");

                // Limite de tamanho (ex: 2MB)
                if (file.Length > 2 * 1024 * 1024)
                    return BadRequest("Imagem maior que 2MB.");

                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                var arrCustomerId = _idEnconder.Decode(customerId);
                int iCustomerId = arrCustomerId.Count > 0 ? arrCustomerId[0] : 0;

                var result = await _cloudimageService.Upload(file, iCustomerId);

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
                ResponseResult<long> objResult = new ResponseResult<long>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<long> objResult = new ResponseResult<long>
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

                ResponseResult<long> objResult = new ResponseResult<long>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Recupera a url de uma imagem para o aplicativo fazer o download da imagem em vez de trazer toda a imagem
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost]
        [Route("GetImageUrlByCustomerId/{customerId}")]
        public async Task<IActionResult> GetImageUrlByCustomerId(string customerId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrCustomerId = _idEnconder.Decode(customerId);
                int iCustomerId = arrCustomerId.Count > 0 ? arrCustomerId[0] : 0;

                var result = await _cloudimageService.GetImageUrlByCustomerId(iCustomerId);

                ResponseResult<string> objResult = new ResponseResult<string>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<string> objResult = new ResponseResult<string>
                {
                    Exchange = "",
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<string> objResult = new ResponseResult<string>
                {
                    Exchange = "",
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<string> objResult = new ResponseResult<string>
                {
                    Exchange = "",
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }


        /// <summary>
        /// Deleta imagem associada ao cliente na Nuvem
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost]
        [Route("DeleteImage/{customerId}")]
        public async Task<IActionResult> DeleteImage(string customerId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrCustomerId = _idEnconder.Decode(customerId);
                int iCustomerId = arrCustomerId.Count > 0 ? arrCustomerId[0] : 0;

                var result = await _cloudimageService.DeleteImage(iCustomerId);

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
        /// Deleta o container do usuário
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost]
        [Route("DeleteContainer")]
        public async Task<IActionResult> DeleteContainer()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _cloudimageService.DeleteContainer();

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
        /// Recupera a url de uma imagem para o aplicativo fazer o download da imagem em vez de trazer toda a imagem
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost]
        [Route("GetCustomerImageList")]
        public async Task<IActionResult> GetCustomerImageList()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _cloudimageService.GetCustomerImageList();

                ResponseResult<List<CustomerImageDto>> objResult = new ResponseResult<List<CustomerImageDto>>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<List<CustomerImageDto>> objResult = new ResponseResult<List<CustomerImageDto>>
                {
                    Exchange = new List<CustomerImageDto>(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<List<CustomerImageDto>> objResult = new ResponseResult<List<CustomerImageDto>>
                {
                    Exchange = new List<CustomerImageDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<List<CustomerImageDto>> objResult = new ResponseResult<List<CustomerImageDto>>
                {
                    Exchange = new List<CustomerImageDto>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        #endregion


        #region Mensagem
        /// <summary>
        /// Inativa um usuário e deleta do azure através do serviço de mensagens
        /// </summary>
        /// <returns>Flag sucesso/erro</returns>
        [HttpPost]
        [Route("InactiveUser/{userId}")]
        public async Task<IActionResult> InactiveUser(string userId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrUserId = _idEnconder.Decode(userId);
                int iUserId = arrUserId.Count > 0 ? arrUserId[0] : 0;

                var result = await _cloudmessageService.DeleteUserNoAzure(iUserId);

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

        #endregion
    }
}
