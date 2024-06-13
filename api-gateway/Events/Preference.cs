using MessagePack;

namespace api_gateway.Events
{

    [MessagePackObject]
    public class Preference
    {
        [Key(0)]
        public int ReservationCount{ get; set; }
        [Key(1)]
        public int PurchaseCount { get; set; }
        public void Add(Preference other)
        {
            this.ReservationCount+= other.ReservationCount;
            this.PurchaseCount+= other.PurchaseCount;
        }
    }

    [MessagePackObject]
    public class PreferenceUpdate
    {
        [Key(0)]
        public string PreferenceType { get; set; }
        [Key(1)]
        public string PreferenceName { get; set; }
        [Key(2)]
        public Preference Preference { get; set; }
    }
}
