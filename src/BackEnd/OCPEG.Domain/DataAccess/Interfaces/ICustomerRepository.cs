using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.DataAccess.Base;

namespace OCPEG.Domain.DataAccess.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<Customer>> GetParse();
        Task<List<ComboOption>> GetByNameCbo(string search);

    }
}
