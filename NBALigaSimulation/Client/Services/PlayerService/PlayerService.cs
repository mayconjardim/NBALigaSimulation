namespace NBALigaSimulation.Client.Services.PlayerService
{
    public class PlayerService : IPlayerService
    {
        private readonly HttpClient _http;

        public PlayerService(HttpClient http)
        {
            _http = http;
        }

        public string Message { get; set; } = "Carregando Jogador...";

        public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<PlayerCompleteDto>>($"api/players/{playerId}");
            return result;
        }
    }
}
