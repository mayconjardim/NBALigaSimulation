using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.PossessionManager;

public static class PossessionActions
{
    
            public static void SimPossessions(Game game, Team[] Teams, int[][] PlayersOnCourt)
            {
                int i = 0;
                string outcome;
                bool substitutions;
    
                game.Offense = 0;
                game.Defense = 1;
    
                while (i < game.NumPossessions * 2)
                {
                    if ((i * game.Dt > 12 && game.PtsQrts[0].Count == 1) ||
                        (i * game.Dt > 24 && game.PtsQrts[0].Count == 2) ||
                        (i * game.Dt > 36 && game.PtsQrts[0].Count == 3))
                    {
                        game.PtsQrts[0].Add(0);
                        game.PtsQrts[1].Add(0);
                        game.T = 12;
                    }
    
                    // Relogio
                    game.T -= game.Dt;
                    if (game.T < 0)
                    {
                        game.T = 0;
                    }
    
                    // Troca de posse
                    game.Offense = (game.Offense == 1) ? 0 : 1;
                    game.Defense = (game.Offense == 1) ? 0 : 1;
    
                    game.UpdateTeamCompositeRatings(Teams, PlayersOnCourt);
    
                    outcome = GetPossessionOutcome(game, Teams, PlayersOnCourt);
    
                    // Troca Offense e Defense para que o receba outra posse quando eles forem trocados novamente no início do loop.
                    if (outcome == "Orb")
                    {
                        game.Offense = (game.Offense == 1) ? 0 : 1;
                        game.Defense = (game.Offense == 1) ? 0 : 1;
                    }
    
                    game.UpdatePlayingTime(Teams, PlayersOnCourt);
    
                    //Injuries();
    
                    if (i % game.SubsEveryN == 0)
                    {
                        substitutions = game.UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                        if (substitutions)
                        {
                            game.UpdateSynergy(Teams, PlayersOnCourt);
                        }
                    }
    
                    i += 1;
                }
            }
            
            public static string GetPossessionOutcome(Game game, Team[] Teams, int[][] PlayersOnCourt)
            {
                if (game.ProbTov(Teams) > new Random().NextDouble())
                {
                    return game.DoTov(Teams, PlayersOnCourt);
                }

                double[] ratios = game.RatingArray(Teams, "Usage", game.Offense, PlayersOnCourt, 10);
                int shooterIndex = game.PickPlayer(ratios);

                return game.DoShot(shooterIndex, Teams, PlayersOnCourt);
            }
}