using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Threading.Channels;
using TransportService.Configuration;


public abstract
class ConsumerServiceBase : BackgroundService, IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IConnection _connection;
    protected readonly IModel _consumeChannel;

    protected readonly string _exchangeName;
    protected readonly string _queueName;
    public ConsumerServiceBase(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
    {
        _logger = logger;
        
        MessageQueueConfig messageQueueConfig = config.GetSection("messageQueueConfig").Get<MessageQueueConfig>()!;
         _exchangeName = messageQueueConfig.exchange;
        _queueName = messageQueueConfig.queue;

        _connection = connectionFactory.CreateConnection();
        _consumeChannel = _connection.CreateModel();
        //Bind queue
        _consumeChannel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        _consumeChannel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true);
        _consumeChannel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: messageQueueConfig.routing);
    }

    public new void Dispose()
    {
        _connection.Dispose();
        _consumeChannel.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_consumeChannel);
        consumer.Received += async (model, ea) =>
        {
            await Task.Run(() => ConsumeMessage(model,ea));
        };
        _consumeChannel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        _logger.Information($"Subscribed to queue {_queueName} in exchange {_exchangeName}");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
        _logger.Information($"Closing");
        return;//Task.CompletedTask;
    }

    protected abstract void ConsumeMessage(object model, BasicDeliverEventArgs ea);

    protected void Reply(BasicDeliverEventArgs ea, byte[] replyBody)
    {
        var replyProperties = _consumeChannel.CreateBasicProperties();
        var replyTo = ea.BasicProperties.ReplyTo;

        replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;
        replyProperties.Headers = ea.BasicProperties.Headers;
        
        _consumeChannel.BasicPublish(exchange: string.Empty,
                         routingKey: replyTo,
                         basicProperties: replyProperties,
                         body: replyBody);
    }
}