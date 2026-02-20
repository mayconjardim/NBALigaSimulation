using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Services.TeamsService;
using NBALigaSimulation.Client.Services.TradesService;
using NBALigaSimulation.Shared.Dtos.Trades;

namespace NBALigaSimulation.Client.Pages.Manager.Trades.TradeCenter;

public partial class Trades
{
    [Inject] private ITradeService TradeService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;

    private List<TradeDto> AllTrades { get; set; } = new();
    private int? MyTeamId { get; set; }
    private string Message { get; set; } = "Carregando...";

    private List<TradeDto> Received => AllTrades.Where(t => t.TeamTwoId == MyTeamId).OrderByDescending(t => t.DateCreated).ToList();
    private List<TradeDto> Sent => AllTrades.Where(t => t.TeamOneId == MyTeamId).OrderByDescending(t => t.DateCreated).ToList();
    private List<TradeDto> TradesFeed => AllTrades.OrderByDescending(t => t.DateCreated).ToList();
    private int PendingReceivedCount => Received.Count(t => t.Response == null);

    protected override async Task OnInitializedAsync()
    {
        var teamResult = await TeamService.GetTeamByUser();
        if (!teamResult.Success || teamResult.Data == null)
        {
            Message = "Não foi possível identificar o time do usuário.";
            return;
        }
        MyTeamId = teamResult.Data.Id;

        var result = await TradeService.GetTradeByTeamId();
        if (!result.Success || result.Data == null)
        {
            Message = result.Message ?? "Não foi possível carregar as propostas.";
            return;
        }
        AllTrades = result.Data;

        Message = string.Empty;
    }

    private bool IsReceived(TradeDto trade) => trade.TeamTwoId == MyTeamId;
    private bool NeedsAction(TradeDto trade) => IsReceived(trade) && trade.Response == null;

    private static string DirectionText(TradeDto trade, int? myTeamId) =>
        trade.TeamTwoId == myTeamId ? "Recebida" : "Enviada";

    private static string DirectionBadge(TradeDto trade, int? myTeamId) =>
        trade.TeamTwoId == myTeamId ? "trade-badge-received" : "trade-badge-sent";

    private static string StatusBadge(bool? response)
    {
        if (response == null) return "trade-badge-pending";
        return response == true ? "trade-badge-accepted" : "trade-badge-rejected";
    }

    private static string StatusText(bool? response)
    {
        if (response == null) return "Pendente";
        return response == true ? "Aceita" : "Rejeitada";
    }
}
