using Microsoft.EntityFrameworkCore;

namespace HotelsQueryService.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<Entities.Country> Countries { get; set; }
        public DbSet<Entities.City> Cities { get; set; }
        public DbSet<Entities.Hotel> Hotels { get; set; }
        public DbSet<Entities.RoomType> Rooms { get; set; }
        public DbSet<Entities.HasRoom> HasRooms { get; set; }
        public DbSet<Entities.Occupancy> Occupancies { get; set; }
        public DbSet<Entities.Reservation> Reservations { get; set; }


    }
}
