using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.FA;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Engine.Finance;

namespace NBALigaSimulation.Client.Pages.Manager.FreeAgency.FAOffer;

public partial class FAOffer
{
    [Parameter]
    public int Id { get; set; }

    private PlayerCompleteDto? player = null;
    private TeamCompleteDto? team = null;
    private int _contract = 0;
    private int _years = 1;
    private int _season;
    private string message = string.Empty;
    private string? ValidationError = null;
    private bool Sending = false;

    private int SalaryCap => SalaryCapConstants.SalaryCap;
    private int MinSalary => player == null ? 0 : SalaryCapConstants.GetMinSalaryForFreeAgent(YearsExperience);
    private int MaxSalary => player == null ? 0 : SalaryCapConstants.GetMaxSalaryForFreeAgent(YearsExperience);
    private int MaxYears => SalaryCapConstants.MaxContractYearsOtherTeam;

    private int YearsExperience => player?.Ratings?.Count ?? 0;

    private decimal GetTeamTotalSalary()
    {
        if (team?.Players == null) return 0;
        return team.Players
            .Where(p => p.Contract != null && p.Contract.Amount > 0)
            .Sum(p => p.Contract!.Amount);
    }

    private int CapSpaceAvailable => Math.Max(0, SalaryCap - (int)GetTeamTotalSalary());

    private static string FormatMoney(int value)
    {
        if (value >= 1_000_000)
            return $"${value / 1_000_000}M";
        if (value >= 1_000)
            return $"${value / 1_000}K";
        return $"${value}";
    }

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando...";
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);

        var result = await PlayerService.GetPlayerById(Id);
        var teamResult = await TeamService.GetTeamByUser();

        if (!teamResult.Success || teamResult.Data == null)
        {
            message = "Time do usuário não encontrado.";
            return;
        }
        team = teamResult.Data;

        if (!result.Success || result.Data == null)
        {
            message = result.Message ?? "Jogador não encontrado.";
            return;
        }

        if (result.Data.TeamId != 21)
        {
            message = "Só é possível fazer oferta a jogadores em free agency (FA).";
            return;
        }
        player = result.Data;
        message = string.Empty;
        _years = Math.Clamp(_years, 1, MaxYears);
        _contract = MinSalary;
    }

    private void SetMinContract()
    {
        _contract = MinSalary;
        _years = 1;
        ValidationError = null;
    }

    private void SetMaxContract()
    {
        _contract = MaxSalary;
        _years = MaxYears;
        ValidationError = null;
    }

    private bool Validate(out string error)
    {
        if (_contract < MinSalary)
        {
            error = $"Valor mínimo para este jogador (EXP {YearsExperience}): {FormatMoney(MinSalary)}.";
            return false;
        }
        if (_contract > MaxSalary)
        {
            error = $"Valor máximo para este jogador (EXP {YearsExperience}): {FormatMoney(MaxSalary)}.";
            return false;
        }
        if (_years < 1 || _years > MaxYears)
        {
            error = $"Anos devem ser entre 1 e {MaxYears}.";
            return false;
        }
        if (GetTeamTotalSalary() + _contract > SalaryCap)
        {
            error = "O time não tem cap space para esta oferta.";
            return false;
        }
        error = string.Empty;
        return true;
    }

    private async Task SendOffer()
    {
        if (player == null || team == null) return;
        if (!Validate(out var err))
        {
            ValidationError = err;
            return;
        }
        ValidationError = null;
        Sending = true;
        try
        {
            var faOffer = new FAOfferDto
            {
                TeamId = team.Id,
                PlayerName = player.Name,
                PlayerId = player.Id,
                Amount = _contract,
                Years = _years,
                Season = _season
            };
            var faResponse = await FAService.CreateOffer(faOffer);
            if (faResponse.Success)
                NavigationManager.NavigateTo("/freeagency");
            else
                ValidationError = faResponse.Message ?? "Erro ao enviar oferta.";
        }
        finally
        {
            Sending = false;
        }
    }
}
