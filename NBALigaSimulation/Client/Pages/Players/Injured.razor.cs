using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players;

public partial class Injured
{
    private List<PlayerCompleteDto>? _players;
    private int _season;
    private bool _loading = true;

    protected override async Task OnInitializedAsync()
    {
        var seasonStr = await LocalStorage.GetItemAsync<string>("season");
        _season = string.IsNullOrEmpty(seasonStr) ? DateTime.Now.Year : int.Parse(seasonStr);

        var result = await PlayerService.GetInjuredPlayers();
        _players = result.Success && result.Data != null ? result.Data : new List<PlayerCompleteDto>();
        _loading = false;
    }
}

