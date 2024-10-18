using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerEdit
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }
    private PlayerRatingDto _rating;

    private async Task HandleValidSubmit()
    {
    }

}