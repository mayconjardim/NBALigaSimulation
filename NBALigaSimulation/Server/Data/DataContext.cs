using Microsoft.EntityFrameworkCore;

namespace NBALigaSimulation.Server.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerRatings> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            modelBuilder.Entity<PlayerRatings>()
                .HasOne(e => e.Player)
                .WithMany(d => d.Ratings)
                .HasForeignKey(e => e.PlayerId);
        }

    }
}
