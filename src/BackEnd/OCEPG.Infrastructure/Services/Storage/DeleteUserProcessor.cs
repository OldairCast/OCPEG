using Azure.Messaging.ServiceBus;

namespace OCEPG.Infrastructure.Services.Storage
{

    /// <summary>
    /// Classe que lê as mensagens da fila de user
    /// </summary>
    public class DeleteUserProcessor
    {
        private readonly ServiceBusProcessor _processor;

        public DeleteUserProcessor(ServiceBusProcessor processor)
        {
            _processor = processor;
        }

        public ServiceBusProcessor GetProcessor() => _processor;
    }
}
