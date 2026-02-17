using NBALigaSimulation.Shared.Dtos.Gm;

namespace NBALigaSimulation.Client.Pages.Gm;

public partial class GmProfile
{
    private GmProfileDto? _profile;
    private bool _loadFailed;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState?.User?.Identity?.IsAuthenticated != true)
        {
            _loadFailed = true;
            return;
        }

        try
        {
            var result = await GmService.GetMyProfile();
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
