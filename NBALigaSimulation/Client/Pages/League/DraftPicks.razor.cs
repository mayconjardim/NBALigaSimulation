namespace NBALigaSimulation.Client.Pages.League
{
    partial class DraftPicks
    {
        public List<TeamDraftPickDto>? picks { get; set; }

        string[] headings = { "YEAR", "ROUND", "TEAM", "ORIGINAL" };

        public string message { get; set; }

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Draft Picks...";

            var result = await LeagueService.GetAllDraftPicks();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                picks = result.Data;
            }
        }


    }
}
