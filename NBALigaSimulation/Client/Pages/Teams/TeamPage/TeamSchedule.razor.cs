using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamSchedule
{
    public class GameWithRecord
    {
        public GameCompleteDto Game { get; set; }
        public string Record { get; set; }
    }

    [Parameter]
    public TeamCompleteDto _team { get; set; }

    private List<GameWithRecord> _gamesWithRecord = new();
    
    string[] headings = { "SEMANA", "DATA", "CASA", "VISITANTE", "V-D", "RESULTADO" };
    
    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetGamesByTeamId(_team.Id);
        
        if (result.Success && result.Data.Any())
        {
            var orderedGames = result.Data.OrderBy(g => g.GameDate).ToList();
            CalculateRecords(orderedGames);
        }
    }

    private void CalculateRecords(List<GameCompleteDto> games)
    {
        _gamesWithRecord.Clear();
        int currentWins = 0;
        int currentLosses = 0;

        foreach(var game in games)
        {
            string record = "—";
            if (game.HomeTeamScore > 0) 
            {
                bool isWinner = (game.HomeTeam == _team.Abrv && game.HomeTeamScore > game.AwayTeamScore) ||
                                (game.AwayTeam == _team.Abrv && game.AwayTeamScore > game.HomeTeamScore);
                if (isWinner)
                {
                    currentWins++;
                }
                else
                {
                    currentLosses++;
                }
                record = $"{currentWins}-{currentLosses}";
            }
            
            _gamesWithRecord.Add(new GameWithRecord
            {
                Game = game,
                Record = record
            });
        }
    }
    
    protected  string GetWinLoss(GameCompleteDto game)
    {
        if (game.HomeTeamScore == 0) return "";

        bool isWinner = (game.HomeTeam == _team.Abrv && game.HomeTeamScore > game.AwayTeamScore) ||
                        (game.AwayTeam == _team.Abrv && game.AwayTeamScore > game.HomeTeamScore);
        
        return isWinner ? "V" : "D";
    }
    
    protected  string GetScoreClass(GameCompleteDto game)
    {
        if (game.HomeTeamScore == 0) return "";
        
        bool isWinner = (game.HomeTeam == _team.Abrv && game.HomeTeamScore > game.AwayTeamScore) ||
                        (game.AwayTeam == _team.Abrv && game.AwayTeamScore > game.HomeTeamScore);

        return isWinner ? "text-win" : "text-loss";
    }
    
    private  void NavigateToTeamPage(int teamID)
    {
        NavigationManager.NavigateTo($"/teams/{teamID}", forceLoad: true);
    }
}
