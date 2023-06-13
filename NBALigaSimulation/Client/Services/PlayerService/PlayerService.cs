using System.Net.Http;

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
            var response = await _http.GetFromJsonAsync<ServiceResponse<PlayerCompleteDto>>($"api/players/{playerId}");
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
        {
            var result = await _http.PostAsJsonAsync("api/players/rosterorder", updatedPlayerList);
            bool success = result.IsSuccessStatusCode;
            var response = new ServiceResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Player updated successfully." : "Failed to update game."
            };

            return response;
        }
    }
}
