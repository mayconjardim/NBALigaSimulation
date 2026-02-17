using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Playoffs;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayoffsService;

public class PlayoffsService : IPlayoffsService
{
    private readonly HttpClient _http;

    public PlayoffsService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs()
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<List<PlayoffsDto>>>("api/playoffs");
        return response;
    }

    public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffsBySeason(int season)
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<List<PlayoffsDto>>>($"api/playoffs/season/{season}");
        return response;
    }

    public async Task<ServiceResponse<bool>> GenerateFirstRound()
    {
        var httpResponse = await _http.PostAsJsonAsync("api/playoffs/generate", new { });
        var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result;
    }

    public async Task<ServiceResponse<bool>> GenerateSecondRound()
    {
        var httpResponse = await _http.PostAsJsonAsync("api/playoffs/generate/confsemis", new { });
        var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result;
    }

    public async Task<ServiceResponse<bool>> GenerateThirdRound()
    {
        var httpResponse = await _http.PostAsJsonAsync("api/playoffs/generate/conffinals", new { });
        var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result;
    }

    public async Task<ServiceResponse<bool>> GenerateFourthRound()
    {
        var httpResponse = await _http.PostAsJsonAsync("api/playoffs/generate/finals", new { });
        var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result;
    }

    public async Task<ServiceResponse<bool>> EndPlayoffs()
    {
        var httpResponse = await _http.PostAsJsonAsync("api/playoffs/end", new { });
        var result = await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        return result;
    }
}

