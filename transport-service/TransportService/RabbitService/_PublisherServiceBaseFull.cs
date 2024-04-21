using System.Collections.Concurrent;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using TransportService.Configuration;
using TransportService.Misc;

namespace TransportService;

public class _PublisherServiceBaseFull : BackgroundService, IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IConnection _connection;
    protected readonly IModel _replyChannel;
    protected readonly string _replyQueueName;

    protected ConcurrentDictionary<Guid,MessageReply> replies;


    public _PublisherServiceBaseFull(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
    {
        MessageQueueConfig messageQueueConfig = config.GetSection("messageQueueConfig").Get<MessageQueueConfig>()!;

        _logger = logger;
        _connection = connectionFactory.CreateConnection();

        _replyQueueName = messageQueueConfig.queue+".reply"+Guid.NewGuid().ToString();
        _replyChannel = _connection.CreateModel();
        //reply queuename?
        _replyChannel.QueueDeclare(queue: _replyQueueName, durable: false, exclusive: true, autoDelete: true, arguments: null);
        replies = new();
    }

    public void Dispose()
    {
        _connection.Dispose();
        _replyChannel.Dispose();
        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_replyChannel);
        consumer.Received += async (model, ea) =>
        {
            await Task.Run(() => FetchReply(model,ea));
            _replyChannel.BasicAck(ea.DeliveryTag,false);
        };
        _replyChannel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: consumer);

        _logger.Information($"Subscribed to queue {_replyQueueName}");
        while (!stoppingToken.IsCancellationRequested)
        {
            
        }
        _logger.Information($"Finishing");
        return Task.CompletedTask;
    }

    private void FetchReply(object model, BasicDeliverEventArgs ea){
            var body = ea.Body.ToArray();
            var messageCorrelationID = Guid.Parse(ea.BasicProperties.CorrelationId);
            var headers = ea.BasicProperties.Headers;//???
            replies[messageCorrelationID].SetReply(body);
            _logger.Information($"Received reply to:{messageCorrelationID}");

    }

    public void PublishRequestNoReply<T>(string exchangeName,string? routingKey, MessageType type, T payload){
        using var publish_channel = _connection.CreateModel();
        publish_channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        Dictionary<string, object> headers = new()
            {
                { "Type", type.ToString() }
            };
        
        IBasicProperties properties = publish_channel.CreateBasicProperties();
        properties.Headers = headers;
        
        var body = MessagePackSerializer.Serialize(payload);
        
        publish_channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: properties, body: body);
        _logger.Information($"Sending");
    }

    public Guid PublishRequestWithReply<T>(string exchangeName, string? routingKey, MessageType type, T payload)
    {
        using var publish_channel = _connection.CreateModel();
        publish_channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);
        
        var messageCorrelationID = Guid.NewGuid();
        replies.TryAdd(messageCorrelationID, new MessageReply(){Type = type});    
        
        Dictionary<string, object> headers = new()
        {
            { "Type", type.ToString() }
        };
        
        IBasicProperties properties = publish_channel.CreateBasicProperties();
        properties.Headers = headers;
        properties.CorrelationId = messageCorrelationID.ToString();
        properties.ReplyTo = _replyQueueName;

        var body = MessagePackSerializer.Serialize<T>(payload);
        publish_channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: properties, body: body);

        return messageCorrelationID;
    }


}
