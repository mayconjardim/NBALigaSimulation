using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Engine.Finance;
using Blazored.LocalStorage;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamContracts
{
    [Parameter]
    public TeamCompleteDto Team { get; set; } = null!;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = null!;

    private int _season;

    protected override async Task OnInitializedAsync()
    {
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);
    }

    private int TotalSalary => Team?.Players?
        .Where(p => p.Contract != null && p.Contract.Amount > 0)
        .Sum(p => p.Contract!.Amount) ?? 0;

    private int TotalForYear(int year) => Team?.Players?
        .Where(p => p.Contract != null && p.Contract.Amount > 0 && p.Contract.Exp >= year)
        .Sum(p => p.Contract!.Amount) ?? 0;

    private int CapSpaceForYear(int year) => Math.Max(0, SalaryCapConstants.SalaryCap - TotalForYear(year));
    private int OverCapForYear(int year) => Math.Max(0, TotalForYear(year) - SalaryCapConstants.SalaryCap);

    private int CapSpace => Math.Max(0, SalaryCapConstants.SalaryCap - TotalSalary);
    private int OverCap => Math.Max(0, TotalSalary - SalaryCapConstants.SalaryCap);

    private static string FormatMoney(int value)
    {
        if (value >= 1_000_000)
        {
            double m = value / 1_000_000.0;
            return m == Math.Floor(m) ? $"${(int)m}M" : $"${m:F1}M";
        }
        if (value >= 1_000)
        {
            double k = value / 1_000.0;
            return k == Math.Floor(k) ? $"${(int)k}K" : $"${k:F1}K";
        }
        return $"${value}";
    }

    private static string FormatMoneyFull(int value)
    {
        return $"${value:N0}";
    }

    private static string SalaryForYear(PlayerContractDto? contract, int year)
    {
        if (contract == null || contract.Amount <= 0 || contract.Exp < year)
            return "-";
        return FormatMoneyFull(contract.Amount);
    }
}
