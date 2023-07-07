using System.Net.Http;

namespace NBALigaSimulation.Client.Services.TeamService
{
    public class TeamService : ITeamService
    {

        private readonly HttpClient _http;

        public TeamService(HttpClient http)
        {
            _http = http;
        }

        public string Message { get; set; } = "Carregando Time...";

        public async Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamSimpleDto>>>($"api/teams");
            return result;
        }

        public async Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamSimpleWithPlayersDto>>>($"api/teams/players");
            return result;
        }

        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<TeamCompleteDto>>($"api/teams/{teamId}");
            return result;
        }

        public async Task<ServiceResponse<TeamCompleteDto>> GetUserTeam()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<TeamCompleteDto>>($"api/teams/profile");
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
    }
}
