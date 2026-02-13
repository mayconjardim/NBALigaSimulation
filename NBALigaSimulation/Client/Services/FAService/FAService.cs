using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.FAService;

public class FAService : IFAService
{
     private readonly HttpClient _http;

        public FAService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto)
        {
            var response = new ServiceResponse<FAOfferDto>();

            var result = await _http.PostAsJsonAsync("api/fa", offerDto);

            if (result.IsSuccessStatusCode)
            {
                var tradeResponse = await result.Content.ReadFromJsonAsync<ServiceResponse<FAOfferDto>>();

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

        public async Task<ServiceResponse<List<FAOfferDto>>> GetOffersByTeamId()
        {
            var result = await _http.GetFromJsonAsync<ServiceResponse<List<FAOfferDto>>>($"api/fa/teams");
            return result;
        }

        public async Task<ServiceResponse<bool>> DeleteOffer(int offerId)
        {
            var response = new ServiceResponse<bool>();

            var result = await _http.DeleteAsync($"api/fa/{offerId}");

            if (result.IsSuccessStatusCode)
            {
                response.Success = true;
                response.Data = true;
                response.Message = "Offer deleted successfully.";
            }
            else
            {
                response.Success = false;
                response.Message = "Offer to delete trade. HTTP status code: " + result.StatusCode;
            }

            return response;
        }

        public async Task<ServiceResponse<FASimulateRoundResultDto>> SimulateFARound(int? seasonYear = null)
        {
            var url = "api/fa/simulate-round";
            if (seasonYear.HasValue)
                url += "?season=" + seasonYear.Value;
            var result = await _http.PostAsync(url, null);
            if (result.IsSuccessStatusCode)
            {
                var data = await result.Content.ReadFromJsonAsync<ServiceResponse<FASimulateRoundResultDto>>();
                return data ?? new ServiceResponse<FASimulateRoundResultDto> { Success = false };
            }
            return new ServiceResponse<FASimulateRoundResultDto>
            {
                Success = false,
                Message = "Falha ao simular rodada. Status: " + result.StatusCode
            };
        }
}