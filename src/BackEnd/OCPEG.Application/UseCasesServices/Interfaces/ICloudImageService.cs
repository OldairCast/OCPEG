using Microsoft.AspNetCore.Http;
using OCPEG.Domain.Dto;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface ICloudImageService
    {
        Task<bool> Upload(IFormFile file, int customerId);

        Task<string> GetImageUrlByCustomerId(int customerId);

        Task<bool> DeleteImage(int customerId);

        Task<bool> DeleteContainer();

        Task<List<CustomerImageDto>> GetCustomerImageList();
    }
}
