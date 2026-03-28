using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public interface IPlayerContractService
{
    Task<ServiceResponse<List<PlayerCompleteDto>>> GetExpiringPlayers(int season);
}
