namespace NBALigaSimulation.Client.Services.FAService
{
    public interface IFAService
    {

        Task<ServiceResponse<FAOfferDto>> CreateOffer(FAOfferDto offerDto);

    }
}
