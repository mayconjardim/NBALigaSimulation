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
        double SynergyFactor = 0.05; // Qual a importância da sinergia?
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

        private void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            CompositeHelper.UpdatePace(Teams);

            NumPossessions = (int)Math.Round(Teams[0].CompositeRating.Ratings["GamePace"] + Teams[1].CompositeRating.Ratings["GamePace"] / 2 * RandomUtils.RandomUniform(0.9, 1.1));

            Dt = 48.0 / (2 * NumPossessions);

            int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };

            UpdatePlayersOnCourt(Teams, PlayersOnCourt);
            UpdateSynergy(Teams, PlayersOnCourt);

            SimPossessions(Teams, PlayersOnCourt);

            // Jogue períodos de prorrogação se necessário
            while (Teams[0].Stats.Find(s => s.GameId == Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == Id)?.Pts)
            {
                if (Overtimes == 0)
                {
                    NumPossessions = (int)Math.Round(NumPossessions * 5.0 / 48); // 5 minutos de posses
                    Dt = 5.0 / (2 * NumPossessions);
                }

                T = 5.0;
                Overtimes++;
                SimPossessions(Teams, PlayersOnCourt);
            }

        }

        private void SimPossessions(Team[] Teams, int[][] PlayersOnCourt)
        {
            int i, outcome;
            bool substitutions;

            Offense = 0;
            Defense = 1;

            i = 0;
            while (i < this.NumPossessions * 2)
            {
                // Troca de posse
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Offense == 1) ? 0 : 1;

                if (i % SubsEveryN == 0)
                {
                    substitutions = UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                    if (substitutions)
                    {
                        UpdateSynergy(Teams, PlayersOnCourt);
                    }
                }

                UpdateTeamCompositeRatings();

                outcome = SimPossession();

                // Troca Offense e Defense para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Offense == 1) ? 0 : 1;
                }

                UpdatePlayingTime();

                //Injuries();

                i += 1;
            }
        }

        public bool UpdatePlayersOnCourt(Team[] Teams, int[][] PlayersOnCourt)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
                // Ovr por fadiga
                double[] ovrs = new double[Teams[t].Players.Count];
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {

                    var player = Teams[t].Players.Find(player => player.RosterOrder == p);
                    if (player != null)
                    {
                        if (player.Stats == null)
                        {
                            player.Stats = new List<PlayerGameStats>();
                        }

                        var lastStats = player.Stats.Find(s => s.GameId == Id);

                        if (lastStats == null || lastStats.GameId != Id)
                        {
                            player.Stats.Add(new PlayerGameStats { GameId = Id, TeamId = Teams[t].Id, Name = player.Name, Season = Season.Year });
                        }
                    }

                    // Jogadores lesionados ou com falta não podem jogar
                    if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = Teams[t].Players[p].Ratings.LastOrDefault().Ovr * Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
                              Teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9, 1.1);
                    }
                }

                // Percorre os jogadores na quadra (na ordem inversa da posição atual da lista)
                int i = 0;
                for (int pp = 0; pp < PlayersOnCourt[t].Length; pp++)
                {
                    int p = PlayersOnCourt[t][pp];
                    PlayersOnCourt[t][i] = p;

                    // Faz um loop pelos jogadores de banco (na ordem da posição atual da lista) para ver se algum deve ser substituído)
                    for (int b = 0; b < Teams[t].Players.Count; b++)
                    {
                        if (PlayersOnCourt[t].Contains(b) == false && ((Teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime > 3 && Teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime > 3
                            && ovrs[b] > ovrs[p]) || Teams[t].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6))
                        {
                            substitutions = true;

                            // Jogador substituto
                            PlayersOnCourt[t][i] = b;
                            p = b;
                            Teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                        }
                    }
                    i += 1;
                }
            }

            // Grave iniciadores se isso ainda não tiver sido feito. Isso deve ser executado na primeira vez que essa função for chamada e nunca mais.
            if (!StartersRecorded)
            {
                for (int t = 0; t < 2; t++)
                {
                    for (int p = 0; p < Teams[t].Players.Count; p++)
                    {
                        if (PlayersOnCourt[t].Contains(p))
                        {
                            RecordStat(t, p, "Gs");
                        }
                    }
                }
                StartersRecorded = true;
            }

            return substitutions;
        }

        public void UpdateSynergy(Team[] Teams, int[][] PlayersOnCourt)
        {
            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].Synergy == null)
                {
                    Teams[t].Synergy = new TeamSynergy();
                }

                // Faz uma lista com todas as habilidades dos jogadores ativos em uma equipe (incluindo duplicatas)
                List<string> allSkills = new List<string>();
                for (int i = 0; i < 5; i++)
                {
                    int p = PlayersOnCourt[t][i];
                    allSkills.AddRange(Teams[t].Players[p].Ratings.LastOrDefault().Skills);
                }
                Dictionary<string, int> skillsCount = new Dictionary<string, int>();
                foreach (string skill in allSkills)
                {
                    if (skillsCount.ContainsKey(skill))
                    {
                        skillsCount[skill]++;
                    }
                    else
                    {
                        skillsCount[skill] = 1;
                    }
                }

                // Sinergia ofensiva
                Teams[t].Synergy.Off = 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("3") && skillsCount["3"] >= 2) ? 3 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("3") && skillsCount["3"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("3") && skillsCount["3"] >= 4) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("B") && skillsCount["B"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("B") && skillsCount["B"] >= 2) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 2) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Ps") && skillsCount["Ps"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("Po") && skillsCount["Po"] >= 1) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Off += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 4) ? 1 : 0;
                Teams[t].Synergy.Off /= 17;

                // Sinergia defensiva
                Teams[t].Synergy.Def = 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("Dp") && skillsCount["Dp"] >= 1) ? 1 : 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("Di") && skillsCount["Di"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 3) ? 1 : 0;
                Teams[t].Synergy.Def += (skillsCount.ContainsKey("A") && skillsCount["A"] >= 4) ? 1 : 0;
                Teams[t].Synergy.Def /= 6;

                // Sinergia rebotes
                Teams[t].Synergy.Reb = 0;
                Teams[t].Synergy.Reb += (skillsCount.ContainsKey("R") && skillsCount["R"] >= 1) ? 3 : 0;
                Teams[t].Synergy.Reb += (skillsCount.ContainsKey("R") && skillsCount["R"] >= 2) ? 1 : 0;
                Teams[t].Synergy.Reb /= 4;
            }
        }

        public void UpdateTeamCompositeRatings(Team[] Teams, int[][] PlayersOnCourt)
        {
            // Atualize apenas aqueles que são realmente usados
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking", "GamePace" };
            string ratings = string.Empty;


            for (int t = 0; t < 2; t++)
            {

                if (Teams[t].CompositeRating == null)
                {
                    Teams[t].CompositeRating = new TeamCompositeRating();
                }

                foreach (string rating in toUpdate)
                {
                    Teams[t].CompositeRating.Ratings[rating] = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        int playerRosterOrder = PlayersOnCourt[t][i];
                        var playerRatings = Teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Ratings.LastOrDefault();
                        double ratingValue = 0;

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

                        Teams[i].CompositeRating.Ratings[rating] += ratingValue * Fatigue(Teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Stats.Find(s => s.GameId == Id).Energy);
                        ratings = rating;

                    }

                    Teams[t].CompositeRating.Ratings[ratings] = Teams[t].CompositeRating.Ratings[ratings] / 5;
                }

                Teams[t].CompositeRating.Ratings["GameDribbling"] += SynergyFactor * Teams[t].Synergy.Off;
                Teams[t].CompositeRating.Ratings["GamePassing"] += SynergyFactor * Teams[t].Synergy.Off;
                Teams[t].CompositeRating.Ratings["GameRebounding"] += SynergyFactor * Teams[t].Synergy.Reb;
                Teams[t].CompositeRating.Ratings["GameDefense"] += SynergyFactor * Teams[t].Synergy.Def;
                Teams[t].CompositeRating.Ratings["GameDefensePerimeter"] += SynergyFactor * Teams[t].Synergy.Def;
                Teams[t].CompositeRating.Ratings["GameBlocking"] += SynergyFactor * Teams[t].Synergy.Def;
            }
        }



    }
}