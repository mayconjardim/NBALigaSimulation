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

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllPlayers()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerCompleteDto>>>($"api/players");
            return result;
        }

        public async Task<ServiceResponse<PlayerCompleteDto>> GetPlayerById(int playerId)
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<PlayerCompleteDto>>($"api/players/{playerId}");
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateRosterOrder(List<PlayerCompleteDto> updatedPlayerList)
        {
            var result = await _http.PutAsJsonAsync("api/players/rosterorder", updatedPlayerList);
            bool success = result.IsSuccessStatusCode;
            var response = new ServiceResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Player updated successfully." : "Failed to update game."
            };

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdatePlayerPtModifier(int playerId, double newPtModifier)
        {

            var result = await _http.PutAsJsonAsync($"api/players/{playerId}/ptmodifier", newPtModifier);

            bool success = result.IsSuccessStatusCode;

            var response = new ServiceResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Player updated successfully." : "Failed to update player."
            };

            return response;
        }

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllFAPlayers()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerCompleteDto>>>($"api/players/fa");
            return result;
        }

        public async Task<ServiceResponse<List<PlayerCompleteDto>>> GetAllDraftPlayers()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerCompleteDto>>>($"api/players/draft");
            return result;
        }

        public async Task<ServiceResponse<List<PlayerSimpleDto>>> GetPlayersSearchSuggestions(string searchText)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerSimpleDto>>>($"api/players/searchsuggestion/{searchText}");
            return result;
        }
    }
}
