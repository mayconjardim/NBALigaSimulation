using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.TeamsService;

public interface ITeamService
{
    
    Task<ServiceResponse<TeamCompleteDto>> GetTeamById(int teamId);
    Task<ServiceResponse<List<TeamSimpleDto>>> GetAllTeams();


}