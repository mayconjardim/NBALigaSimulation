using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.FAService
{
    public interface IFAService
    {
        Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto);
        Task<ServiceResponse<List<FAOfferDto>>> GetOffersByTeamId();
        Task<ServiceResponse<bool>> DeleteOffer(int offerId);
        /// <summary>Simula uma rodada da free agency: processa ofertas pendentes e define aceites (um jogador = uma oferta vencedora).</summary>
        Task<ServiceResponse<FASimulateRoundResultDto>> SimulateFARound(int? seasonYear = null);
    }
}
