using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.GameService;

public class GameService : IGameService
{
    
    private readonly HttpClient _http;

    public GameService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<ServiceResponse<List<GameCompleteDto>>> GetGamesByTeamId(int teamId)
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<List<GameCompleteDto>>>($"api/games/{teamId}");
        return response;
    }
}