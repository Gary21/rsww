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
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace OrderService.RequestHandler
{
    public class OrderRequestHandler : ConsumerServiceBase
    {

        private readonly IDbContextFactory<PostgresRepository> _repositoryFactory;
        private readonly PublisherServiceBase _publisher;

        public OrderRequestHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, PublisherServiceBase publisher, IDbContextFactory<PostgresRepository> repositoryFactory,IHostApplicationLifetime appLifetime) 
            : base(logger, connectionFactory, config.GetSection("orderConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!,appLifetime)
        {
            _repositoryFactory = repositoryFactory;
            _publisher = publisher;
            using var repository = repositoryFactory.CreateDbContext();
            //repository.Database.EnsureDeleted();
            repository.Database.EnsureCreated();
            //repository.SaveChanges();
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
            using var repository = _repositoryFactory.CreateDbContext();
            if (message.Order is null || repository.Orders.All(order => message.Order.Id != order.Id))
            {
                repository.Orders.Add(message.Order);
                await repository.SaveChangesAsync();
                _logger.Information($"ADD {MessagePackSerializer.ConvertToJson(body)}");
            }
            else
            {
                _logger.Warning($"ADD rejected, duplicate Id {message.Order.Id}");
            }
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<GetRequest>(body);
            using var repository = _repositoryFactory.CreateDbContext();

            _logger.Information($"GET {MessagePackSerializer.ConvertToJson(body)}");

            var orders = await repository.Orders
                .Where(o => message.Filters.Ids == null || message.Filters.Ids.Count() == 0 || message.Filters.Ids.Contains(o.Id) )
                .Where(o => message.Filters.UserIds == null || message.Filters.UserIds.Count() == 0 || message.Filters.UserIds.Contains(o.UserId))
                .ToListAsync();

            Reply(ea, MessagePackSerializer.Serialize<List<Order>>(orders));
        }

        private async void Delete(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<DeleteRequest>(body);
            using var repository = _repositoryFactory.CreateDbContext();

            _logger.Information($"DELETE {MessagePackSerializer.ConvertToJson(body)}");

            try
            {
                var orderToDelete = await repository.Orders.FirstAsync(o => o.Id == message.Id);
                repository.Orders.Remove(orderToDelete);
                await repository.SaveChangesAsync();
            }
            catch(Exception ex) {
                _logger.Warning($"Cannot delete {MessagePackSerializer.ConvertToJson(body)}");
            }

        }
    }
}
