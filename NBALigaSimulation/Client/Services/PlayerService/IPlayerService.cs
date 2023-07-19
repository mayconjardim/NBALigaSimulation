namespace NBALigaSimulation.Client.Services.PlayerService
{
    public interface IPlayerService
    {

        Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int productId);
        Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList);
        Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier);
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFAPlayers();
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers();

    }
}
