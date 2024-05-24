using MessagePack;

namespace api_gateway.Queries;

public class ReservationQuery
{
    [Key(0)]
    public int HotelId { get; set; }
    [Key(1)]
    public string DepartureCity { get; set; }
    [Key(2)]
    public string RoomType { get; set; }
    [Key(3)]
    public string Date { get; set; }
    [Key(4)]
    public int Adults { get; set; }
    [Key(5)]
    public int Children18 { get; set; }
    [Key(6)]
    public int Children10 { get; set; }
    [Key(7)]
    public int Children3 { get; set; }
}