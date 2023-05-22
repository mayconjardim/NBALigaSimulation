namespace NBALigaSimulation.Client.Services.GameService
{
    public interface IGameService
    {

        Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId);
        Task<GameCompleteDto> CreateGame(CreateGameDto game);
        Task<ServiceResponse<bool>> UpdateGame(int gameId);
    }
}
