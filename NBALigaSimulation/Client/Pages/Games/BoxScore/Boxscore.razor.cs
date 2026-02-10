using System.Globalization;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Games.BoxScore;

public partial class Boxscore
{
    private GameCompleteDto? game = null;
    private string message = string.Empty;
    private TeamGameStatsDto? homeTeamStat = null;
    private TeamGameStatsDto? awayTeamStat = null;


    [Parameter]
    public int Id { get; set; }

    protected override async Task OnInitializedAsync()
    {
        message = "Carregando jogo...";

        var result = await GameService.GetGameById(Id);
        if (!result.Success)
        {
            message = result.Message;
        }
        else
        {
            game = result.Data;

            foreach (var gameStat in game.TeamGameStats)
            {
                if (game.HomeTeamId == gameStat.TeamId)
                {
                    homeTeamStat = gameStat;
                }
                else if (game.AwayTeamId == gameStat.TeamId)
                {
                    awayTeamStat = gameStat;
                }
            }

        }
    }

    public string Format(double numero)
    {
        return numero.ToString("0.0", CultureInfo.InvariantCulture);
    }

    public string FormatPct(int made, int attempted)
    {
        if (attempted == 0) return "—";
        var pct = (100.0 * made / attempted);
        return pct.ToString("0.0", CultureInfo.InvariantCulture) + "%";
    }
}