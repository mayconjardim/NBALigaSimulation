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
    Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFAPlayers();

    
}