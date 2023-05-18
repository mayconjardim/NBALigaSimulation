using NBALigaSimulation.Shared.Models;

namespace NBALigaSimulation.Shared.Engine
{
    public static class DefenseHelper
    {
        public static double ProbTov(Team[] teams)
        {
            int o = 0;
            int d = 1;

            double defenseRating = teams[d].CompositeRating.Ratings["GameDefense"];
            double dribblingRating = teams[o].CompositeRating.Ratings["GameDribbling"];
            double passingRating = teams[o].CompositeRating.Ratings["GamePassing"];

            return 0.13 * (1 + defenseRating) / (1 + 0.5 * (dribblingRating + passingRating));
        }

        public static double ProbStl(Team[] teams)
        {
            int o = 0; // Index of the offensive team
            int d = 1; // Index of the defensive team

            double defensePerimeterRating = teams[d].CompositeRating.Ratings["GameDefensePerimeter"];
            double dribblingRating = teams[o].CompositeRating.Ratings["GameDribbling"];
            double passingRating = teams[o].CompositeRating.Ratings["GamePassing"];

            return 0.55 * defensePerimeterRating / (0.5 * (dribblingRating + passingRating));
        }

        public static double ProbBlk(Team[] teams, int defense)
        {
            return 0.1 * teams[defense].CompositeRating.Ratings["GameBlocking"];
        }

    }
}
