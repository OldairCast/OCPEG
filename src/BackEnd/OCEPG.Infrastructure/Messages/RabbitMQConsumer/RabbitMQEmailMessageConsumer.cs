using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OCPEG.Application.UseCasesServices.Interfaces.ServiceExternal.Message;
using OCPEG.Domain.BusinessObject.Message;
using OCPEG.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

//implementa um Consumer RabbitMQ como um BackgroundService do .NET, ou seja,
//um processo que fica rodando em segundo plano dentro da aplicação,
//conectado ao RabbitMQ, aguardando mensagens na fila checkoutqueueEmail.

//Portanto, a responsabilidade dessa classe é:
//Consumir mensagens da fila checkoutqueueEmail, processá-las e confirmar ou rejeitar cada mensagem de acordo com o resultado.
public sealed class RabbitMQEmailMessageConsumer : BackgroundService
{
    private const string QueueName = "checkoutqueueEmail";

    //Exchange é o componente que recebe mensagens publicadas e decide para quais filas elas serão encaminhadas.
    private const string ExchangeName = "checkout.email.exchange";

    //A Routing Key ajuda o Exchange a decidir para qual fila a mensagem deve ser encaminhada.
    private const string RoutingKey = "checkout.email";

    //DLX significa: Dead Letter Exchange - É um Exchange utilizado para mensagens que não puderam ser processadas normalmente.
    private const string DeadLetterExchangeName = "checkout.email.dlx";

    //DLQ significa: Dead Letter Queue - É onde as mensagens rejeitadas podem ficar armazenadas. Isso é muito útil para análise posterior.
    private const string DeadLetterQueueName = "checkoutqueueEmail.dlq";

    //É a Routing Key usada quando a mensagem é encaminhada para a estrutura de Dead Letter.
    private const string DeadLetterRoutingKey = "checkout.email.dead";

    private readonly RabbitMQSettings _settings;
    //private readonly IRabbitMQMessageSender _rabbitMQMessageSender;

    private IConnection? _connection;
    private IChannel? _channel;

    private string? _consumerTag;

    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMQEmailMessageConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMQSettings> options)
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }


    //public RabbitMQEmailMessageConsumer(
    //    IRabbitMQMessageSender rabbitMQMessageSender,
    //    IOptions<RabbitMQSettings> options)
    //{
    //    _rabbitMQMessageSender = rabbitMQMessageSender;
    //    _settings = options.Value;
    //}

    //É o método principal executado pelo BackgroundService.
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await StartRabbitMqAsync(stoppingToken);

                await ConsumeAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                NLogManager.LogError(
                    $"Erro no consumidor RabbitMQ. " +
                    $"Queue={QueueName}. Exception={ex}");

                //Liberação dos recursos
                await DisposeRabbitMqAsync();

                if (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        //Espera 5 segundos. Depois o while volta ao início.
                        // Falha
                        // ↓
                        //Dispose
                        // ↓
                        //5 segundos
                        // ↓
                        //Tenta conectar
                        // ↓
                        //Falha ?
                        // ↓
                        //5 segundos
                        // ↓
                        //Tenta novamente
                        await Task.Delay(
                            TimeSpan.FromSeconds(5),
                            stoppingToken);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            }
        }
    }

    //Criar a conexão, criar o channel e configurar a infraestrutura necessária no RabbitMQ.
    private async Task StartRabbitMqAsync(
        CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password,

            Port = _settings.Port,
            VirtualHost = _settings.VirtualHost,

            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };

        NLogManager.LogInfo(
            $"Conectando ao RabbitMQ. Host={_settings.HostName}");

        _connection = await factory.CreateConnectionAsync(
            cancellationToken);

        _channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);

        await ConfigureTopologyAsync(
            _channel,
            cancellationToken);

        /*
         * Limita a quantidade de mensagens não confirmadas.
         *
         * Com prefetch = 1:
         * o consumidor recebe uma mensagem,
         * processa,
         * faz ACK,
         * e somente então recebe outra.
         */
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false,
            cancellationToken: cancellationToken);

        NLogManager.LogInfo(
            $"RabbitMQ conectado. Queue={QueueName}");
    }

    private static async Task ConfigureTopologyAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {
        // ============================================================
        // 1. Exchange principal
        // ============================================================

        await channel.ExchangeDeclareAsync(
            exchange: ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);


        // ============================================================
        // 2. Dead Letter Exchange
        // ============================================================

        await channel.ExchangeDeclareAsync(
            exchange: DeadLetterExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);


        // ============================================================
        // 3. Dead Letter Queue
        // ============================================================

        await channel.QueueDeclareAsync(
            queue: DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);


        // ============================================================
        // 4. Binding da DLQ -> DLX
        // ============================================================

        await channel.QueueBindAsync(
            queue: DeadLetterQueueName,
            exchange: DeadLetterExchangeName,
            routingKey: DeadLetterRoutingKey,
            cancellationToken: cancellationToken);


        // ============================================================
        // 5. Queue principal
        //
        // Mensagens rejeitadas com requeue=false serão encaminhadas
        // para o Dead Letter Exchange.
        // ============================================================

        var queueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = DeadLetterExchangeName,
            ["x-dead-letter-routing-key"] = DeadLetterRoutingKey
        };

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments,
            cancellationToken: cancellationToken);


        // ============================================================
        // 6. Binding da Queue principal -> Exchange principal
        // ============================================================

        await channel.QueueBindAsync(
            queue: QueueName,
            exchange: ExchangeName,
            routingKey: RoutingKey,
            cancellationToken: cancellationToken);
    }


    private async Task ConsumeAsync(
        CancellationToken stoppingToken)
    {
        if (_channel is null)
            throw new InvalidOperationException(
                "RabbitMQ channel não foi inicializado.");

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            await ProcessMessageAsync(
                eventArgs,
                stoppingToken);
        };

        _consumerTag = await _channel.BasicConsumeAsync(
            queue: QueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        NLogManager.LogInfo(
            $"Consumer RabbitMQ iniciado. " +
            $"Queue={QueueName}, ConsumerTag={_consumerTag}");

        /*
         * O consumer é baseado em eventos.
         *
         * Mantemos o ExecuteAsync vivo enquanto
         * o serviço estiver rodando.
         */
        await Task.Delay(
            Timeout.InfiniteTimeSpan,
            stoppingToken);
    }

    private async Task ProcessMessageAsync(
        BasicDeliverEventArgs eventArgs,
        CancellationToken stoppingToken)
    {
        if (_channel is null)
            return;

        try
        {
            var content = Encoding.UTF8.GetString(
                eventArgs.Body.Span);

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new InvalidOperationException(
                    "Mensagem RabbitMQ vazia.");
            }

            var message = JsonSerializer.Deserialize<EmailHeaderVO>(
                content);

            if (message is null)
            {
                throw new JsonException(
                    "Não foi possível desserializar EmailHeaderVO.");
            }

            /*
             * Só fazemos ACK depois que o processamento
             * foi concluído com sucesso.
             */
            await ProcessEmailMessageAsync(
                message,
                stoppingToken);

            await _channel.BasicAckAsync(
                deliveryTag: eventArgs.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);

            NLogManager.LogInfo(
                $"Mensagem RabbitMQ processada com sucesso. " +
                $"Queue={QueueName}, " +
                $"DeliveryTag={eventArgs.DeliveryTag}");
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            /*
             * Durante shutdown não fazemos ACK.
             *
             * A mensagem poderá ser entregue novamente
             * quando outro consumidor assumir.
             */
            return;
        }
        catch (JsonException ex)
        {
            await RejectMessageAsync(
                eventArgs,
                ex,
                "JSON inválido");
        }
        catch (Exception ex)
        {
            await RejectMessageAsync(
                eventArgs,
                ex,
                "Erro durante processamento");
        }
    }


    private async Task ProcessEmailMessageAsync(
        EmailHeaderVO message,
        CancellationToken cancellationToken)
    {
        await using var scope =
            _scopeFactory.CreateAsyncScope();

        var rabbitMQMessageSender =
            scope.ServiceProvider
                .GetRequiredService<IRabbitMQMessageSender>();

        NLogManager.LogInfo(
            $"Processando mensagem RabbitMQ. " +
            $"Message={message}");

        await rabbitMQMessageSender.SendMessageAsync(
            message,
            "orderpaymentprocessqueue");
    }

    //private async Task ProcessEmailMessageAsync(
    //    EmailHeaderVO message,
    //    CancellationToken cancellationToken)
    //{
    //    NLogManager.LogInfo(
    //        $"Simulando a ação de processamento da mensagem " +
    //        $"message={message}");

    //    //await _rabbitMQMessageSender.SendMessageAsync(
    //    //    message,
    //    //    "orderpaymentprocessqueue");
    //}

    private async Task RejectMessageAsync(
        BasicDeliverEventArgs eventArgs,
        Exception exception,
        string reason)
    {
        if (_channel is null)
            return;

        NLogManager.LogError(
            $"Erro processando mensagem RabbitMQ. " +
            $"Queue={QueueName}, " +
            $"DeliveryTag={eventArgs.DeliveryTag}, " +
            $"Reason={reason}, " +
            $"Exception={exception}");

        /*
         * IMPORTANTE:
         *
         * requeue=false evita o loop infinito.
         *
         * A mensagem será encaminhada para a DLQ
         * através do Dead Letter Exchange configurado
         * na queue.
         */
        await _channel.BasicNackAsync(
            deliveryTag: eventArgs.DeliveryTag,
            multiple: false,
            requeue: false);
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        NLogManager.LogInfo(
            $"Parando RabbitMQ consumer. Queue={QueueName}");

        await DisposeRabbitMqAsync();

        await base.StopAsync(cancellationToken);
    }

    private async Task DisposeRabbitMqAsync()
    {
        try
        {
            if (_channel is not null)
            {
                if (!string.IsNullOrWhiteSpace(_consumerTag))
                {
                    try
                    {
                        await _channel.BasicCancelAsync(
                            _consumerTag);
                    }
                    catch
                    {
                        // Channel pode já estar fechado.
                    }

                    _consumerTag = null;
                }

                try
                {
                    await _channel.CloseAsync();
                }
                catch
                {
                    // Channel pode já estar fechado.
                }

                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                try
                {
                    await _connection.CloseAsync();
                }
                catch
                {
                    // Connection pode já estar fechada.
                }

                await _connection.DisposeAsync();
                _connection = null;
            }
        }
        catch (Exception ex)
        {
            NLogManager.LogError(
                $"Erro ao liberar recursos RabbitMQ. " +
                $"Exception={ex}");
        }
    }
}

