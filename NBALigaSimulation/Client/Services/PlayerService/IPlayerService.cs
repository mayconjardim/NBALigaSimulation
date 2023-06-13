namespace NBALigaSimulation.Client.Services.PlayerService
{
    public interface IPlayerService
    {
        Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int productId);
        Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList);


    }
}
