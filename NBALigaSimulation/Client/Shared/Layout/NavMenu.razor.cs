using Microsoft.JSInterop;
using NBALigaSimulation.Client.Services.TradesService;
using NBALigaSimulation.Shared.Dtos.Teams;
using NBALigaSimulation.Shared.Models.Users;

namespace NBALigaSimulation.Client.Shared.Layout;

public partial class NavMenu
{
    protected List<TeamSimpleDto> _east;
    protected bool _isLogged = false;
    protected int PendingTradesCount { get; set; }
    protected bool _isAdmin = false;
    private UserLogin user = new();
    private string userTeam = string.Empty;
    private string userName = string.Empty;
    private int _season;

    
    protected override async Task OnInitializedAsync()
    {
        if (!(await LocalStorage.ContainKeyAsync("_isLogged")))
        {
            await LocalStorage.SetItemAsync("_isLogged", false);
        }
        
        if (!(await LocalStorage.ContainKeyAsync("_isAdmin")))
        {
            await LocalStorage.SetItemAsync("_isAdmin", false);
        }
        
        _isLogged = await LocalStorage.GetItemAsync<bool>("_isLogged");
        _isAdmin = await LocalStorage.GetItemAsync<bool>("_isAdmin");

        if (_isLogged)
        {
            userTeam = await LocalStorage.GetItemAsync<string>("team");
            userName = await LocalStorage.GetItemAsync<string>("username");
            await LoadPendingTradesCount();
        }
      
        var response = await SeasonService.GetLastSeason();
        if (response.Success)
        {
            _season = response.Data.Year;
            await LocalStorage.SetItemAsync("season", response.Data.Year.ToString());
        }
        else
        {
            var seasonStr = await LocalStorage.GetItemAsync<string>("season");
            _season = int.TryParse(seasonStr, out var s) ? s : DateTime.Now.Year;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("addScrollListener");
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
            await LocalStorage.SetItemAsync("_isAdmin", result.Data.IsAdmin);
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
        await LocalStorage.SetItemAsync("_isAdmin", false);
        NavigationManager.Refresh();
    }
    
    
    private async Task FecharMenu()
    {
        await JSRuntime.InvokeVoidAsync("fecharmenu");
    }
    
    private async Task FecharMOdal()
    {
        await JSRuntime.InvokeVoidAsync("fecharmodal");
    }

    private async Task LoadPendingTradesCount()
    {
        var result = await TradeService.GetTradeByTeamId();
        if (result.Success && result.Data != null)
        {
            var teamId = await LocalStorage.GetItemAsync<int>("teamId");
            PendingTradesCount = result.Data.Count(t => t.TeamTwoId == teamId && t.Response == null);
        }
    }
}