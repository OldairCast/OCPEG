using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.Dto;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface IProductService : IBaseService<Product, ProductDto>
    {
        Task<List<Product>> GetProductAll();
    }
}
