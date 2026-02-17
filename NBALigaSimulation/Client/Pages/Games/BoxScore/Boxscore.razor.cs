using System.Globalization;
using Microsoft.AspNetCore.Components;
using NBALigaSimulation.Shared.Dtos.Games;
using NBALigaSimulation.Shared.Dtos.Players;
using NBALigaSimulation.Shared.Dtos.Teams;

namespace NBALigaSimulation.Client.Pages.Games.BoxScore;

public partial class Boxscore
{
    private GameCompleteDto? game = null;
    private string message = string.Empty;
    private TeamGameStatsDto? homeTeamStat = null;
    private TeamGameStatsDto? awayTeamStat = null;
    private int? playerOfTheGameId = null;

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

            CalculatePlayerOfTheGame();
        }
    }

    private void CalculatePlayerOfTheGame()
    {
        if (game == null) return;

        var allPlayerStats = new List<PlayerGameStatsDto>();
        if (game.HomePlayerGameStats != null)
            allPlayerStats.AddRange(game.HomePlayerGameStats);
        if (game.AwayPlayerGameStats != null)
            allPlayerStats.AddRange(game.AwayPlayerGameStats);

        if (!allPlayerStats.Any()) return;

        var playerWithMaxGameScore = allPlayerStats
            .OrderByDescending(p => CalculateGameScore(p))
            .FirstOrDefault();

        if (playerWithMaxGameScore != null)
        {
            playerOfTheGameId = playerWithMaxGameScore.PlayerId;
        }
    }

    private double CalculateGameScore(PlayerGameStatsDto stats)
    {
        return stats.Pts
            + 0.4 * stats.Fg
            - 0.7 * stats.Fga
            - 0.4 * (stats.Fta - stats.Ft)
            + 0.7 * stats.Orb
            + 0.3 * stats.Drb
            + stats.Stl
            + 0.7 * stats.Ast
            + 0.7 * stats.Blk
            - 0.4 * stats.Pf
            - stats.Tov;
    }

    private bool IsPlayerOfTheGame(PlayerGameStatsDto playerStat)
    {
        return playerOfTheGameId.HasValue && playerStat.PlayerId == playerOfTheGameId.Value;
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