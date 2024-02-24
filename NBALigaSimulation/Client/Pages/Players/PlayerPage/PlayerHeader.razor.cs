using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerHeader
{
    
    [Parameter]
    public PlayerCompleteDto? player { get; set; }

    private PlayerRegularStatsDto stats = null;
    
}