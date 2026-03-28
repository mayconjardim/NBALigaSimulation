using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public interface IPlayerEvolutionService
    {
        Task<ServiceResponse<bool>> GenerateTrainingCampAsync(Season season);
        Task<ServiceResponse<bool>> HandleOffSeasonRecoveryAsync(int healingAmount);
        Task<ServiceResponse<bool>> RegenerateAllScoutReports();
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetInjuredPlayers();
    }
}
