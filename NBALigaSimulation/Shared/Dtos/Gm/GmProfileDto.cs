namespace NBALigaSimulation.Shared.Dtos.Gm;

public class GmProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int? TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string TeamAbrv { get; set; } = string.Empty;
    public string TeamRegion { get; set; } = string.Empty;
    public int Championships { get; set; }
    public int TotalWins { get; set; }
    public int TotalLosses { get; set; }
    public int TotalGames { get; set; }
    public string TotalWinPct { get; set; } = string.Empty;
    public List<GmSeasonRecordDto> SeasonRecords { get; set; } = new();
}
