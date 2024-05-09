namespace HotelsQueryService.Queries
{
    public class HotelsReserveQuery
    {
        public int HotelId { get; set; }
        public int RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int ReservationId { get; set; }
    }
}
