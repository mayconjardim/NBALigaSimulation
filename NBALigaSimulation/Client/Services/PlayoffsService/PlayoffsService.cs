namespace NBALigaSimulation.Client.Services.PlayoffsService
{
    public class PlayoffsService : IPlayoffsService
    {

        private readonly HttpClient _http;

        public PlayoffsService(HttpClient http)
        {
            _http = http;
        }

        public string Message { get; set; } = "Carregando Playoffs...";

        public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayoffsDto>>>($"api/playoffs");
            return result;
        }

        public async Task<ServiceResponse<PlayoffsDto>> GetPlayoffsById(int Id)
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<PlayoffsDto>>($"api/playoffs/{Id}");
            return response;
        }

        public async Task<ServiceResponse<bool>> Generate2Round()
        {
            var payload = new ServiceResponse<bool>();
            var response = await _http.PostAsJsonAsync("api/playoffs/generate/confsemis", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return result;
        }

        public async Task<ServiceResponse<bool>> Generate3Round()
        {
            var payload = new ServiceResponse<bool>();
            var response = await _http.PostAsJsonAsync("api/playoffs/generate/conffinals", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return result;
        }

        public async Task<ServiceResponse<bool>> Generate4Round()
        {
            var payload = new ServiceResponse<bool>();
            var response = await _http.PostAsJsonAsync("api/playoffs/generate/finals", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return result;
        }

        public async Task<ServiceResponse<bool>> EndPlayoffs()
        {
            var payload = new ServiceResponse<bool>();
            var response = await _http.PostAsJsonAsync("api/playoffs/end", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return result;
        }
    }
}
