using Microsoft.AspNetCore.Components;


namespace NBALigaSimulation.Client.Pages.Trade
{
    partial class TradeOffer
    {

        private TradeDto? trade = null;
        private string message = string.Empty;

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            message = "Carregando Time...";

            var result = await TradeService.GetTradeById(Id);
            if (!result.Success)
            {
                message = result.Message;
            }
            else
            {
                trade = result.Data;
            }
        }

    }
}
