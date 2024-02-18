using NBALigaSimulation.Shared.Dtos.Drafts;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.DraftService
{
    public interface IDraftService
    {

        Task<ServiceResponse<List<DraftDto>>> GetLastDraft();
        Task<ServiceResponse<DraftLotteryDto>> GetLastLottery();
        Task<ServiceResponse<bool>> GenerateLottery();
        Task<ServiceResponse<bool>> GenerateDraft();
        Task<ServiceResponse<bool>> SelectDraftedPlayer(DraftPlayerDto request);

    }
}
