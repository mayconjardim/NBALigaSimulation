using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;

namespace NBALigaSimulation.Client.Pages.Stats.PlayerStatsPage;

public partial class PlayerStatsPage
{
    private PlayerStatsResponse _statsResponse = new PlayerStatsResponse();
    private List<PlayerRegularStatsDto> _playerStats;
    private int _currentPage = 1;
    private int _pageSize = 20;
    private string _stat;
    private string _message = string.Empty;
    private int _season = 2007;

    private string[] _statsColumns = { "PLAYER", "NAME", "TEAM", "GP", "MPG", "PPG", "APG", "RPG", "ORB", "DRB", "SPG", "BPG", "TPG", "FPG", "FG%", "FT%", "3P%", "TS%" };

    protected override async Task OnInitializedAsync()
    {
        await GetAllPlayerRegularStats();
    }
    
    private async Task GetAllPlayerRegularStats()
    {
        var result = await StatsService.GetAllPlayerRegularStats(_currentPage, _pageSize, _season, _stat);

        if (result.Success)
        {
            _statsResponse = result.Data ?? new PlayerStatsResponse();
            _playerStats = _statsResponse.Stats ?? new List<PlayerRegularStatsDto>();
            _currentPage = _statsResponse.CurrentPage;
        }
        else
        {
            _message = result.Message;
        }
    }
}

    
