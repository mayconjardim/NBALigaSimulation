namespace NBALigaSimulation.Shared.Dtos.FA;

public class FASimulateRoundResultDto
{
    public int SigningsCount { get; set; }

    public List<FASigningDto> Signings { get; set; } = new List<FASigningDto>();

    public string Message { get; set; } = string.Empty;
}

public class FASigningDto
{
    public string PlayerName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public int Amount { get; set; }
    public int Years { get; set; }
}
