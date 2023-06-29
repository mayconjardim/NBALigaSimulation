namespace NBALigaSimulation.Client.Services.SeasonService
{
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
    }
}
