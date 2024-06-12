using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using Serilog;

namespace PreferencesService.Services
{
    public class PreferencesPublisherService : PublisherServiceBase
    {
        public PreferencesPublisherService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime) 
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<ServiceConfig>()!, appLifetime)
        {
        }
    }
}
