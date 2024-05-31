using CatalogRequestService.Filters;
using MessagePack;

namespace CatalogRequestService.Requests
{
    [MessagePackObject]
    public class TripReserveRequest
    {
        [Key(0)]
        public int HotelId { get; set; }
        [Key(1)]
        public string RoomType { get; set; }
        [Key(2)]
        public int PeopleNumber { get; set; }
        [Key(3)]
        public DateTime CheckInDate { get; set; }
        [Key(4)]
        public DateTime CheckOutDate { get; set; }
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
        [Key(8)]
        public int TransportId { get; set; }
        [Key(9)]
        public int ClientId { get; set; }
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

        public static TripReserveRequest AdaptReservationQueryToTripQuery(ReservationQueryGateway reservationQuery)
        {
            DateTime tmp1, tmp2;
            if (!DateTime.TryParseExact(reservationQuery.Date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out tmp1))
            {
                tmp1 = DateTime.Now.Date;
            }

            if (!DateTime.TryParseExact(reservationQuery.Date, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out tmp2))
            {
                tmp2 = DateTime.Now.Date.AddDays(7);
            }

            // string : roomTypeName (Capacity)
            var roomType = reservationQuery.RoomType.Split(" ")[0] ?? null;

            return new TripReserveRequest
            {
                HotelId = reservationQuery.HotelId,
                RoomType = roomType,
                PeopleNumber = reservationQuery.Adults + reservationQuery.Children18 + reservationQuery.Children10 + reservationQuery.Children3,
                CheckInDate = tmp1,
                CheckOutDate = tmp2,
                TransportId = reservationQuery.TransportId,
                ClientId = reservationQuery.ClientId
            };
        }
    }


}
