using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamHeader
{
    
    [Parameter] 
    public TeamCompleteDto _team { get; set; }
    
    private string message;

    public TeamRegularStatsRankDto _stats;
    private string ppg = string.Empty;
    private int ppgRank;
    private string rpg = string.Empty;
    private int rpgRank;
    private string apg = string.Empty;
    private int apgRank;
    private string oppg = string.Empty;
    private int oppgRank;
    private int winPctRank;
    private string _gmUsername = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var result = await StatsService.GetAllTeamRegularStatsRank();

        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            _stats = result.Data.FirstOrDefault(t => t.TeamId == _team.Id);

            if (_stats != null)
            {
                ppg = _stats.Ppg;
                rpg = _stats.Rpg;
                apg = _stats.Apg;
                oppg = _stats.Oppg;

                var sortedStats = result.Data.ToList();
                ppgRank = sortedStats.OrderByDescending(t => Convert.ToDouble(t.Ppg)).ToList().FindIndex(t => t.TeamId == _team.Id) + 1;
                rpgRank = sortedStats.OrderByDescending(t => Convert.ToDouble(t.Rpg)).ToList().FindIndex(t => t.TeamId == _team.Id) + 1;
                apgRank = sortedStats.OrderByDescending(t => Convert.ToDouble(t.Apg)).ToList().FindIndex(t => t.TeamId == _team.Id) + 1;
                oppgRank = sortedStats.OrderBy(t => Convert.ToDouble(t.Oppg)).ToList().FindIndex(t => t.TeamId == _team.Id) + 1;
                winPctRank = sortedStats.Where(t => t.Conference == _team.Conference).OrderByDescending(t => Convert.ToDouble(t.WinPct)).ToList().FindIndex(t => t.TeamId == _team.Id) + 1;

            }
        }

        try
        {
            var gmResult = await GmService.GetProfileByTeamId(_team.Id);
            if (gmResult.Success && gmResult.Data != null && !string.IsNullOrEmpty(gmResult.Data.Username))
                _gmUsername = gmResult.Data.Username;
        }
        catch
        {
            _gmUsername = string.Empty;
        }
    }
   
    
}