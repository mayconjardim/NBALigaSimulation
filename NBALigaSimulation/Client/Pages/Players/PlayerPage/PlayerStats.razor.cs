using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Services.StatsService;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerStats
{
    [Inject] private IStatsService StatsService { get; set; }

    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    private List<PlayerRegularStatsDto> _regularStats;
    private bool _loading = true;

    string[] _stats = { "YEAR", "AGE", "TEAM", "GP", "GS" ,"MIN",
        "A" ,"M", "%",
        "A" ,"M", "%",
        "A" ,"M", "%",
        "OFF", "DEF" , "TOT",
        "AST", "TO", "STL",
        "BLK", "PF" , "PTS",
        "PER"};

    string[] _locations = { "YEAR", "AGE", "TEAM", "GP", "GS" ,"MIN", "A" , "M", "%", "A" , "M", "%", "A" , "M", "%", "A" , "M", "%",};

    protected override async Task OnParametersSetAsync()
    {
        _regularStats = null;
        _loading = true;
        if (_player != null)
        {
            var result = await StatsService.GetPlayerRegularStatsByPlayerId(_player.Id);
            if (result?.Success == true && result.Data != null)
                _regularStats = result.Data;
            else
                _regularStats = _player.RegularStats ?? new List<PlayerRegularStatsDto>();
        }
        _loading = false;
    }
}