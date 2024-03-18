using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.GameService;

public interface IGameService
{
    
    Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId);

}