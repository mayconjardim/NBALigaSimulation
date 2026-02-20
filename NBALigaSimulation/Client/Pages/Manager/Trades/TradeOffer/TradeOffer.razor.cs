using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Utilities;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Engine.Finance;

namespace NBALigaSimulation.Client.Pages.Manager.Trades.TradeOffer;

public partial class TradeOffer
{
    [Parameter]
    public int Id { get; set; }

    [Inject] private ITradeService TradeService { get; set; } = null!;
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;

    private TradeDto? Trade { get; set; }
    private int _season;
    private int? MyTeamId { get; set; }
    private int _teamOneSalary;
    private int _teamTwoSalary;
    private string Message { get; set; } = "Carregando...";
    private string? ActionError { get; set; }
    private bool Processing { get; set; }

    private bool IsProposer => MyTeamId == Trade?.TeamOneId;
    private bool IsReceiver => MyTeamId == Trade?.TeamTwoId;
    private bool CanRespond => IsReceiver && Trade?.Response == null;

    protected override async Task OnInitializedAsync()
    {
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);

        var teamResult = await TeamService.GetTeamByUser();
        if (teamResult.Success && teamResult.Data != null)
            MyTeamId = teamResult.Data.Id;

        var result = await TradeService.GetTradeById(Id);
        if (!result.Success || result.Data == null)
        {
            Message = result.Message ?? "Proposta não encontrada.";
            return;
        }
        Trade = result.Data;
        Message = string.Empty;

        if (Trade != null)
        {
            var t1 = await TeamService.GetTeamById(Trade.TeamOneId);
            var t2 = await TeamService.GetTeamById(Trade.TeamTwoId);
            _teamOneSalary = t1.Success && t1.Data?.Players != null
                ? t1.Data.Players.Where(p => p.Contract != null && p.Contract.Amount > 0).Sum(p => p.Contract!.Amount) : 0;
            _teamTwoSalary = t2.Success && t2.Data?.Players != null
                ? t2.Data.Players.Where(p => p.Contract != null && p.Contract.Amount > 0).Sum(p => p.Contract!.Amount) : 0;
        }
    }

    private int OutgoingOne => Trade?.Players?.Where(p => p.TeamId == Trade.TeamOneId).Sum(p => p.Contract?.Amount ?? 0) ?? 0;
    private int OutgoingTwo => Trade?.Players?.Where(p => p.TeamId == Trade.TeamTwoId).Sum(p => p.Contract?.Amount ?? 0) ?? 0;

    private int CapSpaceOne => Math.Max(0, SalaryCapConstants.SalaryCap - _teamOneSalary);
    private int CapSpaceTwo => Math.Max(0, SalaryCapConstants.SalaryCap - _teamTwoSalary);
    private int CapSpaceOneAfter => Math.Max(0, SalaryCapConstants.SalaryCap - (_teamOneSalary - OutgoingOne + OutgoingTwo));
    private int CapSpaceTwoAfter => Math.Max(0, SalaryCapConstants.SalaryCap - (_teamTwoSalary - OutgoingTwo + OutgoingOne));

    private static string FormatMoney(int value) => $"${value:N0}";
    private static string SalaryWithYears(PlayerContractDto? c, int season)
    {
        if (c == null || c.Amount <= 0 || c.Exp < season) return "-";
        int yrs = Math.Max(1, c.Exp - season + 1);
        return $"{FormatMoney(c.Amount)} ({yrs} ano{(yrs > 1 ? "s" : "")})";
    }

    private async Task AcceptTrade()
    {
        if (Trade == null || !CanRespond) return;
        await RespondToTrade(true);
    }

    private async Task RejectTrade()
    {
        if (Trade == null || !CanRespond) return;
        await RespondToTrade(false);
    }

    private async Task RespondToTrade(bool accept)
    {
        ActionError = null;
        Processing = true;
        try
        {
            var dto = new TradeDto
            {
                Id = Trade!.Id,
                TeamOneId = Trade.TeamOneId,
                TeamTwoId = Trade.TeamTwoId,
                Response = accept
            };
            var response = await TradeService.UpdateTrade(dto);
            if (response.Success)
            {
                Trade.Response = accept;
                StateHasChanged();
            }
            else
            {
                ActionError = response.Message ?? "Erro ao atualizar proposta.";
            }
        }
        finally
        {
            Processing = false;
        }
    }

    private async Task DeleteTrade()
    {
        if (Trade == null || !IsProposer) return;
        ActionError = null;
        Processing = true;
        try
        {
            var response = await TradeService.DeleteTrade(Trade.Id);
            if (response.Success)
                NavigationManager.NavigateTo("/trades");
            else
                ActionError = response.Message ?? "Erro ao excluir proposta.";
        }
        finally
        {
            Processing = false;
        }
    }

    private static string StatusText(bool? response)
    {
        if (response == null) return "Pendente";
        return response == true ? "Aceita" : "Rejeitada";
    }
}
