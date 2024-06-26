﻿using NBALigaSimulation.Shared.Engine.GameSim.PlayerManager;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.Ratings
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

                    //Usage
                    player.CompositeRating.Ratings["Usage"] = 0;
                    Dictionary<string, double> usage = new Dictionary<string, double>();
                    usage.Add("ins", player.Ratings.LastOrDefault().Ins);
                    usage.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    usage.Add("fg", player.Ratings.LastOrDefault().Fg);
                    usage.Add("tp", player.Ratings.LastOrDefault().Tp);
                    usage.Add("spd", player.Ratings.LastOrDefault().Spd);
                    usage.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    usage.Add("drb", player.Ratings.LastOrDefault().Drb);
                    usage.Add("oiq", player.Ratings.LastOrDefault().Oiq);

                    List<string> usageAttributes = new List<string> { "ins", "dnk", "fg", "tp", "spd", "hgt", "drb", "oiq" };
                    List<double> usageWeights = new List<double> { 1.5, 1, 1, 1, 0.5, 0.5, 0.5, 0.5 };

                    Random random = new Random();

                    if (player.KeyPlayer)
                    {

                        double randomNumber;

                        if (player.Ratings.LastOrDefault().CalculateOvr < 50)
                        {
                            randomNumber = random.NextDouble() * (0.580000 - 0.550000) + 0.550000;
                            player.CompositeRating.Ratings["Usage"] = randomNumber;
                        }
                        else if (player.Ratings.LastOrDefault().CalculateOvr < 65)
                        {
                            randomNumber = random.NextDouble() * (0.620000 - 0.600000) + 0.600000;
                            player.CompositeRating.Ratings["Usage"] = randomNumber;
                        }
                        else
                        {
                            randomNumber = random.NextDouble() * (0.650000 - 0.630000) + 0.630000;
                            player.CompositeRating.Ratings["Usage"] = randomNumber;
                        }

                        if (player.PtModifier <= 1.25)
                        {
                            randomNumber = random.NextDouble() * (0.580000 - 0.550000) + 0.550000;
                            player.CompositeRating.Ratings["Usage"] = randomNumber;
                        }

                    }
                    else
                    {
                        double randomNumber;

                        player.CompositeRating.Ratings["Usage"] = Converter.Composite(usage, usageAttributes, usageWeights);

                        if (player.PtModifier <= 1.25 && player.CompositeRating.Ratings["Usage"] > 0.55)
                        {
                            randomNumber = random.NextDouble() * (0.540000 - 0.520000) + 0.520000;
                            player.CompositeRating.Ratings["Usage"] = randomNumber;
                        }

                    }

                    //Pace
                    player.CompositeRating.Ratings["Pace"] = 0;
                    Dictionary<string, double> pace = new Dictionary<string, double>();
                    pace.Add("spd", player.Ratings.LastOrDefault().Spd);
                    pace.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    pace.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    pace.Add("tp", player.Ratings.LastOrDefault().Tp);
                    pace.Add("drb", player.Ratings.LastOrDefault().Reb);
                    pace.Add("pss", player.Ratings.LastOrDefault().Pss);
                    List<string> paceAttributes = new List<string> { "spd", "jmp", "dnk", "tp", "drb", "pss" };
                    player.CompositeRating.Ratings["Pace"] = Converter.Composite(pace, paceAttributes);

                    //Dribbling
                    player.CompositeRating.Ratings["Dribbling"] = 0;
                    Dictionary<string, double> dribbling = new Dictionary<string, double>();
                    dribbling.Add("drb", player.Ratings.LastOrDefault().Drb);
                    dribbling.Add("spd", player.Ratings.LastOrDefault().Spd);
                    List<string> dribblingAttributes = new List<string> { "drb", "spd" };
                    List<double> dribblingWeights = new List<double> { 1, 1 };
                    player.CompositeRating.Ratings["Dribbling"] = Converter.Composite(dribbling, dribblingAttributes, dribblingWeights);

                    //Passing
                    player.CompositeRating.Ratings["Passing"] = 0;
                    Dictionary<string, double> passing = new Dictionary<string, double>();
                    passing.Add("drb", player.Ratings.LastOrDefault().Drb);
                    passing.Add("pss", player.Ratings.LastOrDefault().Pss);
                    passing.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    List<string> passingAttributes = new List<string> { "drb", "pss", "oiq" };
                    List<double> passingWeights = new List<double> { 0.2, 3.0, 0.2 };
                    player.CompositeRating.Ratings["Passing"] = Converter.Composite(passing, passingAttributes, passingWeights);

                    //Turnovers
                    player.CompositeRating.Ratings["Turnovers"] = 0;
                    Dictionary<string, double> turnovers = new Dictionary<string, double>();
                    turnovers.Add("ins", player.Ratings.LastOrDefault().Ins);
                    turnovers.Add("pss", player.Ratings.LastOrDefault().Pss);
                    turnovers.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    List<string> turnoversAttributes = new List<string> { "50", "ins", "pss", "oiq" };
                    List<double> turnoversWeights = new List<double> { 0.5, 1, 1, -1 };
                    player.CompositeRating.Ratings["Turnovers"] = Converter.Composite(turnovers, turnoversAttributes, turnoversWeights);

                    //ShootingAtRim
                    player.CompositeRating.Ratings["ShootingAtRim"] = 0;
                    Dictionary<string, double> shootingAtRim = new Dictionary<string, double>();
                    shootingAtRim.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingAtRim.Add("stre", player.Ratings.LastOrDefault().Stre);
                    shootingAtRim.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    shootingAtRim.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    List<string> shootingAtRimAttributes = new List<string> { "hgt", "stre", "dnk", "oiq" };
                    List<double> shootingAtRimWeights = new List<double> { 0.75, 0.2, 0.6, 0.4, 0.2};
                    player.CompositeRating.Ratings["ShootingAtRim"] = Converter.Composite(shootingAtRim, shootingAtRimAttributes, shootingAtRimWeights);

                    //ShootingLowPost
                    player.CompositeRating.Ratings["ShootingLowPost"] = 0;
                    Dictionary<string, double> shootingLowPost = new Dictionary<string, double>();
                    shootingLowPost.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    shootingLowPost.Add("stre", player.Ratings.LastOrDefault().Stre);
                    shootingLowPost.Add("spd", player.Ratings.LastOrDefault().Spd);
                    shootingLowPost.Add("ins", player.Ratings.LastOrDefault().Ins);
                    shootingLowPost.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    List<string> shootingLowPostAttributes = new List<string> { "hgt", "stre", "spd", "ins", "oiq" };
                    List<double> shootingLowPostWeights = new List<double> { 2, 0.6, 0.2, 1, 0.2};
                    player.CompositeRating.Ratings["ShootingLowPost"] = Converter.Composite(shootingLowPost, shootingLowPostAttributes, shootingLowPostWeights);

                    //ShootingMidRange
                    player.CompositeRating.Ratings["shootingMidRange"] = 0;
                    Dictionary<string, double> shootingMidRange = new Dictionary<string, double>();
                    shootingMidRange.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    shootingMidRange.Add("fg", player.Ratings.LastOrDefault().Fg);
                    shootingMidRange.Add("stre", player.Ratings.LastOrDefault().Stre);
                    List<string> shootingMidRangeAttributes = new List<string> { "oiq", "fg", "stre" };
                    List<double> shootingMidRangeWeights = new List<double> { -0.5, 1, 0.2 };
                    player.CompositeRating.Ratings["ShootingMidRange"] = Converter.Composite(shootingMidRange, shootingMidRangeAttributes, shootingMidRangeWeights);

                    //ShootingThreePointer
                    player.CompositeRating.Ratings["ShootingThreePointer"] = 0;
                    Dictionary<string, double> shootingThreePointer = new Dictionary<string, double>();
                    shootingThreePointer.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    shootingThreePointer.Add("tp", player.Ratings.LastOrDefault().Tp);
                    List<string> shootingThreePointerAttributes = new List<string> { "oiq", "tp" };
                    List<double> shootingThreePointerWeights = new List<double> { 0.1, 1 };
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
                    rebounding.Add("stre", player.Ratings.LastOrDefault().Stre);
                    rebounding.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    rebounding.Add("reb", player.Ratings.LastOrDefault().Reb);
                    rebounding.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    rebounding.Add("diq", player.Ratings.LastOrDefault().Diq);
                    List<string> reboundingAttributes = new List<string> { "hgt", "stre", "jmp", "reb", "oiq", "diq" };
                    List<double> reboundingWeights = new List<double> { 5.0, 0.1, 0.1, 5.0, 0.5, 0.5 };
                    player.CompositeRating.Ratings["Rebounding"] = Converter.Composite(rebounding, reboundingAttributes, reboundingWeights);

                    //Stealing
                    player.CompositeRating.Ratings["Stealing"] = 0;
                    Dictionary<string, double> stealing = new Dictionary<string, double>();
                    stealing.Add("spd", player.Ratings.LastOrDefault().Spd);
                    stealing.Add("diq", player.Ratings.LastOrDefault().Diq);
                    List<string> stealingAttributes = new List<string> { "50", "spd", "diq" };
                    List<double> stealingWeights = new List<double> { 1, 1, 2 };
                    player.CompositeRating.Ratings["Stealing"] = Converter.Composite(stealing, stealingAttributes, stealingWeights);

                    //Blocking
                    player.CompositeRating.Ratings["Blocking"] = 0;
                    Dictionary<string, double> blocking = new Dictionary<string, double>();
                    blocking.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    blocking.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    blocking.Add("diq", player.Ratings.LastOrDefault().Diq);
                    List<string> blockingAttributes = new List<string> { "hgt", "jmp", "diq" };
                    List<double> blockingWeights = new List<double> { 4, 1.5, 0.5 };
                    player.CompositeRating.Ratings["Blocking"] = Converter.Composite(blocking, blockingAttributes, blockingWeights);

                    //Fouling
                    player.CompositeRating.Ratings["Fouling"] = 0;
                    Dictionary<string, double> fouling = new Dictionary<string, double>();
                    fouling.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    fouling.Add("diq", player.Ratings.LastOrDefault().Diq);
                    fouling.Add("spd", player.Ratings.LastOrDefault().Spd);
                    List<string> foulingAttributes = new List<string> { "50", "hgt", "diq", "spd" };
                    List<double> foulingWeights = new List<double> { 3, 1, -1, -1 };
                    player.CompositeRating.Ratings["Fouling"] = Converter.Composite(fouling, foulingAttributes, foulingWeights);

                    //Drawing Fouls
                    player.CompositeRating.Ratings["DrawingFouls"] = 0;
                    Dictionary<string, double> drawingFouls = new Dictionary<string, double>();
                    drawingFouls.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    drawingFouls.Add("spd", player.Ratings.LastOrDefault().Spd);
                    drawingFouls.Add("drb", player.Ratings.LastOrDefault().Drb);
                    drawingFouls.Add("dnk", player.Ratings.LastOrDefault().Dnk);
                    drawingFouls.Add("oiq", player.Ratings.LastOrDefault().Oiq);
                    List<string> drawingFoulsAttributes = new List<string> { "hgt", "spd", "drb", "dnk", "oiq" };
                    List<double> drawingFoulsWeights = new List<double> { 1, 1, 1, 1, 1 };
                    player.CompositeRating.Ratings["DrawingFouls"] = Converter.Composite(drawingFouls, drawingFoulsAttributes, drawingFoulsWeights);

                    //Defense
                    player.CompositeRating.Ratings["Defense"] = 0;
                    Dictionary<string, double> defense = new Dictionary<string, double>();
                    defense.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    defense.Add("stre", player.Ratings.LastOrDefault().Stre);
                    defense.Add("spd", player.Ratings.LastOrDefault().Spd);
                    defense.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    defense.Add("diq", player.Ratings.LastOrDefault().Diq);
                    List<string> defenseAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "diq" };
                    List<double> defenseWeights = new List<double> { 1, 1, 1, 0.5, 2 };
                    player.CompositeRating.Ratings["Defense"] = Converter.Composite(defense, defenseAttributes, defenseWeights);

                    //DefenseInterior
                    player.CompositeRating.Ratings["DefenseInterior"] = 0;
                    Dictionary<string, double> defenseInterior = new Dictionary<string, double>();
                    defenseInterior.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    defenseInterior.Add("stre", player.Ratings.LastOrDefault().Stre);
                    defenseInterior.Add("spd", player.Ratings.LastOrDefault().Spd);
                    defenseInterior.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    defenseInterior.Add("diq", player.Ratings.LastOrDefault().Diq);
                    List<string> defenseInteriorAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "diq" };
                    List<double> defenseInteriorWeights = new List<double> { 2.5, 1, 0.5, 0.5, 2 };
                    player.CompositeRating.Ratings["DefenseInterior"] = Converter.Composite(defenseInterior, defenseInteriorAttributes, defenseInteriorWeights);

                    //DefensePerimeter
                    player.CompositeRating.Ratings["DefensePerimeter"] = 0;
                    Dictionary<string, double> defensePerimeter = new Dictionary<string, double>();
                    defensePerimeter.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    defensePerimeter.Add("stre", player.Ratings.LastOrDefault().Stre);
                    defensePerimeter.Add("spd", player.Ratings.LastOrDefault().Spd);
                    defensePerimeter.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    defensePerimeter.Add("diq", player.Ratings.LastOrDefault().Diq);
                    List<string> defensePerimeterAttributes = new List<string> { "hgt", "stre", "spd", "jmp", "diq" };
                    List<double> defensePerimeterWeights = new List<double> { 0.5, 0.5, 2, 0.5, 1 };
                    player.CompositeRating.Ratings["DefensePerimeter"] = Converter.Composite(defensePerimeter, defensePerimeterAttributes, defensePerimeterWeights);

                    //Endurance
                    player.CompositeRating.Ratings["Endurance"] = 0;
                    Dictionary<string, double> endurance = new Dictionary<string, double>();
                    endurance.Add("endu", player.Ratings.LastOrDefault().Endu);
                    List<string> enduranceAttributes = new List<string> { "50", "endu" };
                    List<double> enduranceWeights = new List<double> { 1, 1 };
                    player.CompositeRating.Ratings["Endurance"] = Converter.Composite(endurance, enduranceAttributes, enduranceWeights);


                    //Athleticism
                    player.CompositeRating.Ratings["Athleticism"] = 0;
                    Dictionary<string, double> athleticism = new Dictionary<string, double>();
                    athleticism.Add("stre", player.Ratings.LastOrDefault().Stre);
                    athleticism.Add("spd", player.Ratings.LastOrDefault().Spd);
                    athleticism.Add("jmp", player.Ratings.LastOrDefault().Jmp);
                    athleticism.Add("hgt", player.Ratings.LastOrDefault().Hgt);
                    List<string> athleticismAttributes = new List<string> { "stre", "spd", "jmp", "hgt" };
                    List<double> athleticismWeights = new List<double> { 1, 1, 1, 0.75 };
                    player.CompositeRating.Ratings["Endurance"] = Converter.Composite(athleticism, athleticismAttributes, athleticismWeights);

                }

            }

        }
        public static void UpdateTeamCompositeRatings(Game game, Team[] teams, int[][] playersOnCourt)
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
                    var playerRatings = teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).CompositeRating;
                    double ratingValue = 0;
                    string rats = string.Empty;

                    foreach (string rating in toUpdate)
                    {
                        teams[i].CompositeRating.Ratings[rating] = 0;

                        if (rating == "GameDribbling")
                        {
                            ratingValue = playerRatings.Ratings["Dribbling"];
                        }
                        else if (rating == "GamePassing")
                        {
                            ratingValue = playerRatings.Ratings["Passing"];
                        }
                        else if (rating == "GameRebounding")
                        {
                            ratingValue = playerRatings.Ratings["Rebounding"];
                        }
                        else if (rating == "GameDefense")
                        {
                            ratingValue = playerRatings.Ratings["Defense"];
                        }
                        else if (rating == "GameDefensePerimeter")
                        {
                            ratingValue = playerRatings.Ratings["DefensePerimeter"];
                        }
                        else if (rating == "GameBlocking")
                        {
                            ratingValue = playerRatings.Ratings["Blocking"];
                        }

                        teams[i].CompositeRating.Ratings[rating] += ratingValue * PlayerActions.Fatigue(teams[i].Players.Find(player 
                            => player.RosterOrder == playerRosterOrder).Stats.Find(s => s.GameId == game.Id).Energy);
                        rats = rating;
                    }

                    teams[i].CompositeRating.Ratings[rats] = teams[i].CompositeRating.Ratings[rats] / 5;

                }

                teams[i].CompositeRating.Ratings["GameDribbling"] += game.SynergyFactor * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GamePassing"] += game.SynergyFactor * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GameRebounding"] += game.SynergyFactor * teams[i].Synergy.Reb;
                teams[i].CompositeRating.Ratings["GameDefense"] += game.SynergyFactor * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameDefensePerimeter"] += game.SynergyFactor * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameBlocking"] += game.SynergyFactor * teams[i].Synergy.Def;
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
                    double ratingValue = 0;

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
                    teams[t].CompositeRating.Ratings["GamePace"] += teams[t].Players.Find(p => p.RosterOrder == i).CompositeRating.Ratings["Pace"];
                }

                teams[t].CompositeRating.Ratings["GamePace"] /= numPlayers;
                teams[t].CompositeRating.Ratings["GamePace"] = teams[t].CompositeRating.Ratings["GamePace"] * 15 + 100;
            }
        }

        public static double GetRatingValue(string ratingName, Player playerRatings)
        {
            double ratingValue = 0;

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
