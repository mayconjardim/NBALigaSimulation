using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine
{
    public static class OffenseHelper
    {

        public static double ProbAst(Team[] teams, int Offense, int Defense)
        {
            return 0.7 * (2 + teams[Offense].CompositeRating.Ratings["GamePassing"]) / (2 + teams[Defense].CompositeRating.Ratings["GameDefense"]);
        }


    }
}
