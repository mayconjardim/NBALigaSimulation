using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerHeader
{

    [Parameter] public PlayerCompleteDto _player { get; set; }

    private PlayerRatingDto _ratings;
    private PlayerRegularStatsDto _stats;
    
    protected override async Task OnInitializedAsync()
    {

        if (_player.RegularStats.Any() )
        {
            _stats = _player.RegularStats.LastOrDefault();
        }

        if (_player.Ratings != null)
        {
            _ratings = _player.Ratings.LastOrDefault();
        }
       
    }

}