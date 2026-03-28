using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Playoffs;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
        return await _http.SafeGetAsync<List<PlayoffsDto>>("api/playoffs");
    }

    public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffsBySeason(int season)
    {
        return await _http.SafeGetAsync<List<PlayoffsDto>>($"api/playoffs/season/{season}");
    }

    public async Task<ServiceResponse<bool>> GenerateFirstRound()
    {
        return await _http.SafePostAsync<object, bool>("api/playoffs/generate", new { });
    }

    public async Task<ServiceResponse<bool>> GenerateSecondRound()
    {
        return await _http.SafePostAsync<object, bool>("api/playoffs/generate/confsemis", new { });
    }

    public async Task<ServiceResponse<bool>> GenerateThirdRound()
    {
        return await _http.SafePostAsync<object, bool>("api/playoffs/generate/conffinals", new { });
    }

    public async Task<ServiceResponse<bool>> GenerateFourthRound()
    {
        return await _http.SafePostAsync<object, bool>("api/playoffs/generate/finals", new { });
    }

    public async Task<ServiceResponse<bool>> EndPlayoffs()
    {
        return await _http.SafePostAsync<object, bool>("api/playoffs/end", new { });
    }
}

