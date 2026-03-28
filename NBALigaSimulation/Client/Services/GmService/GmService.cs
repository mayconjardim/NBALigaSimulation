using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Gm;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

namespace NBALigaSimulation.Client.Services.GmService;

public class GmService : IGmService
{
    private readonly HttpClient _http;

    public GmService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<GmProfileDto>> GetMyProfile()
    {
        return await _http.SafeGetAsync<GmProfileDto>("api/gm/profile");
    }

    public async Task<ServiceResponse<GmProfileDto>> GetProfileByTeamId(int teamId)
    {
        return await _http.SafeGetAsync<GmProfileDto>($"api/gm/team/{teamId}");
    }
}
