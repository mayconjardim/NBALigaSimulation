using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine
{
    public static class ArrayHelper
    {

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

        public static int PickPlayer(double[] ratios, int exempt = -1)
        {
            if (exempt != -1)
            {
                ratios[exempt] = 0;
            }

            double totalRatio = ratios.Sum();
            double rand = new Random().NextDouble() * totalRatio;
            double cumulativeRatio = 0;

            for (int i = 0; i < ratios.Length; i++)
            {
                cumulativeRatio += ratios[i];
                if (rand < cumulativeRatio)
                {
                    return i;
                }
            }

            return ratios.Length - 1;
        }

    }
}
