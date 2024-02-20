using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.DefenseManager;

public static class DefenseActions
{
    
    public static string DoBlk(Game game, int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
    {
        int p = PlayersOnCourt[game.Offense][shooter];

        double[] blockingRatios = RatingArray(Teams, "Blocking", game.Defense, PlayersOnCourt, 10);
        int p2 = PlayersOnCourt[game.Defense][PickPlayer(blockingRatios)];

        RecordStat(game.Offense, p, "Fga", Teams);

        if (type == "AtRim")
        {
            RecordStat(game.Offense, p, "FgaAtRim", Teams);

            string[] names = { Teams[game.Defense].Players[p2].Name, Teams[game.Offense].Players[p].Name };
        }
        else if (type == "LowPost")
        {
            RecordStat(game.Offense, p, "FgaLowPost", Teams);

            string[] names = { Teams[game.Defense].Players[p2].Name, Teams[game.Offense].Players[p].Name };
        }
        else if (type == "MidRange")
        {
            RecordStat(game.Offense, p, "FgaMidRange", Teams);

            string[] names = { Teams[game.Defense].Players[p2].Name, Teams[game.Offense].Players[p].Name };
        }
        else if (type == "ThreePointer")
        {
            RecordStat(game.Offense, p, "Tpa", Teams);

            string[] names = { Teams[game.Defense].Players[p2].Name, Teams[game.Offense].Players[p].Name };
        }

        RecordStat(game.Defense, p2, "Blk", Teams);

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
            ratios = RatingArray(Teams, "Rebounding", game.Defense, PlayersOnCourt, 10);
            p = PlayersOnCourt[game.Defense][PickPlayer(ratios)];
            RecordStat(game.Defense, p, "Drb", Teams);
            RecordStat(game.Defense, p, "Trb", Teams);

            string[] name = { Teams[game.Defense].Players[p].Name };

            return "Drb";
        }

        ratios = RatingArray(Teams, "Rebounding", game.Offense, PlayersOnCourt, 5);
        int oP = PlayersOnCourt[game.Offense][PickPlayer(ratios)];
        RecordStat(Offense, oP, "Orb", Teams);
        RecordStat(Offense, oP, "Trb", Teams);

        string[] names = { Teams[game.Offense].Players[oP].Name };

        return "Orb";
    }
    
    public static void DoPf(Game game, int t, Team[] Teams, int[][] PlayersOnCourt)
    {

        double[] ratios = RatingArray(Teams, "Fouling", t, PlayersOnCourt, 2);
        int p = PlayersOnCourt[t][PickPlayer(ratios)];
        RecordStat(game.Defense, p, "Pf", Teams);

        string[] names = { Teams[game.Defense].Players[p].Name };

        var player = Teams[game.Defense].Players.Find(player => player.RosterOrder == p);

        if (player.Stats.Find(s => s.GameId == game.Id).Pf >= 6)
        {
            UpdatePlayersOnCourt(Teams, PlayersOnCourt);
            UpdateSynergy(Teams, PlayersOnCourt);
        }
    }
    
    public static string DoStl(Game game,int pStoleFrom, Team[] Teams, int[][] PlayersOnCourt)
    {
        double[] ratios = RatingArray(Teams, "Stealing", game.Defense, PlayersOnCourt, 4);
        int playerIndex = PickPlayer(ratios);
        var p = PlayersOnCourt[game.Defense][playerIndex];

        RecordStat(game.Defense, p, "Stl", Teams);

        return "Stl";
    }

    
}