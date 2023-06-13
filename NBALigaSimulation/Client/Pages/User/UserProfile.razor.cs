using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;

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

        private string DropedItem = "";
        private string replacedItem = "";
        public List<string> Items = new List<string>()
{
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8",
        "9",
    };
        public void OnItemDrop(string item)
        {
            DropedItem = item;
            StateHasChanged();
        }
        public void OnReplacedItemDrop(string item)
        {
            replacedItem = item;
            StateHasChanged();
        }
    }
}


