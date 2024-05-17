using MessagePack;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitUtilities;
using Serilog;
using TransportRequestService.Entities;

namespace TransportRequestService.Publisher
{
    public class TransportPublisherService : PublisherServiceBase
    {
        public TransportPublisherService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config)
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!)
        { }
    }
}
