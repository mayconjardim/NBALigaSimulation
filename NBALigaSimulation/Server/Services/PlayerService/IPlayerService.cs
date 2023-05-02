namespace NBALigaSimulation.Server.Services.PlayerService
{
    public interface IPlayerService
    {

        Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllPlayers();
        Task<ServiceResponse<Player>> GetPlayerById(int playerId);

    }
}
