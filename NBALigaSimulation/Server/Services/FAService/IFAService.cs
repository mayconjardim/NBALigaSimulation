using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.FAService
{
    public interface IFAService
    {

        Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto);
        Task<ServiceResponse<List<FAOfferDto>>> GetOffersByTeamId();
        Task<ServiceResponse<bool>> DeleteOffer(int offerId);

    }
}
