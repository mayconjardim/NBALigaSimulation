using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Shared.Models.Utils;

public class PageableResponse<T>
{
    
    public List<T> Response { get; set; } = new List<T>();
    public int Pages { get; set; }
    public int CurrentPage { get; set; }
    
}