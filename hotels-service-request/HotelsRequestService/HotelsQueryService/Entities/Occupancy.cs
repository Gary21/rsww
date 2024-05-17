﻿using System.ComponentModel.DataAnnotations.Schema;

using EF = Microsoft.EntityFrameworkCore;
using DA = System.ComponentModel.DataAnnotations;
using MP = MessagePack;

namespace HotelsQueryService.Entities
{
    [EF.PrimaryKey("HotelId", "RoomNumber", "CheckIn")]
    [MP.MessagePackObject]
    public class Occupancy
    {
        [DA.Key, Column(Order = 0)]
        [MP.Key(0)]
        required
        public int HotelId { get; set; }

        [DA.Key, Column(Order = 1)]
        [MP.Key(1)]
        required
        public int RoomNumber { get; set; }

        [DA.Key, Column(Order = 2)]
        [MP.Key(2)]
        required
        public DateTime CheckIn { get; set; }

        [DA.Required]
        [MP.Key(3)]
        required
        public DateTime CheckOut { get; set; }

        [DA.Required]
        [MP.Key(4)]
        required
        public int ReservationId { get; set; }

        [DA.Required]
        [MP.Key(5)]
        required
        public Room HasRoom { get; set; }
    }
}