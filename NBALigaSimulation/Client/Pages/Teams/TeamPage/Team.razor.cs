using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class Team
{
    
    private TeamCompleteDto _team;
    private string _message = string.Empty;

    [Parameter]
    public int TeamId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
     
        var result = await TeamService.GetTeamById(TeamId);
        
        if (result.Success)
        {
            _team = result.Data;
        }
        else
        {
            _message = result.Message;
        }
    }
    
}