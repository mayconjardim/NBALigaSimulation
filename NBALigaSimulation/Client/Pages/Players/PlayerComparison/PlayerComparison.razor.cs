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
    private List<PlayerSimpleDto> suggestions = new List<PlayerSimpleDto>();
    private int _season = 0;
    private string searchText = string.Empty;
    private string searchText2 = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        _season = _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));
    }
    
    private async Task<AutoCompleteDataProviderResult<PlayerSimpleDto>> CustomersDataProvider(AutoCompleteDataProviderRequest<PlayerSimpleDto> request)
    {

        var response  = await PlayerService.GetPlayersSearchSuggestions(searchText);
        List<PlayerSimpleDto> players = new List<PlayerSimpleDto>();
        if (response.Success)
        {
            players = response.Data;
        }
        return await Task.FromResult(new AutoCompleteDataProviderResult<PlayerSimpleDto> { Data = players, TotalCount = players.Count() });
    }
    
    private async void OnAutoCompleteChanged(PlayerSimpleDto player)
    {
        var response  = await PlayerService.GetPlayerById(player.Id);
        if (response.Success)
        {
                _player1 = response.Data;
                _player1Stats = _player1.RegularStats.LastOrDefault();
                _playerRating1 = _player1.Ratings.LastOrDefault();
        }
    }
    
    
    private  void NavigateToPlayerPage(int playerId)
    {
        NavigationManager.NavigateTo($"/players/{playerId}");
    }
    


  

}