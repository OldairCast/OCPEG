using OCPEG.Domain.DataAccess.Base;
using OCPEG.Domain.BusinessObject;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface ICustomerTypeRepository : IBaseRepository<CustomerType>
    {
        public string? Name { get; set; }
    }
}
