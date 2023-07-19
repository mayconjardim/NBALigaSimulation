using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace NBALigaSimulation.Client.Pages.Draft
{
    partial class DraftPlayerList
    {

        [Parameter]
        public int team { get; set; }

        [Parameter]
        public int pick { get; set; }

        public string teamName { get; set; }


        private List<PlayerCompleteDto> players = new List<PlayerCompleteDto>();
        private string message = string.Empty;
        string messageCssClass = string.Empty;

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
                teamName = await LocalStorage.GetItemAsync<string>("team");
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

        private async Task DraftPlayer(int playerId)
        {

            DraftPlayerDto DratPlayer = new DraftPlayerDto
            {
                PlayerId = playerId,
                Pick = pick,
                Round = Round(pick),
                Year = season,
                Team = teamName,
                TeamId = team
            };

            var response = await DraftService.SelectDraftedPlayer(DratPlayer);

            if (response.Success)
            {
                messageCssClass = "success";
                Snackbar.Add("Jogador draftado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/draft");
                NavigationManager.NavigateTo("/draft");
            }
            else
            {
                messageCssClass = "error";
                Snackbar.Add("Jogador não foi draftado!", Severity.Success);

            }

        }

        private int Round(int pick)
        {
            if (pick < 23)
            {
                return 1;
            }
            return 2;
        }
    }
}
