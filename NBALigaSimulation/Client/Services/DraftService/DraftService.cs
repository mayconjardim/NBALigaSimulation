namespace NBALigaSimulation.Client.Services.DraftService
{
    public class DraftService : IDraftService
    {
        private readonly HttpClient _http;

        public DraftService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<List<DraftDto>>> GetLastDraft()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<List<DraftDto>>>($"api/draft");
            return response;
        }

        public async Task<ServiceResponse<DraftLotteryDto>> GetLastLottery()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<DraftLotteryDto>>($"api/draft/lotto");
            return response;
        }

        public async Task<ServiceResponse<bool>> GenerateLottery()
        {
            var payload = new ServiceResponse<bool>();
            var response = await _http.PostAsJsonAsync($"api/draft/generate/lottery", payload);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

            return result;
        }

        public async Task<ServiceResponse<bool>> SelectDraftedPlayer(DraftPlayerDto request)
        {
            var response = new ServiceResponse<bool>();
            var result = await _http.PutAsJsonAsync("api/draft/select", request);
            return response;
        }

    }
}
