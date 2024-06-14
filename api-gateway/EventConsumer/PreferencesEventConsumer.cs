using api_gateway.Controllers;
using api_gateway.Events;
using MessagePack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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

        protected async override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
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
                    await UpdatePreferences(ea);  //updated preferences
                    break;
                case MessageType.RESERVE:   //offer reserved or bought
                    await InformOfferBought(ea);
                    break;
                case MessageType.UPDATE:
                    await UpdateTourChanges(ea);  //new changes in offer
                    break;

                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }



        private async Task UpdateTourChanges(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var change = MessagePackSerializer.Deserialize<Changes>(body);
            await InformOfferChange(change);
            var json = JsonSerializer.Serialize(change);
            foreach (var socket in _webSocketService.ChangesSockets)
            {
                try
                {
                    await socket.Value.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
                }
                catch
                {
                 //   await socket.Value.CloseAsync(WebSocketCloseStatus.Empty,"",cancellationToken);
                    _webSocketService.ChangesSockets.Remove(socket.Key, out _);
                    //    continue;
                }
            }
        }

        private async Task UpdatePreferences(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var preferences = MessagePackSerializer.Deserialize<List<PreferenceUpdate>>(body);
            foreach (var socket in _webSocketService.PreferencesSockets)
            {
                try
                {
                    foreach (var pref in preferences)
                    {
                        var json = /*MessagePackSerializer.SerializeToJson*/JsonSerializer.Serialize(pref);
                        await socket.Value.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
                    }
                }
                catch
                {
                    _webSocketService.PreferencesSockets.Remove(socket.Key, out _);
                    //continue;
                }
            }

        }
        private async Task InformOfferChange(Changes change)
        {
            if (change.ResourceType == "hotel")
            {
                var hotelId = change.Id;

                if (_webSocketService.HotelsSockets.TryGetValue(hotelId, out var sockets))
                {
                    string json;
                    if (change.Availability)
                    {
                        json = "UNAVAILABLE";
                    }
                    else
                    {
                        json = change.PriceChange.ToString();
                    }
                    //var json = /*MessagePackSerializer.SerializeToJson*/JsonSerializer.Serialize(change);
                    foreach (var socket in sockets)
                    {
                        try
                        {
                            await socket.Value.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
                        }
                        catch
                        {
                            sockets.Remove(socket.Key, out _);
                           // continue;
                        }
                    }
                }
            }

        }
        private async Task InformOfferBought(BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var hotelPreference = MessagePackSerializer.Deserialize<PreferenceUpdate>(body);
            var hotelId = hotelPreference.PreferenceName;

            if (_webSocketService.HotelsSockets.TryGetValue(hotelId, out var sockets))
            {
                string json = "";
                if (hotelPreference.Preference.PurchaseCount > 0)
                {
                    json = "PURCHASE";
                    //hotel room bought
                }
                else if (hotelPreference.Preference.ReservationCount > 0)
                {
                    json = "RESERVE";
                    //hotel reserved
                }
                //var json = JsonSerializer.Serialize/*MessagePackSerializer.SerializeToJson*/(hotelPreference);
                foreach (var socketPair in sockets)
                {
                    try
                    {
                        var socket = socketPair.Value;
                        await socket.SendAsync(UTF8Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, cancellationToken); //end of message = true?
                    }
                    catch
                    {
                        sockets.Remove(socketPair.Key, out _);
                       // continue;
                    }
                }
            }

        }
    }
}
