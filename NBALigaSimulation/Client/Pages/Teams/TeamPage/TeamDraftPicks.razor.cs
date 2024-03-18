using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamDraftPicks
{
    
    [Parameter]
    public TeamCompleteDto _team { get; set; }
    public List<TeamDraftPickDto> _picks { get; set; }

    string[] headings = { "YEAR", "ROUND", "TEAM", "ORIGINAL" };


    protected override async Task OnInitializedAsync()
    {

        if (_team != null)
        {
            _picks = _team.DraftPicks;
        }

    }

}
    
