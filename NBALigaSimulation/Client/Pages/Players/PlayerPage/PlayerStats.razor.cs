using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerStats
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    public List<PlayerRegularStatsDto> _regularStats;
    
    
    string[] _stats = { "YEAR", "AGE", "TEAM", "GP", "GS" ,"MIN",
        "A" ,"M", "%",
        "A" ,"M", "%",
        "A" ,"M", "%",
        "OFF", "DEF" , "TOT",
        "AST", "TO", "STL", 
        "BLK", "PF" , "PTS",
        "PER"};

    string[] _locations = { "YEAR", "AGE", "TEAM", "GP", "GS" ,"MIN", "A" , "M", "%", "A" , "M", "%", "A" , "M", "%", "A" , "M", "%",};
    
    protected override void OnInitialized()
    {
        
        if (_player != null)
        {
            _regularStats = _player.RegularStats;
        }
        
    }

}