using OCPEG.Domain.BusinessObject;
using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Application.UseCasesServices.Interfaces
{
    public interface IResolutionService
    {
        Task<Resolution> GetByCallNumber(string callNumber);

        Task<bool> Insert(Resolution resolution);

        Task<bool> SendMessage(Message message);

        Task<bool> ReadMessage();

    }
}
