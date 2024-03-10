using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.PlayersService
{
    public interface IPlayerService
    {

        Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId);
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllPlayers();
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFaPlayers();
        Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers();
        Task<ServiceResponse<PlayerCompleteDto>> CreatePlayer(CreatePlayerDto request);
        Task<ServiceResponse<List<PlayerCompleteDto>>> CreatePlayers(List<CreatePlayerDto> request);
        Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText);
        Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList);
        Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier);
        Task<ServiceResponse<bool>> GenerateContracts();

    }
}
