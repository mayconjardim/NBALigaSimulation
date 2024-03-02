using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerLogs
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    public List<PlayerGameStatsDto> _gameLogs;
    
    protected override void OnInitialized()
    {
        
        if (_player != null)
        {
            _gameLogs = _player.Stats;
        }
        
    }
    
}