using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerComparison
    {

        private PlayerCompleteDto? playerOne = null;
        private PlayerCompleteDto? playerTwo = null;
        private string message = string.Empty;

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Jogador...";

            var result = await PlayerService.GetPlayerById(Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                playerOne = result.Data;
            }
        }

    }
}
