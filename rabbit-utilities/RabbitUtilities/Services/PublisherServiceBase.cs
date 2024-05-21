using System.Collections.Concurrent;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using RabbitUtilities.Configuration;
using RabbitUtilities.Misc;
using System.Runtime.InteropServices;
using RabbitUtilities.Common;

namespace RabbitUtilities;

public class PublisherServiceBase : RabbitClient,IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IConnection _connection;
    protected readonly string _replyQueueName;
    protected readonly IConnectionFactory _connectionFactory;
    public string ReplyQueueName { get => _replyQueueName; }

    ILogger RabbitClient._logger => _logger;

    protected ConcurrentDictionary<Guid,MessageReply> replies;
    private int messagePollingDelay = 10;


    public PublisherServiceBase(ILogger logger, IConnectionFactory connectionFactory, ServiceConfig? config, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _replyQueueName = $"{config?.name ?? ""}.reply.{Guid.NewGuid()}";
        replies = new();
        _connectionFactory = connectionFactory;

        ((RabbitClient)this).ConnectToRabbit(connectionFactory, appLifetime.ApplicationStopping, out _connection);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }

    public void PublishToFanoutNoReply<T>(string exchangeName, MessageType type, T payload)
    {
        using var publish_channel = _connection.CreateModel();
        publish_channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);

        Dictionary<string, object> headers = new()
        {
            { "Type", type.ToString() },
            { "Date", DateTime.UtcNow.ToString() }
        };

        IBasicProperties properties = publish_channel.CreateBasicProperties();
        properties.Headers = headers;

        var body = MessagePackSerializer.Serialize(payload);

        publish_channel.BasicPublish(exchange: exchangeName, routingKey: String.Empty, basicProperties: properties, body: body);
        _logger.Information($"Sent {type} message to {exchangeName} fanout without reply.");
    }

    public void PublishRequestNoReply<T>(string exchangeName,string? routingKey, MessageType type, T payload)
    {
        using var publish_channel = _connection.CreateModel();
        publish_channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);

        Dictionary<string, object> headers = new()
        {
            { "Type", type.ToString() },
            { "Date", DateTime.UtcNow.ToString() }
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
            { "Type", type.ToString() },
            { "Date", DateTime.UtcNow.ToString() }
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

    public async Task<byte[]> GetReply(Guid messageCorrelationID, CancellationToken cancellationToken)
    {
        //Wait for reply
        while (!replies[messageCorrelationID].IsReady && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(messagePollingDelay);
        }
        replies.TryRemove(messageCorrelationID,out var message);
        message!.TryGetReply(out var payload);
        
        return payload ?? Array.Empty<byte>();
    }

}
