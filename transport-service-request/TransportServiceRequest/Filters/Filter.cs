using MessagePack;

namespace TransportRequestService.Filters
{
    [MessagePackObject]
    public class Filter
    {
        [Key(0)]
        public IEnumerable<string>? Ids { get; set; } = null;//long?
        [Key(1)]
        public IEnumerable<string>? Types { get; set; } = null;
        [Key(2)]
        public IEnumerable<DateTime>? DepartureDates { get; set; } = null;
        [Key(3)]
        public IEnumerable<DateTime>? ArrivalDates { get; set; } = null;
        [Key(4)]
        public IEnumerable<string>? Destinations { get; set; } = null;
        [Key(5)]
        public IEnumerable<string>? Origins { get; set; } = null;
        //[Key(6)]
        //public int SeatsNumber { get; set; }
        //[Key(7)]
        //public int SeatsTaken { get; set; }
        //[Key(8)]
        //public decimal PricePerTicket { get; set; }
       

    }
}
