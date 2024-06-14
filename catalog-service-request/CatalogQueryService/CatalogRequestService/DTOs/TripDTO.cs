using MessagePack;

namespace CatalogRequestService.DTOs
{
    [MessagePackObject]
    public class TripDTO
    {
        [Key(0)] public int HotelId { get; set; }
        [Key(1)] public int RoomTypeId { get; set; }
        [Key(2)] public int TransportThereId { get; set; }
        [Key(3)] public int TransportBackId { get; set; }
        [Key(4)] public string? DestinationCity { get; set; }
        [Key(5)] public string? OriginCity { get; set; }
        [Key(6)] public string? DateStart { get; set; }
        [Key(7)] public string? DateEnd { get; set; }
        [Key(8)] public string? Price { get; set; }
        [Key(9)] public int PeopleNumber { get; set; }
    }


    //[MessagePackObject]
    //public class TripDTO
    //{
    //    [Key(0)]
    //    public int HotelId { get; set; }
    //    [Key(1)]
    //    public List<int> TransportThereIds { get; set; }
    //    [Key(2)]
    //    public List<int> TransportBackIds { get; set; }
    //    [Key(3)]
    //    public decimal Price { get; set; }

    //    public TripDTO(int hotelId) 
    //    { 
    //        HotelId = hotelId; 
    //        TransportThereIds = new List<int>();
    //        TransportBackIds = new List<int>();
    //    }
    //}
}
