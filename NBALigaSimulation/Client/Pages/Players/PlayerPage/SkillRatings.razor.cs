using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Client.Utilities;
using NBALigaSimulation.Shared.Dtos.Players;

namespace NBALigaSimulation.Client.Pages.Players.PlayerPage;

public partial class SkillRatings
{
    
    [Parameter]
    public PlayerRatingDto _rating { get; set; }

    private Dictionary<string, int> _ratings = new();

    protected override void OnInitialized()
    {
        if (_rating != null)
        {
            _ratings = new Dictionary<string, int>
            {
                { "Inside", _rating.Ins },
                { "Dunk", _rating.Dnk },
                { "Mid-Range", _rating.Fg },
                { "Three-Point", _rating.Tp },
                { "Free Throw", _rating.Ft },
                { "Offensive IQ", _rating.Oiq },
                { "Defensive IQ", _rating.Diq },
                { "Passing", _rating.Pss },
                { "Ball Handling", _rating.Drb },
                { "Rebounding", _rating.Reb },
                { "Endurance", _rating.Endu },
                { "Strength", _rating.Stre },
                { "Speed", _rating.Spd },
                { "Jump", _rating.Jmp }
            };
        }
    }

    private string GetRatingBarClass(int rating) => Util.GetBadgeClass(rating);
} 