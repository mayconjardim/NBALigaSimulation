namespace NBALigaSimulation.Shared.Models
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

        public static double Composite(Dictionary<string, double> ratings, List<string> components, List<double>? weights = null)
        {
            double r = 0;
            double rmax = 0;
            double divideBy = 0;

            if (weights == null)
            {
                weights = new List<double>();
                for (int i = 0; i < components.Count; i++)
                {
                    weights.Add(1);
                }
            }

            for (int i = 0; i < components.Count; i++)
            {
                string component = components[i];
                double rcomp = weights[i] * ratings[component];

                r += rcomp;
                divideBy += 100 * weights[i];
            }

            r /= divideBy;
            if (r > 1)
            {
                r = 1;
            }
            else if (r < 0)
            {
                r = 0;
            }

            return r;
        }
        public static bool hasSkill(List<int> skills, List<double> weights)
        {

            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < skills.Count; i++)
            {
                numerator += skills[i] * weights[i];
                denominator += 100 * weights[i];
            }

            if (numerator / denominator > 0.75)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

    }
}
