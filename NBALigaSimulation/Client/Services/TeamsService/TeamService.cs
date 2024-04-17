using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.TeamsService;

public class TeamService : ITeamService
{
    
    private readonly HttpClient _http;

    public TeamService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId)
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<TeamCompleteDto>>($"api/teams/{teamId}");
        if (response.Success == false)
        {
            return new ServiceResponse<TeamCompleteDto>() { Message = "Time não encontrado!" };
        }
        return response;
    }

    public async Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams()
    {
        var response = await _http.GetFromJsonAsync<ServiceResponse<List<TeamSimpleDto>>>($"api/teams");
        return response;
    }
    
    public async Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamSimpleWithPlayersDto>>>($"api/teams/players");
        return result;
    }

    public async Task<ServiceResponse<TeamCompleteDto>> GetTeamByUser()
    {
        var result = await _http.GetFromJsonAsync<ServiceResponse<TeamCompleteDto>>($"api/teams/GetTeamByLoggedUser");
        return result;
    }

    public async Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto)
    {
        var response = await _http.PutAsJsonAsync($"api/teams/{teamId}/gameplan", teamGameplanDto);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ServiceResponse<bool> { Success = false, Message = errorMessage };
        }

        return new ServiceResponse<bool> { Success = true };
    }

    public async Task<ServiceResponse<bool>> UpdateKeyPlayers(int teamId, List<PlayerCompleteDto> players)
    {
        var response = await _http.PutAsJsonAsync($"api/teams/{teamId}/keys", players);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            return new ServiceResponse<bool> { Success = false, Message = errorMessage };
        }

        return new ServiceResponse<bool> { Success = true };
    }
}