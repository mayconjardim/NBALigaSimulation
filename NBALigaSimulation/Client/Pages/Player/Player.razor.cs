using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class Player
    {

        private PlayerCompleteDto? player = null;
        private string message = string.Empty;

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Jogador...";

            var result = await PlayerService.GetPlayerById(Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                player = result.Data;
            }
        }

    }
}
