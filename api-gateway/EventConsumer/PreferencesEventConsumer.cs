using api_gateway.Controllers;
using api_gateway.E;
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
                case MessageType.GET:
                    UpdatePreferences(ea);  //updated preferences
                    break;
                case MessageType.RESERVE:   //offer reserved or bought
                    InformOfferBought(ea);
                    break;
                case MessageType.UPDATE:
                    UpdateTourChanges(ea);  //new changes in offer
                    break;

                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }



        private void UpdateTourChanges(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var change = MessagePackSerializer.Deserialize<Changes>(body);
            var json = MessagePackSerializer.SerializeToJson(change);
            foreach(var socket in _webSocketService.ChangesSockets) {
                socket.SendAsync(UTF8Encoding.UTF8.GetBytes(json),WebSocketMessageType.Text,true, cancellationToken); //end of message = true?
            }
        }

        private void UpdatePreferences(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var preferences = MessagePackSerializer.Deserialize<List<PreferenceUpdate>>(body);
            var json = MessagePackSerializer.SerializeToJson(preferences);
            foreach (var socket in _webSocketService.PreferencesSockets)
            {
                socket.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
            }
            
        }

        private void InformOfferBought(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var hotelPreference = MessagePackSerializer.Deserialize<PreferenceUpdate>(body);
            var hotelId = hotelPreference.PreferenceName;


            if (hotelPreference.Preference.PurchaseCount > 0)
            {
                //hotel room bought
            }
            else if (hotelPreference.Preference.ReservationCount > 0)
            {
                //hotel reserved
            }
            var json = MessagePackSerializer.SerializeToJson(hotelPreference);

            List<Guid> socketsToClose = new();
            if (_webSocketService.HotelsSockets.TryGetValue(hotelId, out var sockets))
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

                foreach (var socketGuid in socketsToClose)
                {
                    _webSocketService.HotelsSockets[hotelId].Remove(socketGuid, out var _);
                }
            }

        }
    }
}
