using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.AwardsService
{
    public class AwardsService : IAwardsService
    {
        private readonly HttpClient _http;

        public AwardsService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<bool>> GenerateAwards(int season)
        {
            var response = await _http.PostAsJsonAsync($"api/awards/generate/{season}", new { });
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
            return result ?? new ServiceResponse<bool> { Success = false, Message = "Erro ao gerar awards" };
        }

        public async Task<ServiceResponse<List<PlayerAwardsDto>>> GetAllAwards()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<PlayerAwardsDto>>>("api/awards");
            return response ?? new ServiceResponse<List<PlayerAwardsDto>> { Success = false, Message = "Erro ao buscar awards" };
        }
    }
}
