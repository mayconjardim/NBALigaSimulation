
namespace NBALigaSimulation.Server.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerRatings> Ratings { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<TeamGameStats> TeamGameStats { get; set; }
        public DbSet<PlayerGameStats> PlayerGameStats { get; set; }

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

            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasMany(p => p.Stats)
                    .WithOne()
                    .HasForeignKey(s => s.PlayerId);
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.HomeTeam)
                    .WithMany()
                    .HasForeignKey(e => e.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AwayTeam)
                    .WithMany()
                    .HasForeignKey(e => e.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasMany(t => t.Stats)
                    .WithOne()
                    .HasForeignKey(s => s.TeamId);
            });


        }
    }
}
