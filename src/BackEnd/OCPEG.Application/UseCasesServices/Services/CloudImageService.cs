using AutoMapper;
using Microsoft.AspNetCore.Http;
using OCPEG.Application.Extension;
using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Domain.Dto;
using OCPEG.Framework;
using OCPEG.Framework.Exception;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class CloudImageService : ICloudImageService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerEfRepository _customerEFRepository;
        private readonly string _repository = string.Empty;
        private readonly IMapper _mapper;


        public CloudImageService(
            IBlobStorageService blobStorageService,
            IAccountService accountService,
            ICustomerService customerService,
            ICustomerRepository customerRepository, 
            ICustomerEfRepository customerEFRepository, IMapper mapper)
        {
            _blobStorageService = blobStorageService;
            _accountService = accountService;
            _customerService = customerService;
            _customerRepository = customerRepository;
            _customerEFRepository = customerEFRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Faz o upload de uma imagem de cliente para a Nuvem
        /// </summary>
        /// <param name="file"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<bool> Upload(IFormFile file, int customerId)
        {
            try
            {
                var objUser = await _accountService.GetUserLogged();
                var user = objUser;

                Customer objCustomer = await _customerService.GetById(customerId);
                if (objCustomer == null)
                {
                    throw new BusinessLogicCustomException("Cliente não encontrado");
                }

                var fileStream = file.OpenReadStream();

                (var isValidImage, var extension) = fileStream.ValidateAndGetImageExtension();

                if (!isValidImage)
                {
                    throw new BusinessLogicCustomException(String.Format("{0} {1}","Tipo de Imagem incorreta:", extension));
                }

                await _blobStorageService.Upload(user, fileStream, customerId.ToString());

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
        /// Recupera a url de uma imagem para o aplicativo fazer o download da imagem em vez de trazer toda a imagem
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<string> GetImageUrlByCustomerId(int customerId)
        {
            try
            {
                var user = await _accountService.GetUserLogged();
                string url = "";

                Customer objCustomer = await _customerService.GetById(customerId);
                if (objCustomer == null)
                {
                    throw new BusinessLogicCustomException("Cliente não encontrado");
                }

                if (!string.IsNullOrEmpty(user.UserIdentifier.ToString()))
                {
                    url = await _blobStorageService.GetFileUrl(user, customerId.ToString());
                }

                return url;
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
        /// Deleta imagem associada ao cliente
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteImage(int customerId)
        {
            try
            {
                var user = await _accountService.GetUserLogged();

                Customer objCustomer = await _customerService.GetById(customerId);
                if (objCustomer == null)
                {
                    throw new BusinessLogicCustomException("Cliente não encontrado");
                }

                if (!string.IsNullOrEmpty(user.UserIdentifier.ToString()))
                {
                   await _blobStorageService.Delete(user, customerId.ToString());
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
        /// Deleta o container do usuário
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteContainer()
        {
            try
            {
                var user = await _accountService.GetUserLogged();

                if (!string.IsNullOrEmpty(user.UserIdentifier.ToString()))
                {
                    await _blobStorageService.DeleteContainer(user.UserIdentifier);
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
        /// Traz uma lista de clientes com a url da imagem associada
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<List<CustomerImageDto>> GetCustomerImageList()
        {
            try
            {
                List<Customer> customers = _repository == "EF"
                    ? await _customerEFRepository.GetAll()
                    : await _customerRepository.GetParse();

                var user = await _accountService.GetUserLogged();

                List<CustomerImageDto> customerImage = await _blobStorageService.MapCustomerImageUrl(customers, user, _mapper);

                return customerImage;
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
    }
}
