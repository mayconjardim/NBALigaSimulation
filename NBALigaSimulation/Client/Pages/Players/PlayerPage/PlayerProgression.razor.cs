using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class PlayerProgression
{

    [Parameter]
    public PlayerCompleteDto _player { get; set; }

    List<PlayerRatingDto> _ratings;
    
    string[] _headings =
    {
        "YEAR", "AGE", "OVR", "POT", "HGT", "STR", "SPD", "JMP", "END", "INS", "DNK", "FT", "2PT"
        , "3PT", "OIQ", "DIQ", "DRB", "PSS", "REB", "SKILLS"
    };
    
    protected override void OnInitialized()
    {
        
        if (_player != null)
        {
            _ratings = _player.Ratings;
        }
        
    }
    
}