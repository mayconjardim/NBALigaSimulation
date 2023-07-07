namespace NBALigaSimulation.Server.Services.League
{
    public interface ILeagueService
    {

        Task<ServiceResponse<List<TeamDraftPickDto>>> GetAllDraftPicks();

    }
}
