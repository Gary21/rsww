using MessagePack;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitUtilities;
using Serilog;
using TransportService.Entities;

namespace TransportService.TransportService
{
    public class TransportPublisherTest : PublisherServiceBase
    {
        public TransportPublisherTest(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config) 
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!)
        {

        }

        //public async void Send()
        //{
        //    for (int i = 0; i < 10; i++)
        //    {
        //        Console.WriteLine($"Sending message");
        //        var transport = new Transport() { Id = i.ToString(), Origin = "Polska", Destination = "Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };

        //        var msgId = this.PublishRequestWithReply<Transport>("resources/transport", "query", MessageType.ADD, transport);

        //        var body = MessagePackSerializer.Deserialize<Transport>(await GetReply(msgId));

        //        _logger.Information($"Received Reply {body.Id}");
        //    }
        //}

    }
}
