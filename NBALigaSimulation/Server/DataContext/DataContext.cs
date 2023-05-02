using Microsoft.EntityFrameworkCore;

namespace NBALigaSimulation.Server.DataContext
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerRatings> Ratings { get; set; }

    }
}
