using System.Text.RegularExpressions;
using BlazorBootstrap;
using Microsoft.JSInterop;
using NBALigaSimulation.Shared.Dtos.Players;
namespace NBALigaSimulation.Client.Pages.Players.PlayerComparison;

public partial class PlayerComparison
{
    
    private PlayerCompleteDto _player1;
    private PlayerCompleteDto _player2;
    private PlayerRegularStatsDto _player1Stats;
    private PlayerRegularStatsDto _player2Stats;
    private PlayerRatingDto _playerRating1;
    private PlayerRatingDto _playerRating2;
    private int _season = 0;
    private string searchText = string.Empty;
    private string searchText2 = string.Empty;

    private List<PlayerSimpleDto> players = new List<PlayerSimpleDto>();
    
    protected override async Task OnInitializedAsync()
    {
        _season = _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));

        var response = await PlayerService.GetAllSimplePlayers();
        if (response.Success)
        {
            players = response.Data;
        }

    }
    
    private async Task OnChange(dynamic args)
    {

        string player = args;
        var regex = ExtractPlayerId(player);

        if (regex != -1)
        {
            var response  = await PlayerService.GetPlayerById(regex);
            _player1 = response.Data;
            _player1Stats = _player1.RegularStats.LastOrDefault();
            _playerRating1 = _player1.Ratings.LastOrDefault();
        }

    }

    private async Task OnChange2(dynamic args)
    {
        
        string player = args;
        var regex = ExtractPlayerId(player);

        if (regex != -1)
        {
            var response  = await PlayerService.GetPlayerById(regex);
            _player2 = response.Data;
            _player2Stats = _player2.RegularStats.LastOrDefault();
            _playerRating2 = _player2.Ratings.LastOrDefault();
        }

    }


  private void NavigateToPlayerPage(int playerId)
  {
      NavigationManager.NavigateTo($"/players/{playerId}");
  }
  
  static int ExtractPlayerId(string input)
  {
      Regex regex = new Regex(@"\((\d+)\)");

      Match match = regex.Match(input);

      if (match.Success)
      {
          string numberString = match.Groups[1].Value;
          if (int.TryParse(numberString, out int playerId))
          {
              return playerId;
          }
      }

      return -1;
  }

}