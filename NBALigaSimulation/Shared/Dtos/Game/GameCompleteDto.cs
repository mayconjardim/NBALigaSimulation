using NBALigaSimulation.Shared.Models;
using System;

namespace NBALigaSimulation.Shared.Dtos
{
    public class GameCompleteDto
    {

        public int Id { get; set; }


        public TeamCompleteDto HomeTeam { get; set; }
        public TeamCompleteDto AwayTeam { get; set; }
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
        public List<string> PlayByPlay { get; set; }

        //Sim
        double synergyFactor = 0.1;
        bool startersRecorded = false;
        int subsEveryN = 6;
        int overtimes = 0;
        int t = 12;
        int o;
        int d;

        public void GameSim()
        {

            TeamCompleteDto[] team = { HomeTeam, AwayTeam };
            int numPossessions = (int)Math.Round(((98 + 101) / 2) * new Random().NextDouble() * 0.2 + 0.9 * (98 + 101) / 2);
            int[][] playersOnCourt = new int[2][] { HomeTeam.Players.Select(p => p.Id).ToArray(), AwayTeam.Players.Select(p => p.Id).ToArray() };

            UpdateTeamCompositeRatings(team, playersOnCourt);

            SimPossessions(numPossessions, playersOnCourt, team);

        }

        public void UpdateTeamCompositeRatings(TeamCompleteDto[] teams, int[][] playersOnCourt)
        {
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking" };

            for (int t = 0; t < 2; t++)
            {
                TeamCompositeDto compositeRating = teams[t].CompositeRating;
                compositeRating.CompositeRating.Clear(); // Limpa os valores existentes no dicionário

                foreach (string rating in toUpdate)
                {
                    double ratingSum = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        int p = playersOnCourt[t][i];
                        object playerRatingObj = teams[t].Players[p].GetType().GetMethod(rating)?.Invoke(teams[t].Players[p], null);
                        double playerRating = Convert.ToDouble(playerRatingObj);
                        ratingSum += playerRating;
                    }

                    double averageRating = ratingSum / 5;
                    compositeRating.CompositeRating[rating] = averageRating;
                    //double playerEnergy = Fatigue(teams[t].Players[p].Stat.Energy);
                    //ratingSum += playerRating * playerEnergy;
                }

                //compositeRating.CompositeRating["dribbling"] += synergyFactor * teams[t].Synergy.Off;
                //compositeRating.CompositeRating["passing"] += synergyFactor * teams[t].Synergy.Off;
                //compositeRating.CompositeRating["rebounding"] += synergyFactor * teams[t].Synergy.Reb;
                //compositeRating.CompositeRating["defense"] += synergyFactor * teams[t].Synergy.Def;
                //compositeRating.CompositeRating["defensePerimeter"] += synergyFactor * teams[t].Synergy.Def;
                //compositeRating.CompositeRating["blocking"] += synergyFactor * teams[t].Synergy.Def;
            }
        }

        public void SimPossessions(int numPossessions, int[][] playersOnCourt, TeamCompleteDto[] teams)
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

                string outcome = SimPossession(playersOnCourt, teams);

                // Swap o and d so that o will get another possession when they are swapped again at the beginning of the loop.
                if (outcome == "orb")
                {
                    o = (o == 1) ? 0 : 1;
                    d = (o == 1) ? 0 : 1;
                }

                //UpdatePlayingTime();

                //Injuries();
                this.PlayByPlay.Add(outcome);

                i += 1;
            }
        }

        public string SimPossession(int[][] playersOnCourt, TeamCompleteDto[] teams)
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
        }

        public double ProbTov(TeamCompleteDto[] teams)
        {
            int o = 0; // Índice da equipe ofensiva
            int d = 1; // Índice da equipe defensiva

            double defenseRating = teams[d].CompositeRating.CompositeRating["GameDefense"];
            double dribblingRating = teams[o].CompositeRating.CompositeRating["GameDribbling"];
            double passingRating = teams[o].CompositeRating.CompositeRating["GamePassing"];

            return 0.15 * (1 + defenseRating) / (1 + 0.5 * (dribblingRating + passingRating));
        }

        private string DoTov(int[][] playersOnCourt, TeamCompleteDto[] teams)
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

        private string DoStl(int[][] playersOnCourt, TeamCompleteDto[] teams)
        {
            int d = this.d; // Index of the defensive team

            double[] ratios = RatingArray(playersOnCourt, teams, "GameStealing", d);
            int p = playersOnCourt[d][PickPlayer(ratios)];
            //gameSim.RecordStat(d, p, "stl");

            return "stl";
        }

        private double[] RatingArray(int[][] playersOnCourt, TeamCompleteDto[] teams, string methodName, int t, double power = 1)
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

        private double ProbStl(TeamCompleteDto[] teams)
        {
            int o = 0; // Index of the offensive team
            int d = 1; // Index of the defensive team

            double defensePerimeterRating = teams[d].CompositeRating.CompositeRating["GameDefensePerimeter"];
            double dribblingRating = teams[o].CompositeRating.CompositeRating["GameDribbling"];
            double passingRating = teams[o].CompositeRating.CompositeRating["GamePassing"];

            return 0.55 * defensePerimeterRating / (0.5 * (dribblingRating + passingRating));
        }

        public string DoShot(int shooter, int[][] playersOnCourt, TeamCompleteDto[] teams)
        {

            int o = this.o; // Index of the offensive team

            int p = playersOnCourt[o][shooter];

            return teams[o].Players[p].FullName;
        }


    }
}
