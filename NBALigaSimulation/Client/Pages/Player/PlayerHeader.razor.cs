using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerHeader
    {

        [Parameter]
        public PlayerCompleteDto? player { get; set; }

        private PlayerRegularStatsDto stats = null;

        protected override void OnInitialized()
        {
            if (player.RegularStats != null && player.RegularStats.Any())
            {
                stats = player.RegularStats.Last();
            }
        }

    }
}


