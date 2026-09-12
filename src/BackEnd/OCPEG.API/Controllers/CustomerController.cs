using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Amqp.Transaction;
using OCPEG.API.Filters;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using Sqids;
using static OCPEG.Framework.Enums;


namespace OCPEG.Api.Controllers
{
    /// <summary>
    /// Cliente
    /// </summary>    
    [ServiceFilter(typeof(LogFilter))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogFilter))]
    public class CustomerController : ControllerBaseLocal
    {
        private readonly ICustomerService _customerService;
        private readonly SqidsEncoder<int> _idEnconder;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerService"></param>
        /// <param name="mapper"></param>
        /// <param name="idEnconder"></param>
        public CustomerController(ICustomerService customerService, IMapper mapper, SqidsEncoder<int> idEnconder) : base(mapper)
        {
            _customerService = customerService;
            _idEnconder = idEnconder;
        }

        /// <summary>
        /// Retorna lista de clientes
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var customers = await _customerService.GetAllCustomer();
                List<CustomerDto> customersDto = new List<CustomerDto>();

                if (customers != null && customers.Count > 0)
                {
                    foreach (var item in customers)
                    {
                        CustomerDto customerDto = _mapper.Map<CustomerDto>(item);
                        customersDto.Add(customerDto);
                    }
                }
                
                ResponseResult<List<CustomerDto>> objResult = new ResponseResult<List<CustomerDto>>
                {
                    Exchange = customersDto,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<Customer> objResult = new ResponseResult<Customer>
                {
                    Exchange = new Customer(),
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
        /// Obtém dados do cliente por id
        /// </summary>
        /// <param name="customerId">Id do cliente</param>
        /// <returns>Objeto de negocio CustomerDto</returns>
        [AcceptVerbs("GET")]
        [Route("GetById/{customerId}")]
        public async Task<IActionResult> GetById(string customerId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrCustomerId = _idEnconder.Decode(customerId);
                int iUserId = arrCustomerId.Count > 0 ? arrCustomerId[0] : 0;

                CustomerDto customerDto = new CustomerDto();

                var customer = await _customerService.GetById(iUserId);
                if (customer != null)
                {
                    customerDto = _mapper.Map<CustomerDto>(customer);
                }
                
                ResponseResult<CustomerDto> objResult = new ResponseResult<CustomerDto>
                {
                    Exchange = customerDto,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<CustomerDto> objResult = new ResponseResult<CustomerDto>
                {
                    Exchange = new CustomerDto(),
                    Status = ResponseResultStatus.NotValidate,
                    Message = OCPegBadRequest(ex)
                };

                return StatusCode(StatusCodes.Status400BadRequest, objResult);
            }
            catch (OcException)
            {
                ResponseResult<CustomerDto> objResult = new ResponseResult<CustomerDto>
                {
                    Exchange = new CustomerDto(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");

                ResponseResult<CustomerDto> objResult = new ResponseResult<CustomerDto>
                {
                    Exchange = new CustomerDto(),
                    Status = ResponseResultStatus.Error,
                    Message = Constants.OcorreuErro
                };

                return StatusCode(StatusCodes.Status500InternalServerError, objResult);
            }
        }

        /// <summary>
        /// Obtém lista de clientes por nome
        /// </summary>
        /// <param name="search">Nome do cliente</param>
        /// <returns>Objeto de negocio ComboOption</returns>
        [AcceptVerbs("GET")]
        [Route("GetByName/{search}")]
        public async Task<IActionResult> GetByName(string search)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var result = await _customerService.GetByNameCbo(search);
                ResponseResult<List<ComboOption>> objResult = new ResponseResult<List<ComboOption>>
                {
                    Exchange = result,
                    Status = ResponseResultStatus.Success,
                    Message = Constants.OperacaoRealizadaSucesso
                };

                return Ok(objResult);

            }
            catch (BusinessLogicCustomException ex)
            {
                ResponseResult<Customer> objResult = new ResponseResult<Customer>
                {
                    Exchange = new Customer(),
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
        /// Insere um novo cliente
        /// </summary>
        /// <returns>Id do cliente criado</returns>
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] CustomerDto customerDto)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (customerDto == null) return BadRequest();

                Customer customer = _mapper.Map<Customer>(customerDto);
                var result = await _customerService.CreateCustomer(customer);

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
        /// Atualiza um cliente
        /// </summary>
        /// <param name="customer">Dados do cliente</param> 
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] CustomerDto customer)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                if (customer == null) return BadRequest();

                Customer customerB = _mapper.Map<Customer>(customer);

                var result = await _customerService.Update(customerB);

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
                ResponseResult<Customer> objResult = new ResponseResult<Customer>
                {
                    Exchange = new Customer(),
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
        /// Exclui um cliente
        /// </summary>
        /// <param name="customerId">Id do cliente</param>
        [HttpDelete]
        [Route("Delete/{customerId}")]
        public async Task<IActionResult> Delete(string customerId)
        {
            try
            {
                NLogManager.LogInfo(Constants.Inicio);

                var arrCustomerId = _idEnconder.Decode(customerId);
                int iCustomerId = arrCustomerId.Count > 0 ? arrCustomerId[0] : 0;

                var result = await _customerService.Delete(iCustomerId);

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
                ResponseResult<Customer> objResult = new ResponseResult<Customer>
                {
                    Exchange = new Customer(),
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

    }

}
