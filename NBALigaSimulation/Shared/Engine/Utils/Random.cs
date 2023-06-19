namespace NBALigaSimulation.Shared.Engine
{
    public static class RandomUtils
    {

        private static Random random = new Random();

        public static double RandomUniform(double a, double b)
        {
            return random.NextDouble() * (b - a) + a;
        }

        public static double Sigmoid(double x, double a, double b)
        {
            return 1 / (1 + Math.Exp(-(a * (x - b))));
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static string Ordinal(int x)
        {
            string suffix;

            if (x >= 11 && x <= 13)
            {
                suffix = "th";
            }
            else if (x % 10 == 1)
            {
                suffix = "st";
            }
            else if (x % 10 == 2)
            {
                suffix = "nd";
            }
            else if (x % 10 == 3)
            {
                suffix = "rd";
            }
            else
            {
                suffix = "th";
            }

            return x.ToString() + suffix;
        }

        public static T Bound<T>(T x, T min, T max) where T : IComparable<T>
        {
            if (x.CompareTo(min) < 0)
            {
                return min;
            }
            if (x.CompareTo(max) > 0)
            {
                return max;
            }
            return x;
        }

        public static double Gauss(double mu = 0, double sigma = 1)
        {
            double u1 = 1.0 - random.NextDouble();
            double u2 = 1.0 - random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = mu + sigma * randStdNormal;
            return randNormal;
        }

    }

}
