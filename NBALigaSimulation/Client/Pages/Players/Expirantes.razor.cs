using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players;

public partial class Expirantes
{
    private List<PlayerCompleteDto>? _players;
    private int _season;
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);

        var result = await PlayerService.GetExpiringPlayers(_season);
        _players = result.Success && result.Data != null ? result.Data : new List<PlayerCompleteDto>();
        _loading = false;
    }

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
}
