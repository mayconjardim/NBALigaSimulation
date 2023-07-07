namespace NBALigaSimulation.Server.Services.TeamService
{
    public interface ITeamService
    {

        Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams();
        Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers();
        Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId);
        Task<ServiceResponse<TeamCompleteDto>> GetTeamByUser();
        Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto);

    }
}
