using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public interface IPlayersService
{
    
    Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId);
    
}