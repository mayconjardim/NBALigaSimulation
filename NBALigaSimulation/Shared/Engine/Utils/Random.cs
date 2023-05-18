namespace NBALigaSimulation.Shared.Engine
{
    public static class RandomUtils
    {

        private static Random random = new Random();

        public static double RandomUniform(double a, double b)
        {
            return random.NextDouble() * (b - a) + a;
        }

    }
}
