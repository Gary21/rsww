using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitUtilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPublisherService.SecondPublisher
{

    public class Reply2Service : ReplyService
    {
        public Reply2Service(ILogger logger, [FromKeyedServices("Transaction")] IConnectionFactory connectionFactory, Publisher2Service publisherService, IHostApplicationLifetime appLifetime) 
            : base(logger, connectionFactory, publisherService, appLifetime)
        {
        }
    }
    public class Publisher2Service : PublisherServiceBase
    {
        public Publisher2Service(ILogger logger, [FromKeyedServices("Transaction")] IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime)
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<RabbitUtilities.Configuration.ServiceConfig>()!, appLifetime)
        {

        }

    }
    
}
