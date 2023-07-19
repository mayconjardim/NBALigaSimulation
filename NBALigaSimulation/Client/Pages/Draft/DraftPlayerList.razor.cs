using Microsoft.AspNetCore.Components;
namespace NBALigaSimulation.Client.Pages.Draft
{
    partial class DraftPlayerList
    {

        [Parameter]
        public int team { get; set; }

        [Parameter]
        public int pick { get; set; }

        private List<PlayerCompleteDto> players = new List<PlayerCompleteDto>();
        private string message = string.Empty;

        private string selectedPosition { get; set; } = string.Empty;
        private int season { get; set; }


        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Jogadores...";

            var result = await PlayerService.GetAllDraftPlayers();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                players = result.Data;
                season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
            }

        }

        private void HandlePositionChanged(string value)
        {

            if (value == "ALL")
            {
                value = "";
            }

            selectedPosition = value;
        }

        private List<PlayerCompleteDto> filteredPlayers
        {
            get
            {
                if (string.IsNullOrEmpty(selectedPosition))
                    return players;

                return players.Where(p => p.Pos == selectedPosition).ToList();
            }
        }

        private void DraftPlayer(int playerId)
        {



        }

    }
}
