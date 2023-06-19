using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Linq.Expressions;

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

        private string Position(string pos)
        {
            switch (pos)
            {
                case "PG":
                    return "Point Guard";
                case "SG":
                    return "Shooting Guard";
                case "SF":
                    return "Small Forward";
                case "PF":
                    return "Power Forward";
                case "C":
                    return "Center";
                case "G":
                    return "Guard";
                case "GF":
                    return "Guard/Forward";
                case "F":
                    return "Forward";
                case "FC":
                    return "Forward/Center";
                default:
                    return string.Empty;
            }
        }

    }
}
