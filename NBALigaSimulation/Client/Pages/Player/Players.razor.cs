namespace NBALigaSimulation.Client.Pages.Player
{
    partial class Players
    {

        private List<PlayerCompleteDto> players = new List<PlayerCompleteDto>();

        string message = string.Empty;
        private string selectedPosition { get; set; } = string.Empty;
        private int season { get; set; }

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Jogadores...";

            var result = await PlayerService.GetAllPlayers();
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



        private List<PlayerCompleteDto> filteredPlayers
        {
            get
            {
                if (string.IsNullOrEmpty(selectedPosition))
                    return players;

                return players.Where(p => p.Pos == selectedPosition).ToList();
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

    }
}
