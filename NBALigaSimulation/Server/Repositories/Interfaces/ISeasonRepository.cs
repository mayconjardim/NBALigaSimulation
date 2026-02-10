using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Server.Repositories.Interfaces
{
    public interface ISeasonRepository : IGenericRepository<Season>
    {
        Task<Season?> GetLastSeasonAsync();
        Task<List<Season>> GetAllOrderedAsync();
        Task<List<Team>> GetHumanTeamsAsync();
        Task<List<Game>> GetGamesBySeasonAsync(int seasonId);
        Task AddGamesAsync(IEnumerable<Game> games);
        Task RemoveGamesAsync(IEnumerable<Game> games);
        Task<List<Game>> GetGamesWithTeamsBySeasonAsync(int seasonId);
        Task<List<Player>> GetPlayersWithRatingsAsync();
    }
}
