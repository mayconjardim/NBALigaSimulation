using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamRoster
{
    
    [Parameter] 
    public TeamCompleteDto _team { get; set; }

    List<PlayerCompleteDto> _roster { get; set; }
    
    string[] _headings =
    {
        "PLAYER", "POS"
    };
    
    protected override async Task OnInitializedAsync()
    {

        if (_team.Players != null)
        {
            _roster = _team.Players;
        }
       
    }
    
}