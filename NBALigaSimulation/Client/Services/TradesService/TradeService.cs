using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

namespace NBALigaSimulation.Client.Services.TradesService;

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
            return await _http.SafeGetAsync<List<TradeDto>>("api/trades");
        }

        public async Task<ServiceResponse<List<TradeDto>>> GetTradeByTeamId()
        {
            return await _http.SafeGetAsync<List<TradeDto>>("api/trades/teams");
        }

        public async Task<ServiceResponse<TradeDto>> GetTradeById(int tradeId)
        {
            return await _http.SafeGetAsync<TradeDto>($"api/trades/{tradeId}");
        }

        public async Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto)
        {
            return await _http.SafePostAsync<TradeCreateDto, TradeDto>("api/trades", tradeDto);
        }

        public async Task<ServiceResponse<bool>> DeleteTrade(int tradeId)
        {
            var result = await _http.DeleteAsync($"api/trades/{tradeId}");
            if (result.IsSuccessStatusCode)
            {
                return new ServiceResponse<bool> { Success = true, Data = true, Message = "Troca excluída com sucesso." };
            }
            return new ServiceResponse<bool> { Success = false, Message = "Erro ao excluir troca." };
        }

        public async Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto)
        {
            return await _http.SafePutAsync<TradeDto, bool>("api/trades/update/", dto);
        }

}