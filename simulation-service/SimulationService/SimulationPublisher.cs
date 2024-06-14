using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationService
{
    public class SimulationPublisher : PublisherServiceBase
    {
        // ServiceConfig
        public SimulationPublisher(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime) 
            : base(logger, connectionFactory, config.GetSection("serviceInfo").Get<ServiceConfig>(), appLifetime)
        {
        }
    }
}
