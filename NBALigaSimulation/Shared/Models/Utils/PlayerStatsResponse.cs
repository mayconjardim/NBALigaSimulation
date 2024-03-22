using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Shared.Models.Utils;

public class PlayerStatsResponse
{
    
    public List<PlayerRegularStatsDto> Stats { get; set; } = new List<PlayerRegularStatsDto>();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
    
}