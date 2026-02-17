namespace NBALigaSimulation.Shared.Dtos.Gm;

public class GmSeasonRecordDto
{
    public int Season { get; set; }
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Games { get; set; }
    public string WinPct { get; set; } = string.Empty;
    public int PlayoffWins { get; set; }
    public int PlayoffLosses { get; set; }
}
