using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface ICustomerService: IBaseService<Customer, CustomerDto>
    {
        Task<List<ComboOption>> GetByNameCbo(string search);

        Task<List<Customer>> GetAllCustomer();

        Task<long> CreateCustomer(Customer customer);
    }
}
