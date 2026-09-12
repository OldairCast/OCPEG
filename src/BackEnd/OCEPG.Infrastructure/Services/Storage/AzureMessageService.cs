using Azure.Messaging.ServiceBus;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;
using OCPEG.Domain.BusinessObject;

namespace OCEPG.Infrastructure.Services.Storage
{
    public class AzureMessageService: ICloudSendMessageService
    {
        private readonly ServiceBusSender _serviceBusSender;

        public AzureMessageService(ServiceBusSender serviceBusSender)
        {
            _serviceBusSender = serviceBusSender;
        }

        public async Task SendMessage(User user)
        {
            await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(user.UserIdentifier.ToString()));
        }
    }
}
