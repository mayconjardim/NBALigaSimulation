using Microsoft.AspNetCore.Components;


namespace NBALigaSimulation.Client.Pages.Team
{
    partial class TeamRoster
    {

        [Parameter]
        public TeamCompleteDto? team { get; set; }

        private List<PlayerCompleteDto>? players => team.Players;

        private void NavigateToPlayerPage(int playerId)
        {
            NavigationManager.NavigateTo($"/player/{playerId}");
        }

    }
}
