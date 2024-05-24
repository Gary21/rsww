using MessagePack;

namespace CatalogRequestService.Requests
{
    [MessagePackObject]
    public class TripReserveRequest
    {
        [Key(0)]
        public int HotelId { get; set; }
        [Key(1)]
        public int RoomTypeId { get; set; }
        [Key(2)]
        public DateTime CheckInDate { get; set; }
        [Key(3)]
        public DateTime CheckOutDate { get; set; }
        [Key(4)]
        public int PeopleNumber { get; set; }
        [Key(5)]
        public int TransportId { get; set; }
        [Key(6)]
        public int ClientId { get; set; }
        
    }

    [MessagePackObject]
    public class ReservationQueryGateway
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

    public class ReservationQueryGatewayToMeAdapter
    {
        public static TripReserveRequest Adapt(ReservationQueryGateway reservationQueryGateway)
        {
            return new TripReserveRequest
            {
                HotelId = reservationQueryGateway.HotelId,
                CheckInDate = DateTime.Parse(reservationQueryGateway.Date),
                CheckOutDate = DateTime.Parse(reservationQueryGateway.Date).AddDays(7),
                PeopleNumber = reservationQueryGateway.Adults + reservationQueryGateway.Children18 + reservationQueryGateway.Children10 + reservationQueryGateway.Children3,
            };
        }
    }
}
