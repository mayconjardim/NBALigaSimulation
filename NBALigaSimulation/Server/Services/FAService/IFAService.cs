namespace NBALigaSimulation.Server.Services.FAService
{
    public interface IFAService
    {

        Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto);

    }
}
