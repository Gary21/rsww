using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using RabbitUtilities.Common;
namespace RabbitUtilities;

public class ReplyService : BackgroundService, RabbitClient,IDisposable
{
    protected readonly ILogger _logger;
    protected readonly IConnectionFactory _connectionFactory;
    protected IConnection _connection;
    protected IModel _replyChannel;
    protected readonly string _replyQueueName;

    PublisherServiceBase _publisherService;

    ILogger RabbitClient._logger => _logger;
    public ReplyService(ILogger logger, IConnectionFactory connectionFactory, PublisherServiceBase publisherService, IHostApplicationLifetime appLifetime)
    {
        _publisherService = publisherService;
        _replyQueueName = _publisherService.ReplyQueueName;
        _connectionFactory = connectionFactory;
        _logger = logger;

        ((RabbitClient)this).ConnectToRabbit(_connectionFactory, appLifetime.ApplicationStopping, out _connection);
        _replyChannel = _connection?.CreateModel();
        if (_replyChannel is null)
            return;

        _replyChannel.QueueDeclare(queue: _replyQueueName, durable: false, exclusive: true, autoDelete: true, arguments: null);
    }

    public new void Dispose()
    {
        _connection.Dispose();
        _replyChannel.Dispose();
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_replyChannel);
        consumer.Received += async (model, ea) =>
        {
            await Task.Run(() => FetchReply(model, ea));
            _replyChannel.BasicAck(ea.DeliveryTag, false);
        };
        _replyChannel.BasicConsume(queue: _replyQueueName, autoAck: false, consumer: consumer);
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
