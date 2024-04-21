// See https://aka.ms/new-console-template for more information
using MessagePack;
using RabbitMQ.Client;
using Serilog.Core;
using System.Threading.Channels;


var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

string routing = "query";
string exchange = "resources/transport";

channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Topic, durable: true);


for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"Sending message");
    var transport = new Transporttt() { Id2 = i.ToString(), Origin = "Polska", Destination = "Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type3 = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };
    Dictionary<string, object> headers = new();
    headers["Type"] = 1;

    IBasicProperties prop = channel.CreateBasicProperties();
    prop.Headers = headers;

    var body = MessagePackSerializer.Serialize(transport); //Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: exchange, routingKey: routing, basicProperties: prop, body: body);
    //logger.Information($" [x] Sent {transport.Id}");
    Thread.Sleep(200 + Random.Shared.Next() % 300);
}

[MessagePackObject]
public class Transporttt
{
    [Key(0)]
    public string Id2 { get; set; } //long?
    [Key(1)]
    public string Type3 { get; set; }
    [Key(2)]
    public DateTime DepartureDate { get; set; }
    [Key(3)]
    public DateTime ArrivalDate { get; set; }
    [Key(4)]
    public string Destination { get; set; }
    [Key(5)]
    public string Origin { get; set; }
    [Key(6)]
    public int SeatsNumber { get; set; }
    [Key(7)]
    public int SeatsTaken { get; set; }
    [Key(8)]
    public decimal PricePerTicket { get; set; }

}

