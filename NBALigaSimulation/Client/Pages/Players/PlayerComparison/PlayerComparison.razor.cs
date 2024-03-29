using System.Text.RegularExpressions;
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
    private List<PlayerSimpleDto> suggestions = new List<PlayerSimpleDto>();
    private string _playerName = string.Empty;
    private string _message = string.Empty;
    private string searchText = string.Empty;
    private string searchText2 = string.Empty;
    private int player1Id = 0;
    private int player2Id = 0;
    
    protected override async Task OnInitializedAsync()
    {
        var result = await PlayerService.GetAllSimplePlayers();
        if (result.Success)
        {
            suggestions = result.Data;
        }
       
        await JSRuntime.InvokeVoidAsync("initializeAutocomplete", suggestions, "tags1");
        await JSRuntime.InvokeVoidAsync("initializeAutocomplete", suggestions, "tags2");
        
    }
    
    private  void NavigateToPlayerPage(int playerId)
    {
        NavigationManager.NavigateTo($"/players/{playerId}");
    }
    
    public async Task SearchPlayers(int number)
    {
        int playerId = number == 1 ? ExtractPlayerId(searchText) : ExtractPlayerId(searchText2);

        if (playerId != 0)
        {
            var response  = await PlayerService.GetPlayerById(playerId);
            if (response.Success)
            {
                if (number == 1)
                {
                    _player1 = response.Data;
                    _player1Stats = _player1.RegularStats.LastOrDefault();
                    _playerRating1 = _player1.Ratings.LastOrDefault();
                }
                else if (number == 2)
                {
                    _player2 = response.Data;
                    _player2Stats = _player2.RegularStats.LastOrDefault();
                    _playerRating2 = _player2.Ratings.LastOrDefault();
                }

                StateHasChanged();
            }
        }
    }

    private int ExtractPlayerId(string searchText)
    {
        Match match = Regex.Match(searchText, @"\((\d+)\)");
        if (match.Success)
        {
            return int.Parse(match.Groups[1].Value);
        }
        return 0;
    }

}