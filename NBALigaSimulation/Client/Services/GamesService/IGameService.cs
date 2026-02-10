using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.GameService;

public interface IGameService
{
    
    Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId);
    Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames();
    Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId);
    Task<GameCompleteDto> CreateGame(CreateGameDto game);
    Task<ServiceResponse<bool>> UpdateGame(int gameId);
    Task<ServiceResponse<bool>> SimGameByDateRegular();
    Task<ServiceResponse<bool>> SimGameByDatePlayoffs();
    Task<ServiceResponse<bool>> SimGameByRound(int roundNumber);
    Task<ServiceResponse<bool>> SimPlayoffsByRound(int playoffRound);
    Task<ServiceResponse<bool>> SimAll();

    
    
}