using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public interface IPlayerEvolutionService
{
    Task<ServiceResponse<List<PlayerCompleteDto>>> GetInjuredPlayers();
}
