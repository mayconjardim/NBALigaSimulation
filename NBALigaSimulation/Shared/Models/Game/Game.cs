using NBALigaSimulation.Shared.Dtos;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

namespace NBALigaSimulation.Shared.Models
{
    public class Game
    {

        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        [ForeignKey("HomeTeamId")]
        public Team HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        [ForeignKey("AwayTeamId")]
        public Team AwayTeam { get; set; }
        public int HomeQ1 { get; set; }
        public int HomeQ2 { get; set; }
        public int HomeQ3 { get; set; }
        public int HomeQ4 { get; set; }
        public int HomeOT { get; set; }
        public int AwayQ1 { get; set; }
        public int AwayQ2 { get; set; }
        public int AwayQ3 { get; set; }
        public int AwayQ4 { get; set; }
        public int AwayOT { get; set; }
        public ICollection<GamePlayByPlay> PlayByPlays { get; set; } = new HashSet<GamePlayByPlay>();

        //Sim
        [NotMapped]
        double synergyFactor = 0.1;
        [NotMapped]
        bool startersRecorded = false;
        [NotMapped]
        int subsEveryN = 6;
        [NotMapped]
        int overtimes = 0;
        [NotMapped]
        int t = 12;
        [NotMapped]
        int o;
        [NotMapped]
        int d;

        public delegate void MetodoInternoDelegate();
        public delegate string MetodoInternoString();


        public void GameSim()
        {

            Team[] teams = { HomeTeam, AwayTeam };
            teams[0].CompositeRating = new TeamCompositeRating();
            teams[1].CompositeRating = new TeamCompositeRating();
            int numPossessions = (int)Math.Round(((98 + 101) / 2) * new Random().NextDouble() * 0.2 + 0.9 * (98 + 101) / 2);
            int[][] playersOnCourt = new int[2][] { HomeTeam.Players.Select(p => p.Id).ToArray(), AwayTeam.Players.Select(p => p.Id).ToArray() };

            UpdateCompositeRating(teams);








            /*

           MetodoInternoString SimPossession = () =>
           {

               Random random = new Random();

               // Turnover?
               if (ProbTov(teams) > random.NextDouble())
               {
                   return DoTov(playersOnCourt, teams); // tov
               }

               // Shot if there is no turnover
               double[] ratios = RatingArray(playersOnCourt, teams, "GameUsage", o);
               int shooter = PickPlayer(ratios);

               return DoShot(shooter, playersOnCourt, teams); // fg, orb, or drb

           };


           MetodoInternoDelegate SimPossessions = () =>
           {

               int i = 0;
               while (i < numPossessions * 2)
               {
                   // Possession change
                   o = (o == 1) ? 0 : 1;
                   d = (o == 1) ? 0 : 1;

                   //if (i % subsEveryN == 0)
                   //{
                   //    bool substitutions = UpdatePlayersOnCourt();
                   //    if (substitutions)
                   //    {
                   //        UpdateSynergy();
                   //    }
                   //}

                   //UpdateTeamCompositeRatings(teams, playersOnCourt);

                   string outcome = SimPossession();

                   // Swap o and d so that o will get another possession when they are swapped again at the beginning of the loop.
                   if (outcome == "orb")
                   {
                       o = (o == 1) ? 0 : 1;
                       d = (o == 1) ? 0 : 1;
                   }

                   //UpdatePlayingTime();

                   //Injuries();
                   this.PlayByPlays.Add(new GamePlayByPlay
                   {
                       Play = outcome
                   });

                   i += 1;
               }

           };


           UpdateCompositeRating.Invoke();
           SimPossession.Invoke();
           */


        }

        public void UpdateCompositeRating(Team[] teams)
        {
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking" };

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 5; j++)
                {

                    var playerRatings = teams[i].Players[j].Ratings.LastOrDefault();
                    double ratingValue = 0;

                    foreach (string rating in toUpdate)
                    {
                        teams[i].CompositeRating.Ratings[rating] = 0;

                        if (rating == "GameDribbling")
                        {
                            ratingValue = playerRatings.GameDribbling;
                        }
                        else if (rating == "GamePassing")
                        {
                            ratingValue = playerRatings.GamePassing;
                        }
                        else if (rating == "GameRebounding")
                        {
                            ratingValue = playerRatings.GameRebounding;
                        }
                        else if (rating == "GameDefense")
                        {
                            ratingValue = playerRatings.GameDefense;
                        }
                        else if (rating == "GameDefensePerimeter")
                        {
                            ratingValue = playerRatings.GameDefensePerimeter;
                        }
                        else if (rating == "GameBlocking")
                        {
                            ratingValue = playerRatings.GameBlocking;
                        }

                        teams[i].CompositeRating.Ratings[rating] += ratingValue;
                    }
                }
            }

            foreach (var kvp in teams[0].CompositeRating.Ratings)
            {
                string ratings = kvp.Key;
                double value = kvp.Value;
                Console.WriteLine($"{ratings}: {value}");
            }

            Console.WriteLine("--------------");

            foreach (var kvp in teams[1].CompositeRating.Ratings)
            {
                string ratings = kvp.Key;
                double value = kvp.Value;
                Console.WriteLine($"{ratings}: {value}");
            }
        }

        private double GetPlayerRating(Player player, string rating)
        {
            PropertyInfo ratingProperty = player.Ratings.GetType().GetProperty(rating);

            if (ratingProperty != null)
            {
                object ratingValue = ratingProperty.GetValue(player.Ratings);

                if (ratingValue != null && double.TryParse(ratingValue.ToString(), out double parsedRating))
                {
                    return parsedRating;
                }
            }

            return 0; // Valor padrão caso não seja possível obter a pontuação do jogador
        }


        public double ProbTov(Team[] teams)
        {
            int o = 0; // Índice da equipe ofensiva
            int d = 1; // Índice da equipe defensiva

            double defenseRating = teams[d].CompositeRating.Ratings["GameDefense"];
            double dribblingRating = teams[o].CompositeRating.Ratings["GameDribbling"];
            double passingRating = teams[o].CompositeRating.Ratings["GamePassing"];

            return 0.15 * (1 + defenseRating) / (1 + 0.5 * (dribblingRating + passingRating));
        }

        private string DoTov(int[][] playersOnCourt, Team[] teams)
        {
            Random random = new Random();

            int o = this.o; // Índice da equipe ofensiva
            int p;
            double[] ratios;

            ratios = RatingArray(playersOnCourt, teams, "GameTurnovers", o, 0.5);
            p = playersOnCourt[o][PickPlayer(ratios)];
            // RecordStat(o, p, "tov");

            if (ProbStl(teams) > random.NextDouble())
            {
                return DoStl(playersOnCourt, teams); // "stl"
            }

            return "tov";
        }

        private string DoStl(int[][] playersOnCourt, Team[] teams)
        {
            int d = this.d; // Index of the defensive team

            double[] ratios = RatingArray(playersOnCourt, teams, "GameStealing", d);
            int p = playersOnCourt[d][PickPlayer(ratios)];
            //gameSim.RecordStat(d, p, "stl");

            return "stl";
        }

        private double[] RatingArray(int[][] playersOnCourt, Team[] teams, string methodName, int t, double power = 1)
        {
            double[] array = new double[5];
            for (int i = 0; i < 5; i++)
            {
                int p = playersOnCourt[t][i];
                double playerRating = 0;
                var method = typeof(PlayerRatingDto).GetMethod(methodName);
                if (method != null)
                {
                    var playerRatings = teams[t].Players[p].Ratings;
                    var ratingValue = method.Invoke(playerRatings, null);
                    if (ratingValue != null && double.TryParse(ratingValue.ToString(), out var parsedRating))
                    {
                        playerRating = parsedRating;
                    }
                }
                array[i] = Math.Pow(playerRating, power);
            }

            return array;
        }

        private int PickPlayer(double[] ratios, int exempt = -1)
        {
            if (exempt != -1)
            {
                ratios[exempt] = 0;
            }

            double totalRatio = ratios.Sum();
            double rand = new Random().NextDouble() * totalRatio;
            double cumulativeRatio = 0;

            for (int i = 0; i < ratios.Length; i++)
            {
                cumulativeRatio += ratios[i];
                if (rand < cumulativeRatio)
                {
                    return i;
                }
            }

            return ratios.Length - 1;
        }

        private double ProbStl(Team[] teams)
        {
            int o = 0; // Index of the offensive team
            int d = 1; // Index of the defensive team

            double defensePerimeterRating = teams[d].CompositeRating.Ratings["GameDefensePerimeter"];
            double dribblingRating = teams[o].CompositeRating.Ratings["GameDribbling"];
            double passingRating = teams[o].CompositeRating.Ratings["GamePassing"];

            return 0.55 * defensePerimeterRating / (0.5 * (dribblingRating + passingRating));
        }

        public string DoShot(int shooter, int[][] playersOnCourt, Team[] teams)
        {

            int o = this.o; // Index of the offensive team

            int p = playersOnCourt[o][shooter];

            return teams[o].Players[p].LastName;
        }


    }
}
