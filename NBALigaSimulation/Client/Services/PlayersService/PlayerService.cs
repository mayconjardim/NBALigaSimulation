using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

namespace NBALigaSimulation.Client.Services.PlayersService;

public class PlayerService : IPlayerService
{
    private readonly HttpClient _http;

    public PlayerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
    {
        return await _http.SafeGetAsync<PlayerCompleteDto>($"api/players/{playerId}");
    }

    public async Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
    {
        return await _http.SafePutAsync<List<PlayerCompleteDto>, bool>("api/players/rosterorder", updatedPlayerList);
    }

    public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText)
    {
        return await _http.SafeGetAsync<List<PlayerSimpleDto>>($"api/players/searchsuggestion/{searchText}");
    }

    public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllSimplePlayers()
    {
        return await _http.SafeGetAsync<List<PlayerSimpleDto>>("api/players/simple");
    }

    public async Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier)
    {
        return await _http.SafePutAsync<double, bool>($"api/players/{playerId}/ptmodifier", newPtModifier);
    }
    
    public async Task<ServiceResponse<PageableResponse<PlayerCompleteDto>>> GetAllFaPlayers(int currentPage, int pageSize, int season, bool isAscending, string sortedColumn,
        string position = null)
    {
        var url = $"api/players/FAPlayers?page={currentPage}&pageSize={pageSize}&season={season}&isAscending={isAscending}&sortedColumn={sortedColumn}";

        if (!string.IsNullOrEmpty(position))
        {
            url += $"&position={Uri.EscapeDataString(position)}";
        }  

        return await _http.SafeGetAsync<PageableResponse<PlayerCompleteDto>>(url);
    }
    
    public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers()
    {
        return await _http.SafeGetAsync<List<PlayerCompleteDto>>("api/players/draft");
    }

    public async Task<ServiceResponse<PlayerCompleteDto>> EditPlayer(CreatePlayerDto playerDto)
    {
        return await _http.SafePostAsync<CreatePlayerDto, PlayerCompleteDto>("api/players/edit-player", playerDto);
    }
}
