using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Users;

namespace NBALigaSimulation.Client.Shared.Layout;

public partial class NavMenu
{
    protected List<TeamSimpleDto> _east;
    protected bool _isLogged = false;
    private UserLogin user = new();
    private string userTeam = string.Empty;
    private string userName = string.Empty;

    
    protected override async Task OnInitializedAsync()
    {
        if (!(await LocalStorage.ContainKeyAsync("_isLogged")))
        {
            await LocalStorage.SetItemAsync("_isLogged", false);
        }
        
        _isLogged = await LocalStorage.GetItemAsync<bool>("_isLogged");

        if (_isLogged)
        {
            userTeam = await LocalStorage.GetItemAsync<string>("team");
            userName = await LocalStorage.GetItemAsync<string>("username");
        }
      
        var response = await SeasonService.GetLastSeason();
        if (response.Success)
        {
            await LocalStorage.SetItemAsync("season", response.Data.Year.ToString());
        }
    }
    
    private async Task HandleLogin()
    {
        var result = await AuthService.Login(user);
        if (result.Success)
        {
            await LocalStorage.SetItemAsync("authToken", result.Data.Token);
            await LocalStorage.SetItemAsync("teamId", result.Data.TeamId);
            await LocalStorage.SetItemAsync("username", user.Username); 
            await LocalStorage.SetItemAsync("team", result.Data.Team); 
            await LocalStorage.SetItemAsync("_isLogged", true);
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _isLogged = true;
            NavigationManager.Refresh();
        }
       
    }
 
    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("authToken");
        await LocalStorage.RemoveItemAsync("teamId");
        await LocalStorage.RemoveItemAsync("team");
        await LocalStorage.RemoveItemAsync("username");
        await LocalStorage.SetItemAsync("_isLogged", false);
        NavigationManager.Refresh();
    }
}