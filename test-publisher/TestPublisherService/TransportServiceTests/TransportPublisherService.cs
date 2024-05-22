using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitUtilities;
using Serilog;
using TestPublisherService.SecondPublisher;
using TransportRequestService.Entities;

namespace TransportRequestService.TransportServiceTests
{
    public class ReplyMain : ReplyService
    {
        public ReplyMain(ILogger logger, [FromKeyedServices("Main")] IConnectionFactory connectionFactory, TransportPublisherService publisherService, IHostApplicationLifetime appLifetime)
            : base(logger, connectionFactory, publisherService, appLifetime)
        {
        }
    }

    public class TransportPublisherService : PublisherServiceBase
    {
        public TransportPublisherService(ILogger logger, [FromKeyedServices("Main")] IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime) 
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!, appLifetime)
        {

        }

    }
}
