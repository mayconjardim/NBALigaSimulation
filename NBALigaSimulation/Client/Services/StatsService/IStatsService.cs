using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.StatsService;

public interface IStatsService
{
    
    Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats();
    Task<ServiceResponse<PlayerStatsResponse>> GetAllPlayerRegularStats(int page, int pageSize, int season, bool isAscending, string position, string stat = null);
    Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank();


}