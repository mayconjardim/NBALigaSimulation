using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

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
            UpdatedPlayerList = new List<PlayerCompleteDto>(team.Players); // Guarda a lista atualizada
            StateHasChanged();
        }

        public int Age(int season, int age)
        {
            return age - season;
        }



    }
}


