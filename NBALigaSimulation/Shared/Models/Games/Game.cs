using System.ComponentModel.DataAnnotations.Schema;
using NBALigaSimulation.Shared.Engine.Gameplan;
using NBALigaSimulation.Shared.Engine.GameSim.ProbabilityManager;
using NBALigaSimulation.Shared.Engine.Utils;
using NBALigaSimulation.Shared.Models.Players;
using NBALigaSimulation.Shared.Models.Seasons;
using NBALigaSimulation.Shared.Models.Teams;

namespace NBALigaSimulation.Shared.Models.Games
{
    public class Game
    {
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public Season Season { get; set; }
        public int Type { get; set; }
        public int HomeTeamId { get; set; }
        [ForeignKey("HomeTeamId")]
        public Team? HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        [ForeignKey("AwayTeamId")]
        public Team AwayTeam { get; set; }
        public bool Happened { get; set; } = false;
        public DateTime GameDate { get; set; }
        public List<TeamGameStats> TeamGameStats { get; set; } = new List<TeamGameStats>();
        public List<PlayerGameStats> PlayerGameStats { get; set; } = new List<PlayerGameStats>();

        // Atributos Globais da Simulação
        [NotMapped]
        public int NumPossessions; // Quantidade posses de uma partida
        [NotMapped]
        public bool StartersRecorded = false; // Usado para rastrear se os titulares *reais* foram gravados ou não.
        [NotMapped]
        public int SubsEveryN = 6; // Quantas posses esperar antes de fazer substituições
        [NotMapped]
        public int Overtimes = 0; // Números de overtimes 
        [NotMapped]
        public double SynergyFactor = 0.01; // Qual a importância da sinergia?
        [NotMapped]
        public double FatigueFactor = 0.055;
        [NotMapped]
        public int Offense; // Time que está atacando
        [NotMapped]
        public int Defense; // Time que está defendendo
        [NotMapped]
        public double T = 12.00; // Tempo por quarto
        [NotMapped]
        public double Dt = 0; // Tempo decorrido por posse
        [NotMapped]
        public List<List<int>> PtsQrts = new List<List<int>> { new List<int>(), new List<int>() };

      

   

        public void UpdatePlayingTime(Team[] Teams, int[][] PlayersOnCourt)
        {

            for (int t = 0; t < 2; t++)
            {
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    if (PlayersOnCourt[t].Contains(p))
                    {
                        RecordStat(t, p, "Min", Teams, 1, Dt);
                        RecordStat(t, p, "CourtTime", Teams, 1, Dt);
                        // Isso costumava ser 0,04. Aumente mais para diminuir o PT
                        double energyChange = -Dt *
                                   FatigueFactor *
                                   (1 - Teams[t].Players[p].CompositeRating.Ratings["Endurance"]);
                        RecordStat(t, p, "Energy", Teams, 1, energyChange);
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy < 0)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 0;
                        }
                    }
                    else
                    {
                        RecordStat(t, p, "BenchTime", Teams, 1, Dt);
                        RecordStat(t, p, "Energy", Teams, 1, (Dt * 0.1));
                        if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy > 1)
                        {
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy = 1;
                        }
                    }
                }
            }
        }

 


  
        
        
        
        
    

       

       
        
        public double[] RatingArray(Team[] Teams, string rating, int t, int[][] PlayersOnCourt, double power = 1)
        {
            double[] array = new double[5];
            double total = 0;


            for (int i = 0; i < 5; i++)
            {
                int p = PlayersOnCourt[t][i];
                double compositeRating = Teams[t].Players[p].CompositeRating.Ratings[rating];

                if (rating == "Fouling")
                {
                    int pf = Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf;
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

                array[i] = Math.Pow(compositeRating * Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy), power);
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


        public void RecordStat(int t, int p, string s, Team[] teams, int amount = 1, double amntDouble = 1.0)
        {
            amount = amount != 0 ? amount : 1;
            RecordHelper.RecordStatHelperPlayer(GameDate, t, p, s, Id, teams, Type, Season.Year, amount, amntDouble);
            if (s != "Gs" && s != "CourtTime" && s != "BenchTime" && s != "Energy")
            {
                RecordHelper.RecordStatHelperTeam(t, p, s, Id, teams, Season.Year, amount);
                if (s == "Pts")
                {
                    int length = PtsQrts[t].Count;
                    if (length > 0)
                    {
                        PtsQrts[t][length - 1] += amount;
                    }
                    else
                    {
                        PtsQrts[t].Add(amount);
                    }
                }

            }
        }

        public double Fatigue(double energy)
        {
            energy += 0.05;
            if (energy > 1)
            {
                energy = 1;
            }

            return energy;
        }

        public int PickPlayer(double[] ratios, int? exempt = null)
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

    }

}