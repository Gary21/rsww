using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;

namespace HotelsRequestService.HotelPublisher
{
    public class HotelPublisherService : PublisherServiceBase
    {
        public HotelPublisherService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime)
    : base((Serilog.ILogger)logger, connectionFactory, config.GetSection("serviceInfo").Get<ServiceConfig>()!, appLifetime)
        {
        }
    }
}
