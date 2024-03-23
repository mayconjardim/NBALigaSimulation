using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayersService;

public interface IPlayerService
{
    
    Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId);
    Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText);
    
}