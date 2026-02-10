using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Drafts;

namespace NBALigaSimulation.Client.Pages.League;

public partial class Draft : ComponentBase
{
    [Inject] public IDraftService DraftService { get; set; }

    private bool _loadingInitial = true;
    private bool _isLoading = false;
    private string _loadingAction = string.Empty;
    private string _message = string.Empty;
    private bool _isError = false;

    private DraftLotteryDto? _lottery;
    private List<DraftDto> _draft = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        _loadingInitial = false;
    }

    private async Task LoadData()
    {
        _message = string.Empty;

        var lotteryResponse = await DraftService.GetLastLottery();
        if (lotteryResponse.Success)
        {
            _lottery = lotteryResponse.Data;
        }

        var draftResponse = await DraftService.GetLastDraft();
        if (draftResponse.Success && draftResponse.Data != null)
        {
            _draft = draftResponse.Data;
        }
        else
        {
            _draft = new List<DraftDto>();
        }
    }

    private async Task ExecuteAsync(Func<Task> action, string actionName)
    {
        if (_isLoading) return;

        _isLoading = true;
        _loadingAction = actionName;
        _message = string.Empty;

        try
        {
            await action();
        }
        finally
        {
            _isLoading = false;
            _loadingAction = string.Empty;
            StateHasChanged();
        }
    }

    private async Task OnGenerateLottery()
    {
        await ExecuteAsync(async () =>
        {
            var response = await DraftService.GenerateLottery();
            _isError = !response.Success;
            _message = response.Message ?? (_isError ? "Erro ao gerar loteria." : "Loteria gerada com sucesso.");
            await LoadData();
        }, "lottery");
    }

    private async Task OnGenerateDraft()
    {
        await ExecuteAsync(async () =>
        {
            var response = await DraftService.GenerateDraft();
            _isError = !response.Success;
            _message = response.Message ?? (_isError ? "Erro ao gerar draft." : "Draft gerado com sucesso.");
            await LoadData();
        }, "draft");
    }
}

