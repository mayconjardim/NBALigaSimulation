using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Users;

namespace NBALigaSimulation.Client.Shared.Layout;

public partial class NavMenu
{
    private List<TeamSimpleDto> _teams = new List<TeamSimpleDto>();
    protected List<TeamSimpleDto> _east;
    protected List<TeamSimpleDto> _west;
    protected bool _isLogged = false;
    private UserLogin user = new();

    protected override async Task OnInitializedAsync()
    {
        if (!(await LocalStorage.ContainKeyAsync("_isLogged")))
        {
            await LocalStorage.SetItemAsync("_isLogged", false);
        }
        
        _isLogged = await LocalStorage.GetItemAsync<bool>("_isLogged");
      
        var response = await SeasonService.GetLastSeason();
        if (response.Success)
        {
            await LocalStorage.SetItemAsync("season", response.Data.Year.ToString());
        }
        
        var result = await TeamService.GetAllTeams();
        if (result.Success)
        {
            _teams = result.Data;
            _east = _teams.Where(t => t.Conference == "East").ToList();
            _west = _teams.Where(t => t.Conference == "West").ToList();
        }
    }
    
    private async Task HandleLogin()
    {
        var result = await AuthService.Login(user);
        if (result.Success)
        {
            await LocalStorage.SetItemAsync("authToken", result.Data);
            await LocalStorage.SetItemAsync("_isLogged", true);
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _isLogged = true;
            NavigationManager.Refresh();
        }
       
    }
    
    private  void NavigateToTeamPage(int teamId)
    {
        NavigationManager.NavigateTo($"/teams/{teamId}");
    }
 
    private async Task Logout()
    {
        await LocalStorage.SetItemAsync("authToken", "");
        await LocalStorage.SetItemAsync("_isLogged", false);
        NavigationManager.Refresh();
    }
}