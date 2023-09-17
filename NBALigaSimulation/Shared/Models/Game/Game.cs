using NBALigaSimulation.Shared.Engine;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace NBALigaSimulation.Shared.Models
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
        public List<GamePlayByPlay> PlayByPlay { get; set; } = new List<GamePlayByPlay>();

        // Atributos Globais da Simulação
        [NotMapped]
        int NumPossessions; // Quantidade posses de uma partida
        [NotMapped]
        bool StartersRecorded = false; // Usado para rastrear se os titulares *reais* foram gravados ou não.
        [NotMapped]
        int SubsEveryN = 6; // Quantas posses esperar antes de fazer substituições
        [NotMapped]
        int Overtimes = 0; // Números de overtimes 
        [NotMapped]
        double SynergyFactor = 0.01; // Qual a importância da sinergia?
        [NotMapped]
        int Offense; // Time que está atacando
        [NotMapped]
        int Defense; // Time que está defendendo
        [NotMapped]
        double T = 12.00; // Tempo por quarto
        [NotMapped]
        double Dt = 0; // Tempo decorrido por posse
        [NotMapped]
        List<List<int>> PtsQtrs = new List<List<int>>();

        public void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            CompositeHelper.UpdatePlayersCompositeRating(Teams);
            CompositeHelper.UpdatePace(Teams);

            NumPossessions = (int)Math.Round((Teams[0].CompositeRating.Ratings["GamePace"] +
                Teams[1].CompositeRating.Ratings["GamePace"]) / 2 * RandomUtils.RandomUniform(0.9, 1.1));

            Dt = 48.0 / (2 * NumPossessions);

            int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

            UpdatePlayersOnCourt(Teams, PlayersOnCourt);
            UpdateSynergy(Teams, PlayersOnCourt);
            HomeCourtAdvantage(Teams, PlayersOnCourt);


            SimRegulation(Teams, PlayersOnCourt);

            // Jogue períodos de prorrogação se necessário
            while (Teams[0].Stats.Find(s => s.GameId == Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == Id)?.Pts)
            {
                SimOvertime(Teams, PlayersOnCourt);
            }

        }

        private void SimRegulation(Team[] Teams, int[][] PlayersOnCourt)
        {
            Offense = 0;
            Defense = 1;
            int quarter = 1;

            while (true)
            {
                while (T > 0)
                {
                    SimPossession(Teams, PlayersOnCourt);
                }
                quarter += 1;

                if (quarter == 5)
                {
                    break;
                }
            }
        }

        private void SimOvertime(Team[] Teams, int[][] PlayersOnCourt)
        {
            T = (int)Math.Ceiling(0.4 * 5);
            Overtimes += 1;

            Offense = (new Random().NextDouble() < 0.5) ? 0 : 1;
            Defense = (Offense == 0) ? 1 : 0;

            while (T > 0)
            {
                SimPossession(Teams, PlayersOnCourt);
            }
        }

        private void SimPossession(Team[] Teams, int[][] PlayersOnCourt)
        {
            T -= Dt;
            double possessionTime = Dt;

            if (T < 0)
            {
                possessionTime += T;
                T = 0;
            }

            Offense = (Offense == 1) ? 0 : 1;
            Defense = (Offense == 1) ? 0 : 1;

            UpdateTeamCompositeRatings(Teams, PlayersOnCourt);

            string outcome = GetPossessionOutcome(Teams, PlayersOnCourt);

            if (outcome == "Orb")
            {
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Offense == 1) ? 0 : 1;
            }

            UpdatePlayingTime(possessionTime);

            //Injuries();

            if (RandomUtils.RandInt(1, SubsEveryN + 1) == 1)
            {
                bool substitutions = UpdatePlayersOnCourt(Teams, PlayersOnCourt);

                if (substitutions)
                {
                    UpdateSynergy(Teams, PlayersOnCourt);
                }
            }
        }

        private bool UpdatePlayersOnCourt(Team[] teams, int[][] PlayersOnCourt)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
                double[] ovrs = new double[teams[t].Players.Count];

                for (int p = 0; p < teams[t].Players.Count; p++)
                {

                    var player = teams[t].Players.Find(player => player.RosterOrder == p);
                    if (player != null)
                    {
                        if (player.Stats == null)
                        {
                            player.Stats = new List<PlayerGameStats>();
                        }

                        var lastStats = player.Stats.Find(s => s.GameId == Id);

                        if (lastStats == null || lastStats.GameId != Id)
                        {
                            player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = teams[t].Id, Name = player.Name, Season = Season.Year });
                        }
                    }

                    if (teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().CalculateOvr * Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                              teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9, 1.1);
                    }
                }
                int i = 0;
                for (int pp = 0; pp < PlayersOnCourt[t].Length; pp++)
                {
                    int p = PlayersOnCourt[t][pp];
                    PlayersOnCourt[t][i] = p;

                    for (int b = 0; b < teams[t].Players.Count; b++)
                    {
                        if (!PlayersOnCourt[t].Contains(b) && ((teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime > 3
                                && teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime > 3
                                && ovrs[b] > ovrs[p]) || teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6))
                        {
                            List<string> pos = new List<string>();
                            for (int j = 0; j < PlayersOnCourt[t].Length; j++)
                            {
                                if (j != pp)
                                {
                                    int playerPos = PlayersOnCourt[t][j];

                                    string position = teams[t].Players.Find(player => player.RosterOrder == playerPos).Pos;

                                    pos.Add(position);
                                }
                            }

                            string playersPos2 = teams[t].Players.Find(player => player.RosterOrder == b).Pos;

                            pos.Add(playersPos2);
                            int numG = 0, numPG = 0, numF = 0, numC = 0;
                            foreach (string position in pos)
                            {
                                if (position.Contains("G"))
                                {
                                    numG++;
                                }
                                if (position == "PG")
                                {
                                    numPG++;
                                }
                                if (position.Contains("F"))
                                {
                                    numF++;
                                }
                                if (position == "C")
                                {
                                    numC++;
                                }
                            }
                            if ((numG < 2 && numPG == 0) || (numF < 2 && numC == 0))
                            {
                                if (Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) > 0.7)
                                {
                                    continue;
                                }
                            }

                            substitutions = true;

                            // Substitute player
                            PlayersOnCourt[t][i] = b;
                            p = b;

                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);

                        }
                    }
                    i += 1;
                }
            }

            if (!StartersRecorded)
            {
                for (int z = 0; z < 2; z++)
                {
                    for (int p = 0; p < teams[z].Players.Count; p++)
                    {
                        int playerRosterOrder = teams[z].Players[p].RosterOrder; // Armazena o ID do jogador em uma variável separada

                        if (PlayersOnCourt[z].Any(play => play == playerRosterOrder))
                        {
                            RecordStat(z, playerRosterOrder, "Gs", teams);
                        }
                    }
                }
                StartersRecorded = true;
            }

            return substitutions;
        }

        private void UpdateSynergy(Team[] Teams, int[][] PlayersOnCourt)
        {
            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].Synergy == null)
                {
                    Teams[t].Synergy = new TeamSynergy();
                }

                // Conta todas as habilidades *fracionárias* dos jogadores ativos em uma equipe (incluindo duplicatas)
                Dictionary<string, double> skillsCount = new Dictionary<string, double>
                {
                    { "3", 0 },
                    { "A", 0 },
                    { "B", 0 },
                    { "Di", 0 },
                    { "Dp", 0 },
                    { "Po", 0 },
                    { "Ps", 0 },
                    { "R", 0 }
                };

                for (int i = 0; i < 5; i++)
                {
                    int p = PlayersOnCourt[t][i];

                    skillsCount["3"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingThreePointer"], 15, 0.7);
                    skillsCount["B"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Dribbling"], 15, 0.7);
                    skillsCount["A"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Athleticism"], 15, 0.7);
                    skillsCount["Di"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefenseInterior"], 15, 0.7);
                    skillsCount["Dp"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["DefensePerimeter"], 15, 0.7);
                    skillsCount["Po"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["ShootingLowPost"], 15, 0.7);
                    skillsCount["Ps"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Passing"], 15, 0.7);
                    skillsCount["R"] += RandomUtils.Sigmoid(Teams[t].Players[p].CompositeRating.Ratings["Rebounding"], 15, 0.7);
                }

                // Sinergia ofensiva de base
                Teams[t].Synergy.Off = 0;
                Teams[t].Synergy.Off += 5 * RandomUtils.Sigmoid(skillsCount["3"], 3, 2);
                Teams[t].Synergy.Off += 3 * RandomUtils.Sigmoid(skillsCount["B"], 15, 0.75) + RandomUtils.Sigmoid(skillsCount["B"], 5, 1.75);
                Teams[t].Synergy.Off += 3 * RandomUtils.Sigmoid(skillsCount["Ps"], 15, 0.75) + RandomUtils.Sigmoid(skillsCount["Ps"], 5, 1.75)
                    + RandomUtils.Sigmoid(skillsCount["Ps"], 5, 2.75);
                Teams[t].Synergy.Off += RandomUtils.Sigmoid(skillsCount["Po"], 15, 0.75);
                Teams[t].Synergy.Off += RandomUtils.Sigmoid(skillsCount["A"], 15, 1.75) + RandomUtils.Sigmoid(skillsCount["A"], 5, 2.75);
                Teams[t].Synergy.Off /= 17;

                // Punir as equipes por não terem múltiplas habilidades de perímetro
                double perimFactor = RandomUtils.Clamp(Math.Sqrt(1 + skillsCount["B"] + skillsCount["Ps"] + skillsCount["3"]) - 1, 0, 2) / 2;
                Teams[t].Synergy.Off *= 0.5 + 0.5 * perimFactor;

                // Sinergia defensiva
                Teams[t].Synergy.Def = 0;
                Teams[t].Synergy.Def += RandomUtils.Sigmoid(skillsCount["Dp"], 15, 0.75);
                Teams[t].Synergy.Def += 2 * RandomUtils.Sigmoid(skillsCount["Di"], 15, 0.75);
                Teams[t].Synergy.Def += RandomUtils.Sigmoid(skillsCount["A"], 5, 2) + RandomUtils.Sigmoid(skillsCount["A"], 5, 3.25);
                Teams[t].Synergy.Def /= 6;

                // Recuperando a sinergia
                Teams[t].Synergy.Reb = 0;
                Teams[t].Synergy.Reb += RandomUtils.Sigmoid(skillsCount["R"], 15, 0.75) + RandomUtils.Sigmoid(skillsCount["R"], 5, 1.75);
                Teams[t].Synergy.Reb /= 4;
            }
        }

        private void HomeCourtAdvantage(Team[] Teams, int[][] PlayersOnCourt)
        {
            double factor;
            for (int t = 0; t < 2; t++)
            {
                if (t == 0)
                {
                    factor = 1.01;  // Bonus pro time da casa
                }
                else
                {
                    factor = 0.99;  // Penalty pro time de fora
                }

                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    foreach (string r in Teams[t].Players[p].CompositeRating.Ratings.Keys.ToList())
                    {
                        Teams[t].Players[p].CompositeRating.Ratings[r] *= factor;
                    }
                }
            }
        }

    }

}