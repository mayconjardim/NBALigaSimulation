using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamList;

public partial class TeamsList
{
    private List<TeamSimpleDto> _teams = new List<TeamSimpleDto>();
    
    protected List<TeamSimpleDto> _east;
    protected List<TeamSimpleDto> _west;
    
    string[] headings = { "NAME", "CONFERENCE" };

    
    protected override async Task OnInitializedAsync()
    {

        var result = await TeamService.GetAllTeams();
        
        if (result.Success)
        {
            _teams = result.Data;
            _east = _teams.Where(t => t.Conference == "East").ToList();
            _west = _teams.Where(t => t.Conference == "West").ToList();
        }

    }
    
    private  void NavigateToTeamPage(int teamId)
    {
        NavigationManager.NavigateTo($"/teams/{teamId}");
    }

}