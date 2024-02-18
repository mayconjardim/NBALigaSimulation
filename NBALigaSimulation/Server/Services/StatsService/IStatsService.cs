using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.StatsService
{
	public interface IStatsService
	{

		Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats();
		Task<ServiceResponse<List<TeamPlayoffsStatsDto>>> GetAllTeamPlayoffsStats();
		Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank();
		Task<ServiceResponse<List<PlayerRegularStatsDto>>> GetAllPlayerRegularStats();
		Task<ServiceResponse<List<PlayerPlayoffsStatsDto>>> GetAllPlayerPlayoffsStats();
	}
}
