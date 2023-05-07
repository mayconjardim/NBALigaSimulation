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

        public async Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<TeamCompleteDto>>($"api/teams/{teamId}");
            return result;
        }

    }
}
