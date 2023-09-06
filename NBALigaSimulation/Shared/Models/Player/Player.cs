using NBALigaSimulation.Shared.Engine;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Models
{
    public class Player
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Pos { get; set; } = string.Empty;
        public Born Born { get; set; }
        public PlayerDraft? Draft { get; set; }
        public int Hgt { get; set; }
        public int Weight { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public Team Team { get; set; }
        public int TeamId { get; set; }
        public decimal PtModifier { get; set; }
        public int RosterOrder { get; set; }
        public PlayerContract? Contract { get; set; }
        public List<PlayerRatings> Ratings { get; set; } = new List<PlayerRatings>();
        public List<PlayerGameStats> Stats { get; set; } = new List<PlayerGameStats>();
        public List<PlayerRegularStats> RegularStats { get; set; } = new List<PlayerRegularStats>();
        public List<PlayerPlayoffsStats> PlayoffsStats { get; set; } = new List<PlayerPlayoffsStats>();
        public List<PlayerAwards> PlayerAwards { get; set; } = new List<PlayerAwards>();

        [NotMapped]
        public PlayerCompositeRating CompositeRating { get; set; }

        private static Random random = new Random();

        public PlayerRatings TrainingCamp(Season season)
        {

            Random random = new Random();
            decimal coachingRank = 15.5m;
            int years = 1;
            int age = season.Year - Born.Year;

            var lastRatings = Ratings.Last();
            var ratings = new PlayerRatings
            {

                Hgt = lastRatings.Hgt,
                Str = lastRatings.Str,
                Spd = lastRatings.Spd,
                Jmp = lastRatings.Jmp,
                End = lastRatings.End,
                Ins = lastRatings.Ins,
                Dnk = lastRatings.Dnk,
                Ft = lastRatings.Ft,

                Fg = lastRatings.Fg,
                Tp = lastRatings.Tp,
                Blk = lastRatings.Blk,
                Stl = lastRatings.Stl,
                Drb = lastRatings.Drb,
                Pss = lastRatings.Pss,
                Reb = lastRatings.Reb,
                Pot = lastRatings.Pot,
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

                decimal baseChange = ArrayHelper.CalcBaseChange(age, ratings.Pot - ratings.Ovr);

                if (baseChange >= 0)
                {
                    baseChange *= (coachingRank - 1) * -0.5m / (20 - 1) + 1.25m;
                }
                else
                {
                    baseChange *= (coachingRank - 1) * 0.5m / (20 - 1) + 0.75m;
                }


                decimal baseChangeLocal;
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
                    baseChangeLocal = baseChange - 2.5m;
                }

                ratings.Spd = ArrayHelper.LimitRating(ratings.Spd + RandomUtils.Bound(baseChangeLocal * RandomUtils.RandomUniform(0.5m, 1.5m), -20, 10));
                ratings.Jmp = ArrayHelper.LimitRating(ratings.Jmp + RandomUtils.Bound(baseChangeLocal * RandomUtils.RandomUniform(0.5m, 1.5m), -20, 10));
                ratings.End = ArrayHelper.LimitRating(ratings.End + RandomUtils.Bound(baseChangeLocal * RandomUtils.RandomUniform(0.5m, 1.5m), -20, 10));


                ratings.Drb = ArrayHelper.LimitRating(ratings.Drb + RandomUtils.Bound(baseChange * RandomUtils.RandomUniform(0.5m, 1.5m), -1, 10));
                ratings.Pss = ArrayHelper.LimitRating(ratings.Pss + RandomUtils.Bound(baseChange * RandomUtils.RandomUniform(0.5m, 1.5m), -1, 10));
                ratings.Reb = ArrayHelper.LimitRating(ratings.Reb + RandomUtils.Bound(baseChange * RandomUtils.RandomUniform(0.5m, 1.5m), -1, 10));


                ratings.Str = ArrayHelper.LimitRating(ratings.Str + baseChange * RandomUtils.RandomUniform(0.5m, 1.5m));
                ratings.Dnk = ArrayHelper.LimitRating(ratings.Dnk + baseChange * RandomUtils.RandomUniform(0.5m, 1.5m));
                ratings.Blk = ArrayHelper.LimitRating(ratings.Blk + baseChange * RandomUtils.RandomUniform(0.5m, 1.5m));
                ratings.Stl = ArrayHelper.LimitRating(ratings.Stl + baseChange * RandomUtils.RandomUniform(0.5m, 1.5m));

                decimal baseChangeLocals;
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
                    baseChangeLocals = baseChange + 2.5m;
                }

                ratings.Ins = ArrayHelper.LimitRating(ratings.Ins + baseChangeLocals * RandomUtils.RandomUniform(0.5m, 1.5m));
                ratings.Ft = ArrayHelper.LimitRating(ratings.Ft + baseChangeLocals * RandomUtils.RandomUniform(0.5m, 1.5m));
                ratings.Fg = ArrayHelper.LimitRating(ratings.Fg + baseChangeLocals * RandomUtils.RandomUniform(0.5m, 1.5m));
                ratings.Tp = ArrayHelper.LimitRating(ratings.Tp + baseChangeLocals * RandomUtils.RandomUniform(0.5m, 1.5m));

                ratings.Pot += -2 + (int)Math.Round(RandomUtils.Gauss(0, 2));
                if (ratings.Ovr > ratings.Pot || age > 28)
                {
                    ratings.Pot = ratings.Ovr;
                }
            }

            if (ratings.Ovr > ratings.Pot || age > 28)
            {
                ratings.Pot = ratings.Ovr;
            }

            return ratings;
        }


    }
}
