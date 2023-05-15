using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine
{
    public static class CompositeHelper
    {

        public static void UpdateCompositeRating(Team[] teams, int[][] playersOnCourt)
        {
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking" };

            for (int i = 0; i < 2; i++)
            {

                if (teams[i].CompositeRating == null)
                {
                    teams[i].CompositeRating = new TeamCompositeRating();
                }

                for (int j = 0; j < 5; j++)
                {

                    int playerId = playersOnCourt[i][j];
                    var playerRatings = teams[i].Players.Find(player => player.Id == playerId).Ratings.LastOrDefault();
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
                    // Caso o nome do rating não seja reconhecido, mantém o valor 0.
                    break;
            }

            return ratingValue;
        }


    }
}
