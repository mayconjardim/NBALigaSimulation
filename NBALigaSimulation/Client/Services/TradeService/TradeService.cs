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

        public async Task<ServiceResponse<TradeDto>> GetTradeByTeamId(int teamId)
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<TradeDto>>($"api/trades/{teamId}");
            return result;
        }
    }
}
