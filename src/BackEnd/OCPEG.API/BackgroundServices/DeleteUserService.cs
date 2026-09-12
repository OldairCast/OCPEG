using Azure.Messaging.ServiceBus;
using OCEPG.Infrastructure.Services.Storage;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Storage;

namespace OCPEG.API.BackgroundServices
{
    /// <summary>
    /// Classe que executa em segundo plano a leitura da fila
    /// </summary>
    public class DeleteUserService : BackgroundService
    {
        //Toda classe que tiver uma herança com BackgroundService será executada em segundo plano

        private readonly IServiceProvider _services;
        private readonly ServiceBusProcessor _processor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="processor"></param>
        public DeleteUserService(IServiceProvider services, DeleteUserProcessor processor)
        {
            _processor = processor.GetProcessor();
            _services = services;
        }

        /// <summary>
        /// Função que será implementada por herança do BackgroundService
        /// _processor.ProcessMessageAsync irá executar a minha função toda vez que ele receber uma mensagem da fila
        /// Nesse caso, a minha função é ProcessMessageAsync
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;

            _processor.ProcessErrorAsync += ExceptionReceivedHandler;

            //A linha abaixo significa que => agora você irá começar a ouvir a fila e executar a função sempre que executar uma mensagem
            await _processor.StartProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs eventArgs)
        {
            var message = eventArgs.Message.Body.ToString();

            var userIdentifier = int.Parse(message);

            var scope = _services.CreateScope();

            var deleteUserUseCase = scope.ServiceProvider.GetRequiredService<ICloudMessageService>();

            await deleteUserUseCase.DeleteUser(userIdentifier);
        }

        private static Task ExceptionReceivedHandler(ProcessErrorEventArgs _) => Task.CompletedTask;


        /// <summary>
        /// Liberar recursos da memória
        /// </summary>
        ~DeleteUserService() => Dispose();

        /// <inheritdoc/>
        public override void Dispose()
        {
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
