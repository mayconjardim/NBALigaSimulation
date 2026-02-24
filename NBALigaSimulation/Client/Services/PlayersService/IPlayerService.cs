using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public interface IPlayerService
{
    
    Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId);
    Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList);
    Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText);
    Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllSimplePlayers();
    Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier);
    Task<ServiceResponse<PageableResponse<PlayerCompleteDto>>> GetAllFaPlayers(int currentPage, int pageSize,
        int season, bool isAscending, string sortedColumn, string position = null);
    Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers();
    Task<ServiceResponse<List<PlayerCompleteDto>>> GetExpiringPlayers(int season);
    Task<ServiceResponse<List<PlayerCompleteDto>>> GetInjuredPlayers();
    Task<ServiceResponse<PlayerCompleteDto>> EditPlayer(CreatePlayerDto request);


}