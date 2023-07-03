namespace NBALigaSimulation.Client.Services.StatsService
{
    public interface IStatsService
    {

        Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats();

    }
}
