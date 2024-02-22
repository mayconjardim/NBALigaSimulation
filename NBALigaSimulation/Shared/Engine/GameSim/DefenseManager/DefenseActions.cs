using NBALigaSimulation.Shared.Engine.GameSim.CourtManager;
using NBALigaSimulation.Shared.Engine.GameSim.PlayerManager;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.DefenseManager;

public static class DefenseActions
{
    
    public static string DoBlk(Game game, int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
    {
        int p = PlayersOnCourt[game.Offense][shooter];

        double[] blockingRatios = PlayerActions.RatingArray(game, Teams, "Blocking", game.Defense, PlayersOnCourt, 10);
        int p2 = PlayersOnCourt[game.Defense][PlayerActions.PickPlayer(blockingRatios)];

        PlayerActions.RecordStat(game, game.Offense, p, "Fga", Teams);

        if (type == "AtRim")
        {
            PlayerActions.RecordStat(game, game.Offense, p, "FgaAtRim", Teams);
        }
        else if (type == "LowPost")
        {
            PlayerActions.RecordStat(game, game.Offense, p, "FgaLowPost", Teams);
        }
        else if (type == "MidRange")
        {
            PlayerActions.RecordStat(game, game.Offense, p, "FgaMidRange", Teams);
        }
        else if (type == "ThreePointer")
        {
            PlayerActions.RecordStat(game, game.Offense, p, "Tpa", Teams);
        }

        PlayerActions.RecordStat(game, game.Defense, p2, "Blk", Teams);

        return DoReb(game, Teams, PlayersOnCourt);
    }
    
    public static string DoReb(Game game, Team[] Teams, int[][] PlayersOnCourt)
    {
        int p;
        double[] ratios;

        if (new Random().NextDouble() < 0.15)
        {
            return null;
        }
        
        double defensiveReboundChance = (0.75 * (2 + Teams[game.Defense].CompositeRating.Ratings["GameRebounding"])) /
                                        (1 * (2 + Teams[game.Offense].CompositeRating.Ratings["GameRebounding"]));

        if (defensiveReboundChance > new Random().NextDouble())
        {
            ratios = PlayerActions.RatingArray(game, Teams, "Rebounding", game.Defense, PlayersOnCourt, 10);
            p = PlayersOnCourt[game.Defense][PlayerActions.PickPlayer(ratios)];
            PlayerActions.RecordStat(game, game.Defense, p, "Drb", Teams);
            PlayerActions.RecordStat(game, game.Defense, p, "Trb", Teams);

            string[] name = { Teams[game.Defense].Players[p].Name };

            return "Drb";
        }

        ratios = PlayerActions.RatingArray(game, Teams, "Rebounding", game.Offense, PlayersOnCourt, 5);
        int oP = PlayersOnCourt[game.Offense][PlayerActions.PickPlayer(ratios)];
        PlayerActions.RecordStat(game, game.Offense, oP, "Orb", Teams);
        PlayerActions.RecordStat(game, game.Offense, oP, "Trb", Teams);

        string[] names = { Teams[game.Offense].Players[oP].Name };

        return "Orb";
    }
    
    public static void DoPf(Game game, int t, Team[] Teams, int[][] PlayersOnCourt)
    {

        double[] ratios = PlayerActions.RatingArray(game, Teams, "Fouling", t, PlayersOnCourt, 2);
        int p = PlayersOnCourt[t][PlayerActions.PickPlayer(ratios)];
        PlayerActions.RecordStat(game, game.Defense, p, "Pf", Teams);

        string[] names = { Teams[game.Defense].Players[p].Name };

        var player = Teams[game.Defense].Players.Find(player => player.RosterOrder == p);

        if (player.Stats.Find(s => s.GameId == game.Id).Pf >= 6)
        {
            CourtActions.UpdatePlayersOnCourt(game, Teams, PlayersOnCourt);
            CourtActions.UpdateSynergy(game, Teams, PlayersOnCourt);
        }
    }
    
    public static string DoStl(Game game,int pStoleFrom, Team[] Teams, int[][] PlayersOnCourt)
    {
        double[] ratios = PlayerActions.RatingArray(game, Teams, "Stealing", game.Defense, PlayersOnCourt, 4);
        int playerIndex = PlayerActions.PickPlayer(ratios);
        var p = PlayersOnCourt[game.Defense][playerIndex];

        PlayerActions.RecordStat(game, game.Defense, p, "Stl", Teams);

        return "Stl";
    }

    
}