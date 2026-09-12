using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Framework;

namespace OCPEG.Web.Services.IServices
{
    public interface ICustomerHost
    {
        #region Cliente
        Task<ResponseResult<List<CustomerDto>>> GetCustomerAll(string token);

        Task<ResponseResult<long>> CreateCustomer(string token, CustomerDto customer);

        Task<ResponseResult<CustomerDto>> GetCustomerById(string token, string customerId);
        
        Task<ResponseResult<List<ComboOption>>> GetCustomerByName(string token, string name);

        Task<ResponseResult<bool>> UpdateCustomer(string token, CustomerDto customer);

        Task<ResponseResult<bool>> DeleteCustomerById(string token, string customerId);
        #endregion


        #region Tipo de Cliente
        Task<ResponseResult<List<CustomerType>>> GetCustomerTypeAll(string token);

        Task<ResponseResult<int>> CreateCustomerType(string token, CustomerType customerType);

        Task<ResponseResult<CustomerType>> GetCustomerTypeById(string token, int id);

        Task<ResponseResult<bool>> UpdateCustomerType(string token, CustomerType customerType);

        Task<ResponseResult<bool>> DeleteCustomerTypeById(string token, int id);
        #endregion
    }
}
