using System.Text.RegularExpressions;
using BlazorBootstrap;
using Microsoft.JSInterop;
using NBALigaSimulation.Shared.Dtos.Players;
namespace NBALigaSimulation.Client.Pages.Players.PlayerComparison;

public partial class PlayerComparison
{
    
    private PlayerCompleteDto _player1;
    private PlayerCompleteDto _player2;
    private PlayerSimpleDto _playerSimple1;
    private PlayerSimpleDto _playerSimple2;
    private PlayerRegularStatsDto _player1Stats;
    private PlayerRegularStatsDto _player2Stats;
    private PlayerRatingDto _playerRating1;
    private PlayerRatingDto _playerRating2;
    private int _season = 0;
    private string searchText = string.Empty;
    private string searchText2 = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        _season = _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
    }
    
    
    private  void NavigateToPlayerPage(int playerId)
    {
        NavigationManager.NavigateTo($"/players/{playerId}");
    }
    


  

}