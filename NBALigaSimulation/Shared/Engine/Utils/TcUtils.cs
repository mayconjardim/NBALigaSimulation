
namespace NBALigaSimulation.Shared.Engine
{
    public static class TcUtils
    {

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
