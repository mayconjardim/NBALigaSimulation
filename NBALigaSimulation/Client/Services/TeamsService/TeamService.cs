using System.Net.Http.Json;
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
        
        var response = await _http.GetAsync($"api/teams/{teamId}");

        if (response.IsSuccessStatusCode)
        {
            var team = await response.Content.ReadFromJsonAsync<TeamCompleteDto>();
            return new ServiceResponse<TeamCompleteDto> { Data = team, Success = true};
        }
        else
        {
            return new ServiceResponse<TeamCompleteDto> { Success = false, Message = "Não foi possivel encontrar o time!"};
        }
      
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
}