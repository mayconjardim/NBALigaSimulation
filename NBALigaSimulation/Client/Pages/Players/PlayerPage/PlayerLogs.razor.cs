using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerLogs
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    private List<PlayerGameStatsDto> _gameLogs;

    string[] _headings = { "Rodada", "OPP", "MIN", "FG", "3PT", "FT", "OFF", "REB", "AST", "TO", "STL", "BLK", "PF", "PTS" };
    
    protected override void OnInitialized()
    {
        
        if (_player != null)
        {
            _gameLogs = _player.Stats;
        }
        
    }
    
}