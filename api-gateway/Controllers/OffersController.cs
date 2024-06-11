using api_gateway.DTOs;
using api_gateway.EventConsumer;
using api_gateway.Publisher;
using api_gateway.Queries;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using RabbitUtilities;
using System.Net.WebSockets;
using System.Text.Json;

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

    [HttpGet("GetHotels")]
    public async Task<IEnumerable<HotelDTO>> GetHotels([FromQuery] HotelsQuery query)
    {
        var queryJson = JsonSerializer.Serialize(query);
        _logger.LogInformation($"=>| GET :: Hotels - {queryJson}");

        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("GetHotels", data);
        //var hotelQueryPayload = MessagePackSerializer.Serialize(query);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var hotels = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(bytes);

        var hotelsJson = JsonSerializer.Serialize(hotels);
        _logger.LogInformation($"<=| GET :: Hotels - {hotelsJson}");

        return hotels;
    }

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

        WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        _webSocketService.AddHotelSocket(id.ToString(), webSocket);

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
    public async Task<int> MakeReservation([FromQuery] ReservationQuery query)
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
    public async Task<bool> ValidateReservation([FromQuery] ReservationQuery query)
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
    public async Task<IEnumerable<string>> GetHotelRooms([FromQuery] int id)
    {
        _logger.LogInformation($"=>| GET :: HotelRooms - {id}");

        var data = MessagePackSerializer.Serialize(id);
        var payload = new KeyValuePair<string, byte[]>("GetHotelRooms", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<IEnumerable<string>>(bytes);

        _logger.LogInformation($"<=| GET :: HotelRooms - {JsonSerializer.Serialize(rooms)}");

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
}