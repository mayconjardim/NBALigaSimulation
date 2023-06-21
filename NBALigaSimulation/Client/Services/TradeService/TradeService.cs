using System.Net.Http;

namespace NBALigaSimulation.Client.Services.TradeService
{
    public class TradeService : ITradeService
    {

        private readonly HttpClient _http;

        public TradeService(HttpClient http)
        {
            _http = http;
        }

        public string Message { get; set; } = "Carregando Trades...";

        public async Task<ServiceResponse<List<TradeDto>>> GetAllTrades()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<TradeDto>>>($"api/trades");
            return result;
        }

        public async Task<ServiceResponse<List<TradeDto>>> GetTradeByTeamId()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<TradeDto>>>($"api/trades/teams");
            return result;
        }

        public async Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<TradeDto>>($"api/trades/{tradeId}");
            return result;
        }

        public async Task<ServiceResponse<TradeDto>> CreateTrade(TradeDto tradeDto)
        {
            var response = await _http.PostAsJsonAsync("api/trades", tradeDto);
            var result = await response.Content.ReadFromJsonAsync<ServiceResponse<TradeDto>>();
            return result;
        }

    }
}
