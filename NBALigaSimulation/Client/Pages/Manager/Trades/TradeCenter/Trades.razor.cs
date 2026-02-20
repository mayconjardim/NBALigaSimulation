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

    protected override async Task OnInitializedAsync()
    {
        var teamResult = await TeamService.GetTeamByUser();
        if (teamResult.Success && teamResult.Data != null)
            MyTeamId = teamResult.Data.Id;

        var result = await TradeService.GetTradeByTeamId();
        if (result.Success && result.Data != null)
            AllTrades = result.Data;

        Message = string.Empty;
    }

    private static string StatusBadge(bool? response)
    {
        if (response == null) return "bg-warning";
        return response == true ? "bg-success" : "bg-secondary";
    }

    private static string StatusText(bool? response)
    {
        if (response == null) return "Pendente";
        return response == true ? "Aceita" : "Rejeitada";
    }
}
