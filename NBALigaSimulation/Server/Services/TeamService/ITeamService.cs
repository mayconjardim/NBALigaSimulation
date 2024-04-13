using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Server.Services.TeamService
{
    public interface ITeamService
    {

        Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId);
        Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams();
        Task<ServiceResponse<List<TeamSimpleWithPlayersDto>>> GetAllTeamsWithPlayers();
        Task<ServiceResponse<TeamCompleteDto>> GetTeamByLoggedUser();
        Task<ServiceResponse<bool>> UpdateTeamGameplan(int teamId, TeamGameplanDto teamGameplanDto);
        Task<ServiceResponse<bool>> UpdateKeyPlayers(int teamId, List<PlayerCompleteDto> players);

    }
}
