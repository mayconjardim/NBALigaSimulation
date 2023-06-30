using NBALigaSimulation.Shared.Dtos;

namespace NBALigaSimulation.Client.Services.FAService
{
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

    }
}
