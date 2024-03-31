using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.StatsService;

public class StatsService : IStatsService
{
    
    private readonly HttpClient _http;

    public StatsService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<PageableStatsResponse<PlayerRegularStatsDto>>> GetAllPlayerRegularStats(int page, int pageSize, int season,
        bool isAscending, string position, string stat = null)
    {
        var url = $"api/stats/players?page={page}&pageSize={pageSize}&season={season}&isAscending={isAscending}";

        if (!string.IsNullOrEmpty(position))
        {
            url += $"&position={Uri.EscapeDataString(position)}";
        }  
            
        if (!string.IsNullOrEmpty(stat))
        {
            url += $"&stat={Uri.EscapeDataString(stat)}";
        }


        return await _http.GetFromJsonAsync<ServiceResponse<PageableStatsResponse<PlayerRegularStatsDto>>>(url);
    }

    
    public async Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamRegularStatsDto>>>($"api/stats/teams");
        return result;
    }
    
    public async Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamRegularStatsRankDto>>>($"api/stats/teams/ranks");
        return result;
    }
    
}