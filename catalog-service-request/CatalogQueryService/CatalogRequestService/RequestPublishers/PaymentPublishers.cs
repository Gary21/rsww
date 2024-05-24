using RabbitUtilities;
using RabbitMQ.Client;

namespace CatalogRequestService.RequestPublishers
{

    public class Reply2Service : ReplyService
    {
        public Reply2Service(Serilog.ILogger logger, [FromKeyedServices("Transaction")] IConnectionFactory connectionFactory, Publisher2Service publisherService, IHostApplicationLifetime appLifetime)
            : base(logger, connectionFactory, publisherService, appLifetime)
        {
        }
    }
    public class Publisher2Service : PublisherServiceBase
    {
        public Publisher2Service(Serilog.ILogger logger, [FromKeyedServices("Transaction")] IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime)
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!, appLifetime)
        {

        }

    }

}
