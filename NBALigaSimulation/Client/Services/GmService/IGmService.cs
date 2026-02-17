using NBALigaSimulation.Shared.Dtos.Gm;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.GmService;

public interface IGmService
{
    Task<ServiceResponse<GmProfileDto>> GetMyProfile();
    Task<ServiceResponse<GmProfileDto>> GetProfileByTeamId(int teamId);
}
