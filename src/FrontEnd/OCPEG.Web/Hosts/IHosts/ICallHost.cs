using OCPEG.Application.Communication.Requests;
using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.Dto;
using OCPEG.Framework;

namespace OCPEG.Web.Services.IServices
{
    public interface ICallHost
    {
        #region Atendimento
        Task<ResponseResult<List<CallDto>>> GetCallAll(string token);

        Task<ResponseResult<List<CallDto>>> GetCallbyParams(string token, CallRequest request);

        Task<ResponseResult<Call>> CreateCall(string token, CallRequest callRequest);

        Task<ResponseResult<bool>> UpdateCall(string token, CallRequest callRequest);
        #endregion


        #region Resolução
        Task<ResponseResult<bool>> CreateResolution(string token, Resolution resolution);

        Task<ResponseResult<Resolution>> GetResolutionbyParams(string token, string callNumber);

        Task<ResponseResult<bool>> SendMessageAsync(string token, Message message);

        Task<ResponseResult<bool>> ReadMessageAsync(string token);
        #endregion


        #region Assunto
        Task<ResponseResult<List<Product>>> GetProductAll(string token);

        Task<ResponseResult<int>> CreateProduct(string token, Product product);

        Task<ResponseResult<Product>> GetProductById(string token, int id);

        Task<ResponseResult<bool>> UpdateProduct(string token, Product product);

        Task<ResponseResult<bool>> DeleteProductById(string token, int id);
        #endregion
    }
}
