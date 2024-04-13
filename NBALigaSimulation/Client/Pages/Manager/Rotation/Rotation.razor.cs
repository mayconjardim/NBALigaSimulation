using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Manager.Rotation;

public partial class Rotation
{
    
    private TeamCompleteDto team;
    private string message;
    
    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Time...";

        var result = await TeamService.GetTeamByUser();
        if (!result.Success)
        {

            message = result.Message;
        }
        else
        {
            team = result.Data;
        }
    }
    
}