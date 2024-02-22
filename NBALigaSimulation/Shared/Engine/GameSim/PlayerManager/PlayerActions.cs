using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Games;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Engine.GameSim.PlayerManager;

public static class PlayerActions
{
    
    public static int PickPlayer(double[] ratios, int? exempt = null)
    {
        if (exempt.HasValue)
        {
            ratios[exempt.Value] = 0;
        }

        double sum = ratios.Sum();

        if (sum == 0)
        {
            List<int> candidates = Enumerable.Range(0, ratios.Length).Where(i => i != exempt).ToList();
            Random random = new Random();
            return candidates[random.Next(candidates.Count)];
        }

        double rand = new Random().NextDouble() * sum;

        double runningSum = 0;

        for (int i = 0; i < ratios.Length; i++)
        {
            runningSum += ratios[i];
            if (rand < runningSum)
            {
                return i;
            }
        }

        return 0;
    }
    
    public static double[] RatingArray(Game game, Team[] Teams, string rating, int t, int[][] PlayersOnCourt, double power = 1)
    {
        double[] array = new double[5];
        double total = 0;


        for (int i = 0; i < 5; i++)
        {
            int p = PlayersOnCourt[t][i];
            double compositeRating = Teams[t].Players[p].CompositeRating.Ratings[rating];

            if (rating == "Fouling")
            {
                int pf = Teams[t].Players[p].Stats.Find(s => s.GameId == game.Id).Pf;
                if (pf == 6 - 1)
                {
                    compositeRating *= 0.8;
                }
                else if (pf == 6)
                {
                    compositeRating *= 0.5;
                }
                else if (pf > 6)
                {
                    compositeRating *= 0.25;
                }
            }

            array[i] = Math.Pow(compositeRating * Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == game.Id).Energy), power);
            total += array[i];
        }

        double floor = 0.05 * total;

        for (int i = 0; i < 5; i++)
        {
            if (array[i] < floor)
            {
                array[i] = floor;
            }
        }

        return array;
    }
    
    public static void UpdatePlayingTime(Game game, Team[] Teams, int[][] PlayersOnCourt)
    {

        for (int t = 0; t < 2; t++)
        {
            for (int p = 0; p < Teams[t].Players.Count; p++)
            {
                if (PlayersOnCourt[t].Contains(p))
                {
                    RecordStat(game, t, p, "Min", Teams, 1, game.Dt);
                    RecordStat(game, t, p, "CourtTime", Teams, 1, game.Dt);
                    // Isso costumava ser 0,04. Aumente mais para diminuir o PT
                    double energyChange = -game.Dt *
                                          game.FatigueFactor *
                                          (1 - Teams[t].Players[p].CompositeRating.Ratings["Endurance"]);
                    RecordStat(game, t, p, "Energy", Teams, 1, energyChange);
                    if (Teams[t].Players[p].Stats.Find(s => s.GameId == game.Id)?.Energy < 0)
                    {
                        Teams[t].Players[p].Stats.Find(s => s.GameId == game.Id).Energy = 0;
                    }
                }
                else
                {
                    RecordStat(game, t, p, "BenchTime", Teams, 1, game.Dt);
                    RecordStat(game, t, p, "Energy", Teams, 1, (game.Dt * 0.1));
                    if (Teams[t].Players[p].Stats.Find(s => s.GameId == game.Id)?.Energy > 1)
                    {
                        Teams[t].Players[p].Stats.Find(s => s.GameId == game.Id).Energy = 1;
                    }
                }
            }
        }
    }
    
    public static void RecordStat(Game game, int t, int p, string s, Team[] teams, int amount = 1, double amntDouble = 1.0)
    {
        amount = amount != 0 ? amount : 1;
        RecordHelper.RecordStatHelperPlayer(game.GameDate, t, p, s, game.Id, teams, game.Type, game.Season.Year, amount, amntDouble);
        if (s != "Gs" && s != "CourtTime" && s != "BenchTime" && s != "Energy")
        {
            RecordHelper.RecordStatHelperTeam(t, p, s, game.Id, teams, game.Season.Year, amount);
            if (s == "Pts")
            {
                int length = game.PtsQrts[t].Count;
                if (length > 0)
                {
                    game.PtsQrts[t][length - 1] += amount;
                }
                else
                {
                    game.PtsQrts[t].Add(amount);
                }
            }

        }
    }

    public static double Fatigue(double energy)
    {
        energy += 0.05;
        if (energy > 1)
        {
            energy = 1;
        }

        return energy;
    }
    
}