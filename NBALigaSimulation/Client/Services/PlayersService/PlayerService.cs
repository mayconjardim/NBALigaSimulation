using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

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
        var response = await _http.GetFromJsonAsync<ServiceResponse<PlayerCompleteDto>>($"api/players/{playerId}");
        return response;
    }

    public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText)
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerSimpleDto>>>($"api/players/searchsuggestion/{searchText}");
        return result;
    }

    public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetAllSimplePlayers()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerSimpleDto>>>($"api/players/simple");
        return result;
    }
}