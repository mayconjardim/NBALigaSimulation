using Microsoft.AspNetCore.Components;


namespace NBALigaSimulation.Client.Pages.User
{
    partial class UserProfile
    {

        string[] headings = { "", "Name", "Pos" };

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

    }

}


