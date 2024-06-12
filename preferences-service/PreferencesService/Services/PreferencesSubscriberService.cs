
using MessagePack;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PreferencesService.DataStores;
using PreferencesService.Entities;
using PreferencesService.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using Serilog;
using System.Text;

namespace PreferencesService.Services
{
    public class PreferencesSubscriberService : SubscriberServiceBase
    {

        private readonly DataStore _store;
        private readonly IConfiguration _configuration;
        private readonly PublisherServiceBase publisherService;
        private string preferencesEventsExchange;

        public PreferencesSubscriberService(ILogger logger, IConnectionFactory connectionFactory, IConfiguration config, IHostApplicationLifetime appLifetime, PublisherServiceBase publisherServiceBase, DataStore dataStore) 
            : base(logger, connectionFactory, config.GetSection("resourceEvents").Get<SubscriberConfig>()!, appLifetime)
        {
            publisherService = publisherServiceBase;
            _store = dataStore;
            _configuration = config;
            preferencesEventsExchange = config.GetSection("preferencesEvents").Get<SubscriberConfig>()!.exchange;
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
                case MessageType.RESERVE:   //RESERVE
                    HandleEvent(ea,type);
                    break;
                case MessageType.ADD:       //BUY
                    HandleEvent(ea, type);
                    break;

                case MessageType.UPDATE:   //CHANGE PRICE/AVAILABILITY
                    ChangeHandle(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }
        private static Preference GetPreference(MessageType msgType, int count) => msgType switch
        {
            MessageType.RESERVE => new Preference() { ReservationCount=count, PurchaseCount = 0 },
            MessageType.ADD => new Preference() { ReservationCount = 0, PurchaseCount = count },
            _ => new Preference() { }
        };           

        private void HandleEvent(BasicDeliverEventArgs ea, MessageType msgType)
        {
            var payload = MessagePackSerializer.Deserialize<KeyValuePair<string, byte[]>>(ea.Body);
            if (payload.Key=="hotel")//hotel Event
            {
                var hotelEvent = MessagePackSerializer.Deserialize<HotelReservationEvent>(payload.Value);
                var newPreference = GetPreference(msgType, /*hotelEvent.people*/ 1);
                var newPreferencePeople = GetPreference(msgType, /*hotelEvent.people*/ 1);

                List<PreferenceUpdate> updates = new();
                updates.Add(_store.AddPreference(DataStore.HotelName, hotelEvent.HotelName, newPreferencePeople));
                updates.Add(_store.AddPreference(DataStore.DestinationCity, hotelEvent.DestinationCity, newPreferencePeople));
                updates.Add(_store.AddPreference(DataStore.DestinationCountry, hotelEvent.DestinationCountry, newPreferencePeople));
                updates.Add(_store.AddPreference(DataStore.RoomType, hotelEvent.RoomType, newPreference));

                var hotelId = new PreferenceUpdate() { 
                    PreferenceType = "HotelID", 
                    PreferenceName = hotelEvent.Id.ToString(), 
                    Preference = newPreference 
                };
                
                publisherService.PublishToFanoutNoReply<List<PreferenceUpdate>>(_exchangeName, MessageType.GET, updates);//Update Preferences
                publisherService.PublishToFanoutNoReply<PreferenceUpdate>(_exchangeName, MessageType.RESERVE, hotelId); //Inform if viewed

            }
            else if(payload.Key =="transport")//transport event
            {
                var transportEvent = MessagePackSerializer.Deserialize<TransportReservationEvent>(payload.Value);
                var newPreference = GetPreference(msgType, transportEvent.Seats);

                List<PreferenceUpdate> updates = new();
                updates.Add(_store.AddPreference(DataStore.TransportType, transportEvent.TransportType, newPreference));

                publisherService.PublishToFanoutNoReply<List<PreferenceUpdate>>(_exchangeName,MessageType.GET , updates);

            }

        }

        private void ChangeHandle(BasicDeliverEventArgs ea)
        {
            var payload = MessagePackSerializer.Deserialize<KeyValuePair<string, byte[]>>(ea.Body);

            if (payload.Key == "hotel")//hotel Event
            {
                var hotelEvent = MessagePackSerializer.Deserialize<HotelChangeEvent>(payload.Value);

                
                var newChange = new Changes() { ResourceType = payload.Key, Name = hotelEvent.HotelName, PriceChange = hotelEvent.Price, /*Availability*/};
                _store.AddChange(newChange);
                publisherService.PublishToFanoutNoReply<Changes>(_exchangeName, MessageType.UPDATE, newChange);

            }
            else if (payload.Key == "transport")//transport event
            {
                var transportEvent = MessagePackSerializer.Deserialize<TransportChangeEvent>(payload.Value);

                var newChange = new Changes() { ResourceType = payload.Key, Name = transportEvent.DestinationCity, PriceChange = transportEvent.PriceChange };
                _store.AddChange(newChange);           
                publisherService.PublishToFanoutNoReply<Changes>(_exchangeName, MessageType.UPDATE, newChange);

            }
        }


    }
}
