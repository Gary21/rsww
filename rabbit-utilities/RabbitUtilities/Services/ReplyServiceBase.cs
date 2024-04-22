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

public class ReplyService : BackgroundService, IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IConnection _connection;
    protected readonly IModel _replyChannel;
    protected readonly string _replyQueueName;


    PublisherServiceBase _publisherService;


    public ReplyService(ILogger logger, IConnectionFactory connectionFactory, PublisherServiceBase publisherService)
    {
        _publisherService = publisherService;
        _replyQueueName = _publisherService.ReplyQueueName;

        _logger = logger;
        _connection = connectionFactory.CreateConnection();
        _replyChannel = _connection.CreateModel();

        _replyChannel.QueueDeclare(queue: _replyQueueName, durable: false, exclusive: true, autoDelete: true, arguments: null);
        
        var consumer = new EventingBasicConsumer(_replyChannel);
        consumer.Received += async (model, ea) =>
        {
            await Task.Run(() => FetchReply(model, ea));
            _replyChannel.BasicAck(ea.DeliveryTag, false);
        };
        _replyChannel.BasicConsume(queue: _replyQueueName, autoAck: false, consumer: consumer);
    }

    public new void Dispose()
    {
        _connection.Dispose();
        _replyChannel.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Information($"Subscribed to queue {_replyQueueName}");
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000);
        }
        _logger.Information($"Closing reply service in queue {_replyQueueName}");
        return;
    }

    private void FetchReply(object model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var messageCorrelationID = Guid.Parse(ea.BasicProperties.CorrelationId);
        //var headers = ea.BasicProperties.Headers;//???

        _publisherService.SetReply(messageCorrelationID, body);
        _logger.Information($"Received reply to: {messageCorrelationID}");
    }
}
