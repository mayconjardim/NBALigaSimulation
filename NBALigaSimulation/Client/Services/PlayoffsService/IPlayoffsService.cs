using NBALigaSimulation.Shared.Dtos.Playoffs;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.PlayoffsService;

public interface IPlayoffsService
{
    Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffs();
    Task<ServiceResponse<List<PlayoffsDto>>> GetPlayoffsBySeason(int season);
    Task<ServiceResponse<bool>> GenerateFirstRound();
    Task<ServiceResponse<bool>> GenerateSecondRound();
    Task<ServiceResponse<bool>> GenerateThirdRound();
    Task<ServiceResponse<bool>> GenerateFourthRound();
    Task<ServiceResponse<bool>> EndPlayoffs();
}

