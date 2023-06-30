namespace NBALigaSimulation.Client.Pages.FA
{
    partial class FA
    {

        private List<PlayerCompleteDto> players = new List<PlayerCompleteDto>();
        private List<FAOfferDto> faOffers = new List<FAOfferDto>();

        private string message = string.Empty;
        private string selectedPosition { get; set; } = string.Empty;
        private int season { get; set; }
        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Trocas...";

            var result = await PlayerService.GetAllFAPlayers();
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

    }
}
