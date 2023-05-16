using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine
{
    public static class ArrayHelper
    {

        public static int PickPlayer(double[] ratios, int exempt = -1)
        {
            if (exempt != -1)
            {
                ratios[exempt] = 0;
            }

            Random rand = new Random();
            double randomValue = rand.NextDouble() * (ratios[0] + ratios[1] + ratios[2] + ratios[3] + ratios[4]);
            int pick;

            if (randomValue < ratios[0])
            {
                pick = 0;
            }
            else if (randomValue < (ratios[0] + ratios[1]))
            {
                pick = 1;
            }
            else if (randomValue < (ratios[0] + ratios[1] + ratios[2]))
            {
                pick = 2;
            }
            else if (randomValue < (ratios[0] + ratios[1] + ratios[2] + ratios[3]))
            {
                pick = 3;
            }
            else
            {
                pick = 4;
            }

            return pick;
        }

        public static double[] RatingArray(int[][] playersOnCourt, Team[] teams, string rating, int t, double power = 1)
        {
            double[] array = new double[5];
            power = power != null ? power : 1;

            for (int i = 0; i < 5; i++)
            {
                int p = playersOnCourt[t][i];
                var player = teams[t].Players.Find(player => player.Id == p);


                if (player != null)
                {
                    var playerRatings = player.Ratings.LastOrDefault();
                    array[i] = Math.Pow(CompositeHelper.GetRatingValue(rating, playerRatings), power);
                }
            }

            return array;
        }


    }
}
