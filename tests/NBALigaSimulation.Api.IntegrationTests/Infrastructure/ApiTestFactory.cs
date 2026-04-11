using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NBALigaSimulation.Server.Data;
using NBALigaSimulation.Server.Repositories.Interfaces;
using NBALigaSimulation.Server.Services.AuthService;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;
using NBALigaSimulation.Shared.Models.Users;

namespace NBALigaSimulation.Api.IntegrationTests.Infrastructure;

public class ApiTestFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"nbaliga-tests-{Guid.NewGuid()}";

    public FakeAuthService FakeAuth { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<DataContext>));
            services.RemoveAll(typeof(IDbContextOptionsConfiguration<DataContext>));
            services.RemoveAll<DataContext>();
            services.RemoveAll<IAuthService>();

            services.AddSingleton<IAuthService>(FakeAuth);
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            SeedDatabase(context);
        });
    }

    private static void SeedDatabase(DataContext context)
    {
        var teamOne = new Team { Id = 1, Name = "Rio Stars", Region = "Rio", Abrv = "RIO", Conference = "East", IsHuman = true };
        var teamTwo = new Team { Id = 2, Name = "Sampa Kings", Region = "Sao Paulo", Abrv = "SPK", Conference = "West", IsHuman = true };
        var freeAgents = new Team { Id = 21, Name = "Free Agents", Region = "League", Abrv = "FA", Conference = "None", IsHuman = false };

        var user = new User
        {
            Id = 100,
            Username = "integration-user",
            PasswordHash = new byte[] { 1 },
            PasswordSalt = new byte[] { 2 },
            DataCreated = DateTime.UtcNow,
            TeamId = 1,
            IsAdmin = false
        };

        var season = new Season { Id = 1, Year = 2026 };

        var playerOne = CreatePlayer(10, "Guard One", "PG", teamOne.Id, 85, 88);
        var playerTwo = CreatePlayer(11, "Wing Two", "SG", teamTwo.Id, 82, 84);
        var freeAgent = CreatePlayer(12, "Free Agent", "SF", freeAgents.Id, 78, 80);

        playerOne.Contract = new PlayerContract { Id = 1000, PlayerId = playerOne.Id, Amount = 8_000_000, Exp = 2028 };
        playerTwo.Contract = new PlayerContract { Id = 1001, PlayerId = playerTwo.Id, Amount = 9_000_000, Exp = 2028 };

        playerTwo.RegularStats.Add(new PlayerRegularStats
        {
            Id = 2000,
            Name = playerTwo.Name,
            Pos = playerTwo.Pos,
            PlayerId = playerTwo.Id,
            TeamId = teamTwo.Id,
            TeamAbrv = teamTwo.Abrv,
            Season = 2025
        });

        freeAgent.RegularStats.Add(new PlayerRegularStats
        {
            Id = 2001,
            Name = freeAgent.Name,
            Pos = freeAgent.Pos,
            PlayerId = freeAgent.Id,
            TeamId = freeAgents.Id,
            TeamAbrv = freeAgents.Abrv,
            Season = 2025
        });

        var pickOne = new TeamDraftPicks { Id = 3000, TeamId = teamOne.Id, Original = teamOne.Abrv, Year = 2027, Round = 1 };
        var pickTwo = new TeamDraftPicks { Id = 3001, TeamId = teamTwo.Id, Original = teamTwo.Abrv, Year = 2027, Round = 1 };

        context.Teams.AddRange(teamOne, teamTwo, freeAgents);
        context.Users.Add(user);
        context.Seasons.Add(season);
        context.Players.AddRange(playerOne, playerTwo, freeAgent);
        context.TeamDraftPicks.AddRange(pickOne, pickTwo);
        context.SaveChanges();
    }

    private static Player CreatePlayer(int id, string name, string pos, int teamId, int oiq, int pot)
    {
        var player = new Player
        {
            Id = id,
            Name = name,
            Pos = pos,
            Born = new Born { Year = 2000, Loc = "Brazil" },
            College = "Integration U",
            Hgt = 78,
            Weight = 210,
            ImgUrl = "Images/Utils/blank.png",
            TeamId = teamId,
            PtModifier = 1,
            RosterOrder = id
        };

        player.Ratings.Add(new PlayerRatings
        {
            Id = id * 10,
            PlayerId = id,
            Season = 2025,
            Diq = 70,
            Dnk = 70,
            Drb = 75,
            Endu = 80,
            Fg = 72,
            Ft = 76,
            Hgt = 78,
            Ins = 71,
            Jmp = 74,
            Oiq = oiq,
            Pot = pot,
            Pss = 74,
            Reb = 68,
            Spd = 77,
            Stre = 69,
            Tp = 73
        });

        player.Ratings.Add(new PlayerRatings
        {
            Id = id * 10 + 1,
            PlayerId = id,
            Season = 2026,
            Diq = 71,
            Dnk = 71,
            Drb = 76,
            Endu = 81,
            Fg = 73,
            Ft = 77,
            Hgt = 78,
            Ins = 72,
            Jmp = 75,
            Oiq = oiq,
            Pot = pot,
            Pss = 75,
            Reb = 69,
            Spd = 78,
            Stre = 70,
            Tp = 74
        });

        return player;
    }
}
