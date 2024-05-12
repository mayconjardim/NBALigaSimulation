using NBALigaSimulation.Shared.Dtos.Trades;

namespace NBALigaSimulation.Client.Pages.Manager.Trades.TradeCenter;

public partial class Trades
{
    
    private List<TradeDto> trades = new List<TradeDto>();
    
    private string message = string.Empty;
    
    string[] headings = { "INIT", "WITH", "STATUS", "REMOVE"};
    
    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Trocas...";

        var result = await TradeService.GetTradeByTeamId();
        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            trades = result.Data;
        }
    }
    
    private async Task DeleteTradeOffer(int tradeId)
    {
           
        var tradeResponse = await TradeService.DeleteTrade(tradeId);

        if (tradeResponse.Success)
        {
            //Snackbar.Add("Proposta deletade com sucesso!", Severity.Success);
            trades.RemoveAll(t => t.Id == tradeId);
            StateHasChanged();
        }
        else
        {
           // Snackbar.Add("Proposta não foi deleta!", Severity.Error);

        }
    }
}