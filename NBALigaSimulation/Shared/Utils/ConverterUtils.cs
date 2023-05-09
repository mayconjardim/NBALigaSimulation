namespace NBALigaSimulation.Shared.Utils
{
    public static class ConverterUtils
    {

        public static int InchesToCm(double inches)
        {
            const double cmPerInch = 2.54;
            return (int)(inches * cmPerInch);
        }

        public static int LbsToKg(double lbs)
        {
            const double kgPerLbs = 0.45359237;
            return (int)(lbs * kgPerLbs);
        }

    }
}
