using OCPEG.Domain.DataAccess.Base;
using OCPEG.Domain.BusinessObject;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface ICustomerEfRepository : IBaseRepository<Customer>
    {
        new Task<long> Create(Customer customer);
    }
}
