using api_gateway.Controllers;
using api_gateway.Events;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace api_gateway.EventConsumer
{
    public class PreferencesEventConsumer : RabbitUtilities.SubscriberServiceBase
    {
        private CancellationToken cancellationToken;
        private WebSocketService _webSocketService;

        public PreferencesEventConsumer(Serilog.ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime, WebSocketService webSocketService) 
            : base(logger, connectionFactory, config.GetSection("preferencesEvents").Get<RabbitUtilities.Configuration.SubscriberConfig>(), appLifetime)
        {
            _webSocketService = webSocketService;
            cancellationToken = appLifetime.ApplicationStopping;
        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;

            if (!headers.TryGetValue("Type", out object? typeObj))
                return;
            var type = (MessageType)Enum.Parse(typeof(MessageType), ASCIIEncoding.ASCII.GetString((byte[])typeObj));

            if (!headers.TryGetValue("Date", out object? dateObj))
                return;
            DateTime.TryParse(ASCIIEncoding.ASCII.GetString((byte[])dateObj), out var date);

            switch (type)
            {
                case MessageType.RESERVE:
                    UpdatePreferences(ea);
                    break;
                case MessageType.UPDATE:
                    UpdateTourChanges(ea);
                    break;

                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }



        private void UpdateTourChanges(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var row = MessagePackSerializer.Deserialize<ChangesEvents>(body);
            var json = MessagePackSerializer.SerializeToJson(row);
            foreach(var socket in _webSocketService.ChangesSockets) {
                socket.SendAsync(UTF8Encoding.UTF8.GetBytes(json),WebSocketMessageType.Text,true, cancellationToken); //end of message = true?
            }
        }

        private void UpdatePreferences(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var row = MessagePackSerializer.Deserialize<PreferencesEvents>(body);
            var json = MessagePackSerializer.SerializeToJson(row);
            foreach (var socket in _webSocketService.PreferencesSockets)
            {
                socket.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
            }
            
        }
    }
}
