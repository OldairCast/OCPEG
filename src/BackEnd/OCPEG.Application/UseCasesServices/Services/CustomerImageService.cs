using OCPEG.Application.UseCasesServices.Interfaces;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.DataAccess.Interfaces;
using OCPEG.Framework;
using OCPEG.Framework.Exception;

namespace OCPEG.Application.UseCasesServices.Services
{
    public class CustomerImageService : ICustomerImageService
    {
        private readonly ICustomerImageRepository _imageRepository;

        public CustomerImageService(ICustomerImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        /// <summary>
        /// Insere uma imagem
        /// </summary>
        /// <returns>Id do Objeto Cliente criado</returns>
        public async Task<long> Upload(CustomerImage image)
        {
            try
            {
                long imageId = await _imageRepository.Upload(image);

                return imageId;
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
