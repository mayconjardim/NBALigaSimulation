namespace NBALigaSimulation.Client.Services.PlayerService
{
    public interface IPlayerService
    {

        string Message { get; set; }

        Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int productId);
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllPlayers();
        Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList);
        Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, decimal newPtModifier);
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFAPlayers();
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers();
        Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText);
    }
}
