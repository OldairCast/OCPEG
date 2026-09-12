using AutoMapper;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;
using static OCPEG.Framework.Enums;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerEfRepository _customerEFRepository;
        private readonly string _repository = string.Empty;

        public CustomerService(ICustomerRepository customerRepository, ICustomerEfRepository customerEFRepository)
        {
            _customerRepository = customerRepository;
            _customerEFRepository = customerEFRepository;
        }

        /// <summary>
        /// Obtém dados do Cliente por id
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <returns>Objeto de negocio CustomerDto</returns>
        public async Task<Customer> GetById(int id)
        {
            try
            {
                Customer customer;

                if (_repository == "EF")
                {
                    customer = await _customerEFRepository.GetById(id);
                }
                else
                {
                    customer = await _customerRepository.GetById(id);
                }

                return customer;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Retorna lista de Clientes
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerDto</returns>
        public async Task<List<Customer>> GetAllCustomer()
        {
            try
            {
                List<Customer> customers = _repository == "EF"
                                          ? await _customerEFRepository.GetAll() 
                                          : await _customerRepository.GetAll();

                return customers;
            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Obtém Lista de clientes pela busca por nome
        /// </summary>
        /// <param name="id">Id do cliente</param>
        /// <returns>Lista de clientes</returns>
        public async Task<List<ComboOption>> GetByNameCbo(string search)
        {
            try
            {
                List<ComboOption> customers = await _customerRepository.GetByNameCbo(search);

                return customers;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Insere um novo Cliente
        /// </summary>
        /// <returns>Id do Objeto Cliente criado</returns>
        public async Task<long> CreateCustomer(Customer customer)
        {
            try
            {
                Validate(customer);
                
                long customerId;

                if (_repository == "EF")
                {
                    customerId = await _customerEFRepository.Create(customer);
                }
                else
                {
                    customerId = await _customerRepository.Create(customer);
                }

                return customerId;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Exclui um Cliente
        /// </summary>
        /// <param name="id">Id do Cliente</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                if (_repository == "EF")
                {
                    await _customerEFRepository.Delete(id);
                }
                else
                {
                    await _customerRepository.Delete(id);
                }

                return true;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um Cliente
        /// </summary>
        /// <param name="customer">Dados do Cliente</param> 
        public async Task<bool> Update(Customer customer)
        {
            try
            {
                Validate(customer);

                if (_repository == "EF")
                {
                    await _customerEFRepository.Update(customer);
                }
                else
                {
                    await _customerRepository.Update(customer);
                }

                return true;

            }
            catch (BusinessLogicCustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NLogManager.LogError($"{ex}");
                throw new OcException(ex.Message);
            }
        }

        /// <summary>
        /// Valida as informações da requisição
        /// </summary>
        private static void Validate(Customer customer)
        {
            if (string.IsNullOrEmpty(customer.Name))
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.NAME));
            }

            if (string.IsNullOrEmpty(customer.Email))
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.EMAIL));
            }
        }


        public Task<List<CustomerDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<int> Create(Customer entity)
        {
            throw new NotImplementedException();
        }
    }
}