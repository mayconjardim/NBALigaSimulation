using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerHeader
    {

        [Parameter]
        public PlayerCompleteDto? player { get; set; }

    }
}
