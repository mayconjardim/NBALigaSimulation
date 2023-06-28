namespace NBALigaSimulation.Server.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<TeamGameStats> TeamGameStats { get; set; }
        public DbSet<TeamGameplan> TeamGameplans { get; set; }
        public DbSet<TeamDraftPicks> TeamDraftPicks { get; set; }
        public DbSet<PlayerGameStats> PlayerGameStats { get; set; }
        public DbSet<PlayerContract> PlayerContracts { get; set; }
        public DbSet<GamePlayByPlay> PlayByPlays { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradePicks> TradePicks { get; set; }
        public DbSet<FAOffer> FAOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<User>()
            .HasOne(u => u.Team)
            .WithOne()
            .HasForeignKey<User>(u => u.TeamId)
            .IsRequired(false);

            modelBuilder.Entity<Season>()
              .HasMany(s => s.Games)
              .WithOne(g => g.Season)
              .HasForeignKey(g => g.SeasonId);

            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId);

            modelBuilder.Entity<Player>()
          .OwnsOne(p => p.Born);

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

            modelBuilder.Entity<Player>()
            .HasOne(p => p.Contract)
            .WithOne(p => p.Player)
            .HasForeignKey<PlayerContract>(c => c.PlayerId);

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

            modelBuilder.Entity<PlayerGameStats>()
            .HasOne(p => p.Game)
            .WithMany(g => g.PlayerGameStats)
            .HasForeignKey(p => p.GameId);

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.HasMany(t => t.Stats)
                    .WithOne()
                    .HasForeignKey(s => s.TeamId);
            });

            modelBuilder.Entity<Team>()
            .HasMany(t => t.DraftPicks)
            .WithOne(dp => dp.Team)
            .HasForeignKey(dp => dp.TeamId);

            modelBuilder.Entity<Team>()
            .HasOne(p => p.Gameplan)
            .WithOne(g => g.Team)
            .HasForeignKey<TeamGameplan>(g => g.TeamId);

            modelBuilder.Entity<Trade>(entity =>
           {
               entity.HasKey(t => t.Id);
               entity.Property(t => t.DateCreated).HasDefaultValueSql("GETDATE()");

               entity.HasOne(t => t.TeamOne)
                   .WithMany()
                   .HasForeignKey(t => t.TeamOneId)
                   .OnDelete(DeleteBehavior.Restrict);

               entity.HasOne(t => t.TeamTwo)
                   .WithMany()
                   .HasForeignKey(t => t.TeamTwoId)
                   .OnDelete(DeleteBehavior.Restrict);
           });


            modelBuilder.Entity<TradePlayer>(entity =>
            {
                entity.HasKey(tp => new { tp.PlayerId, tp.TradePlayerId });

                entity.HasOne(tp => tp.Player)
                    .WithMany()
                    .HasForeignKey(tp => tp.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tp => tp.Trade)
                    .WithMany(t => t.TradePlayers)
                    .HasForeignKey(tp => tp.TradePlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<TradePicks>(entity =>
            {
                entity.HasKey(tp => new { tp.DraftPickId, tp.TradePickId });

                entity.HasOne(tp => tp.DraftPick)
                    .WithMany()
                    .HasForeignKey(tp => tp.DraftPickId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(tp => tp.Trade)
                    .WithMany(t => t.TradePicks)
                    .HasForeignKey(tp => tp.TradePickId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<FAOffer>()
           .HasOne(f => f.Player)
           .WithMany()
           .HasForeignKey(f => f.PlayerId);

        }
    }
}
