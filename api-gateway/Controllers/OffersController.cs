using api_gateway.DTOs;
using api_gateway.EventConsumer;
using api_gateway.Events;
using api_gateway.Publisher;
using api_gateway.Queries;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using RabbitUtilities;
using System;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace api_gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OffersController : ControllerBase
{
    private readonly ILogger<OffersController> _logger;
    private readonly PublisherServiceBase _publisherService;
    private readonly CancellationToken _token;
    private readonly WebSocketService _webSocketService;

    public OffersController(ILogger<OffersController> logger, PublisherServiceBase publisherService, WebSocketService webSocketService , IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _publisherService = publisherService;
        _token = appLifetime.ApplicationStopping;
        _webSocketService = webSocketService;
    }

    [HttpGet("GetDestinations")]
    public async Task<IEnumerable<string>> GetDestinations()
    {
        var data = MessagePackSerializer.Serialize("");
        var payload = new KeyValuePair<string, byte[]>("GetDestinations", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var destinations = MessagePackSerializer.Deserialize<IEnumerable<string>>(bytes);

        return destinations;
    }

    [HttpGet("GetTrips")]
    public async Task<IEnumerable<TripDTO>> GetTrips([FromQuery] TripDTO query)
    {
        var queryJson = JsonSerializer.Serialize(query);
        _logger.LogInformation($"=>| GET :: Trips - {queryJson}");

        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("GetTrips", data);
        //var hotelQueryPayload = MessagePackSerializer.Serialize(query);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var hotels = MessagePackSerializer.Deserialize<IEnumerable<TripDTO>>(bytes);

        var hotelsJson = JsonSerializer.Serialize(hotels);
        _logger.LogInformation($"<=| GET :: Trips - {hotelsJson}");

        return hotels;
    }

    //[HttpGet("GetHotels")]
    //public async Task<IEnumerable<HotelDTO>> GetHotels([FromQuery] HotelsQuery query)
    //{
    //    var queryJson = JsonSerializer.Serialize(query);
    //    _logger.LogInformation($"=>| GET :: Hotels - {queryJson}");

    //    var data = MessagePackSerializer.Serialize(query);
    //    var payload = new KeyValuePair<string, byte[]>("GetHotels", data);
    //    //var hotelQueryPayload = MessagePackSerializer.Serialize(query);
    //    var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
    //    var bytes = await _publisherService.GetReply(messageId, _token);
    //    var hotels = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(bytes);

    //    var hotelsJson = JsonSerializer.Serialize(hotels);
    //    _logger.LogInformation($"<=| GET :: Hotels - {hotelsJson}");

    //    return hotels;
    //}

    //[HttpGet("AddSocket")]
    //public async Task AddSocket(int id)
    //{
    //    _logger.LogInformation($"Add Socket {id}");

    //    WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
    //    _webSocketService.AddHotelSocket(id.ToString(), webSocket);
    //    _webSocketService.AddPreferencesSocket(webSocket);
    //    _webSocketService.AddChangesSocket(webSocket);
    //}

    [HttpGet("GetHotel")]
    public async Task<HotelDTO> GetHotel(int id)
    {
        _logger.LogInformation($"=>| GET :: Hotel - {id}");

        var data = MessagePackSerializer.Serialize(id);
        var payload = new KeyValuePair<string, byte[]>("GetHotel", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var hotel = MessagePackSerializer.Deserialize<HotelDTO>(bytes);

        var hotelJson = JsonSerializer.Serialize(hotel);
        _logger.LogInformation($"<=| GET :: Hotel - {hotelJson}");

        return hotel;
    }



    [HttpGet("GetAvailability")]
    public async Task<bool> GetAvailability([FromQuery] int hotelId)
    {
        _logger.LogInformation($"=>| GET :: Availability - {hotelId}");

        var data = MessagePackSerializer.Serialize(hotelId);
        var payload = new KeyValuePair<string, byte[]>("GetAvailability", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var availability = MessagePackSerializer.Deserialize<bool>(bytes);

        _logger.LogInformation($"<=| GET :: Availability - {availability}");

        return availability;
    }

    [HttpPost("MakeReservation")]
    public async Task<int> MakeReservation([FromQuery] TripDTO query)
    {
        _logger.LogInformation($"=>| POST :: MakeReservation - {JsonSerializer.Serialize(query)}");

        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("MakeReservation", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "request", MessageType.RESERVE, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var reservation = MessagePackSerializer.Deserialize<int>(bytes);

        _logger.LogInformation($"<=| POST :: MakeReservation - {reservation}");

        return reservation;
    }

    [HttpGet("ValidateReservation")]
    public async Task<bool> ValidateReservation([FromQuery] TripDTO query)
    {
        _logger.LogInformation($"=>| GET :: ValidateReservation - {JsonSerializer.Serialize(query)}");

        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("ValidateReservation", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var reservation = MessagePackSerializer.Deserialize<bool>(bytes);

        _logger.LogInformation($"<=| GET :: ValidateReservation - {reservation}");

        return reservation;
    }

    [HttpPost("BuyReservation")]
    public async Task<bool> BuyReservation([FromQuery] int reservationId)
    {
        _logger.LogInformation($"=>| POST :: BuyReservation - {reservationId}");

        var data = MessagePackSerializer.Serialize(reservationId);
        var payload = new KeyValuePair<string, byte[]>("BuyReservation", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "request", MessageType.ADD, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var purchase = MessagePackSerializer.Deserialize<bool>(bytes);

        _logger.LogInformation($"<=| POST :: BuyReservation - {purchase}");

        return purchase;
    }

    [HttpGet("GetHotelRooms")]
    public async Task<IEnumerable<RoomTypeDTO>> GetHotelRooms([FromQuery] int id)
    {
        _logger.LogInformation($"=>| GET :: HotelRooms - {id}");

        var data = MessagePackSerializer.Serialize(id);
        var payload = new KeyValuePair<string, byte[]>("GetHotelRooms", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<IEnumerable<RoomTypeDTO>>(bytes);

        _logger.LogInformation($"<=| GET :: HotelRooms - {JsonSerializer.Serialize(rooms)}");

        return rooms;
    }


    [HttpGet("GetRoomType")]
    public async Task<RoomTypeDTO> GetRoomType([FromQuery] int id)
    {
        _logger.LogInformation($"=>| GET :: RoomType - {id}");

        var data = MessagePackSerializer.Serialize(id);
        var payload = new KeyValuePair<string, byte[]>("GetRoomType", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<RoomTypeDTO>(bytes);

        _logger.LogInformation($"<=| GET :: RoomType  - {JsonSerializer.Serialize(rooms)}");

        return rooms;
    }


    [HttpGet("FindTransports")]
    public async Task<IEnumerable<TransportDTO>> FindTransports([FromQuery] TransportDTO query)
    {
        _logger.LogInformation($"=>| GET :: FindTransports - {query}");

        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("FindTransports", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<IEnumerable<TransportDTO>>(bytes);

        _logger.LogInformation($"<=| GET :: FindTransports  - {JsonSerializer.Serialize(rooms)}");

        return rooms;
    }




    [HttpPost("Login")]
    public async Task<int> Login([FromQuery] string? username, [FromQuery] string? password)
    {
        if (username == null || password == null)
        {
            return -1;
        }
        var database = new Dictionary<string, KeyValuePair<string, int>>
        {
            { "user1", new KeyValuePair<string, int>("pass1", 11111111) },
            { "user2", new KeyValuePair<string, int>("pass2", 22222222) },
            { "user3", new KeyValuePair<string, int>("pass3", 33333333) },
            { "user4", new KeyValuePair<string, int>("pass4", 44444444) },
            { "user5", new KeyValuePair<string, int>("pass5", 55555555) },
            { "user6", new KeyValuePair<string, int>("pass6", 66666666) },
            { "user7", new KeyValuePair<string, int>("pass7", 77777777) }
        };

        if (database.ContainsKey(username) && database[username].Key == password)
        {
            return database[username].Value;
        }
        return -1;
    }


    [HttpGet("GetPreferences")]
    public async Task<IDictionary<string,IDictionary<string,Preference>>> GetPreferences()
    {
        _logger.LogInformation($"=>| GET :: Preferences");
        var payload = "GetPreferences";
//        var payload = new KeyValuePair<string, int>("GetPreferences",0);
        var messageId = _publisherService.PublishRequestWithReply("preferences", "query", MessageType.GET, payload);
        
        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<IDictionary<string, IDictionary<string, Preference>>>(bytes);
        
        _logger.LogInformation($"<=| GET :: Preferences - {JsonSerializer.Serialize(rooms)}");

        return rooms;
    }


    [HttpGet("HotelWebsocket")]
    public async void HotelWebsocket([FromQuery] int hotelId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                var gid = _webSocketService.AddHotelSocket(hotelId.ToString(), webSocket);

                //try
                //{
                //    while (!HttpContext.RequestAborted.IsCancellationRequested)
                //    {
                //    }
                //}
                //catch { }
                while (webSocket.State == WebSocketState.Open)
                {

                    webSocket.ReceiveAsync(new ArraySegment<byte>(new byte[16]), _token);
                    //await Task.Delay(10);
                }
                _webSocketService.RemoveHotelSocket(hotelId.ToString(),gid);
            }
        }
        else
        {
            Ok();
        }

    }

    [HttpGet("PreferencesWebsocket")]
    public async void PreferencesWebsocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {
                
                var preferences = await GetPreferences();
                foreach(var pref in preferences)
                {
                    foreach (var value in pref.Value) {
                        var updt = new PreferenceUpdate() { 
                            PreferenceType = pref.Key, PreferenceName = value.Key, 
                            Preference= new Preference() { 
                                PurchaseCount = value.Value.PurchaseCount, 
                                ReservationCount = value.Value.ReservationCount } 
                        };
                        try
                        {
                            await webSocket.SendAsync(UTF8Encoding.UTF8.GetBytes(JsonSerializer.Serialize(updt)), WebSocketMessageType.Text, true, _token);
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
                var id = _webSocketService.AddPreferencesSocket(webSocket);
                while (webSocket.State == WebSocketState.Open)
                {

                    webSocket.ReceiveAsync(new ArraySegment<byte>(new byte[16]), _token);
                    //await Task.Delay(10);
                }
                //try { 
                //    while (!HttpContext.RequestAborted.IsCancellationRequested)
                //    {
                //        await Task.Delay(10);
                //    } 
                //}
                //catch { }
                _webSocketService.RemovePreferencesSocket(id);
            }
        }
        else
        {
            Ok();
        }
        
    }



    [HttpGet("GetLastChanges")]
    public async Task<IEnumerable<Changes>> GetLastChanges()
    {
        _logger.LogInformation($"=>| GET :: Changes");
        var payload = "GetLastChanges";
        //var payload = new KeyValuePair<string, int>("GetLastChanges", 0);
        var messageId = _publisherService.PublishRequestWithReply("preferences", "query", MessageType.GET, payload);

        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<IEnumerable<Changes>>(bytes);

        _logger.LogInformation($"<=| GET :: Changes - {JsonSerializer.Serialize(rooms)}");

        return rooms;
    }

    [HttpGet("ChangesWebsocket")]
    public async void ChangesWebsocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            
            if (webSocket != null && webSocket.State == WebSocketState.Open)
            {

                var changes = await GetLastChanges();
                foreach (var change in changes)
                {
                    try
                    {
                        await webSocket.SendAsync(UTF8Encoding.UTF8.GetBytes(JsonSerializer.Serialize(change)), WebSocketMessageType.Text, true, _token);
                    }
                    catch
                    {
                        break;
                    }
                }
                var id = _webSocketService.AddChangesSocket(webSocket);
                while (webSocket.State == WebSocketState.Open)
                {

                    webSocket.ReceiveAsync(new ArraySegment<byte>(new byte[16]), _token);

                }
                //while (webSocket.State == WebSocketState.Open)
                //{
                //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(), CancellationToken.None).ConfigureAwait(false);
                //}

                //try
                //{
                //    while ( true/*!HttpContext.RequestAborted.IsCancellationRequested*/)
                //    {

                //        await Task.Delay(100);
                //    }
                //}
                //catch { }
                _webSocketService.RemoveChangesSocket(id);
            }
        }
        else
        {
            Ok();
        }

    }
}