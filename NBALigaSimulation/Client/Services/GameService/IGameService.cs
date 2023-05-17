namespace NBALigaSimulation.Client.Services.GameService
{
    public interface IGameService
    {

        Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId);

    }
}
