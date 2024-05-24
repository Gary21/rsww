using RabbitMQ.Client;
using RabbitUtilities;
using ILogger = Serilog.ILogger;

namespace api_gateway.Publisher
{
    public class GatewayPublisherService : PublisherServiceBase
    {
        public GatewayPublisherService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime)
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!,appLifetime)
        {

        }

    }
}