using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Team
{
    partial class TeamDraftPicks
    {

        [Parameter]
        public TeamCompleteDto? team { get; set; }
        public List<TeamDraftPickDto>? picks { get; set; }

        string[] headings = { "YEAR", "ROUND", "TEAM", "ORIGINAL" };

        public string message { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Draft Picks...";

            if (team != null)
            {

                picks = team.DraftPicks;
            }

        }

    }
}
