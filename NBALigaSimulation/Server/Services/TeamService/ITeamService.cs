namespace NBALigaSimulation.Server.Services.TeamService
{
    public interface ITeamService
    {

        Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams();
        Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId);

    }
}
