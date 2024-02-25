using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerRating
{
    
    [Parameter]
    public PlayerCompleteDto? player { get; set; }
    
}