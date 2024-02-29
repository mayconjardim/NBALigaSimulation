using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerStats
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    public List<PlayerRegularStatsDto> _regularStats;
    
    string[] stats = { "YEAR", "TEAM", "GP", "GS" ,"MIN", "M" , "A", "%", "M", "A" , "%", "M" , "A", "%" ,
        "OFF", "DEF" , "TOT", "AST", "TO", "STL",  "BLK", "PF" , "PTS", "PER"};

    string[] locations = { "YEAR", "TEAM", "GP", "GS" ,"MIN", "M" , "A", "%", "M" , "A", "%", "M" , "A", "%", "M" , "A", "%",};
    
    protected override void OnInitialized()
    {
        
        if (_player != null)
        {
            _regularStats = _player.RegularStats;
        }
        
    }

}