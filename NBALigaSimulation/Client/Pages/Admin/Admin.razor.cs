using Blazored.LocalStorage;
using Microsoft.JSInterop;
using NBALigaSimulation.Client.Services.GameService;
using NBALigaSimulation.Client.Services.SeasonsService;
using NBALigaSimulation.Client.Services.DraftService;
using NBALigaSimulation.Client.Services.PlayoffsService;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Seasons;
using System.Linq;

namespace NBALigaSimulation.Client.Pages.Admin;

public partial class Admin
{
    private bool _isAdmin = false;
    private bool _isLoading = false;
    private string _loadingAction = string.Empty;
    private string _message = string.Empty;
    private string _messageType = "success";
    private CompleteSeasonDto? _currentSeason;
    private int _nextRound = 0;
    private int _totalRounds = 0;
    private int _selectedRound = 1;
    private int _selectedPlayoffRound = 1;

    protected override async Task OnInitializedAsync()
    {
        _isAdmin = await LocalStorage.GetItemAsync<bool>("_isAdmin");
        
        if (_isAdmin)
        {
            await LoadSeasonInfo();
        }
    }

    private async Task LoadSeasonInfo()
    {
        var seasonResponse = await SeasonService.GetLastSeason();
        if (seasonResponse.Success)
        {
            _currentSeason = seasonResponse.Data;
        }

        var gamesResponse = await GameService.GetAllGames();
        if (gamesResponse.Success && gamesResponse.Data != null)
        {
            var unsimulatedGames = gamesResponse.Data.Where(g => !g.Happened && !string.IsNullOrEmpty(g.Week)).ToList();
            
            if (unsimulatedGames.Count > 0)
            {
                _nextRound = unsimulatedGames
                    .Select(g => int.TryParse(g.Week, out int round) ? round : 0)
                    .Where(r => r > 0)
                    .OrderBy(r => r)
                    .FirstOrDefault();
            }

            var allRounds = gamesResponse.Data
                .Where(g => !string.IsNullOrEmpty(g.Week))
                .Select(g => int.TryParse(g.Week, out int round) ? round : 0)
                .Where(r => r > 0)
                .ToList();

            _totalRounds = allRounds.Count > 0 ? allRounds.Max() : 0;
        }
    }

    private async Task ShowMessage(string message, string type = "success")
    {
        _message = message;
        _messageType = type;
        await Task.Delay(5000);
        ClearMessage();
    }

    private void ClearMessage()
    {
        _message = string.Empty;
    }

    private async Task ExecuteAction(Func<Task> action, string actionName)
    {
        if (_isLoading) return;

        _isLoading = true;
        _loadingAction = actionName;
        _message = string.Empty;

        try
        {
            await action();
            await LoadSeasonInfo(); // Recarrega informações após ação
        }
        catch (Exception ex)
        {
            await ShowMessage($"Erro: {ex.Message}", "error");
        }
        finally
        {
            _isLoading = false;
            _loadingAction = string.Empty;
        }
    }

    private async Task CreateSeason()
    {
        await ExecuteAction(async () =>
        {
            var response = await SeasonService.CreateSeason();
            if (response.Success)
            {
                await ShowMessage($"Temporada criada com sucesso! Ano: {response.Data?.Year}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao criar temporada: {response.Message}", "error");
            }
        }, "CreateSeason");
    }

    private async Task GenerateSchedule()
    {
        await ExecuteAction(async () =>
        {
            var response = await SeasonService.GenerateSchedule();
            if (response.Success)
            {
                await ShowMessage($"Cronograma gerado com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar cronograma: {response.Message}", "error");
            }
        }, "GenerateSchedule");
    }

    private async Task CleanSchedule()
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", "Tem certeza que deseja limpar o cronograma? Esta ação não pode ser desfeita."))
        {
            return;
        }

        await ExecuteAction(async () =>
        {
            var response = await SeasonService.CleanSchedule();
            if (response.Success)
            {
                await ShowMessage($"Cronograma limpo com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao limpar cronograma: {response.Message}", "error");
            }
        }, "CleanSchedule");
    }

    private async Task SimNextRound()
    {
        await ExecuteAction(async () =>
        {
            if (_nextRound == 0)
            {
                await ShowMessage("Não há rodadas para simular!", "error");
                return;
            }

            var response = await GameService.SimGameByRound(_nextRound);
            if (response.Success)
            {
                await ShowMessage($"Rodada {_nextRound} simulada com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao simular rodada: {response.Message}", "error");
            }
        }, "SimNextRound");
    }

    private async Task SimRound()
    {
        if (_selectedRound <= 0)
        {
            await ShowMessage("Por favor, informe um número de rodada válido.", "error");
            return;
        }

        await ExecuteAction(async () =>
        {
            var response = await GameService.SimGameByRound(_selectedRound);
            if (response.Success)
            {
                await ShowMessage($"Rodada {_selectedRound} simulada com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao simular rodada: {response.Message}", "error");
            }
        }, "SimRound");
    }

    private async Task SimGameByDateRegular()
    {
        await ExecuteAction(async () =>
        {
            var response = await GameService.SimGameByDateRegular();
            if (response.Success)
            {
                await ShowMessage($"Simulação por data concluída! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro na simulação: {response.Message}", "error");
            }
        }, "SimGameByDateRegular");
    }

    private async Task SimPlayoffRound()
    {
        await ExecuteAction(async () =>
        {
            var response = await GameService.SimPlayoffsByRound(_selectedPlayoffRound);
            if (response.Success)
            {
                await ShowMessage($"Rodada {_selectedPlayoffRound} dos playoffs simulada com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao simular playoffs: {response.Message}", "error");
            }
        }, "SimPlayoffRound");
    }

    private async Task SimAll()
    {
        if (!await JSRuntime.InvokeAsync<bool>("confirm", "Tem certeza que deseja simular TODOS os jogos? Esta ação pode demorar muito tempo."))
        {
            return;
        }

        await ExecuteAction(async () =>
        {
            var response = await GameService.SimAll();
            if (response.Success)
            {
                await ShowMessage($"Todos os jogos foram simulados! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao simular jogos: {response.Message}", "error");
            }
        }, "SimAll");
    }

    private async Task GenerateTrainingCamp()
    {
        await ExecuteAction(async () =>
        {
            var response = await SeasonService.GenerateTrainingCamp();
            if (response.Success)
            {
                await ShowMessage($"Training Camp gerado com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar Training Camp: {response.Message}", "error");
            }
        }, "GenerateTrainingCamp");
    }

    private async Task GenerateDraftLottery()
    {
        await ExecuteAction(async () =>
        {
            var response = await DraftService.GenerateLottery();
            if (response.Success)
            {
                await ShowMessage($"Loteria do draft gerada com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar loteria do draft: {response.Message}", "error");
            }
        }, "GenerateDraftLottery");
    }

    private async Task GenerateDraft()
    {
        await ExecuteAction(async () =>
        {
            var response = await DraftService.GenerateDraft();
            if (response.Success)
            {
                await ShowMessage($"Draft gerado com sucesso! {response.Message}", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar draft: {response.Message}", "error");
            }
        }, "GenerateDraft");
    }

    private async Task GeneratePlayoffsFirstRound()
    {
        await ExecuteAction(async () =>
        {
            var response = await PlayoffsService.GenerateFirstRound();
            if (response.Success)
            {
                await ShowMessage("Playoffs - 1ª rodada gerada com sucesso!", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar 1ª rodada dos playoffs: {response.Message}", "error");
            }
        }, "GeneratePlayoffsFirstRound");
    }

    private async Task GeneratePlayoffsSecondRound()
    {
        await ExecuteAction(async () =>
        {
            var response = await PlayoffsService.GenerateSecondRound();
            if (response.Success)
            {
                await ShowMessage("Playoffs - Semis de Conferência geradas com sucesso!", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar semis de conferência: {response.Message}", "error");
            }
        }, "GeneratePlayoffsSecondRound");
    }

    private async Task GeneratePlayoffsThirdRound()
    {
        await ExecuteAction(async () =>
        {
            var response = await PlayoffsService.GenerateThirdRound();
            if (response.Success)
            {
                await ShowMessage("Playoffs - Finais de Conferência geradas com sucesso!", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar finais de conferência: {response.Message}", "error");
            }
        }, "GeneratePlayoffsThirdRound");
    }

    private async Task GeneratePlayoffsFourthRound()
    {
        await ExecuteAction(async () =>
        {
            var response = await PlayoffsService.GenerateFourthRound();
            if (response.Success)
            {
                await ShowMessage("Playoffs - Finais da NBA geradas com sucesso!", "success");
            }
            else
            {
                await ShowMessage($"Erro ao gerar finais da NBA: {response.Message}", "error");
            }
        }, "GeneratePlayoffsFourthRound");
    }

    private async Task EndPlayoffs()
    {
        await ExecuteAction(async () =>
        {
            var response = await PlayoffsService.EndPlayoffs();
            if (response.Success)
            {
                await ShowMessage("Playoffs finalizados e temporada marcada como concluída!", "success");
            }
            else
            {
                await ShowMessage($"Erro ao finalizar playoffs: {response.Message}", "error");
            }
        }, "EndPlayoffs");
    }
}
