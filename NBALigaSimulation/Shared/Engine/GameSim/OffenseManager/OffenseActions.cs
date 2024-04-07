using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.GameSim.DefenseManager;
using NBALigaSimulation.Shared.Engine.GameSim.PlayerManager;
using NBALigaSimulation.Shared.Engine.GameSim.ProbabilityManager;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.OffenseManager;

public static class OffenseActions
{
    
     public static string DoShot(Game game, int shooter, Team[] Teams, int[][] PlayersOnCourt)
     {

            int p = PlayersOnCourt[game.Offense][shooter];
            var player = Teams[game.Offense].Players.Find(player => player.RosterOrder == p);

            double currentFatigue = PlayerActions.Fatigue(Teams[game.Offense].Players[p].Stats.Find(s => s.GameId == game.Id).Energy);

            int? passer = null;

            if (ProbabilityActions.ProbAst(game, Teams) > new Random().NextDouble())
            {
                double[] ratios = PlayerActions.RatingArray(game, Teams, "Passing", game.Offense, PlayersOnCourt, 10);
                passer = PlayerActions.PickPlayer(ratios, shooter);
            }

            double shootingThreePointerScaled = player.CompositeRating.Ratings["ShootingThreePointer"];

            if (Teams[game.Offense].Gameplan.Focus == 3)
            {
                shootingThreePointerScaled += GameplanUtils.GameplanFocus(Teams[game.Offense].Gameplan.Focus);
            }
            
            if (Teams[game.Offense].Gameplan.Focus == 1)
            {
                shootingThreePointerScaled += GameplanUtils.GameplanFocus(Teams[game.Offense].Gameplan.Focus);
            }

            if (shootingThreePointerScaled > 0.55)
            {
                shootingThreePointerScaled = 0.55 + (shootingThreePointerScaled - 0.55) * (0.3 / 0.45);
            }

            double shootingThreePointerScaled2 = shootingThreePointerScaled;

            if (shootingThreePointerScaled2 < 0.35)
            {
                shootingThreePointerScaled2 = 0 + shootingThreePointerScaled2 * (0.1 / 0.30);
            }
            else if (shootingThreePointerScaled2 < 0.45)
            {
                shootingThreePointerScaled2 = 0.1 + (shootingThreePointerScaled2 - 0.30) * (0.30 / 0.1);
            }

            int? diff = Teams[game.Defense].Stats.Find(s => s.GameId == game.Id)?.Pts - 
                        Teams[game.Offense].Stats.Find(s => s.GameId == game.Id)?.Pts;
            bool forceThreePointer = (diff >= 3 && diff <= 10 && game.T <= 10.0 / 60.0 && new Random().NextDouble() > game.T)
                || (game.T == 0 && game.Dt <= 2.5 / 60);

            double probAndOne;
            double probMake;
            double probMissAndFoul;
            string type;

            if (forceThreePointer || new Random().NextDouble() < 0.67 * shootingThreePointerScaled2 * 1)
            {
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = shootingThreePointerScaled * 0.6 + 0.30;
                probAndOne = 0.01;
                probMake *= 1;

            }
            else
            {
                double r1 = 0.8 * new Random().NextDouble() * player.CompositeRating.Ratings["ShootingMidRange"];
                double r2 = new Random().NextDouble() * (player.CompositeRating.Ratings["ShootingAtRim"] + 
                                                         game.SynergyFactor * (Teams[game.Offense].Synergy.Off - Teams[game.Defense].Synergy.Def));
                double r3 = new Random().NextDouble() * (player.CompositeRating.Ratings["ShootingLowPost"] + 
                                                         game.SynergyFactor * (Teams[game.Offense].Synergy.Off - Teams[game.Defense].Synergy.Def));

                if (r1 > r2 && r1 > r3)
                {
                    type = "MidRange";
                    probMissAndFoul = 0.09;
                    probMake = player.CompositeRating.Ratings["ShootingMidRange"] * 0.48 + 0.42;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    type = "AtRim";
                    probMissAndFoul = 0.39;
                    probMake = player.CompositeRating.Ratings["ShootingAtRim"] * 1.58 + 0.54;
                    probAndOne = 0.25;
                }
                else
                {
                    type = "LowPost";
                    probMissAndFoul = 0.35;
                    probMake = player.CompositeRating.Ratings["ShootingLowPost"] * 0.43 + 0.34;
                    probAndOne = 0.15;
                }


                probMake *= 1;
            }

            double foulFactor = 0.69 * (Math.Pow(player.CompositeRating.Ratings["DrawingFouls"] / 0.5, 2)) * 1;

            probMissAndFoul *= foulFactor;
            probAndOne *= foulFactor;
            probMake = (probMake - 0.25 * Teams[game.Defense].CompositeRating.Ratings["GameDefense"] + game.SynergyFactor 
                * (Teams[game.Offense].Synergy.Off - Teams[game.Defense].Synergy.Def)) * currentFatigue;

            if (game.T == 0 && game.Dt < 6.0 / 60)
            {
                probMake *= Math.Sqrt(game.Dt / (8.0 / 60));
            }

            if (passer != null)
            {
                probMake += 0.025;
            }

            if (ProbabilityActions.ProbBlk(game,Teams) > new Random().NextDouble())
            {
                return DefenseActions.DoBlk(game, shooter, type, Teams, PlayersOnCourt);
            }

            if (probMake > new Random().NextDouble())
            {
                if (probAndOne > new Random().NextDouble())
                {
                    return DoFg(game, shooter, passer, type, Teams, PlayersOnCourt, true);
                }

                return DoFg(game, shooter, passer, type, Teams, PlayersOnCourt);
            }

            if (probMissAndFoul > new Random().NextDouble())
            {
                if (type == "ThreePointer")
                {
                    return DoFt(game, shooter, 3, Teams, PlayersOnCourt); // fg, orb, or drb
                }
                return DoFt(game, shooter, 2, Teams, PlayersOnCourt); // fg, orb, or drb
            }

            PlayerActions.RecordStat(game, game.Offense, p, "Fga", Teams);
            if (type == "AtRim")
            {
                PlayerActions.RecordStat(game, game.Offense, p, "FgaAtRim", Teams);
            }
            else if (type == "LowPost")
            {
                PlayerActions.RecordStat(game, game.Offense, p, "FgaLowPost", Teams);
            }
            else if (type == "MidRange")
            {
                PlayerActions.RecordStat(game, game.Offense, p, "FgaMidRange", Teams);
            }
            else if (type == "ThreePointer")
            {
                PlayerActions.RecordStat(game, game.Offense, p, "Tpa", Teams);
            }

            return DefenseActions.DoReb(game, Teams, PlayersOnCourt);

     }
     
     public static string DoFg(Game game, int shooter, int? passer, string type, Team[] Teams, int[][] PlayersOnCourt, bool andOne = false)
     {

         int p = PlayersOnCourt[game.Offense][shooter];
         PlayerActions.RecordStat(game, game.Offense, p, "Fga", Teams);
         PlayerActions.RecordStat(game, game.Offense, p, "Fg", Teams);
         PlayerActions.RecordStat(game, game.Offense, p, "Pts", Teams, 2);

         if (type == "AtRim")
         {
             PlayerActions.RecordStat(game, game.Offense, p, "FgaAtRim", Teams);
             PlayerActions.RecordStat(game, game.Offense, p, "FgAtRim", Teams);
             
         }
         else if (type == "LowPost")
         {
             PlayerActions.RecordStat(game, game.Offense, p, "FgaLowPost", Teams);
             PlayerActions.RecordStat(game, game.Offense, p, "FgLowPost", Teams);

         }
         else if (type == "MidRange")
         {
             PlayerActions.RecordStat(game, game.Offense, p, "FgaMidRange", Teams);
             PlayerActions.RecordStat(game, game.Offense, p, "FgMidRange", Teams);
             
         }
         else if (type == "ThreePointer")
         {
             PlayerActions.RecordStat(game, game.Offense, p, "Pts", Teams);
             PlayerActions.RecordStat(game, game.Offense, p, "Tpa", Teams);
             PlayerActions.RecordStat(game, game.Offense, p, "Tp", Teams);
         }

         if (passer.HasValue)
         {
             int p2 = PlayersOnCourt[game.Offense][passer.Value];
             PlayerActions.RecordStat(game, game.Offense, p2, "Ast", Teams);
         }

         if (andOne)
         {
             return DoFt(game, shooter, 1, Teams, PlayersOnCourt);
         }

         return "Fg";
     }
     
     public static string DoFt(Game game, int shooter, int amount, Team[] Teams, int[][] PlayersOnCourt)
     {
            
         DefenseActions.DoPf(game, game.Defense, Teams, PlayersOnCourt);
         int p = PlayersOnCourt[game.Offense][shooter];

         var player = Teams[game.Offense].Players.Find(player => player.RosterOrder == p);
         double ftp = RandomUtils.Bound(player.CompositeRating.Ratings["ShootingFT"] * 0.6 + 0.45, 0, 0.95);

         string outcome = null;
         for (int i = 0; i < amount; i++)
         {
             PlayerActions.RecordStat(game, game.Offense, p, "Fta", Teams);
             if (new Random().NextDouble() < ftp)
             {
                 PlayerActions.RecordStat(game, game.Offense, p, "Ft", Teams);
                 PlayerActions.RecordStat(game, game.Offense, p, "Pts", Teams);
                 outcome = "Fg";
             }
             else
             {
                 outcome = null;
             }
         }

         if (outcome != "Fg")
         {
             outcome = DefenseActions.DoReb(game, Teams, PlayersOnCourt);
         }

         return outcome;
     }
     
     public static string DoTov(Game game, Team[] Teams, int[][] PlayersOnCourt)
     {
         double[] ratios = PlayerActions.RatingArray(game, Teams, "Turnovers", game.Offense, PlayersOnCourt, 2);
         int playerIndex = PlayerActions.PickPlayer(ratios);
         var p = PlayersOnCourt[game.Offense][playerIndex];

         PlayerActions.RecordStat(game, game.Offense, p, "Tov", Teams);

         if (ProbabilityActions.ProbStl(game, Teams) > new Random().NextDouble())
         {
             return DefenseActions.DoStl(game, p, Teams, PlayersOnCourt);
         }

         return "Tov";
     }

    
}