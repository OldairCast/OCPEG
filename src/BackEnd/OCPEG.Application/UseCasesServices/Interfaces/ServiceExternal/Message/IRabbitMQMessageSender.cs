using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.BusinessObject.Message;

namespace OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message
{
    public interface IRabbitMQMessageSender
    {
        Task SendMessageAsync(EmailHeaderVO message, string queueName);

    }
}
