using OCPEG.Application.Communication.Requests;
using OCPEG.Application.Communication.Responses;
using OCPEG.Domain.BusinessObject;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface ICallService
    {
        Task<List<Call>> GetAll();

        Task<List<Call>> GetbyParams(CallRequest request);

        Task<CallResponse> Create(Call call);

        Task<bool> Update(Call call);
    }
}


