using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.AwardsService
{
    public interface IAwardsService
    {
        Task<ServiceResponse<bool>> GenerateAwards(int season);
        Task<ServiceResponse<List<PlayerAwardsDto>>> GetAllAwards();
    }
}
