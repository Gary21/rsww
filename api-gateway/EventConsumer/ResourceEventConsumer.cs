using api_gateway.Controllers;
using api_gateway.Events;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;

namespace api_gateway.EventConsumer
{
    public class ResourceEventConsumer : RabbitUtilities.SubscriberServiceBase
    {

        private CancellationToken cancellationToken;
        private WebSocketService _webSocketService;

        public ResourceEventConsumer(Serilog.ILogger logger, IConnectionFactory connectionFactory,IConfiguration config, IHostApplicationLifetime appLifetime, WebSocketService webSocketService) 
            : base(logger, connectionFactory, config.GetSection("resourceEvents").Get<RabbitUtilities.Configuration.SubscriberConfig>(), appLifetime)
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
                    TripEvent(ea);
                    break;
                case MessageType.ADD:
                    TripEvent(ea);
                    break;

                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private void TripEvent(BasicDeliverEventArgs ea) {
            var body = ea.Body;
            var hotelEvent = MessagePackSerializer.Deserialize<HotelEvents>(body);

            var json = MessagePackSerializer.SerializeToJson(hotelEvent);
            List<Guid> socketsToClose = new();
            if (_webSocketService.HotelsSockets.TryGetValue(hotelEvent.hotelId, out var sockets))
            {
                foreach (var socketPair in sockets)
                {
                    var socket = socketPair.Value;
                    if (socket.State == WebSocketState.Open || socket.State == WebSocketState.Connecting)
                    {
                        socket.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
                    }
                    else
                    {
                        socketsToClose.Add(socketPair.Key);
                    }
                }

                foreach(var socketGuid in socketsToClose)
                {
                    _webSocketService.HotelsSockets[hotelEvent.hotelId].Remove(socketGuid , out var _ );
                }
            }
        }


    }
}
