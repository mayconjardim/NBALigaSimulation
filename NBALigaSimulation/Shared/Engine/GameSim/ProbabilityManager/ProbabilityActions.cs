using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.ProbabilityManager;

public static class ProbabilityActions
{
    
    public static double ProbTov(Game game, Team[] Teams)
    {
        double turnoverFactor = 1;
        double defenseRating = 0.14 * Teams[game.Defense].CompositeRating.Ratings["GameDefense"];
        double dribblingRating = Teams[game.Offense].CompositeRating.Ratings["GameDribbling"];
        double passingRating = Teams[game.Offense].CompositeRating.Ratings["GamePassing"];

        double probability = turnoverFactor * (defenseRating) / (0.5 * (dribblingRating + passingRating));

        return BoundProb(probability);
    }

    public static double ProbStl(Game game, Team[] Teams)
    {
        double stealFactor = 1.09;
        double defensePerimeter = Teams[game.Defense].CompositeRating.Ratings["GameDefensePerimeter"];
        double dribbling = Teams[game.Offense].CompositeRating.Ratings["GameDribbling"];
        double passing = Teams[game.Offense].CompositeRating.Ratings["GamePassing"];

        double probability = stealFactor * ((0.45 * defensePerimeter) / (0.5 * (dribbling + passing)));

        return BoundProb(probability);
    }
    
    public static double ProbBlk(Game game, Team[] Teams)
    {
        return 1 * 0.5 * Math.Pow(Teams[game.Defense].CompositeRating.Ratings["GameBlocking"], 2);
    }
    
    public static double ProbAst(Game game, Team[] Teams)
    {
        double numerator = (0.9 * (2 + Teams[game.Offense].CompositeRating.Ratings["GamePassing"]) +
                            GameplanUtils.GameplanMotion(Teams[game.Offense].Gameplan.Pace));
        double denominator = (2 + Teams[game.Defense].CompositeRating.Ratings["GameDefense"]);

        return (numerator / denominator) * 1;
    }

    public static double BoundProb(double prob)
    {
        double boundedProb = RandomUtils.Bound(prob, 0.001, 0.999);
        return boundedProb;
    }
    
}