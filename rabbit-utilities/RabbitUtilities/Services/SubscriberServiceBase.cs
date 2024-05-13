using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities.Configuration;
using Serilog;

namespace RabbitUtilities
{
    public abstract class SubscriberServiceBase : BackgroundService, IDisposable
    {
        protected readonly ILogger _logger;
        protected readonly IConnection _connection;
        protected readonly IModel _consumeChannel;

        protected readonly string _exchangeName;
        protected readonly string _queueName;

        public SubscriberServiceBase(ILogger logger, IConnectionFactory connectionFactory, SubscriberConfig config)
        {
            _logger = logger;
            _exchangeName = config.exchange;
            _queueName = "event" + Guid.NewGuid().ToString();

            _connection = connectionFactory.CreateConnection();
            _consumeChannel = _connection.CreateModel();

            //Bind queue
            _consumeChannel.QueueDeclare(queue: _queueName);
            _consumeChannel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout, durable: true);
            _consumeChannel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: String.Empty);
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
                await Task.Run(() => ConsumeMessage(model, ea));
            };
            _consumeChannel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            _logger.Information($"Subscribed to queue {_queueName} in exchange {_exchangeName}");
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
            _logger.Information($"Closing consumer of queue {_queueName}");
            return;
        }

        protected abstract void ConsumeMessage(object model, BasicDeliverEventArgs ea);

        protected void Reply(BasicDeliverEventArgs ea, byte[] replyBody)
        {
            var replyProperties = _consumeChannel.CreateBasicProperties();
            var replyTo = ea.BasicProperties.ReplyTo;

            replyProperties.CorrelationId = ea.BasicProperties.CorrelationId;
            replyProperties.Headers = ea.BasicProperties.Headers;

            _logger.Information($"Sent reply to {replyTo} with ID:{replyProperties.CorrelationId}.");

            _consumeChannel.BasicPublish(exchange: string.Empty,
                             routingKey: replyTo,
                             basicProperties: replyProperties,
                             body: replyBody);
            return;
        }
    }
}
