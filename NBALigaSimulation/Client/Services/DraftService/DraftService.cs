namespace NBALigaSimulation.Client.Services.DraftService
{
    public class DraftService : IDraftService
    {
        private readonly HttpClient _http;

        public DraftService(HttpClient http)
        {
            _http = http;
        }

        public Task<ServiceResponse<bool>> GenerateLottery()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<DraftLotteryDto>> GetLastLottery()
        {
            var response = await _http.GetFromJsonAsync<ServiceResponse<DraftLotteryDto>>($"api/draft/lotto");
            return response;
        }

    }
}
