using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
        return await _http.SafeGetAsync<TeamCompleteDto>($"api/teams/{teamId}");
    }

    public async Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams()
    {
        return await _http.SafeGetAsync<List<TeamSimpleDto>>("api/teams");
    }
    
    public async Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers()
    {
        return await _http.SafeGetAsync<List<TeamSimpleWithPlayersDto>>("api/teams/players");
    }

    public async Task<ServiceResponse<TeamCompleteDto>> GetTeamByUser()
    {
        return await _http.SafeGetAsync<TeamCompleteDto>("api/teams/GetTeamByLoggedUser");
    }

    public async Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto)
    {
        return await _http.SafePutAsync<TeamGameplanDto, bool>($"api/teams/{teamId}/gameplan", teamGameplanDto);
    }

    public async Task<ServiceResponse<bool>> UpdateKeyPlayers(int teamId, List<PlayerCompleteDto> players)
    {
        return await _http.SafePutAsync<List<PlayerCompleteDto>, bool>($"api/teams/{teamId}/keys", players);
    }
}