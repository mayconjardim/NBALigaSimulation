using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Services.StatsService;

public interface IStatsService
{
    
    Task<ServiceResponse<List<TeamRegularStatsDto>>> GetAllTeamRegularStats();
    Task<ServiceResponse<List<PlayerRegularStatsDto>>> GetAllPlayerRegularStats();
 
    
}