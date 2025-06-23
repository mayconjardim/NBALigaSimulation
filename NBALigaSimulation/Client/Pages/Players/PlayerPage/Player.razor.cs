using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class Player
{
    
    private PlayerCompleteDto _player;
    private string _message = string.Empty;
    
    public string ActiveTab { get; set; } = "Bio";

    public void SetActiveTab(string tab)
    {
        ActiveTab = tab;
    }

    [Parameter]
    public int PlayerId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
     
        var result = await PlayerService.GetPlayerById(PlayerId);
        
        if (result.Success)
        {
            _player = result.Data;
        }
        else
        {
            _message = result.Message;
        }
    }
    
}