using OCPEG.Domain.BusinessObject;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface ICustomerImageService
    {
        Task<long> Upload(CustomerImage image);
    }
}
