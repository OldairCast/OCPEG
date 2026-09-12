using OCPEG.Domain.BusinessObject;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface ICustomerImageRepository
    {
        Task<long> Upload(CustomerImage image);
    }
}
