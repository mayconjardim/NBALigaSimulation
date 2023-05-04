using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerRatings
    {
        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        private PlayerRatingDto? rating => player.Ratings.LastOrDefault();

    }
}
