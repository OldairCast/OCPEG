using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using OCPEG.Framework.Exception;
using OCPEG.Framework.Resource;
using static OCPEG.Framework.Enums;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ICustomerTypeRepository _customerRepository;

        public CustomerTypeService(ICustomerTypeRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Obtém dados do Tipo de Cliente por id
        /// </summary>
        /// <param name="id">Id do tipo de cliente</param>
        /// <returns>Objeto de negocio CustomerType</returns>
        public async Task<CustomerType> GetById(int id)
        {
            try
            {
                CustomerType customerT = await _customerRepository.GetById(id);
                
                return customerT;

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
        /// Retorna lista de Tipo de Cliente
        /// </summary>
        /// <returns>Lista de Objeto de negocio CustomerType</returns>
        public async Task<List<CustomerType>> GetAll()
        {
            try
            {
                List<CustomerType> customersType = await _customerRepository.GetAll();

                return customersType;

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
        /// Insere um novo Tipo de Cliente
        /// </summary>
        /// <returns>Objeto Tipo de Cliente criado</returns>
        public async Task<int> Create(CustomerType entity)
        {
            try
            {
                Validate(entity);

                var result =  await _customerRepository.Create(entity);
                return result;

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
        /// Atualiza um Tipo de Cliente
        /// </summary>
        /// <param name="customer">Dados do Tipo de Cliente</param> 
        public async Task<bool> Update(CustomerType entity)
        {
            try
            {
                Validate(entity);

                await _customerRepository.Update(entity);

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
        /// Exclui um Tipo de Cliente
        /// </summary>
        /// <param name="id">Id do Tipo de Cliente</param>
        public async Task<bool> Delete(int id)
        {
            try
            {
                await _customerRepository.Delete(id);

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
        private static void Validate(CustomerType customertype)
        {
            if (string.IsNullOrEmpty(customertype.Name))
            {
                throw new BusinessLogicCustomException(string.Format("{0} {1}", OcPegResource.ENTER_THE, OcPegResource.NAME));
            }
        }

    }

}

