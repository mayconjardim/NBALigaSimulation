namespace NBALigaSimulation.Client.Pages.Player
{
    partial class Players
    {

        private List<PlayerCompleteDto> players = new List<PlayerCompleteDto>();

        string message = string.Empty;
        private string selectedPosition { get; set; } = string.Empty;
        private string selectedTeam { get; set; } = string.Empty;

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
                if (string.IsNullOrEmpty(selectedPosition) && string.IsNullOrEmpty(selectedTeam))
                {
                    return players;
                }

                int teamId = 0;

                if (!string.IsNullOrEmpty(selectedTeam))
                {
                    if (selectedTeam == "DRAFT")
                    {
                        teamId = 22;
                    }
                    else if (selectedTeam == "FREE AGENTS")
                    {
                        teamId = 21;
                    }
                }

                if (!string.IsNullOrEmpty(selectedPosition) && !string.IsNullOrEmpty(selectedTeam))
                {
                    return players.Where(p => p.Pos == selectedPosition && p.TeamId == teamId).ToList();
                }

                if (!string.IsNullOrEmpty(selectedPosition) && string.IsNullOrEmpty(selectedTeam))
                {
                    return players.Where(p => p.Pos == selectedPosition).ToList();
                }

                if (string.IsNullOrEmpty(selectedPosition) && !string.IsNullOrEmpty(selectedTeam))
                {
                    return players.Where(p => p.TeamId == teamId).ToList();
                }

                return players;
            }
        }


        private void HandleTeamChanged(string value)
        {

            if (value == "ALL")
            {
                value = "";
            }

            selectedTeam = value;
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
