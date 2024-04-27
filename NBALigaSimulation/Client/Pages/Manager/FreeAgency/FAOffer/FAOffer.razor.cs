using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Manager.FreeAgency.FAOffer;

public partial class FAOffer 
{
    
    
    [Parameter]
    public int Id { get; set; }
    
    private PlayerCompleteDto? player = null;
    private TeamCompleteDto? team = null;
    
    private int _contract = 0;
    private int _years = 1;
    private int _season;
    
    private string message = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando Jogador...";
            
        _season = int.Parse(await LocalStorage.GetItemAsync<string>("season"));

        var result = await PlayerService.GetPlayerById(Id);
        var teamResult = await TeamService.GetTeamByUser();
        if (result.Success)
        {
            if (result.Data?.TeamId == 21)
            {
                team = teamResult.Data;
                player = result.Data;
            }
            else
            {
                message = "Não é possivel enviar proposta pra esse jogador!";
            }
        }
        else
        {
            message = result.Message;
        }





    
}