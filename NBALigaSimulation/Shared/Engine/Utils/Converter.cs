using System.ComponentModel;

namespace NBALigaSimulation.Shared.Engine
{
    public static class Converter
    {

        public static double Composite2(Dictionary<string, double> ratings, List<string> components, List<double>? weights = null)
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

        public static double Composite(Dictionary<string, double> ratings, List<string> components, List<double>? weights = null)
        {
            if (weights == null)
            {
                // Default: array of ones with the same size as components
                weights = new List<double>();
                for (int i = 0; i < components.Count; i++)
                {
                    weights.Add(1);
                }
            }

            double r = 0;
            double divideBy = 0;
            for (int i = 0; i < components.Count; i++)
            {
                double factor = 0;

                if (components[i] is string)
                {
                    // If the component is a string, fetch the value from the rating dictionary
                    string componentKey = (string)components[i];
                    if (ratings.ContainsKey(componentKey))
                    {
                        factor = ratings[componentKey];
                    }
                }
                else if (components[i] is double)
                {
                    // If the component is a double, use it directly as the factor
                    string component = components[i];
                    factor = (double)ratings[component];
                }

                // Special case for height due to rescaling
                if (components[i] == "hgt")
                {
                    factor = (factor - 25) * 2;
                }

                // Apply weights to the factor
                double rcomp = weights[i] * factor;

                r += rcomp;

                divideBy += 100 * weights[i];
            }

            r /= divideBy; // Scale from 0 to 1
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
