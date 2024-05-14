
using Microsoft.EntityFrameworkCore;

namespace OrderService.Repositories
{
    public class PostgresRepository: DbContext
    {
        public PostgresRepository(DbContextOptions<PostgresRepository> options) : base(options)
        { }

        public DbSet<Entities.Order> Orders{ get; set; }

    }
    
}
