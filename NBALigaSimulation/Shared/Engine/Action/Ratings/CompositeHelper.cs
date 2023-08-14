using NBALigaSimulation.Shared.Models;
using System.Net;
using System.Reflection;

namespace NBALigaSimulation.Shared.Engine
{
    public static class CompositeHelper
    {

        public static void UpdatePlayersCompositeRating(Team[] teams)
        {

            foreach (var team in teams)
            {

                foreach (var player in team.Players)
                {

                    if (player.CompositeRating == null)
                    {
                        player.CompositeRating = new PlayerCompositeRating();
                    }

                    //Pace
                    player.CompositeRating.Ratings["Pace"] = 0;
                    Dictionary<string, double> pace = new Dictionary<string, double>();
                    pace.Add("spd", player.Ratings.LastOrDefault().Spd);
                    pace.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    pace.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    pace.Add("tp", player.Ratings.LastOrDefault().Tp);
                    pace.Add("stl", player.Ratings.LastOrDefault().Stl);
                    pace.Add("drb", player.Ratings.LastOrDefault().Reb);
                    pace.Add("pss", player.Ratings.LastOrDefault().Pss);
                    List<string> paceAttributes = new List<string> { "spd", "jmp", "dnk", "tp", "stl", "drb", "pss" };
                    player.CompositeRating.Ratings["Pace"] = Converter.Composite(pace, paceAttributes);

                    //Usage
                    player.CompositeRating.Ratings["Usage"] = 0;
                    Dictionary<string, double> usage = new Dictionary<string, double>();
                    usage.Add("ins", player.Ratings.LastOrDefault().Ins);
                    usage.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    usage.Add("fg", player.Ratings.LastOrDefault().Fg);
                    usage.Add("tp", player.Ratings.LastOrDefault().Tp);
                    List<string> usageAttributes = new List<string> { "ins", "dnk", "fg", "tp" };
                    player.CompositeRating.Ratings["Usage"] = Converter.Composite(usage, usageAttributes);

                    //Dribbling
                    player.CompositeRating.Ratings["Dribbling"] = 0;
                    Dictionary<string, double> dribbling = new Dictionary<string, double>();
                    dribbling.Add("drb", player.Ratings.LastOrDefault().Drb);
                    dribbling.Add("spd", player.Ratings.LastOrDefault().Spd);
                    List<string> dribblingAttributes = new List<string> { "drb", "spd" };
                    player.CompositeRating.Ratings["Dribbling"] = Converter.Composite(usage, usageAttributes);

                    //Passing
                    player.CompositeRating.Ratings["Passing"] = 0;
                    Dictionary<string, double> passing = new Dictionary<string, double>();
                    passing.Add("drb", player.Ratings.LastOrDefault().Drb);
                    passing.Add("pss", player.Ratings.LastOrDefault().Pss);
                    List<string> passingAttributes = new List<string> { "drb", "pss" };
                    List<double> passingWeights = new List<double> { 0.4, 1 };
                    player.CompositeRating.Ratings["Passing"] = Converter.Composite(passing, passingAttributes, passingWeights);

                    //Turnovers
                    player.CompositeRating.Ratings["Turnovers"] = 0;
                    Dictionary<string, double> turnovers = new Dictionary<string, double>();
                    turnovers.Add("drb", player.Ratings.LastOrDefault().Drb);
                    turnovers.Add("pss", player.Ratings.LastOrDefault().Pss);
                    turnovers.Add("spd", player.Ratings.LastOrDefault().Spd);
                    turnovers.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    turnovers.Add("ins", player.Ratings.LastOrDefault().Ins);
                    List<string> turnoversAttributes = new List<string> { "drb", "pss", "spd", "hgt", "ins" };
                    List<double> turnoversWeights = new List<double> { 1, 1, -1, 1, 1 };
                    player.CompositeRating.Ratings["Turnovers"] = Converter.Composite(turnovers, turnoversAttributes, turnoversWeights);

                    //ShootingAtRim
                    player.CompositeRating.Ratings["ShootingAtRim"] = 0;
                    Dictionary<string, double> shootingAtRim = new Dictionary<string, double>();
                    shootingAtRim.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingAtRim.Add("spd", player.Ratings.LastOrDefault().Spd);
                    shootingAtRim.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    shootingAtRim.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    List<string> shootingAtRimAttributes = new List<string> { "hgt", "spd", "jmp", "dnk" };
                    List<double> shootingAtRimWeights = new List<double> { 1, 0.2, 0.6, 0.4 };
                    player.CompositeRating.Ratings["ShootingAtRim"] = Converter.Composite(shootingAtRim, shootingAtRimAttributes, shootingAtRimWeights);

                    //ShootingLowPost
                    player.CompositeRating.Ratings["ShootingLowPost"] = 0;
                    Dictionary<string, double> shootingLowPost = new Dictionary<string, double>();
                    shootingLowPost.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingLowPost.Add("stre", player.Ratings.LastOrDefault().Str);
                    shootingLowPost.Add("spd", player.Ratings.LastOrDefault().Spd);
                    shootingLowPost.Add("ins", player.Ratings.LastOrDefault().Ins);
                    List<string> shootingLowPostAttributes = new List<string> { "hgt", "stre", "spd", "ins" };
                    List<double> shootingLowPostWeights = new List<double> { 1, 0.6, 0.2, 1 };
                    player.CompositeRating.Ratings["ShootingLowPost"] = Converter.Composite(shootingLowPost, shootingLowPostAttributes, shootingLowPostWeights);

                    //ShootingMidRange
                    player.CompositeRating.Ratings["ShootingMidRange"] = 0;
                    Dictionary<string, double> shootingMidRange = new Dictionary<string, double>();
                    shootingMidRange.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingMidRange.Add("fg", player.Ratings.LastOrDefault().Fg);
                    List<string> shootingMidRangeAttributes = new List<string> { "hgt", "fg" };
                    List<double> shootingMidRangeWeights = new List<double> { 0.2, 1 };
                    player.CompositeRating.Ratings["ShootingMidRange"] = Converter.Composite(shootingMidRange, shootingMidRangeAttributes, shootingMidRangeWeights);

                    //ShootingThreePointer
                    player.CompositeRating.Ratings["ShootingThreePointer"] = 0;
                    Dictionary<string, double> shootingThreePointer = new Dictionary<string, double>();
                    shootingThreePointer.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingThreePointer.Add("tp", player.Ratings.LastOrDefault().Tp);
                    List<string> shootingThreePointerAttributes = new List<string> { "hgt", "tp" };
                    List<double> shootingThreePointerWeights = new List<double> { 0.2, 1 };
                    player.CompositeRating.Ratings["ShootingThreePointer"] = Converter.Composite(shootingThreePointer, shootingThreePointerAttributes, shootingThreePointerWeights);

                    //ShootingFT
                    player.CompositeRating.Ratings["ShootingFT"] = 0;
                    Dictionary<string, double> shootingFT = new Dictionary<string, double>();
                    shootingFT.Add("ft", player.Ratings.LastOrDefault().Ft);
                    List<string> shootingFtAttributes = new List<string> { "ft" };
                    player.CompositeRating.Ratings["ShootingFT"] = Converter.Composite(shootingFT, shootingFtAttributes);

                    //Rebounding
                    player.CompositeRating.Ratings["Rebounding"] = 0;
                    Dictionary<string, double> rebounding = new Dictionary<string, double>();
                    rebounding.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    rebounding.Add("stre", player.Ratings.LastOrDefault().Str);
                    rebounding.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    rebounding.Add("reb", player.Ratings.LastOrDefault().Reb);
                    List<string> reboundingAttributes = new List<string> { "hgt", "stre", "jmp", "reb" };
                    List<double> reboundingWeights = new List<double> { 1.5, 0.1, 0.1, 0.7 };
                    player.CompositeRating.Ratings["Rebounding"] = Converter.Composite(rebounding, reboundingAttributes, reboundingWeights);

                    //Stealing
                    player.CompositeRating.Ratings["Stealing"] = 0;
                    Dictionary<string, double> stealing = new Dictionary<string, double>();
                    stealing.Add("spd", player.Ratings.LastOrDefault().Spd);
                    stealing.Add("stl", player.Ratings.LastOrDefault().Stl);
                    List<string> stealingAttributes = new List<string> { "spd", "stl" };
                    player.CompositeRating.Ratings["Stealing"] = Converter.Composite(stealing, stealingAttributes);

                    //Blocking
                    player.CompositeRating.Ratings["Blocking"] = 0;
                    Dictionary<string, double> blocking = new Dictionary<string, double>();
                    blocking.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    blocking.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    blocking.Add("blk", player.Ratings.LastOrDefault().Blk);
                    List<string> blockingAttributes = new List<string> { "hgt", "jmp", "blk" };
                    List<double> blockingWeights = new List<double> { 1.5, 0.5, 0.5 };
                    player.CompositeRating.Ratings["Blocking"] = Converter.Composite(blocking, blockingAttributes, blockingWeights);

                    //Fouling
                    player.CompositeRating.Ratings["Fouling"] = 0;
                    Dictionary<string, double> fouling = new Dictionary<string, double>();
                    fouling.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    fouling.Add("blk", player.Ratings.LastOrDefault().Blk);
                    fouling.Add("spd", player.Ratings.LastOrDefault().Spd);
                    List<string> foulingAttributes = new List<string> { "hgt", "blk", "spd" };
                    List<double> foulingWeights = new List<double> { 1, 1, -1 };
                    player.CompositeRating.Ratings["Fouling"] = Converter.Composite(fouling, foulingAttributes, foulingWeights);

                    //Defense
                    player.CompositeRating.Ratings["Defense"] = 0;
                    Dictionary<string, double> defense = new Dictionary<string, double>();
                    defense.Add("hgt", Hgt);
                    defense.Add("stre", Str);
                    defense.Add("spd", Spd);
                    defense.Add("jmp", Jmp);
                    defense.Add("blk", Blk);
                    defense.Add("stl", Stl);
                    List<string> defenseAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "blk", "stl" };
                    List<double> defenseWeights = new List<double> { 1, 1, 1, 0.5, 1, 1 };
                    player.CompositeRating.Ratings["Defense"] = Converter.Composite(rating, attributes, weights);

                }

            }

        }


        public static void UpdateCompositeRating(Team[] teams, int[][] playersOnCourt)
        {
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking", "GamePace" };

            for (int i = 0; i < 2; i++)
            {

                if (teams[i].CompositeRating == null)
                {
                    teams[i].CompositeRating = new TeamCompositeRating();
                }

                for (int j = 0; j < 5; j++)
                {

                    int playerRosterOrder = playersOnCourt[i][j];
                    var playerRatings = teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Ratings.LastOrDefault();
                    double ratingValue = 0;

                    foreach (string rating in toUpdate)
                    {
                        teams[i].CompositeRating.Ratings[rating] = 0;

                        if (rating == "GameDribbling")
                        {
                            ratingValue = playerRatings.GameDribbling;
                        }
                        else if (rating == "GamePassing")
                        {
                            ratingValue = playerRatings.GamePassing;
                        }
                        else if (rating == "GameRebounding")
                        {
                            ratingValue = playerRatings.GameRebounding;
                        }
                        else if (rating == "GameDefense")
                        {
                            ratingValue = playerRatings.GameDefense;
                        }
                        else if (rating == "GameDefensePerimeter")
                        {
                            ratingValue = playerRatings.GameDefensePerimeter;
                        }
                        else if (rating == "GameBlocking")
                        {
                            ratingValue = playerRatings.GameBlocking;
                        }

                        teams[i].CompositeRating.Ratings[rating] += ratingValue;
                    }
                }

                teams[i].CompositeRating.Ratings["GameDribbling"] += 0.1 * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GamePassing"] += 0.1 * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GameRebounding"] += 0.1 * teams[i].Synergy.Reb;
                teams[i].CompositeRating.Ratings["GameDefense"] += 0.1 * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameDefensePerimeter"] += 0.1 * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameBlocking"] += 0.1 * teams[i].Synergy.Def;

            }
        }

        public static void UpdatePace(Team[] teams)
        {
            for (int t = 0; t < 2; t++)
            {
                if (teams[t].CompositeRating == null)
                {
                    teams[t].CompositeRating = new TeamCompositeRating();
                }

                teams[t].CompositeRating.Ratings["GamePace"] = 0;

                int numPlayers = teams[t].Players.Count;
                if (numPlayers > 7)
                {
                    numPlayers = 7;
                }

                for (int i = 0; i < numPlayers; i++)
                {
                    teams[t].CompositeRating.Ratings["GamePace"] += teams[t].Players.Find(p => p.RosterOrder == i).Ratings.Last().GamePace;
                }

                teams[t].CompositeRating.Ratings["GamePace"] /= numPlayers;
                teams[t].CompositeRating.Ratings["GamePace"] = teams[t].CompositeRating.Ratings["GamePace"] * 15 + 100;
            }
        }

        public static double GetRatingValue(string ratingName, PlayerRatings playerRatings)
        {
            double ratingValue = 0;

            switch (ratingName)
            {
                case "GameEndurance":
                    ratingValue = playerRatings.GameEndurance;
                    break;
                case "GameDefensePerimeter":
                    ratingValue = playerRatings.GameDefensePerimeter;
                    break;
                case "GameDefenseInterior":
                    ratingValue = playerRatings.GameDefenseInterior;
                    break;
                case "GameDefense":
                    ratingValue = playerRatings.GameDefense;
                    break;
                case "GameFouling":
                    ratingValue = playerRatings.GameFouling;
                    break;
                case "GameBlocking":
                    ratingValue = playerRatings.GameBlocking;
                    break;
                case "GameStealing":
                    ratingValue = playerRatings.GameStealing;
                    break;
                case "GameRebounding":
                    ratingValue = playerRatings.GameRebounding;
                    break;
                case "GameShootingFT":
                    ratingValue = playerRatings.GameShootingFT;
                    break;
                case "GameShootingThreePointer":
                    ratingValue = playerRatings.GameShootingThreePointer;
                    break;
                case "GameShootingMidRange":
                    ratingValue = playerRatings.GameShootingMidRange;
                    break;
                case "GameShootingLowPost":
                    ratingValue = playerRatings.GameShootingLowPost;
                    break;
                case "GameShootingAtRim":
                    ratingValue = playerRatings.GameShootingAtRim;
                    break;
                case "GameTurnovers":
                    ratingValue = playerRatings.GameTurnovers;
                    break;
                case "GamePassing":
                    ratingValue = playerRatings.GamePassing;
                    break;
                case "GameDribbling":
                    ratingValue = playerRatings.GameDribbling;
                    break;
                case "GameUsage":
                    ratingValue = playerRatings.GameUsage;
                    break;
                case "GamePace":
                    ratingValue = playerRatings.GamePace;
                    break;
                default:
                    break;
            }

            return ratingValue;
        }


    }
}
