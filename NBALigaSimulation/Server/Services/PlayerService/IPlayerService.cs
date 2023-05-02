namespace NBALigaSimulation.Server.Services.PlayerService
{
    public interface IPlayerService
    {

        Task<ServiceResponse<List<Player>>> GetAllPlayers();
        Task<ServiceResponse<Player>> GetPlayerById(int playerId);

    }
}
