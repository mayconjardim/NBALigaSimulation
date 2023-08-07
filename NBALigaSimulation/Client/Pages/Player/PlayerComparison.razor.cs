using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NBALigaSimulation.Client.Pages.Player
{
    partial class PlayerComparison
    {
        private PlayerCompleteDto? playerOne = null;
        private PlayerCompleteDto? playerTwo = null;
        private int playerTwoId;
        private string message = string.Empty;
        private int season;
        private string searchText = string.Empty;
        private List<PlayerSimpleDto> suggestions = new List<PlayerSimpleDto>();
        protected ElementReference searchInput;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        private IDataService DataService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            message = "Carregando Jogador...";

            var result = await PlayerService.GetPlayerById(Id);
            season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                playerOne = result.Data;
            }
        }

        protected override void OnInitialized()
        {
            DataService.OnDataChanged += DataChangedHandler;
        }

        private async void DataChangedHandler(object sender, int data)
        {
            playerTwoId = data;
            var result = await PlayerService.GetPlayerById(playerTwoId);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                playerTwo = result.Data;
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            DataService.OnDataChanged -= DataChangedHandler;
        }
    }

}
