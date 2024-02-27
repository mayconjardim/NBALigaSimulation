using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerHeader
{

    [Parameter] public PlayerCompleteDto _player { get; set; }

    private PlayerRegularStatsDto _stats;
    private int _season = 0;
    private int _age = 0;
    
    protected override async Task OnInitializedAsync()
    {

        if (_player.RegularStats != null)
        {
            _stats = _player.RegularStats.LastOrDefault();
            _season = _stats.Season;
            _age = (_stats.Season - _player.Born.Year);
        }
       
    }

}