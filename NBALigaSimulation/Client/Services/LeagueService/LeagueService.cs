using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.League;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
        return await _http.SafeGetAsync<List<TeamDraftPickDto>>("api/league/picks");
    }

    public async Task<ServiceResponse<List<SeasonHistoryDto>>> GetSeasonHistory()
    {
        return await _http.SafeGetAsync<List<SeasonHistoryDto>>("api/league/history");
    }
}