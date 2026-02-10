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

    public async Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule()
    {
        var response = await _http.PutAsJsonAsync($"api/seasons/schedule", new { });
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
        return result;
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason()
    {
        var response = await _http.PostAsJsonAsync($"api/seasons", new { });
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
        return result;
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> CleanSchedule()
    {
        var response = await _http.DeleteAsync($"api/seasons/clean-schedule");
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
        return result;
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp()
    {
        var response = await _http.PostAsJsonAsync($"api/seasons/tc", new { });
        var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
        return result;
    }
}