using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerEdit
{
    
    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    public CreatePlayerDto _createplayer { get; set; } = new CreatePlayerDto();
    private PlayerRatingDto _rating;

    private async Task HandleValidSubmit()
    {
        _createplayer.Id = _player.Id;
        _createplayer.Name = _player.Name;
        _createplayer.Pos = _player.Pos;
        _createplayer.Born = _player.Born;
        _createplayer.Hgt = _player.Hgt;
        _createplayer.Weight = _player.Weight;
        _createplayer.ImgUrl = _player.ImgUrl;
        _createplayer.TeamId = _player.TeamId;
        _createplayer.Ratings = _player.Ratings;
        
        var result = await PlayerService.EditPlayer(_createplayer);
        if (result.Success)
        {
            StateHasChanged();
            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        }
    }
    

}