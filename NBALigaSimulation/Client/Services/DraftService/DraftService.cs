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

        public Task<ServiceResponse<bool>> GenerateLottery()
        {
            throw new NotImplementedException();
        }

    }
}
