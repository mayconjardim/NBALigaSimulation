using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Services.PlayoffsService;
using NBALigaSimulation.Shared.Dtos.Playoffs;

namespace NBALigaSimulation.Client.Pages.Playoffs;

public partial class Playoffs
{
    private bool _loading = true;
    private List<PlayoffsDto> _playoffs = new();
    private Dictionary<int, List<PlayoffsDto>> _seriesByRound = new();
    private static readonly Dictionary<int, string> _roundsOrder = new()
    {
        { 1, "1ª Rodada" },
        { 2, "Semi-finais de Conferência" },
        { 3, "Finais de Conferência" },
        { 4, "Finais da NBA" }
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadPlayoffs();
        _loading = false;
    }

    private async Task LoadPlayoffs()
    {
        var response = await PlayoffsService.GetPlayoffs();

        if (response.Success && response.Data != null)
        {
            _playoffs = response.Data;
            GroupByRound();
        }
        else
        {
            _playoffs = new List<PlayoffsDto>();
        }
    }

    private void GroupByRound()
    {
        _seriesByRound = new Dictionary<int, List<PlayoffsDto>>();

        foreach (var serie in _playoffs)
        {
            int round = serie.SeriesId switch
            {
                >= 1 and <= 8 => 1,
                >= 9 and <= 12 => 2,
                13 or 14 => 3,
                15 => 4,
                _ => 1
            };

            if (!_seriesByRound.ContainsKey(round))
            {
                _seriesByRound[round] = new List<PlayoffsDto>();
            }
            _seriesByRound[round].Add(serie);
        }
    }
}
