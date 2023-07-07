namespace NBALigaSimulation.Client.Services.LeagueService
{
    public class LeagueService : ILeagueService
    {

        private readonly HttpClient _http;

        public LeagueService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<TeamDraftPickDto>>>($"api/league/picks");
            return result;
        }
    }
}
