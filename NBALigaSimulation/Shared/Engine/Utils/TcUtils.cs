using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;

namespace NBALigaSimulation.Shared.Engine.Utils
{
    public static class TcUtils
    {

        public static PlayerRatings TrainingCamp(Player player, Season season)
        {

            Random random = new Random();
            double coachingRank = 15.5;
            int years = 1;
            int age = season.Year - player.Born.Year;

            var lastRatings = player.Ratings.Last();
            var ratings = new PlayerRatings
            {

                Diq = lastRatings.Diq,
                Dnk = lastRatings.Dnk,
                Drb = lastRatings.Drb,
                Endu = lastRatings.Endu,
                Fg = lastRatings.Fg,
                Ft = lastRatings.Ft,
                Hgt = lastRatings.Hgt,
                Ins = lastRatings.Ins,
                Jmp = lastRatings.Jmp,
                Spd = lastRatings.Spd,
                Stre = lastRatings.Stre,
                Oiq = lastRatings.Oiq,
                Pot = lastRatings.Pot,
                Pss = lastRatings.Pss,
                Tp = lastRatings.Tp,
                Reb = lastRatings.Reb,

            };

            ratings.Season = season.Year;

            for (int i = 0; i < years; i++)
            {

                if (random.NextDouble() > 0.985 && age <= 23)
                {
                    ratings.Pot += random.Next(5, 25);
                }

                if (random.NextDouble() > 0.995 && age <= 23)
                {
                    ratings.Pot -= random.Next(5, 25);
                }

                double baseChange = ArrayHelper.CalcBaseChange(age, ratings.Pot - ratings.CalculateOvr);

                if (baseChange >= 0)
                {
                    baseChange *= (coachingRank - 1) * -0.5 / (20 - 1) + 1.25;
                }
                else
                {
                    baseChange *= (coachingRank - 1) * 0.5 / (20 - 1) + 0.75;
                }


                double baseChangeLocal;
                if (age <= 24)
                {
                    baseChangeLocal = baseChange;
                }
                else if (age <= 30)
                {
                    baseChangeLocal = baseChange - 1;
                }
                else
                {
                    baseChangeLocal = baseChange - 2.5;
                }

                ratings.Spd = ArrayHelper.LimitRating(ratings.Spd + RandomUtils.Bound(baseChangeLocal * RandomUtils.RandomUniform(0.5, 1.5), -20, 10));
                ratings.Jmp = ArrayHelper.LimitRating(ratings.Jmp + RandomUtils.Bound(baseChangeLocal * RandomUtils.RandomUniform(0.5, 1.5), -20, 10));
                ratings.Endu = ArrayHelper.LimitRating(ratings.Endu + RandomUtils.Bound(baseChangeLocal * RandomUtils.RandomUniform(0.5, 1.5), -20, 10));


                ratings.Drb = ArrayHelper.LimitRating(ratings.Drb + RandomUtils.Bound(baseChange * RandomUtils.RandomUniform(0.5, 1.5), -1, 10));
                ratings.Pss = ArrayHelper.LimitRating(ratings.Pss + RandomUtils.Bound(baseChange * RandomUtils.RandomUniform(0.5, 1.5), -1, 10));
                ratings.Reb = ArrayHelper.LimitRating(ratings.Reb + RandomUtils.Bound(baseChange * RandomUtils.RandomUniform(0.5, 1.5), -1, 10));

                ratings.Stre = ArrayHelper.LimitRating(ratings.Stre + baseChange * RandomUtils.RandomUniform(0.5, 1.5));
                ratings.Dnk = ArrayHelper.LimitRating(ratings.Dnk + baseChange * RandomUtils.RandomUniform(0.5, 1.5));


                double baseChangeLocals;
                if (age <= 24)
                {
                    baseChangeLocals = baseChange;
                }
                else if (age <= 30)
                {
                    baseChangeLocals = baseChange + 1;
                }
                else
                {
                    baseChangeLocals = baseChange + 2.5;
                }

                ratings.Ins = ArrayHelper.LimitRating(ratings.Ins + baseChangeLocals * RandomUtils.RandomUniform(0.5, 1.5));
                ratings.Ft = ArrayHelper.LimitRating(ratings.Ft + baseChangeLocals * RandomUtils.RandomUniform(0.5, 1.5));
                ratings.Fg = ArrayHelper.LimitRating(ratings.Fg + baseChangeLocals * RandomUtils.RandomUniform(0.5, 1.5));
                ratings.Tp = ArrayHelper.LimitRating(ratings.Tp + baseChangeLocals * RandomUtils.RandomUniform(0.5, 1.5));

                ratings.Oiq = ArrayHelper.LimitRating(ratings.Oiq + TcUtils.AgeModifier(age) * RandomUtils.RandomUniform(TcUtils.CalculateChangeLimits(age)[0], TcUtils.CalculateChangeLimits(age)[1]));
                ratings.Diq = ArrayHelper.LimitRating(ratings.Diq + TcUtils.AgeModifier(age) * RandomUtils.RandomUniform(TcUtils.CalculateChangeLimits(age)[0], TcUtils.CalculateChangeLimits(age)[1]));

                ratings.Pot += -2 + (int)Math.Round(RandomUtils.Gauss(0, 2));
                if (ratings.CalculateOvr > ratings.Pot || age > 28)
                {
                    ratings.Pot = ratings.CalculateOvr;
                }
            }

            if (ratings.CalculateOvr > ratings.Pot || age > 28)
            {
                ratings.Pot = ratings.CalculateOvr;
            }

            return ratings;
        }

        public static double AgeModifier(int age)
        {
            if (age <= 21)
            {
                return 4;
            }
            if (age <= 23)
            {
                return 3;
            }
            if (age <= 27)
            {
                return 0;
            }
            if (age <= 29)
            {
                return 0.5;
            }
            if (age <= 31)
            {
                return 1.5;
            }
            return 2;
        }

        public static double[] CalculateChangeLimits(int age)
        {
            if (age > 24)
            {
                return new double[] { -3, 9 };
            }

            return new double[] { -3, 7 + 5 * (24 - age) };
        }

    }
}
