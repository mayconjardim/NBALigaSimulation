namespace NBALigaSimulation.Client.Services.LeagueService
{
    public interface ILeagueService
    {

        Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks();

    }
}
