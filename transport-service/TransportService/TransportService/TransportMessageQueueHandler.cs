using MessagePack;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using System.Text;
using System.Threading.Channels;
using TransportService.Entities;

namespace TransportService.TransportService
{
    public class TransportMessageQueueHandler : ConsumerServiceBase
    {
        public TransportMessageQueueHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory) : base(logger, config, connectionFactory)
        {

        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;
            
            if (!headers.TryGetValue("Type", out object typeObj))
                return;
            
            var typeString = ASCIIEncoding.ASCII.GetString((byte[]) typeObj);

            var type = (MessageType)Enum.Parse(typeof(MessageType),typeString);
            switch (type)
            {
                case MessageType.GET:
                    Get(ea);
                    break;
                case MessageType.ADD:
                    Add(ea);
                    break;
                case MessageType.UPDATE:
                    Update(ea);
                    break;
                case MessageType.DELETE:
                    Delete(ea);
                    break;
                case MessageType.RESERVE:
                    Reserve(ea);
                    break;
                default:
                    break;
            }
            _logger.Information("Consumed");
        }

        private void Get(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<Transport>(body);

        }

        private void Add(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<Transport>(body);
            _logger.Information($"ADD {MessagePackSerializer.ConvertToJson(body)}");
            Reply(ea,body);
            _logger.Information($"Reply {MessagePackSerializer.ConvertToJson(body)}");
            //Task.Delay(2000).Wait();

        }

        private void Update(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<Transport>(body);
        }

        private void Delete(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var id = MessagePackSerializer.Deserialize<long>(body);
        }

        private void Reserve(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<long>(body);
        }

    }
}
