using MessagePack;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Text;
using TransportQueryService.Entities;
using TransportQueryService.Filters;
using TransportQueryService.Queries;


namespace TransportQueryService.QueryHandler
{
    public class TransportQueryHandler : ConsumerServiceBase
    {
        public TransportQueryHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory) 
            : base(logger, connectionFactory, config.GetSection("transportQueryConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!)
        {

        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;
            
            if (!headers.TryGetValue("Type", out object? typeObj))
                return;
            var type = (MessageType)Enum.Parse(typeof(MessageType), ASCIIEncoding.ASCII.GetString((byte[])typeObj));

            if (!headers.TryGetValue("Date", out object? dateObj))
                return;
            DateTime.TryParse(ASCIIEncoding.ASCII.GetString((byte[])dateObj), out var date);


            switch (type)
            {
                case MessageType.GET:
                    Get(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private void Get(BasicDeliverEventArgs ea)
        {
            var message = MessagePackSerializer.Deserialize<TransportGetQuery>(ea.Body.ToArray());
            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"GET Transport {filt_ser}");
            //message.filters
            //message.sort

            //GetFromDB
            var transportObj = new List<Transport>() { new Transport() { Id = Random.Shared.Next().ToString(), Origin = "Polska", Destination = "Egipt" }};

            var serialized = MessagePackSerializer.Serialize(transportObj);
            Reply(ea, serialized);
        }

    }
}
