using OCPEG.Domain.BusinessObject.Common;

namespace OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message
{
    public interface IRabittMQMessageService
    {
        Task PublicMessage(BaseMessage message, string queueName);
    }
}
