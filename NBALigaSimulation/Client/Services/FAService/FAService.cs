using System.Net.Http.Json;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.Utils;
using NBALigaSimulation.Client.Utilities;

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
            return await _http.SafePostAsync<FAOfferDto, FAOfferDto>("api/fa", offerDto);
        }

        public async Task<ServiceResponse<List<FAOfferDto>>> GetOffersByTeamId()
        {
            return await _http.SafeGetAsync<List<FAOfferDto>>("api/fa/teams");
        }

        public async Task<ServiceResponse<bool>> DeleteOffer(int offerId)
        {
            var result = await _http.DeleteAsync($"api/fa/{offerId}");
            if (result.IsSuccessStatusCode)
            {
                return new ServiceResponse<bool> { Success = true, Data = true, Message = "Oferta removida com sucesso." };
            }
            return new ServiceResponse<bool> { Success = false, Message = "Erro ao excluir oferta." };
        }

        public async Task<ServiceResponse<FASimulateRoundResultDto>> SimulateFARound(int? seasonYear = null)
        {
            var url = "api/fa/simulate-round";
            if (seasonYear.HasValue) url += "?season=" + seasonYear.Value;
            
            return await _http.SafePostAsync<object, FASimulateRoundResultDto>(url, new { });
        }
}