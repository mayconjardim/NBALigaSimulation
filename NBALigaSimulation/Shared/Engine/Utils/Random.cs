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


    }
}
