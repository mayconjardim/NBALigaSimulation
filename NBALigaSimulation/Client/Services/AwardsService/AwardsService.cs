using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
            return await _http.SafePostAsync<object, bool>($"api/awards/generate/{season}", new { });
        }

        public async Task<ServiceResponse<List<PlayerAwardsDto>>> GetAllAwards()
        {
            return await _http.SafeGetAsync<List<PlayerAwardsDto>>("api/awards");
        }
    }
}
