using Microsoft.AspNetCore.Components;
using MudBlazor;
using NBALigaSimulation.Shared.Models;
using System.Linq.Expressions;

namespace NBALigaSimulation.Client.Pages.Team
{
    partial class TeamRoster
    {

        [Parameter]
        public TeamCompleteDto? team { get; set; }

        private List<PlayerCompleteDto>? players => team.Players;
        private int season = 0;

        private void NavigateToPlayerPage(int playerId)
        {
            NavigationManager.NavigateTo($"/player/{playerId}");
        }

        protected override async Task OnParametersSetAsync()
        {
            season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
        }

    }
}
