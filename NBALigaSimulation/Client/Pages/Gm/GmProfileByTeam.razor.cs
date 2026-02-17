using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Gm;

namespace NBALigaSimulation.Client.Pages.Gm;

public partial class GmProfileByTeam
{
    [Parameter]
    public int TeamId { get; set; }

    private GmProfileDto? _profile;
    private bool _loadFailed;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await GmService.GetProfileByTeamId(TeamId);
            if (result.Success && result.Data != null)
                _profile = result.Data;
            else
                _loadFailed = true;
        }
        catch
        {
            _loadFailed = true;
        }
    }
}
