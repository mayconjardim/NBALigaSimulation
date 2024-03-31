using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Shared.Models.Utils;

public class PageableStatsResponse<T>
{
    
    public List<T> Stats { get; set; } = new List<T>();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
    
}