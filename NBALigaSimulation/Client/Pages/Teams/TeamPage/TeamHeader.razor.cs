using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamHeader
{
    
    [Parameter] 
    public TeamCompleteDto _team { get; set; }
    
    
}