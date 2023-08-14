using NBALigaSimulation.Shared.Models;
using System.Net;

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
