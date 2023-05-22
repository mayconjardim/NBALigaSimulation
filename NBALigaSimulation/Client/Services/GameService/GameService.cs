
using System.Net.Http.Json;

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

        public async Task<GameCompleteDto> CreateGame(CreateGameDto game)
        {
            var result = await _http.PostAsJsonAsync("api/games", game);

            var newGame = (await result.Content
                .ReadFromJsonAsync<ServiceResponse<GameCompleteDto>>()).Data;

            return newGame;
        }

        public async Task<ServiceResponse<bool>> UpdateGame(int gameId)
        {
            var result = await _http.PutAsJsonAsync($"api/games/update/{gameId}", true);
            bool success = result.IsSuccessStatusCode;
            var response = new ServiceResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Game updated successfully." : "Failed to update game."
            };

            return response;
        }
    }
}
