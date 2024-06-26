﻿using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities.Configuration;
using Serilog;
using TransportRequestService.Entities;

namespace TransportRequestService.TransportServiceTests
{
    public class SubscriberTest : RabbitUtilities.SubscriberServiceBase
    {
        public SubscriberTest(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime applifetime) 
            : base(logger, connectionFactory, config.GetSection("subscriberConfig").Get<RabbitUtilities.Configuration.SubscriberConfig>()!,applifetime)
        {
        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();
            var message = MessagePackSerializer.Deserialize<Transport>(body);
            _logger.Information($"EVENT {MessagePackSerializer.ConvertToJson(body)}");
        }
    }
}
