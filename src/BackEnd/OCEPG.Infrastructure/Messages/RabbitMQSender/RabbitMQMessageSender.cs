using Microsoft.Extensions.Options;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message;
using OCPEG.Domain.BusinessObject.Common;
using OCPEG.Domain.BusinessObject.Message;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace OCEPG.Infrastructure.Messages.RabbitMQSender
{
    /// <summary>
    /// implementa um produtor (Publisher) de mensagens para o RabbitMQ. 
    /// A responsabilidade dele é conectar-se ao broker, 
    /// garantir que uma fila exista e publicar uma mensagem nela.
    /// </summary>
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private IConnection? _connection;

        public RabbitMQMessageSender(IOptions<RabbitMQSettings> options)
        {
            var settings = options.Value;

            _hostName = settings.HostName;
            _userName = settings.UserName;
            _password = settings.Password;
        }

        /// <summary>
        /// Envia a mensagem
        /// </summary>
        /// <param name="message"></param>
        /// <param name="queueName"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task SendMessageAsync(EmailHeaderVO message, string queueName)
        {
            //Verifica se existe uma conexão aberta com o RabbitMQ?
            if (!await ConnectionExistsAsync())
                throw new InvalidOperationException("Não foi possível conectar ao RabbitMQ.");

            await using var channel = await _connection!.CreateChannelAsync();

            //Aqui ele garante que a fila exista.
            ///Se a fila já existir:
            ///✔ não acontece nada.
            ///Se não existir:
            ///✔ ela é criada.
            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,     //A fila não sobrevive ao reinício do RabbitMQ.
                exclusive: false,   //A fila pode ser utilizada por várias conexões.
                autoDelete: false,  //A fila não será apagada automaticamente.
                arguments: null);

            //Converte a mensagem para bytes
            byte[] body = GetMessageAsByteArray(message);

            //Esse é o momento em que a mensagem é enviada ao RabbitMQ.
            await channel.BasicPublishAsync(
                exchange: "", //"" => Default Exchange. O RabbitMQ possui uma exchange padrão embutida.
                routingKey: queueName,
                body: body);
        }

        //Converte a mensagem para bytes
        private byte[] GetMessageAsByteArray(EmailHeaderVO message)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(message, options);

            return Encoding.UTF8.GetBytes(json);
        }

        //Cria uma conexão com RabbitMQ
        private async Task CreateConnectionAsync()
        {
            if (_connection?.IsOpen == true)
                return;

            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                Port = 5672,
                VirtualHost = "/",
                UserName = _userName,
                Password = _password,
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30)
            };

            //É aberta uma conexão TCP com o servidor RabbitMQ.
            //Essa conexão permanece aberta para ser reutilizada.
            _connection = await factory.CreateConnectionAsync();
        }

        private async Task<bool> ConnectionExistsAsync()
        {
            if (_connection?.IsOpen == true)
                return true;

            //Criação do Channel
            await CreateConnectionAsync();

            return _connection?.IsOpen == true;
        }
    }
}
