namespace NBALigaSimulation.Server.Services.StatsService
{
    public interface IStatsService
    {

        Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats();
        Task<ServiceResponse<List<TeamRegularStatsRankDto>>> GetAllTeamRegularStatsRank();
        Task<ServiceResponse<List<PlayerRegularStatsDto>>> GetAllPlayerRegularStats();
    }
}
