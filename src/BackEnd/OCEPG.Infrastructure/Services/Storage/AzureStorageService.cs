using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;

namespace OCEPG.Infrastructure.Services.Storage
{
    public class AzureStorageService: IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        /// <summary>
        /// Faz o upload de uma imagem de cliente para o Azure
        /// </summary>
        /// <param name="user"></param>
        /// <param name="file"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task Upload(User user, Stream file, string customerId)
        {
            var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(customerId);

            await blobClient.UploadAsync(file, overwrite: true);
        }

        /// <summary>
        /// Recupera a url de uma imagem para o aplicativo fazer o download da imagem em vez de trazer toda a imagem
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<string> GetFileUrl(User user, string customerId)
        {
            var containerName = user.UserIdentifier.ToString();

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var exist = await containerClient.ExistsAsync();
            if (!exist)
                return string.Empty;

            var blobClient = containerClient.GetBlobClient(customerId);
            exist = await blobClient.ExistsAsync();
            if (exist.Value)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = customerId.ToString(),
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10),
                };

                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Deleta imagem associada ao cliente
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task Delete(User user, string customerId)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
            var exist = await containerClient.ExistsAsync();
            if (exist.Value)
            {
                await containerClient.DeleteBlobIfExistsAsync(customerId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIdentifier"></param>
        /// <returns></returns>
        public async Task DeleteContainer(Guid userIdentifier)
        {
            var container = _blobServiceClient.GetBlobContainerClient(userIdentifier.ToString());
            await container.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Retorna lista de clientes com imagem
        /// </summary>
        /// <param name="user"></param>   
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<List<CustomerImageDto>> MapCustomerImageUrl(List<Customer> customers, User user, IMapper _mapper)
        {
            var result = customers.Select(async customer =>
            {
                var response = _mapper.Map<CustomerImageDto>(customer);
                response.ImageUrl = await GetFileUrl(user, response.Id);

                return response;
            });

            var response = await Task.WhenAll(result);

            return response.ToList();
        }
    }
}
