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
                weights = Enumerable.Repeat(1.0, components.Count).ToList();
            }

            double numerator = 0;
            double denominator = 0;
            double factor = 0;
            bool fuzz = false;

            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];

                if (component is string)
                {
                    string ratingName = component;
                    if (!ratings.ContainsKey(ratingName))
                    {
                        throw new Exception($"Undefined value for rating \"{ratingName}\"");
                    }

                    double rating = ratings[ratingName];

                    if (fuzz)
                    {
                        factor = rating;
                    }
                    else
                    {
                        factor = rating;
                    }
                }
                else
                {
                    throw new Exception("Invalid component type.");
                }

                numerator += factor * weights[i];
                denominator += 100 * weights[i];
            }

            return RandomUtils.Bound(numerator / denominator, 0, 1);
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
