using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.LeagueService
{
    public interface ILeagueService
    {

        Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks();

    }
}
