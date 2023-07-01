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
                var player = teams[t].Players.Find(player => player.RosterOrder == p);


                if (player != null)
                {
                    var playerRatings = player.Ratings.LastOrDefault();
                    array[i] = Math.Pow(CompositeHelper.GetRatingValue(rating, playerRatings), power);
                }
            }

            return array;
        }

        public static PlayerContract GenContract(Player p, Season season, bool randomizeExp = false, bool randomizeAmount = true, bool noLimit = false)
        {
            int amount, expiration, maxAmount, minAmount, potentialDifference, years;

            var ratings = p.Ratings.Last();

            minAmount = 500;
            maxAmount = 20000;

            amount = (int)(((ratings.Ovr - 1) / 100.0 - 0.45) * 3.3 * (maxAmount - minAmount) + minAmount);
            if (randomizeAmount)
            {
                amount *= (int)RandomUtils.Bound(RandomUtils.Gauss(1, 0.1), 0, 2);
            }

            potentialDifference = (int)Math.Round((ratings.Pot - ratings.Ovr) / 4.0);
            years = 5 - potentialDifference;
            if (years < 2)
            {
                years = 2;
            }

            if (ratings.Pot < 40)
            {
                years = 1;
            }
            else if (ratings.Pot < 50)
            {
                years = 2;
            }
            else if (ratings.Pot < 60)
            {
                years = 3;
            }

            if (randomizeExp)
            {
                Random random = new Random();
                years = random.Next(1, years);

                if (season.Year - p.Born.Year <= 22)
                {
                    amount /= 4; // Max $5 million/year
                }
            }

            expiration = season.Year + years - 1;

            if (!noLimit)
            {
                if (amount < minAmount * 1.1)
                {
                    amount = minAmount;
                }
                else if (amount > maxAmount)
                {
                    amount = maxAmount;
                }
            }
            else
            {
                if (amount < 0)
                {
                    amount = 0;
                }
            }

            amount = 50 * (int)Math.Round((double)amount / 50);  // Make it a multiple of 50k

            return new PlayerContract { PlayerId = p.Id, Amount = amount, Exp = expiration };
        }

        public static double CalcBaseChange(int age, int potentialDifference)
        {

            Random random = new Random();

            double val;

            // Average rating change if there is no potential difference
            if (age <= 21)
            {
                val = 0;
            }
            else if (age <= 25)
            {
                val = 0;
            }
            else if (age <= 29)
            {
                val = -1;
            }
            else if (age <= 31)
            {
                val = -2;
            }
            else
            {
                val = -3;
            }


            if (age <= 21)
            {
                if (random.NextDouble() < 0.75)
                {
                    val += potentialDifference * RandomUtils.RandomUniform(0.2, 0.9);
                }
                else
                {
                    val += potentialDifference * RandomUtils.RandomUniform(0.1, 0.3);
                }
            }
            else if (age <= 25)
            {
                if (random.NextDouble() < 0.25)
                {
                    val += potentialDifference * RandomUtils.RandomUniform(0.2, 0.9);
                }
                else
                {
                    val += potentialDifference * RandomUtils.RandomUniform(0.1, 0.3);
                }
            }
            else
            {
                val += potentialDifference * RandomUtils.RandomUniform(0, 0.1);
            }

            // Noise
            if (age <= 25)
            {
                val += RandomUtils.Bound(RandomUtils.Gauss(0, 5), -4, 10);
            }
            else
            {
                val += RandomUtils.Bound(RandomUtils.Gauss(0, 3), -2, 10);
            }

            return val;
        }

        public static int LimitRating(double rating)
        {
            if (rating > 100)
            {
                return 100;
            }
            if (rating < 0)
            {
                return 0;
            }
            return (int)Math.Floor(rating);
        }


    }
}
