using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.GameSim.PossessionManager;
using NBALigaSimulation.Shared.Engine.Ratings;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.GameStart;

public static class GameSimulation
{
    
    public static void Sim(Game game)
    {

        Team[] Teams = { game.HomeTeam, game.AwayTeam };
        CompositeHelper.UpdatePlayersCompositeRating(Teams);

        var paceFactor = 103.1 / 100;
        paceFactor += 0.025 * RandomUtils.Bound((paceFactor - 1) / 0.2, -1, 1);


        game.NumPossessions = (int)((int)((GameplanUtils.GameplanPace(game.HomeTeam.Gameplan.Pace) + 
                                           GameplanUtils.GameplanPace(game.AwayTeam.Gameplan.Pace)) / 2) * 1.1 * paceFactor);

        game.Dt = 48.0 / (2 * game.NumPossessions);

        int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

        game.UpdatePlayersOnCourt(Teams, PlayersOnCourt);
        game.UpdateSynergy(Teams, PlayersOnCourt);
        game.HomeCourtAdvantage(Teams, PlayersOnCourt);


        PossessionActions.SimPossessions(game, Teams, PlayersOnCourt);

        // Jogue períodos de prorrogação se necessário
        while (Teams[0].Stats.Find(s => s.GameId == game.Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == game.Id)?.Pts)
        {
            if (game.Overtimes == 0)
            {
                game.NumPossessions = (int)Math.Round(game.NumPossessions * 5.0 / 48); // 5 minutos de posses
                game.Dt = 5.0 / (2 * game.NumPossessions);
            }

            game.T = 5.0;
            game.Overtimes++;
            game.PtsQrts[0].Add(0);
            game.PtsQrts[1].Add(0);
            PossessionActions.SimPossessions(game, Teams, PlayersOnCourt);
        }
    }
    
}