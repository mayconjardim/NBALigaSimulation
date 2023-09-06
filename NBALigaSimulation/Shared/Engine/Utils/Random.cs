using System;

namespace NBALigaSimulation.Shared.Engine
{
    public static class RandomUtils
    {

        private static Random random = new Random();

        public static decimal RandomUniform(decimal a, decimal b)
        {
            double randomValue = random.NextDouble();
            decimal decimalRandomValue = (decimal)randomValue;

            return decimalRandomValue * (b - a) + a;
        }

        public static decimal Power(decimal baseValue, int exponent)
        {
            if (exponent < 0)
            {
                throw new ArgumentException("Exponent must be non-negative.");
            }

            decimal result = 1M;

            for (int i = 0; i < exponent; i++)
            {
                result *= baseValue;
            }

            return result;
        }

        public static decimal Exp(decimal x, int numTerms = 15)
        {
            decimal result = 1M;
            decimal factorial = 1M;

            for (int i = 1; i <= numTerms; i++)
            {
                factorial *= i;
                result += (Power(x, i) / factorial);
            }

            return result;
        }

        public static decimal Sigmoid(decimal x, decimal a, decimal b)
        {
            return 1M / (1M + Exp(-(a * (x - b))));
        }

        public static decimal Clamp(decimal value, decimal min, decimal max)
        {
            if (value < min)
                return min;
            else if (value > max)
                return max;
            else
                return value;
        }

        public static void Shuffle<T>(List<T> list)
        {
            Random random = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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

        public static decimal Gauss(decimal mu = 0, decimal sigma = 1)
        {
            decimal u1 = 1.0m - (decimal)random.NextDouble();
            decimal u2 = 1.0m - (decimal)random.NextDouble();
            decimal randStdNormal = (decimal)Math.Sqrt((double)(-2.0m * (decimal)Math.Log((double)u1))) * (decimal)Math.Sin(2.0 * Math.PI * (double)u2);
            decimal randNormal = mu + sigma * randStdNormal;
            return randNormal;
        }


        static System.Random rnd = new System.Random();
        public static decimal GetRandomDecimal(int minValue, int maxValue, int decimalPlaces)
        {
            return System.Math.Round(Convert.ToDecimal(rnd.NextDouble() * (maxValue - minValue) + minValue), decimalPlaces);
        }

    }

}
