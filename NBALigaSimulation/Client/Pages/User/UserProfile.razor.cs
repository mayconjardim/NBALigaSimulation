using Microsoft.AspNetCore.Components;

namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserProfile
    {

        private TeamCompleteDto? team = null;
        private string message = string.Empty;

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Time...";

            var result = await TeamService.GetUserTeam();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                team = result.Data;
            }
        }

    }
}
