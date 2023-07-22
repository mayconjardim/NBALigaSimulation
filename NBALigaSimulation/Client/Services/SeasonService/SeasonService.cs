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

        public async Task<ServiceResponse<CompleteSeasonDto>> CreateSeason()
        {
            var payload = new ServiceResponse<CompleteSeasonDto>();
            var response = await _http.PostAsJsonAsync("api/seasons", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
            return result;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateTrainingCamp()
        {
            var payload = new ServiceResponse<CompleteSeasonDto>();
            var response = await _http.PostAsJsonAsync("api/seasons/tc", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
            return result;
        }

        public async Task<ServiceResponse<CompleteSeasonDto>> GenerateSchedule()
        {
            var payload = new ServiceResponse<CompleteSeasonDto>();
            var response = await _http.PutAsJsonAsync("api/seasons/schedule", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<CompleteSeasonDto>>();
            return result;
        }
    }
}
