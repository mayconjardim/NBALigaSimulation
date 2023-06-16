using Microsoft.AspNetCore.Components;
using static MudBlazor.Colors;


namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserProfile
    {

        string[] headings = { "", "Name", "Pos" };

        private TeamCompleteDto? team = null;
        private TeamGameplanDto? teamGameplan = null;

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
                teamGameplan = team.Gameplan;

            }
        }

        private List<PlayerCompleteDto> UpdatedPlayerList = new List<PlayerCompleteDto>();

        public void OnItemDrop(PlayerCompleteDto item)
        {
            StateHasChanged();
        }
        public void OnReplacedItemDrop(PlayerCompleteDto item)
        {
            int index = team.Players.FindIndex(p => p.Id == item.Id);
            UpdatedPlayerList = new List<PlayerCompleteDto>(team.Players);
            StateHasChanged();
        }

        public int Age(int season, int age)
        {
            return age - season;
        }

        private async Task UpdateRoster()
        {

            if (UpdatedPlayerList.Count > 0)
            {
                var result = await PlayerService.UpdateRosterOrder(UpdatedPlayerList);
                UpdatedPlayerList.Clear();
            }

        }

        List<double> PtOptions = new List<double> { 0.0, 0.75, 1.0, 1.25, 1.75 };

        string GetOptionLabel(double value)
        {

            switch (value)
            {
                case 0.0:
                    return "0";
                case 0.75:
                    return "-";
                case 1.0:
                    return "";
                case 1.25:
                    return "+";
                default:
                    return "++";
            }
        }

        private async Task UpdatePtModifier(ChangeEventArgs e, int playerId)
        {
            double newPtModifier = Convert.ToDouble(e.Value);

            await PlayerService.UpdatePlayerPtModifier(playerId, newPtModifier);
        }


        List<double> GPOptions = new List<double> { 1, 2, 3 };

        string GetGP(double value)
        {
            switch (value)
            {
                case 1:
                    return "Low";
                case 2:
                    return "Medium";
                case 3:
                    return "High";
                default:
                    return string.Empty;
            }
        }

        List<double> DefenseOptions = new List<double> { 1, 2, 3, 4 };

        string GetDefense(double value)
        {

            switch (value)
            {
                case 1:
                    return "Man";
                case 2:
                    return "Help";
                case 3:
                    return "Zone";
                case 4:
                    return "Switch";
                default:
                    return string.Empty;
            }
        }

        private bool isDirty = false;

        private void HandleEventChanged(ChangeEventArgs e)
        {
            isDirty = true;
            StateHasChanged();
        }

        private async Task UpdateGameplan()
        {
            var response = await TeamService.UpdateTeamGameplan(team.Id, teamGameplan);

            if (response.Success)
            {
                isDirty = false;
                StateHasChanged();
            }
            else
            {
                message = response.Message;
            }
        }

    }

}


