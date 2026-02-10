using Microsoft.EntityFrameworkCore;
using NBALigaSimulation.Server.Data;
using NBALigaSimulation.Server.Repositories.Interfaces;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Server.Repositories.Implementations
{
    public class SeasonRepository : GenericRepository<Season>, ISeasonRepository
    {
        public SeasonRepository(DataContext context) : base(context)
        {
        }

        public async Task<Season?> GetLastSeasonAsync()
        {
            return await _context.Seasons
                .OrderBy(s => s.Year)
                .LastOrDefaultAsync();
        }

        public async Task<List<Season>> GetAllOrderedAsync()
        {
            return await _context.Seasons
                .OrderBy(s => s.Year)
                .ToListAsync();
        }

        public async Task<List<Team>> GetHumanTeamsAsync()
        {
            return await _context.Teams
                .Where(t => t.IsHuman)
                .ToListAsync();
        }

        public async Task<List<Game>> GetGamesBySeasonAsync(int seasonId)
        {
            return await _context.Games
                .Where(g => g.SeasonId == seasonId)
                .ToListAsync();
        }

        public async Task AddGamesAsync(IEnumerable<Game> games)
        {
            await _context.Games.AddRangeAsync(games);
        }

        public Task RemoveGamesAsync(IEnumerable<Game> games)
        {
            _context.Games.RemoveRange(games);
            return Task.CompletedTask;
        }

        public async Task<List<Game>> GetGamesWithTeamsBySeasonAsync(int seasonId)
        {
            return await _context.Games
                .Where(g => g.SeasonId == seasonId)
                .Include(t => t.HomeTeam)
                .Include(t => t.AwayTeam)
                .ToListAsync();
        }

        public async Task<List<Player>> GetPlayersWithRatingsAsync()
        {
            return await _context.Players
                .Include(p => p.Ratings)
                .ToListAsync();
        }
    }
}
