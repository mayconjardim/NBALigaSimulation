using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Models.Utils;

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

        public async Task<ServiceResponse<TradeDto>> CreateTrade(TradeCreateDto tradeDto)
        {
            var response = new ServiceResponse<TradeDto>();

            var result = await _http.PostAsJsonAsync("api/trades", tradeDto);

            if (result.IsSuccessStatusCode)
            {
                var tradeResponse = await result.Content.ReadFromJsonAsync<ServiceResponse<TradeDto>>();

                if (tradeResponse.Success)
                {
                    response.Success = true;
                    response.Data = tradeResponse.Data;
                    response.Message = "Trade created successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = tradeResponse.Message;
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Failed to create trade. HTTP status code: " + result.StatusCode;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteTrade(int tradeId)
        {
            var response = new ServiceResponse<bool>();

            var result = await _http.DeleteAsync($"api/trades/{tradeId}");

            if (result.IsSuccessStatusCode)
            {
                response.Success = true;
                response.Data = true;
                response.Message = "Trade deleted successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Failed to delete trade. HTTP status code: " + result.StatusCode;
            }

            return response;
        }



        public async Task<ServiceResponse<bool>> UpdateTrade(TradeDto dto)
        {
            var result = await _http.PutAsJsonAsync($"api/trades/update/", dto);
            bool success = result.IsSuccessStatusCode;
            var response = new ServiceResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Trade updated successfully." : "Failed to update trade."
            };

            return response;
        }

}