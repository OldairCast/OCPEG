using AutoMapper;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;

namespace OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage
{
    public interface IBlobStorageService
    {
        Task Upload(User user, Stream file, string customerId);
        Task<string> GetFileUrl(User user, string customerId);
        Task Delete(User user, string customerId);
        Task DeleteContainer(Guid userIdentifier);
        Task<List<CustomerImageDto>> MapCustomerImageUrl(List<Customer> customers, User user, IMapper _mapper);
    }
}
