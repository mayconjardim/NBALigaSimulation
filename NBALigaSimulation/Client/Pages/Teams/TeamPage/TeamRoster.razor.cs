using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamRoster
{
    
    [Parameter] 
    public TeamCompleteDto _team { get; set; }

    List<PlayerCompleteDto> _roster { get; set; }

    private int season = 0;
    
    string[] _headings =
    {
        "PLAYER", "CUR", "POT", "POS", "HEIGHT", "WEIGHT", "AGE", "EXP", "COLLEGE"
    };
    
    protected override async Task OnInitializedAsync()
    {

        if (_team.Players != null)
        {
            _roster = _team.Players;
        }

        season = _roster.LastOrDefault().Ratings.LastOrDefault().Season;

    }
    
    private  void NavigateToPlayerPage(int playerId)
    {
        NavigationManager.NavigateTo($"/players/{playerId}");
    }
}