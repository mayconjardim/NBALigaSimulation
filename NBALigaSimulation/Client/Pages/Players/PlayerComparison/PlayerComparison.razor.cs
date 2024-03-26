using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Models.Utils;
using pax.BlazorChartJs;

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
    protected ElementReference searchInput;
    
 
    public async Task HandleSearchOne(KeyboardEventArgs args)
    {
        if (args.Key == null || args.Key.Equals("Enter"))
        {
            if (!string.IsNullOrEmpty(searchText) && int.TryParse(searchText, out int selectedPlayerId))
            {
                var response = await PlayerService.GetPlayerById(selectedPlayerId);
                if (response.Success)
                {
                    _player1 = response.Data;
                    _player1Stats = _player1.RegularStats.LastOrDefault();
                    _playerRating1 = _player1.Ratings.LastOrDefault();
                }
            }
        }
        else if (searchText.Length > 1)
        {

            ServiceResponse<List<PlayerSimpleDto>> response = await PlayerService.GetPlayersSearchSuggestions(searchText);
            if (response.Success) 
            {
                suggestions = response.Data; 
            }
        }
    }
    
    public async Task HandleSearchTwo(KeyboardEventArgs args)
    {
        if (args.Key == null || args.Key.Equals("Enter"))
        {
            if (!string.IsNullOrEmpty(searchText2) && int.TryParse(searchText2, out int selectedPlayerId))
            {
                var response = await PlayerService.GetPlayerById(selectedPlayerId);
                if (response.Success)
                {
                    _player2 = response.Data;
                    _player2Stats = _player2.RegularStats.LastOrDefault();
                    _playerRating2 = _player2.Ratings.LastOrDefault(); 
                }
            }
        }
        else if (searchText2.Length > 1)
        {

            ServiceResponse<List<PlayerSimpleDto>> response = await PlayerService.GetPlayersSearchSuggestions(searchText2);
            if (response.Success) 
            {
                suggestions = response.Data; 
            }
        }
    }
    
 
}