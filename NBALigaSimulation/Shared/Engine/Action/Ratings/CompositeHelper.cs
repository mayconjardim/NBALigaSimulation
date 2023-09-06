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
                    Dictionary<string, decimal> pace = new Dictionary<string, decimal>();
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
                    Dictionary<string, decimal> usage = new Dictionary<string, decimal>();
                    usage.Add("ins", player.Ratings.LastOrDefault().Ins);
                    usage.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    usage.Add("fg", player.Ratings.LastOrDefault().Fg);
                    usage.Add("tp", player.Ratings.LastOrDefault().Tp);
                    List<string> usageAttributes = new List<string> { "ins", "dnk", "fg", "tp" };
                    player.CompositeRating.Ratings["Usage"] = Converter.Composite(usage, usageAttributes);

                    //Dribbling
                    player.CompositeRating.Ratings["Dribbling"] = 0;
                    Dictionary<string, decimal> dribbling = new Dictionary<string, decimal>();
                    dribbling.Add("drb", player.Ratings.LastOrDefault().Drb);
                    dribbling.Add("spd", player.Ratings.LastOrDefault().Spd);
                    List<string> dribblingAttributes = new List<string> { "drb", "spd" };
                    player.CompositeRating.Ratings["Dribbling"] = Converter.Composite(usage, usageAttributes);

                    //Passing
                    player.CompositeRating.Ratings["Passing"] = 0;
                    Dictionary<string, decimal> passing = new Dictionary<string, decimal>();
                    passing.Add("drb", player.Ratings.LastOrDefault().Drb);
                    passing.Add("pss", player.Ratings.LastOrDefault().Pss);
                    List<string> passingAttributes = new List<string> { "drb", "pss" };
                    List<decimal> passingWeights = new List<decimal> { 0.4m, 1 };
                    player.CompositeRating.Ratings["Passing"] = Converter.Composite(passing, passingAttributes, passingWeights);

                    //Turnovers
                    player.CompositeRating.Ratings["Turnovers"] = 0;
                    Dictionary<string, decimal> turnovers = new Dictionary<string, decimal>();
                    turnovers.Add("drb", player.Ratings.LastOrDefault().Drb);
                    turnovers.Add("pss", player.Ratings.LastOrDefault().Pss);
                    turnovers.Add("spd", player.Ratings.LastOrDefault().Spd);
                    turnovers.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    turnovers.Add("ins", player.Ratings.LastOrDefault().Ins);
                    List<string> turnoversAttributes = new List<string> { "drb", "pss", "spd", "hgt", "ins" };
                    List<decimal> turnoversWeights = new List<decimal> { 1, 1, -1, 1, 1 };
                    player.CompositeRating.Ratings["Turnovers"] = Converter.Composite(turnovers, turnoversAttributes, turnoversWeights);

                    //ShootingAtRim
                    player.CompositeRating.Ratings["ShootingAtRim"] = 0;
                    Dictionary<string, decimal> shootingAtRim = new Dictionary<string, decimal>();
                    shootingAtRim.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingAtRim.Add("spd", player.Ratings.LastOrDefault().Spd);
                    shootingAtRim.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    shootingAtRim.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    List<string> shootingAtRimAttributes = new List<string> { "hgt", "spd", "jmp", "dnk" };
                    List<decimal> shootingAtRimWeights = new List<decimal> { 1, 0.2m, 0.6m, 0.4m };
                    player.CompositeRating.Ratings["ShootingAtRim"] = Converter.Composite(shootingAtRim, shootingAtRimAttributes, shootingAtRimWeights);

                    //ShootingLowPost
                    player.CompositeRating.Ratings["ShootingLowPost"] = 0;
                    Dictionary<string, decimal> shootingLowPost = new Dictionary<string, decimal>();
                    shootingLowPost.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingLowPost.Add("stre", player.Ratings.LastOrDefault().Str);
                    shootingLowPost.Add("spd", player.Ratings.LastOrDefault().Spd);
                    shootingLowPost.Add("ins", player.Ratings.LastOrDefault().Ins);
                    List<string> shootingLowPostAttributes = new List<string> { "hgt", "stre", "spd", "ins" };
                    List<decimal> shootingLowPostWeights = new List<decimal> { 1, 0.6m, 0.2m, 1 };
                    player.CompositeRating.Ratings["ShootingLowPost"] = Converter.Composite(shootingLowPost, shootingLowPostAttributes, shootingLowPostWeights);

                    //ShootingMidRange
                    player.CompositeRating.Ratings["ShootingMidRange"] = 0;
                    Dictionary<string, decimal> shootingMidRange = new Dictionary<string, decimal>();
                    shootingMidRange.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingMidRange.Add("fg", player.Ratings.LastOrDefault().Fg);
                    List<string> shootingMidRangeAttributes = new List<string> { "hgt", "fg" };
                    List<decimal> shootingMidRangeWeights = new List<decimal> { 0.2m, 1 };
                    player.CompositeRating.Ratings["ShootingMidRange"] = Converter.Composite(shootingMidRange, shootingMidRangeAttributes, shootingMidRangeWeights);

                    //ShootingThreePointer
                    player.CompositeRating.Ratings["ShootingThreePointer"] = 0;
                    Dictionary<string, decimal> shootingThreePointer = new Dictionary<string, decimal>();
                    shootingThreePointer.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingThreePointer.Add("tp", player.Ratings.LastOrDefault().Tp);
                    List<string> shootingThreePointerAttributes = new List<string> { "hgt", "tp" };
                    List<decimal> shootingThreePointerWeights = new List<decimal> { 0.2m, 1 };
                    player.CompositeRating.Ratings["ShootingThreePointer"] = Converter.Composite(shootingThreePointer, shootingThreePointerAttributes, shootingThreePointerWeights);

                    //ShootingFT
                    player.CompositeRating.Ratings["ShootingFT"] = 0;
                    Dictionary<string, decimal> shootingFT = new Dictionary<string, decimal>();
                    shootingFT.Add("ft", player.Ratings.LastOrDefault().Ft);
                    List<string> shootingFtAttributes = new List<string> { "ft" };
                    player.CompositeRating.Ratings["ShootingFT"] = Converter.Composite(shootingFT, shootingFtAttributes);

                    //Rebounding
                    player.CompositeRating.Ratings["Rebounding"] = 0;
                    Dictionary<string, decimal> rebounding = new Dictionary<string, decimal>();
                    rebounding.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    rebounding.Add("stre", player.Ratings.LastOrDefault().Str);
                    rebounding.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    rebounding.Add("reb", player.Ratings.LastOrDefault().Reb);
                    List<string> reboundingAttributes = new List<string> { "hgt", "stre", "jmp", "reb" };
                    List<decimal> reboundingWeights = new List<decimal> { 1.5m, 0.1m, 0.1m, 0.7m };
                    player.CompositeRating.Ratings["Rebounding"] = Converter.Composite(rebounding, reboundingAttributes, reboundingWeights);

                    //Stealing
                    player.CompositeRating.Ratings["Stealing"] = 0;
                    Dictionary<string, decimal> stealing = new Dictionary<string, decimal>();
                    stealing.Add("spd", player.Ratings.LastOrDefault().Spd);
                    stealing.Add("stl", player.Ratings.LastOrDefault().Stl);
                    List<string> stealingAttributes = new List<string> { "spd", "stl" };
                    player.CompositeRating.Ratings["Stealing"] = Converter.Composite(stealing, stealingAttributes);

                    //Blocking
                    player.CompositeRating.Ratings["Blocking"] = 0;
                    Dictionary<string, decimal> blocking = new Dictionary<string, decimal>();
                    blocking.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    blocking.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    blocking.Add("blk", player.Ratings.LastOrDefault().Blk);
                    List<string> blockingAttributes = new List<string> { "hgt", "jmp", "blk" };
                    List<decimal> blockingWeights = new List<decimal> { 1.5m, 0.5m, 0.5m };
                    player.CompositeRating.Ratings["Blocking"] = Converter.Composite(blocking, blockingAttributes, blockingWeights);

                    //Fouling
                    player.CompositeRating.Ratings["Fouling"] = 0;
                    Dictionary<string, decimal> fouling = new Dictionary<string, decimal>();
                    fouling.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    fouling.Add("blk", player.Ratings.LastOrDefault().Blk);
                    fouling.Add("spd", player.Ratings.LastOrDefault().Spd);
                    List<string> foulingAttributes = new List<string> { "hgt", "blk", "spd" };
                    List<decimal> foulingWeights = new List<decimal> { 1, 1, -1 };
                    player.CompositeRating.Ratings["Fouling"] = Converter.Composite(fouling, foulingAttributes, foulingWeights);

                    //Defense
                    player.CompositeRating.Ratings["Defense"] = 0;
                    Dictionary<string, decimal> defense = new Dictionary<string, decimal>();
                    defense.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    defense.Add("stre", player.Ratings.LastOrDefault().Str);
                    defense.Add("spd", player.Ratings.LastOrDefault().Spd);
                    defense.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    defense.Add("blk", player.Ratings.LastOrDefault().Blk);
                    defense.Add("stl", player.Ratings.LastOrDefault().Stl);
                    List<string> defenseAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "blk", "stl" };
                    List<decimal> defenseWeights = new List<decimal> { 1, 1, 1, 0.5m, 1, 1 };
                    player.CompositeRating.Ratings["Defense"] = Converter.Composite(defense, defenseAttributes, defenseWeights);

                    //DefenseInterior
                    player.CompositeRating.Ratings["DefenseInterior"] = 0;
                    Dictionary<string, decimal> defenseInterior = new Dictionary<string, decimal>();
                    defenseInterior.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    defenseInterior.Add("stre", player.Ratings.LastOrDefault().Str);
                    defenseInterior.Add("spd", player.Ratings.LastOrDefault().Spd);
                    defenseInterior.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    defenseInterior.Add("blk", player.Ratings.LastOrDefault().Blk);
                    List<string> defenseInteriorAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "blk" };
                    List<decimal> defenseInteriorWeights = new List<decimal> { 2, 1, 0.5m, 0.5m, 1 };
                    player.CompositeRating.Ratings["DefenseInterior"] = Converter.Composite(defenseInterior, defenseInteriorAttributes, defenseInteriorWeights);

                    //DefensePerimeter
                    player.CompositeRating.Ratings["DefensePerimeter"] = 0;
                    Dictionary<string, decimal> defensePerimeter = new Dictionary<string, decimal>();
                    defensePerimeter.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    defensePerimeter.Add("stre", player.Ratings.LastOrDefault().Str);
                    defensePerimeter.Add("spd", player.Ratings.LastOrDefault().Spd);
                    defensePerimeter.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    defensePerimeter.Add("stl", player.Ratings.LastOrDefault().Stl);
                    List<string> defensePerimeterAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "stl" };
                    List<decimal> defensePerimeterWeights = new List<decimal> { 1, 1, 2, 0.5m, 1 };
                    player.CompositeRating.Ratings["DefensePerimeter"] = Converter.Composite(defensePerimeter, defensePerimeterAttributes, defensePerimeterWeights);

                    //Endurance
                    player.CompositeRating.Ratings["Endurance"] = 0;
                    Dictionary<string, decimal> endurance = new Dictionary<string, decimal>();
                    endurance.Add("endu", player.Ratings.LastOrDefault().End);
                    endurance.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    List<string> enduranceAttributes = new List<string> { "endu", "hgt" };
                    List<decimal> enduranceWeights = new List<decimal> { 1, 1, -0.1m };
                    player.CompositeRating.Ratings["Endurance"] = Converter.Composite(endurance, enduranceAttributes, enduranceWeights);


                    //Athleticism
                    player.CompositeRating.Ratings["Athleticism"] = 0;
                    Dictionary<string, decimal> athleticism = new Dictionary<string, decimal>();
                    athleticism.Add("stre", player.Ratings.LastOrDefault().Str);
                    athleticism.Add("spd", player.Ratings.LastOrDefault().Spd);
                    athleticism.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    athleticism.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    List<string> athleticismAttributes = new List<string> { "stre", "spd", "jmp", "hgt" };
                    List<decimal> athleticismWeights = new List<decimal> { 1, 1, 1, 0.5m };
                    player.CompositeRating.Ratings["Endurance"] = Converter.Composite(athleticism, athleticismAttributes, athleticismWeights);

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
                    var playerRatings = teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder);
                    decimal ratingValue = 0;

                    foreach (string rating in toUpdate)
                    {
                        teams[i].CompositeRating.Ratings[rating] = 0;

                        if (rating == "GameDribbling")
                        {
                            ratingValue = playerRatings.CompositeRating.Ratings["Dribbling"];
                        }
                        else if (rating == "GamePassing")
                        {
                            ratingValue = playerRatings.CompositeRating.Ratings["Passing"];
                        }
                        else if (rating == "GameRebounding")
                        {
                            ratingValue = playerRatings.CompositeRating.Ratings["Rebounding"];
                        }
                        else if (rating == "GameDefense")
                        {
                            ratingValue = playerRatings.CompositeRating.Ratings["Defense"];
                        }
                        else if (rating == "GameDefensePerimeter")
                        {
                            ratingValue = playerRatings.CompositeRating.Ratings["DefensePerimeter"];
                        }
                        else if (rating == "GameBlocking")
                        {
                            ratingValue = playerRatings.CompositeRating.Ratings["Blocking"];
                        }

                        teams[i].CompositeRating.Ratings[rating] += ratingValue;
                    }
                }

                teams[i].CompositeRating.Ratings["GameDribbling"] += 0.1m * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GamePassing"] += 0.1m * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GameRebounding"] += 0.1m * teams[i].Synergy.Reb;
                teams[i].CompositeRating.Ratings["GameDefense"] += 0.1m * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameDefensePerimeter"] += 0.1m * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameBlocking"] += 0.1m * teams[i].Synergy.Def;

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
                    teams[t].CompositeRating.Ratings["GamePace"] += teams[t].Players.Find(p => p.RosterOrder == i).CompositeRating.Ratings["Pace"];
                }

                teams[t].CompositeRating.Ratings["GamePace"] /= numPlayers;
                teams[t].CompositeRating.Ratings["GamePace"] = teams[t].CompositeRating.Ratings["GamePace"] * 15 + 100;
            }
        }

        public static decimal GetRatingValue(string ratingName, Player playerRatings)
        {
            decimal ratingValue = 0;

            switch (ratingName)
            {
                case "GameEndurance":
                    ratingValue = playerRatings.CompositeRating.Ratings["Endurance"];
                    break;
                case "GameDefensePerimeter":
                    ratingValue = playerRatings.CompositeRating.Ratings["DefensePerimeter"];
                    break;
                case "GameDefenseInterior":
                    ratingValue = playerRatings.CompositeRating.Ratings["DefenseInterior"];
                    break;
                case "GameDefense":
                    ratingValue = playerRatings.CompositeRating.Ratings["Defense"];
                    break;
                case "GameFouling":
                    ratingValue = playerRatings.CompositeRating.Ratings["Fouling"];
                    break;
                case "GameBlocking":
                    ratingValue = playerRatings.CompositeRating.Ratings["Blocking"];
                    break;
                case "GameStealing":
                    ratingValue = playerRatings.CompositeRating.Ratings["Stealing"];
                    break;
                case "GameRebounding":
                    ratingValue = playerRatings.CompositeRating.Ratings["Rebounding"];
                    break;
                case "GameShootingFT":
                    ratingValue = playerRatings.CompositeRating.Ratings["ShootingFT"];
                    break;
                case "GameShootingThreePointer":
                    ratingValue = playerRatings.CompositeRating.Ratings["ShootingThreePointer"];
                    break;
                case "GameShootingMidRange":
                    ratingValue = playerRatings.CompositeRating.Ratings["ShootingMidRange"];
                    break;
                case "GameShootingLowPost":
                    ratingValue = playerRatings.CompositeRating.Ratings["ShootingLowPost"];
                    break;
                case "GameShootingAtRim":
                    ratingValue = playerRatings.CompositeRating.Ratings["ShootingAtRim"];
                    break;
                case "GameTurnovers":
                    ratingValue = playerRatings.CompositeRating.Ratings["Turnovers"];
                    break;
                case "GamePassing":
                    ratingValue = playerRatings.CompositeRating.Ratings["Passing"];
                    break;
                case "GameDribbling":
                    ratingValue = playerRatings.CompositeRating.Ratings["Dribbling"];
                    break;
                case "GameUsage":
                    ratingValue = playerRatings.CompositeRating.Ratings["Usage"];
                    break;
                case "GamePace":
                    ratingValue = playerRatings.CompositeRating.Ratings["Pace"];
                    break;
                default:
                    break;
            }

            return ratingValue;
        }


    }
}
