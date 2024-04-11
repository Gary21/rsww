using Microsoft.Extensions.Configuration;
using TransportService.Configuration;
using Serilog;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;
using MessagePack;
using TransportService.Entities;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var messageQueueConfig = config.GetSection("messageQueueConfig").Get<MessageQueueConfig>();


var factory = new ConnectionFactory { HostName = messageQueueConfig.adress, Port = messageQueueConfig.port, UserName = "guest", Password = "guest" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: messageQueueConfig.domain, durable: false, exclusive: false, autoDelete: false, arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = MessagePackSerializer.Deserialize<Transport>(body); //Encoding.UTF8.GetString(body);
    logger.Information($" [x] Received {message}");
};
channel.BasicConsume(queue: messageQueueConfig.domain, autoAck: true, consumer: consumer);

logger.Information($"Subscribed to queue {messageQueueConfig.domain}");
Console.ReadLine();

for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"Sending message");
    Transport transport = new Transport() { Id = i.ToString(), Origin = "Polska", Destination="Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };

    var body = MessagePackSerializer.Serialize(transport); //Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(exchange: string.Empty, routingKey: messageQueueConfig.domain, basicProperties: null, body: body);
    logger.Information($" [x] Sent {transport.Id}");
    Thread.Sleep(200+Random.Shared.Next()%300);
}


Console.ReadLine();

