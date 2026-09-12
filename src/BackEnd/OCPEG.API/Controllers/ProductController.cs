using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OCPEG.API.Filters;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using static OCPEG.Framework.Enums;

namespace OCPEG.Api.Controllers
{
    /// <summary>
    /// Assunto
    /// </summary>    
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class ProductController : ControllerBaseLocal
    {
        private readonly IProductService _productService;
        private readonly ILogger<CallController> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productService"></param>
        /// <param name="logger"></param> 
        /// <param name="mapper"></param>
        public ProductController(IProductService productService, IMapper mapper, ILogger<CallController> logger) : base(mapper)
        {
            _productService = productService;
            _logger = logger;
        }


        /// <summary>
        /// Retorna lista de assunto
        /// </summary>
        /// <returns>Lista de Objeto de negocio Product</returns>
        [AcceptVerbs("GET")]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);
                _logger.LogInformation(Constants.Inicio);

                var result = await _productService.GetProductAll();

                ResponseResult<List<Product>> objResult = new ResponseResult<List<Product>>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<List<Product>> objResult = new ResponseResult<List<Product>>
                {
                    Exchange = new List<Product>(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<List<Product>> objResult = new ResponseResult<List<Product>>
                {
                    Exchange = new List<Product>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<List<Product>> objResult = new ResponseResult<List<Product>>
                {
                    Exchange = new List<Product>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Obtém dados do assunto por id
        /// </summary>
        /// <param name="id">Id do assunto</param>
        /// <returns>Objeto de negocio Product</returns>
        [AcceptVerbs("GET")]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _productService.GetById(id);

                ResponseResult<Product> objResult = new ResponseResult<Product>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<Product> objResult = new ResponseResult<Product>
                {
                    Exchange = new Product(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<Product> objResult = new ResponseResult<Product>
                {
                    Exchange = new Product(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<Product> objResult = new ResponseResult<Product>
                {
                    Exchange = new Product(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Insere um novo assunto
        /// </summary>
        /// <returns>Id do assunto criado</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] Product model)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (model == null) return BadRequest();

                var result = await _productService.Create(model);

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

                ResponseResult<int> objResult = new ResponseResult<int>
                {
                    Exchange = 0,
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Atualiza um assunto
        /// </summary>
        /// <param name="model">Dados do assunto</param> 
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] Product model)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (model == null) return BadRequest();

                var result = await _productService.Update(model);

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
        /// Exclui um assunto
        /// </summary>
        /// <param name="id">Id do assunto</param>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (id == 0) return BadRequest();

                var result = await _productService.Delete(id);

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
