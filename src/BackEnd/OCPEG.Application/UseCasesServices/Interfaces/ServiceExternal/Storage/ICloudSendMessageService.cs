using OCPEG.Domain.BusinessObject;

namespace OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage
{
    public interface ICloudSendMessageService
    {
        Task SendMessage(User user);
    }
}
