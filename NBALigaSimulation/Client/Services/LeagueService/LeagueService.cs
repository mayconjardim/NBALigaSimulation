using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.League;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.LeagueService;

public class LeagueService : ILeagueService
{
    private readonly HttpClient _http;

    public LeagueService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamDraftPickDto>>>($"api/league/picks");
        return result;
    }

    public async Task<ServiceResponse<List<SeasonHistoryDto>>> GetSeasonHistory()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<SeasonHistoryDto>>>("api/league/history");
        return result ?? new ServiceResponse<List<SeasonHistoryDto>> { Success = false };
    }
}