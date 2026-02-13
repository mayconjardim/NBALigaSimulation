using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.StatsService
{
public interface IStatsService
{
        Task<ServiceResponse<List<PlayerRegularStatsDto>>> GetPlayerRegularStatsByPlayerId(int playerId);
        Task<ServiceResponse<PageableResponse<PlayerRegularStatsDto>>> GetAllPlayerRegularStats(int page, int pageSize, int season, bool isAscending, string position, string stat = null);

        Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats(int season, bool isAscending, string stat = null);
		Task<ServiceResponse<List<TeamPlayoffsStatsDto>>> GetAllTeamPlayoffsStats();
		Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank();
		Task<ServiceResponse<List<PlayerPlayoffsStatsDto>>> GetAllPlayerPlayoffsStats();
		Task<ServiceResponse<bool>> CleanStats();
	}
}
