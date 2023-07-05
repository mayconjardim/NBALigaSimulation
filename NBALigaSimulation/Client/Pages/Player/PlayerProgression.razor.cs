using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerProgression
    {

        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        private List<PlayerRatingDto>? ratings { get; set; }

        protected override void OnInitialized()
        {
            ratings = player.Ratings;
        }

    }
}
