using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Teams.TeamPage;

public partial class TeamSchedule
{
    
    [Parameter] 
    public TeamCompleteDto _team { get; set; }

    private List<GameCompleteDto> _games = new List<GameCompleteDto>();
    
    string[] headings = { "DATE", "HOME", "AWAY", "W-L", "RESULT" };
    int totalWins = 0;
    int totalLosses = 0;
    
    protected override async Task OnInitializedAsync()
    {
        var result = await GameService.GetGamesByTeamId(_team.Id);
        
        if (result.Success)
        {
            _games = result.Data;
        }
    }

    protected string GetWinLossNumber(GameCompleteDto game)
    {
        var pageTeam = _team.Abrv;

        if (game.HomeTeamScore > game.AwayTeamScore)
        {
            if (game.HomeTeam == pageTeam)
            {
                totalWins += 1;
            }
            else
            {
                totalLosses += 1;
            }
        }
        else if (game.AwayTeamScore > game.HomeTeamScore)
        {
            if (game.AwayTeam == pageTeam)
            {
                totalWins += 1;
            }
            else
            {
                totalLosses += 1;
            }
        }

        return totalWins + "-" + totalLosses;
    }
    
    protected string GetWinLoss(GameCompleteDto game)
    {
        var pageTeam = _team.Abrv;
        
        if (game.HomeTeamScore > game.AwayTeamScore)
        {
            if (game.HomeTeam == pageTeam)
            {
                return "W";
            }
            else
            {
                return "L";
            }
        }
        else if (game.AwayTeamScore > game.HomeTeamScore)
        {
            if (game.AwayTeam == pageTeam)
            {
                return "W";
            }
            else
            {
                return "L";
            }
        }

        return totalWins + "-" + totalLosses;
    }
    
    protected  string GetScoreClass(GameCompleteDto game)
    {
        var pageTeam = _team.Abrv;
        
        if (game.HomeTeamScore > game.AwayTeamScore)
        {
            if (game.HomeTeam == pageTeam)
            {
                return "text-win";
            }
            else
            {
                return "text-loss";
            }
        }
        else if (game.AwayTeamScore > game.HomeTeamScore)
        {
            if (game.AwayTeam == pageTeam)
            {
                return "text-win";
            }
            else
            {
                return "text-loss";
            }
        }

        return "";
    }

    
    
}