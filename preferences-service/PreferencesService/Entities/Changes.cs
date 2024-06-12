using MessagePack;

namespace PreferencesService.Entities
{
    [MessagePackObject]
    public class Changes
    {
        [Key(0)]
        public string ResourceType{ get; set; }
        [Key(1)]
        public string Id { get; set; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public bool Availability { get; set; }
        [Key(4)]
        public decimal PriceChange { get; set; }

    }
}
