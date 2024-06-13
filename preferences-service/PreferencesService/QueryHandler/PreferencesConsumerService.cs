using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PreferencesService.DataStores;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreferencesService.QueryHandler
{
    public class PreferencesConsumerService : ConsumerServiceBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataStore _store;

        public PreferencesConsumerService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime, DataStore dataStore)
            : base(logger, connectionFactory, config.GetSection("preferencesConsumer").Get<ConsumerConfig>()!, appLifetime)
        {
            _store = dataStore;
            _configuration = config;

        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;

            if (!headers.TryGetValue("Type", out object? typeObj))
                return;
            var type = (MessageType)Enum.Parse(typeof(MessageType), Encoding.ASCII.GetString((byte[])typeObj));

            if (!headers.TryGetValue("Date", out object? dateObj))
                return;
            DateTime.TryParse(Encoding.ASCII.GetString((byte[])dateObj), out var date);

            switch (type)
            {
                case MessageType.GET:
                    ParseQuery(ea);
                    break;

                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private void ParseQuery(BasicDeliverEventArgs ea)
        {
            var query = MessagePackSerializer.Deserialize<string>(ea.Body);
            switch (query)
            {
                case "GetPreferences":
                    GetPreferences(ea);
                    break;
                case "GetLastChanges":
                    GetLastChanges(ea);
                    break;
                default:
                    Reply(ea, Encoding.UTF8.GetBytes("UNKNOWN"));
                    break;
            }
        }


        private void GetLastChanges(BasicDeliverEventArgs ea)
        {
            var payload = MessagePackSerializer.Serialize(_store.LastChanges);
            Reply(ea, payload);
        }

        private void GetPreferences(BasicDeliverEventArgs ea)
        {
            var payload = MessagePackSerializer.Serialize(_store.Preferences);
            Reply(ea, payload);
        }
    }
}
