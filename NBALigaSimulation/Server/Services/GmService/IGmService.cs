using NBALigaSimulation.Shared.Dtos.Gm;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.GmService;

public interface IGmService
{
    Task<ServiceResponse<GmProfileDto>> GetMyProfile();
    /// <summary>Perfil do GM do time (público). Retorna null se o time não tiver usuário associado.</summary>
    Task<ServiceResponse<GmProfileDto>> GetProfileByTeamId(int teamId);
}
