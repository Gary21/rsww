namespace api_gateway.Queries;

public class ReservationQuery
{
    public string HotelId { get; set; }
    public string DepartureCity { get; set; }
    public string Transport { get; set; }
    public string Date { get; set; }
    public int Adults { get; set; }
    public int Children18 { get; set; }
    public int Children10 { get; set; }
    public int Children3 { get; set; }
}