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
    /// Tipo de Cliente
    /// </summary>   
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class CustomerTypeController : ControllerBaseLocal
    {
        private readonly ICustomerTypeService _customerTypeService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerTypeService"></param>
        /// <param name="mapper"></param>
        public CustomerTypeController(ICustomerTypeService customerTypeService, IMapper mapper) : base(mapper)
        {
            _customerTypeService = customerTypeService;
        }


        /// <summary>
        /// Retorna lista de Tipo de Cliente
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerTypeDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _customerTypeService.GetAll();
                
                ResponseResult<List<CustomerType>> objResult = new ResponseResult<List<CustomerType>>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<List<CustomerType>> objResult = new ResponseResult<List<CustomerType>>
                {
                    Exchange = new List<CustomerType>(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<List<CustomerType>> objResult = new ResponseResult<List<CustomerType>>
                {
                    Exchange = new List<CustomerType>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<List<CustomerType>> objResult = new ResponseResult<List<CustomerType>>
                {
                    Exchange = new List<CustomerType>(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Obtém dados do Tipo de Cliente por id
        /// </summary>
        /// <param name="id">Id do tipo de cliente</param>
        /// <returns>Objeto de negocio CustomerType</returns>
        [AcceptVerbs("GET")]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _customerTypeService.GetById(id);

                ResponseResult<CustomerType> objResult = new ResponseResult<CustomerType>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<CustomerType> objResult = new ResponseResult<CustomerType>
                {
                    Exchange = new CustomerType(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<CustomerType> objResult = new ResponseResult<CustomerType>
                {
                    Exchange = new CustomerType(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<CustomerType> objResult = new ResponseResult<CustomerType>
                {
                    Exchange = new CustomerType(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }


        /// <summary>
        /// Insere um novo Tipo de Cliente
        /// </summary>
        /// <returns>Id do Tipo de Cliente criado</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CustomerType model)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (model == null) return BadRequest();

                var result = await _customerTypeService.Create(model);

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
        /// Atualiza um Tipo de Cliente
        /// </summary>
        /// <param name="customer">Dados do Tipo de Cliente</param> 
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] CustomerType customer)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (customer == null) return BadRequest();

                var result = await _customerTypeService.Update(customer);

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
        /// Exclui um Tipo de Cliente
        /// </summary>
        /// <param name="id">Id do Tipo de Cliente</param>
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (id == 0) return BadRequest();

                var result = await _customerTypeService.Delete(id);

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
