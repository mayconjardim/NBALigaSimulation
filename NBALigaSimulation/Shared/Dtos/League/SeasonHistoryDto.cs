namespace NBALigaSimulation.Shared.Dtos.League;

public class SeasonHistoryDto
{
    public int Season { get; set; }

    public int? ChampionTeamId { get; set; }
    public string ChampionTeamName { get; set; } = string.Empty;
    public string ChampionTeamAbrv { get; set; } = string.Empty;
    public string ChampionRegularRecord { get; set; } = string.Empty;
    public string ChampionPlayoffRecord { get; set; } = string.Empty;
    public int? ChampionConfRank { get; set; }

    public int? RunnerUpTeamId { get; set; }
    public string RunnerUpTeamName { get; set; } = string.Empty;
    public string RunnerUpTeamAbrv { get; set; } = string.Empty;
    public string RunnerUpRegularRecord { get; set; } = string.Empty;
    public string RunnerUpPlayoffRecord { get; set; } = string.Empty;
    public int? RunnerUpConfRank { get; set; }

    public List<SeasonAwardDto> Awards { get; set; } = new();
}
