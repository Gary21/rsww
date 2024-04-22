using System.Collections.Concurrent;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using RabbitUtilities.Configuration;
using RabbitUtilities.Misc;

namespace RabbitUtilities;

public class PublisherServiceBase : IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IConnection _connection;
    protected readonly string _replyQueueName;
    public string ReplyQueueName { get => _replyQueueName; }

    protected ConcurrentDictionary<Guid,MessageReply> replies;
    private int messagePollingDelay = 10;


    public PublisherServiceBase(ILogger logger, IConnectionFactory connectionFactory, ServiceConfig? config)
    {
        _logger = logger;
        _connection = connectionFactory.CreateConnection();

        _replyQueueName = $"{config?.name ?? ""} reply"+Guid.NewGuid().ToString();
        replies = new();
    }

    public void Dispose()
    {
        _connection.Dispose();
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
        _logger.Information($"Sent {type} request to {exchangeName}.{routingKey} without reply.");
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
        _logger.Information($"Sent {type} request to {exchangeName}.{routingKey} with ID:{messageCorrelationID}.");
        return messageCorrelationID;
    }

    public void SetReply(Guid messageCorrelationID, byte[] body)
    {
        replies[messageCorrelationID].SetReply(body);
        return;
    }

    public async Task<byte[]> GetReply(Guid messageCorrelationID)
    {
        //Wait for reply
        while (!replies[messageCorrelationID].IsReady)
        {
            await Task.Delay(messagePollingDelay);
        }
        replies.TryRemove(messageCorrelationID,out var message);
        message.TryGetReply(out var payload);
        
        return payload;
    }

}
