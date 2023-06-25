namespace NBALigaSimulation.Server.Services.GameService
{
    public interface IGameService
    {

        Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames();
        Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId);
        Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId);
        Task<ServiceResponse<GameCompleteDto>> CreateGame(CreateGameDto request);
        Task<ServiceResponse<bool>> UpdateGame(int GameId);
        Task<ServiceResponse<bool>> UpdateGames();
    }
}
