namespace NBALigaSimulation.Client.Services.TeamService
{
    public interface ITeamService
    {

        Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId);
        Task<ServiceResponse<TeamCompleteDto>> GetUserTeam();
        Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams();
        Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers();
        Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto);

    }
}
