using MessagePack;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitUtilities;
using Serilog;
using OrderService.Entities;

namespace OrderService.Publisher
{
    public class OrderPublisherService : PublisherServiceBase
    {
        public OrderPublisherService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime)
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!,appLifetime)
        {

        }

    }
}
