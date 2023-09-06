namespace NBALigaSimulation.Shared.Engine
{
    public static class Converter
    {

        public static decimal Composite(Dictionary<string, decimal> ratings, List<string> components, List<decimal>? weights = null)
        {
            decimal r = 0;
            decimal rmax = 0;
            decimal divideBy = 0;

            if (weights == null)
            {
                weights = new List<decimal>();
                for (int i = 0; i < components.Count; i++)
                {
                    weights.Add(1);
                }
            }

            for (int i = 0; i < components.Count; i++)
            {
                string component = components[i];
                decimal rcomp = weights[i] * ratings[component];

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

        public static bool hasSkill(List<int> skills, List<decimal> weights)
        {

            decimal numerator = 0;
            decimal denominator = 0;

            for (int i = 0; i < skills.Count; i++)
            {
                numerator += skills[i] * weights[i];
                denominator += 100 * weights[i];
            }

            if (numerator / denominator > 0.75m)
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
