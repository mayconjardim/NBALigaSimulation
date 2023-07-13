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

        public Task<ServiceResponse<bool>> GeneratePlayoffs()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayoffsDto>>>($"api/playoffs");
            return result;
        }
    }
}
