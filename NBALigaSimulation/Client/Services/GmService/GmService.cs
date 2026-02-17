using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Gm;
using NBALigaSimulation.Shared.Models.Utils;

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
        var response = await _http.GetFromJsonAsync<ServiceResponse<GmProfileDto>>("api/gm/profile");
        return response ?? new ServiceResponse<GmProfileDto> { Success = false, Message = "Resposta inválida." };
    }

    public async Task<ServiceResponse<GmProfileDto>> GetProfileByTeamId(int teamId)
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<GmProfileDto>>($"api/gm/team/{teamId}");
        return response ?? new ServiceResponse<GmProfileDto> { Success = false, Message = "Resposta inválida." };
    }
}
