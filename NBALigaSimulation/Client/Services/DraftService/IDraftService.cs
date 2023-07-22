namespace NBALigaSimulation.Client.Services.DraftService
{
    public interface IDraftService
    {

        Task<ServiceResponse<List<DraftDto>>> GetLastDraft();
        Task<ServiceResponse<DraftLotteryDto>> GetLastLottery();
        Task<ServiceResponse<bool>> GenerateDraft();
        Task<ServiceResponse<bool>> GenerateLottery();
        Task<ServiceResponse<bool>> SelectDraftedPlayer(DraftPlayerDto request);

    }
}
