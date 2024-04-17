using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
    
    [Inject] protected ToastService ToastService { get; set; } = default!;
    
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
    
    private async Task UpdateGameplan(object value, string gameplan)
    {
        int result = Convert.ToInt32(value);
        int gp = result;
        switch(gameplan) 
        {
            case "PACE":
                _gameplan.Pace = gp;
                break;
            case "MOTION":
                _gameplan.Motion = gp;
                break;
            case  "FOCUS":
                _gameplan.Focus = gp;
                break;
            case "DEFENSE":
                _gameplan.Defense = gp;
                break;
            default:
                break;
        }
        
        var response = await TeamService.UpdateTeamGameplan(_team.Id, _gameplan);

        if (response.Success)
        {
            isDirty = false;
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("console.log", _gameplan);
            ToastService.Notify(new(ToastType.Success, $"{gameplan} ATUALIIZADO COM SUCESSO"));
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