using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Text;
using OrderService.Entities;
using OrderService.Repositories;
using OrderService.Requests;

namespace OrderService.RequestHandler
{
    public class OrderRequestHandler : ConsumerServiceBase
    {
        private readonly PostgresRepository _repository;
        private readonly PublisherServiceBase _publisher;

        public OrderRequestHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase publisher, PostgresRepository repository) 
            : base(logger, connectionFactory, config.GetSection("orderConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!)
        {
            _repository = repository;
            _publisher = publisher;
            _repository.Database.EnsureDeleted();
            _repository.Database.EnsureCreated();
            _repository.SaveChanges();
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
                case MessageType.ADD:
                    Add(ea);
                    break;
                case MessageType.DELETE:
                    Delete(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private async void Add(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<AddRequest>(body);
            
            _repository.Orders.Add(message.Order);
            await _repository.SaveChangesAsync();
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<GetRequest>(body);
            _logger.Information($"GET {MessagePackSerializer.ConvertToJson(body)}");

            var orders = await _repository.Orders
                .Where(o => message.Filters.Ids == null || message.Filters.Ids.Count() == 0 || message.Filters.Ids.Contains(o.Id) )
                .Where(o => message.Filters.UserIds == null || message.Filters.UserIds.Count() == 0 || message.Filters.UserIds.Contains(o.UserId))
                .ToListAsync();

            Reply(ea, MessagePackSerializer.Serialize<List<Order>>(orders));
        }

        private async void Delete(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<DeleteRequest>(body);
            _logger.Information($"DELETE {MessagePackSerializer.ConvertToJson(body)}");

            var orderToDelete = await _repository.Orders.FirstAsync(o => o.Id == message.Id);

            _repository.Orders.Remove(orderToDelete);
            await _repository.SaveChangesAsync();
        }
    }
}
