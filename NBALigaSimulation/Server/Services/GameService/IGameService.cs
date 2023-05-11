namespace NBALigaSimulation.Server.Services.GameService
{
    public interface IGameService
    {

        Task<ServiceResponse<List<GameCompleteDto>>> GetAllGames();
        Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId);
        Task<ServiceResponse<bool>> CreateGame(CreateGameDto request);
        Task<ServiceResponse<bool>> UpdateGame(int GameId);
    }
}
