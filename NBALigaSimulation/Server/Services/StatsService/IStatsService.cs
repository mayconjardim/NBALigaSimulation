using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.StatsService
{
	public interface IStatsService
	{

		Task<ServiceResponse<PlayerStatsResponse>> GetAllPlayerRegularStats(int page, int pageSize, int season, string stat = null);
		
		Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats();
		Task<ServiceResponse<List<TeamPlayoffsStatsDto>>> GetAllTeamPlayoffsStats();
		Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank();

	
		Task<ServiceResponse<List<PlayerPlayoffsStatsDto>>> GetAllPlayerPlayoffsStats();
	}
}
