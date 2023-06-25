using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.Team
{
    partial class TeamSchedule
    {

        [Parameter]
        public TeamCompleteDto? team { get; set; }
        List<GameCompleteDto> games { get; set; }
        private string message = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Time...";

            var result = await GameService.GetGamesByTeamId(team.Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                games = result.Data;
            }
        }

    }
}
