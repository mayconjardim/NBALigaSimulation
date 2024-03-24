using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.SeasonsService;

public class SeasonService : ISeasonService
{
  
    private readonly HttpClient _http;

    public SeasonService(HttpClient http)
    {
        _http = http;
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> GetLastSeason()
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<CompleteSeasonDto>>($"api/seasons");
        return response;
    }

    public async Task<ServiceResponse<List<CompleteSeasonDto>>> GetALlSeason()
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<List<CompleteSeasonDto>>>($"api/seasons/all");
        return response;
    }
}