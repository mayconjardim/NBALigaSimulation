using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Utilities;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Dtos.Trades;
using NBALigaSimulation.Shared.Engine.Finance;

namespace NBALigaSimulation.Client.Pages.Manager.Trades.TradeCreate;

public partial class TradeCreate
{
    [Inject] private ITeamService TeamService { get; set; } = null!;
    [Inject] private ITradeService TradeService { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILocalStorageService LocalStorage { get; set; } = null!;

    private TeamCompleteDto? MyTeam { get; set; }
    private TeamCompleteDto? PartnerTeam { get; set; }
    private List<TeamSimpleDto> AllTeams { get; set; } = new();
    private int? SelectedPartnerId { get; set; }
    private int _season;
    private string Message { get; set; } = "Carregando...";
    private string? ValidationError { get; set; }
    private bool Sending { get; set; }

    private HashSet<int> SelectedPlayerIds { get; set; } = new();
    private HashSet<int> SelectedPickIds { get; set; } = new();

    private List<PlayerCompleteDto> MyPlayers => MyTeam?.Players?.OrderBy(p => p.RosterOrder).ThenBy(p => p.Name).ToList() ?? new();
    private List<PlayerCompleteDto> PartnerPlayers => PartnerTeam?.Players?.OrderBy(p => p.RosterOrder).ThenBy(p => p.Name).ToList() ?? new();
    private List<TeamDraftPickDto> MyPicks => MyTeam?.DraftPicks ?? new();
    private List<TeamDraftPickDto> PartnerPicks => PartnerTeam?.DraftPicks ?? new();

    private int MyCapSpace => Math.Max(0, SalaryCapConstants.SalaryCap - (MyTeam?.Players?.Where(p => p.Contract != null && p.Contract.Amount > 0).Sum(p => p.Contract!.Amount) ?? 0));
    private int PartnerCapSpace => Math.Max(0, SalaryCapConstants.SalaryCap - (PartnerTeam?.Players?.Where(p => p.Contract != null && p.Contract.Amount > 0).Sum(p => p.Contract!.Amount) ?? 0));

    private int MySelectedSalary => MyPlayers.Where(p => SelectedPlayerIds.Contains(p.Id)).Sum(p => p.Contract?.Amount ?? 0);
    private int PartnerSelectedSalary => PartnerPlayers.Where(p => SelectedPlayerIds.Contains(p.Id)).Sum(p => p.Contract?.Amount ?? 0);
    private int SalaryMatch => MySelectedSalary - PartnerSelectedSalary;

    private static string FormatMoneyFull(int value) => $"${value:N0}";
    private static string SalaryWithYears(PlayerContractDto? c, int season)
    {
        if (c == null || c.Amount <= 0 || c.Exp < season) return "-";
        int yrs = Math.Max(1, c.Exp - season + 1);
        return $"{FormatMoneyFull(c.Amount)} ({yrs} ano{(yrs > 1 ? "s" : "")})";
    }

    protected override async Task OnInitializedAsync()
    {
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);

        var teamResult = await TeamService.GetTeamByUser();
        if (!teamResult.Success || teamResult.Data == null)
        {
            Message = "Time do usuário não encontrado. Faça login com um time.";
            return;
        }
        MyTeam = teamResult.Data;

        var allResult = await TeamService.GetAllTeams();
        if (allResult.Success && allResult.Data != null)
        {
            AllTeams = allResult.Data.Where(t => t.Id != MyTeam.Id).ToList();
        }

        Message = string.Empty;
    }

    private async Task SelectTeam(int teamId)
    {
        SelectedPartnerId = teamId;
        SelectedPlayerIds.Clear();
        SelectedPickIds.Clear();
        var result = await TeamService.GetTeamById(teamId);
        PartnerTeam = result.Success ? result.Data : null;
    }

    private void TogglePlayer(int playerId)
    {
        if (SelectedPlayerIds.Contains(playerId))
            SelectedPlayerIds.Remove(playerId);
        else
            SelectedPlayerIds.Add(playerId);
    }

    private void TogglePick(int pickId)
    {
        if (SelectedPickIds.Contains(pickId))
            SelectedPickIds.Remove(pickId);
        else
            SelectedPickIds.Add(pickId);
    }

    private bool HasMyAssets => MyPlayers.Any(p => SelectedPlayerIds.Contains(p.Id)) || MyPicks.Any(p => SelectedPickIds.Contains(p.Id));
    private bool HasPartnerAssets => PartnerPlayers.Any(p => SelectedPlayerIds.Contains(p.Id)) || PartnerPicks.Any(p => SelectedPickIds.Contains(p.Id));

    private void ResetOffer()
    {
        SelectedPlayerIds.Clear();
        SelectedPickIds.Clear();
    }

    private async Task SwapTeam()
    {
        SelectedPartnerId = null;
        PartnerTeam = null;
        SelectedPlayerIds.Clear();
        SelectedPickIds.Clear();
        await InvokeAsync(StateHasChanged);
    }

    private async Task SubmitTrade()
    {
        if (MyTeam == null || PartnerTeam == null || !SelectedPartnerId.HasValue)
        {
            ValidationError = "Selecione um time parceiro.";
            return;
        }
        if (!HasMyAssets || !HasPartnerAssets)
        {
            ValidationError = "A proposta deve incluir pelo menos um jogador ou pick de cada time.";
            return;
        }

        ValidationError = null;
        Sending = true;
        try
        {
            var players = new List<PlayerCompleteDto>();
            foreach (var pid in SelectedPlayerIds)
            {
                var p = MyPlayers.FirstOrDefault(x => x.Id == pid) ?? PartnerPlayers.FirstOrDefault(x => x.Id == pid);
                if (p != null) players.Add(p);
            }
            var picks = new List<TeamDraftPickDto>();
            foreach (var pickId in SelectedPickIds)
            {
                var p = MyPicks.FirstOrDefault(x => x.Id == pickId) ?? PartnerPicks.FirstOrDefault(x => x.Id == pickId);
                if (p != null) picks.Add(p);
            }

            var dto = new TradeCreateDto
            {
                TeamOneId = MyTeam.Id,
                TeamTwoId = SelectedPartnerId.Value,
                Players = players,
                DraftPicks = picks
            };

            var response = await TradeService.CreateTrade(dto);
            if (response.Success)
            {
                NavigationManager.NavigateTo("/trades");
            }
            else
            {
                ValidationError = response.Message ?? "Erro ao enviar proposta.";
            }
        }
        finally
        {
            Sending = false;
        }
    }
}
