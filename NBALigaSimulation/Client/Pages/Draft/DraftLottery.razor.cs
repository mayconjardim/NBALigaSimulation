namespace NBALigaSimulation.Client.Pages.Draft
{
    partial class DraftLottery
    {
        private DraftLotteryDto? lottery = null;

        private string message;
        protected override async Task OnInitializedAsync()
        {
            message = "Carregando LOTERIA...";

            var result = await DraftService.GetLastLottery();
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                lottery = result.Data;

            }
        }


    }
}
