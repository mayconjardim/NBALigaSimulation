using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Seasons;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
        return await _http.SafeGetAsync<CompleteSeasonDto>("api/seasons");
    }

    public async Task<ServiceResponse<List<CompleteSeasonDto>>> GetALlSeason()
    {
        return await _http.SafeGetAsync<List<CompleteSeasonDto>>("api/seasons/all");
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule()
    {
        return await _http.SafePutAsync<object, CompleteSeasonDto>("api/seasons/schedule", new { });
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason()
    {
        return await _http.SafePostAsync<object, CompleteSeasonDto>("api/seasons", new { });
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> CleanSchedule()
    {
        var response = await _http.DeleteAsync("api/seasons/clean-schedule");
        return await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>() ?? new ServiceResponse<CompleteSeasonDto> { Success = false };
    }

    public async Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp()
    {
        return await _http.SafePostAsync<object, CompleteSeasonDto>("api/seasons/tc", new { });
    }
}