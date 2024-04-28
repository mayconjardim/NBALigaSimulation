using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.ProbabilityManager;

public static class ProbabilityActions
{
    
    public static double ProbTov(Game game, Team[] Teams)
    {
        double motionGp = GameplanUtils.GameplanMotion(Teams[game.Offense].Gameplan.Motion);
        double defenseGp = GameplanUtils.GameplanDefense(Teams[game.Defense].Gameplan.Defense);

        double offenseValue = 0;
        double defenseValue = 0;
        
        //Aumentar/Diminuir prob depdendo do GP Ataque
        if (Teams[game.Offense].Gameplan.Pace > 1)
        {
            if (Teams[game.Offense].Gameplan.Motion == 1)
            {
                offenseValue = motionGp;
            }
            if (Teams[game.Offense].Gameplan.Motion == 3)
            {
                offenseValue += motionGp;
            }
        }
        //Aumentar/Diminuir prob depdendo do GP Defesa
        var defense = Teams[game.Defense].Gameplan.Defense;
        if (defense is 2 or 3)
        {
            defenseValue = defenseGp;
        }

        double defenseRating = Teams[game.Defense].CompositeRating.Ratings["GameDefense"] + defenseValue;
        
        double offenseRating = Teams[game.Offense].CompositeRating.Ratings["GameDribbling"] 
                               + Teams[game.Offense].CompositeRating.Ratings["GamePassing"] + offenseValue;

        double result = 0.15 * (1 + defenseRating) / (1 + 0.5 * offenseRating);
        
        return result;     
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