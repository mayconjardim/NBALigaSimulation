
namespace NBALigaSimulation.Client.Services.GameService
{
    public class GameService : IGameService
    {
        private readonly HttpClient _http;

        public GameService(HttpClient http)
        {
            _http = http;
        }

        public string Message { get; set; } = "Carregando jogo...";

        public async Task<ServiceResponse<GameCompleteDto>> GetGameById(int gameId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<GameCompleteDto>>($"api/games/{gameId}");
            return result;
        }

    }
}
