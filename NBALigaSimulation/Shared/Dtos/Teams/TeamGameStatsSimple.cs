namespace NBALigaSimulation.Shared.Dtos.Teams;

public class TeamGameStatsSimple
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int TeamId { get; set; }
    public int Season { get; set; }
    public int Pts { get; set; }
    
}