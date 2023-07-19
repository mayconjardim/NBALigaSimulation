namespace NBALigaSimulation.Server.Services.PlayerService
{
    public interface IPlayerService
    {

        Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllPlayers();
        Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId);
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFAPlayers();
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers();
        Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request);
        Task<ServiceResponse<List<PlayerCompleteDto>>> CreatePlayers(List<CreatePlayerDto> request);
        Task<ServiceResponse<List<PlayerSimpleDto>>> SearchPlayers(string searchText);
        Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList);
        Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier);
        Task<ServiceResponse<bool>> GenerateContracts();

    }
}
