using MessagePack;

namespace SimulationService.Filters
{
    [MessagePackObject]
    public class Filter
    {
        [Key(0)]
        public IEnumerable<int>? Ids { get; set; } = null;//long?
        [Key(1)]
        public IEnumerable<string>? Types { get; set; } = null;
        [Key(2)]
        public IEnumerable<DateTime>? DepartureDates { get; set; } = null;
        [Key(3)]
        public IEnumerable<DateTime>? ArrivalDates { get; set; } = null;
        [Key(4)]
        public IEnumerable<string>? CountryDestinations { get; set; } = null;
        [Key(5)]
        public IEnumerable<string>? CityDestinations { get; set; } = null;
        [Key(6)]
        public IEnumerable<string>? CountryOrigins { get; set; } = null;
        [Key(7)]
        public IEnumerable<string>? CityOrigins { get; set; } = null;
        [Key(8)]
        public int? AvailableSeats { get; set; } = null;
        //[Key(6)]
        //public int SeatsNumber { get; set; }
        //[Key(7)]
        //public int SeatsTaken { get; set; }
        //[Key(8)]
        //public decimal PricePerTicket { get; set; }
        public Filter() { }


    }

    [MessagePackObject]
    public class OrderFilter
    {
        [Key(0)]
        public IEnumerable<int>? Ids { get; set; } = null;//long?
        [Key(1)]
        public IEnumerable<int>? UserIds { get; set; } = null;

    }
}
