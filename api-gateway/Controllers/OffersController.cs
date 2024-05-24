using api_gateway.DTOs;
using api_gateway.Publisher;
using api_gateway.Queries;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using RabbitUtilities;

namespace api_gateway.Controllers;

[ApiController]
[Route("[controller]")]
public class OffersController : ControllerBase
{

    private readonly ILogger<OffersController> _logger;
    private readonly PublisherServiceBase _publisherService;
    private readonly CancellationToken _token;

    public OffersController(ILogger<OffersController> logger, PublisherServiceBase publisherService, IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _publisherService = publisherService;
        _token = appLifetime.ApplicationStopping;
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
        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("GetHotels", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var hotels = MessagePackSerializer.Deserialize<IEnumerable<HotelDTO>>(bytes);

        return hotels;
    }
    
    [HttpGet("GetHotel")]
    public async Task<HotelDTO> GetHotel(int id)
    {
        var data = MessagePackSerializer.Serialize(id);
        var payload = new KeyValuePair<string, byte[]>("GetHotel", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var hotel = MessagePackSerializer.Deserialize<HotelDTO>(bytes);

        return hotel;
    }
    
    [HttpGet("GetAvailability")]
    public async Task<bool> GetAvailability([FromQuery] int hotelId)
    {
        var data = MessagePackSerializer.Serialize(hotelId);
        var payload = new KeyValuePair<string, byte[]>("GetAvailability", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var availability = MessagePackSerializer.Deserialize<bool>(bytes);

        return availability;
    }
    
    [HttpPost("MakeReservation")]
    public async Task<int> MakeReservation([FromQuery] ReservationQuery query)
    {
        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("MakeReservation", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "request", MessageType.RESERVE, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var reservation = MessagePackSerializer.Deserialize<int>(bytes);

        return reservation;
    }
    
    [HttpGet("ValidateReservation")]
    public async Task<bool> ValidateReservation([FromQuery] ReservationQuery query)
    {
        var data = MessagePackSerializer.Serialize(query);
        var payload = new KeyValuePair<string, byte[]>("ValidateReservation", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var reservation = MessagePackSerializer.Deserialize<bool>(bytes);

        return reservation;
    }
    
    [HttpPost("BuyReservation")]
    public async Task<bool> BuyReservation([FromQuery] int reservationId)
    {
        var data = MessagePackSerializer.Serialize(reservationId);
        var payload = new KeyValuePair<string, byte[]>("BuyReservation", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "request", MessageType.ADD, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var purchase = MessagePackSerializer.Deserialize<bool>(bytes);

        return purchase;
    }
    
    [HttpGet("GetHotelRooms")]
    public async Task<IEnumerable<string>> GetHotelRooms([FromQuery] int hotelId)
    {
        var data = MessagePackSerializer.Serialize(hotelId);
        var payload = new KeyValuePair<string, byte[]>("GetHotelRooms", data);
        var messageId = _publisherService.PublishRequestWithReply("catalog", "query", MessageType.GET, payload);
        var bytes = await _publisherService.GetReply(messageId, _token);
        var rooms = MessagePackSerializer.Deserialize<IEnumerable<string>>(bytes);

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