﻿using MessagePack;

namespace api_gateway.DTOs
{
    [MessagePackObject]
    public class RoomTypeDTO
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Capacity { get; set; }
        [Key(2)]
        public string Name { get; set; }
    }
}
