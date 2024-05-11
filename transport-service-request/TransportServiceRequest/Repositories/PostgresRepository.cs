
using Microsoft.EntityFrameworkCore;

namespace TransportRequestService.Repositories
{
    public class PostgresRepository: DbContext
    {
        public PostgresRepository(DbContextOptions<PostgresRepository> options) : base(options)
        { }

        public DbSet<Entities.Transport> Transports{ get; set; }

        public DbSet<Entities.TransportEvent> TransportEvents { get; set; }
    }
    
}
