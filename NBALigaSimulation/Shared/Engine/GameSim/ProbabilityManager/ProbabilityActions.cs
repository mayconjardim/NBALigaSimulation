using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.ProbabilityManager;

public static class ProbabilityActions
{
    
    public static double ProbTov(Game game, Team[] Teams)
    {
        double defenseRating = Teams[game.Defense].CompositeRating.Ratings["GameDefense"];
        double offenseRating = Teams[game.Offense].CompositeRating.Ratings["GameDribbling"] 
                               + Teams[game.Offense].CompositeRating.Ratings["GamePassing"];
        
        return 0.15 * (1 + defenseRating) / (1 + 0.5 * offenseRating)  ;
    }
    
    
    public static double ProbStl(Game game, Team[] Teams)
    {
        double defensePerimeter = Teams[game.Defense].CompositeRating.Ratings["GameDefensePerimeter"];
        double offenseRating = Teams[game.Offense].CompositeRating.Ratings["GameDribbling"] +
                               Teams[game.Offense].CompositeRating.Ratings["GamePassing"];
        return 0.55 * defensePerimeter / (0.5 * offenseRating);
    }
    
    public static double ProbBlk(Game game, Team[] Teams)
    {
        return 1 * 0.5 * Math.Pow(Teams[game.Defense].CompositeRating.Ratings["GameBlocking"], 2);
    }
    
    public static double ProbAst(Game game, Team[] Teams)
    {
        double numerator = (0.7 * (2 + Teams[game.Offense].CompositeRating.Ratings["GamePassing"]));
        double denominator = (2 + Teams[game.Defense].CompositeRating.Ratings["GameDefense"]);
        return (numerator / denominator) * 1;
    }

    public static double BoundProb(double prob)
    {
        double boundedProb = RandomUtils.Bound(prob, 0.001, 0.999);
        return boundedProb;
    }
    
}