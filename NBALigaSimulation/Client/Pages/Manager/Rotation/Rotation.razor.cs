using Microsoft.JSInterop;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Manager.Rotation;

public partial class Rotation
{
    
    private TeamCompleteDto _team;
    private List<PlayerCompleteDto> _players = new List<PlayerCompleteDto>();
    private List<PlayerCompleteDto> UpdatedPlayerList = new List<PlayerCompleteDto>();
    private int _season = 0;
    private string message;

    private PlayerCompleteDto player1Selected = null;
    private PlayerCompleteDto player2Selected = null;
    
    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Time...";
  
        await LoadData();
        StateHasChanged();
    }
    
    private async Task LoadData()
    {
        message = "Carregando Time...";
        
        _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));

        var result = await TeamService.GetTeamByUser();
        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            _team = result.Data;
            _players = result.Data.Players;
            await JSRuntime.InvokeVoidAsync("console.log", _players);

        }
    }
    
    List<double> PtOptions = new List<double> { 0.0, 0.75, 1.0, 1.25, 1.75, 2.0 };

    string GetOptionLabel(double value)
    {

        switch (value)
        {
            case 0.0:
                return "0";
            case 0.75:
                return "-";
            case 1.0:
                return "";
            case 1.25:
                return "+";
            case 1.75:
                return "++";
            default:
                return "+++";
        }
    }
    private async Task ChangeRosterOrder(PlayerCompleteDto player)
    {
        if (player1Selected == null)
        {
            player1Selected = player;
            StateHasChanged();
    
        }
        else if (player2Selected == null && player1Selected != player)
        {
            player2Selected = player;
            StateHasChanged();
 

            var player1Order = player1Selected.RosterOrder;
            var player2Order = player2Selected.RosterOrder;

            _players.Where(p => p.Id == player1Selected.Id).FirstOrDefault().RosterOrder = player2Order;
            _players.Where(p => p.Id == player2Selected.Id).FirstOrDefault().RosterOrder = player1Order;
            
            UpdatedPlayerList = new List<PlayerCompleteDto>(_players.OrderBy(p => p.RosterOrder));
            

            await UpdateRoster();
            StateHasChanged();
            
            player1Selected = null;
            player2Selected = null;
        }
    }

    private async Task UpdateRoster()
    {

        if (UpdatedPlayerList.Count > 0)
        {
            var result = await PlayerService.UpdateRosterOrder(UpdatedPlayerList);
            UpdatedPlayerList.Clear();
        }
        await LoadData();
        StateHasChanged();
    }
    
    private async Task UpdatePtModifier(object value, int playerId)
    {
        double result = Convert.ToDouble(value);
        double newPtModifier = result;
        await PlayerService.UpdatePlayerPtModifier(playerId, newPtModifier);
        await LoadData();
    }
    
}