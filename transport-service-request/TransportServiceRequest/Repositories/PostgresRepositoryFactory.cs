using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportRequestService.Repositories
{
    public class PostgresRepositoryFactory
    {
        private readonly DbContextOptions<PostgresRepository> _options;
        public PostgresRepositoryFactory(DbContextOptions<PostgresRepository> options) { 
            this ._options = options;
        }

        public PostgresRepository GetRepository()
        {
            return new PostgresRepository(_options);
        }

    }
}
