using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Manager.Gameplan;

public partial class Gameplan 
{
    
    
    private TeamCompleteDto _team;
    private TeamGameplanDto _gameplan;
    private List<PlayerCompleteDto> keyPlayers;
    private string _message;
    private bool isDirty = false;
    
    protected override async Task OnInitializedAsync()
    {
        var result = await TeamService.GetTeamByUser();
        if (!result.Success)
        {

            _message = result.Message;
        }
        else
        {
            _team = result.Data;
            _gameplan = _team.Gameplan;
        }
    }
    
    private void HandleEventChanged(ChangeEventArgs e)
    {
        isDirty = true;
        StateHasChanged();
    }
    
    private async Task UpdateGameplan()
    {
        var response = await TeamService.UpdateTeamGameplan(team.Id, teamGameplan);

        if (response.Success)
        {
            isDirty = false;
            StateHasChanged();
           // Snackbar.Add("GAMEPLAN ATUALIIZADO COM SUCESSO", Severity.Success);

        }
        else
        {
            _message = response.Message;
        }
    }
    
    List<double> GPaceOptions = new List<double> { 1, 2, 3 };
    
    string GetGPace(double value)
    {
        switch (value)
        {
            case 1:
                return "Low";
            case 2:
                return "Medium";
            case 3:
                return "High";
            default:
                return string.Empty;
        }
    }
    
    
    string GetFocus(double value)
    {
        switch (value)
        {
            case 1:
                return "Inside";
            case 2:
                return "Balanced";
            case 3:
                return "Outside";
            default:
                return string.Empty;
        }
    }

    List<double> DefenseOptions = new List<double> { 1, 2, 3, 4 };

    string GetDefense(double value)
    {

        switch (value)
        {
            case 1:
                return "Man";
            case 2:
                return "Help";
            case 3:
                return "Zone";
            case 4:
                return "Switch";
            default:
                return string.Empty;
        }
    }
}