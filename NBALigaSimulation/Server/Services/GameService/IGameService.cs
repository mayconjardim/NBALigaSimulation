using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.GameService
{
	public interface IGameService
	{

		Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId);
		Task<ServiceResponse<List<GameCompleteDto>>> GetGamesBetweenTeams(int teamAId, int teamBId);
		Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId);
		Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames();
		Task<ServiceResponse<GameCompleteDto>> CreateGame(CreateGameDto request);
		Task<ServiceResponse<bool>> UpdateGame(int GameId);
		Task<ServiceResponse<bool>> UpdateGames();
		Task<ServiceResponse<bool>> SimGameByDateRegular();
		Task<ServiceResponse<bool>> SimGameByDatePlayoffs();
		Task<ServiceResponse<bool>> SimGameByRound(int roundNumber);
		Task<ServiceResponse<bool>> SimPlayoffsByRound(int playoffRound);
		Task<ServiceResponse<bool>> SimAll();



	}
}
