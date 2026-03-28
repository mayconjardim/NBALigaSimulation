using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public interface IPlayerContractService
    {
        Task<ServiceResponse<bool>> GenerateContracts();
        Task<ServiceResponse<bool>> GenerateContractsForTest();
        Task<ServiceResponse<bool>> HandleExpiredContractsAsync(int nextSeasonYear);
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetExpiringPlayers(int season);
    }
}
