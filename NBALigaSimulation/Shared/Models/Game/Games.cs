using NBALigaSimulation.Shared.Engine;
using NBALigaSimulation.Shared.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBALigaSimulation.Shared.Model
{
    public class Games
    {

        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        [ForeignKey("HomeTeamId")]
        public Team? HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        [ForeignKey("AwayTeamId")]
        public Team? AwayTeam { get; set; }
        public List<TeamGameStats>? TeamGameStats { get; set; }
        public List<PlayerGameStats>? PlayerGameStats { get; set; }
        public List<GamePlayByPlay> PlayByPlay { get; set; }

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
        int T = 12; // Tempo por quarto
        [NotMapped]
        int Dt;     // Tempo decorrido por posse

        [NotMapped] // Jogadores em quadra
        int[][] PlayersOnCourt = new int[][] { new int[] { 0, 1, 2, 3, 4 }, new int[] { 0, 1, 2, 3, 4 } };


        public void GameSim()
        {

            Team[] Teams = { HomeTeam, AwayTeam };
            NumPossessions = (int)Math.Round(92.7 + 98.7 / 2 * RandomUtils.RandomUniform(0.9, 1.1));
            Dt = 48 / (2 * NumPossessions);

            // Simula o jogo até o final da partida
            SimPossessions(Teams);

            // Jogue períodos de prorrogação se necessário
            while (Teams[0].Stats.Find(s => s.GameId == Id)?.Pts == Teams[1].Stats.Find(s => s.GameId == Id)?.Pts)
            {
                if (Overtimes == 0)
                {
                    NumPossessions = (int)Math.Round(NumPossessions * 5.0 / 48); // 5 minutos de posses
                    Dt = (int)5.0 / (2 * NumPossessions);
                }

                T = 5;
                Overtimes++;
                Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                Teams[1].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                RecordPlay("Overtime");
                SimPossessions(Teams);
            }

        }

        public void SimPossessions(Team[] Teams)
        {
            int i = 0;
            while (i < NumPossessions * 2)
            {
                // Acompanha os quartos
                if ((i * Dt > 12 && Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Count == 1) ||
                    (i * Dt > 24 && Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Count == 2) ||
                    (i * Dt > 36 && Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Count == 3))
                {
                    Teams[0].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                    Teams[1].Stats.Find(s => s.GameId == Id)?.PtsQtrs.Add(new PtsQtr { Points = 0 });
                    T = 12;
                    RecordPlay("Quarter");
                }

                // Clock
                T -= Dt;
                if (T < 0)
                {
                    T = 0;
                }

                // Troca de posse
                Offense = (Offense == 1) ? 0 : 1;
                Defense = (Offense == 1) ? 0 : 1;

                UpdateTeamCompositeRatings();

                string outcome = SimPossession();

                // Troca o e d para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Offense == 1) ? 0 : 1;
                }

                UpdatePlayingTime();

                //Injuries();

                if (i % SubsEveryN == 0)
                {
                    bool substitutions = UpdatePlayersOnCourt(Teams);
                    if (substitutions)
                    {
                        UpdateSynergy(Teams);
                    }
                }

                i += 1;
            }
        }

        public bool UpdatePlayersOnCourt(Team[] Teams)
        {
            bool substitutions = false;

            for (int t = 0; t < 2; t++)
            {
                // Valores gerais dimensionados por fadiga
                List<double> ovrs = new List<double>();
                for (int p = 0; p < Teams[t].Players.Count; p++)
                {
                    // Jogadores com falta não podem jogar
                    if (Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Pf >= 6)
                    {
                        ovrs[p] = double.NegativeInfinity;
                    }
                    else
                    {
                        ovrs[p] = Teams[t].Players[p].Ratings.Last().Ovr * Fatigue(Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Energy)
                            * Teams[t].Players[p].PtModifier * RandomUtils.RandomUniform(0.9, 1.1);
                    }
                }

                int i = 0;
                for (int pp = 0; pp < PlayersOnCourt[t].Length; pp++)
                {
                    int p = PlayersOnCourt[t][pp];
                    PlayersOnCourt[t][i] = p;

                    for (int b = 0; b < Teams[t].Players.Count; b++)
                    {
                        if (!PlayersOnCourt[t].Contains(b)
                            && ((Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.CourtTime > 3
                            && Teams[t].Players[b].Stats.Find(s => s.GameId == Id)?.BenchTime > 3
                            && ovrs[b] > ovrs[p]) || Teams[t].Players[p].Stats.Find(s => s.GameId == Id)?.Pf >= 6)
                            && (Teams[t].Players[b].Stats.Find(s => s.GameId == Id)?.Pf < 6))
                        {
                            substitutions = true;

                            PlayersOnCourt[t][i] = b;
                            Teams[t].Players[b].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[b].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).CourtTime = RandomUtils.RandomUniform(-2, 2);
                            Teams[t].Players[p].Stats.Find(s => s.GameId == Id).BenchTime = RandomUtils.RandomUniform(-2, 2);

                            if (PlayByPlay != null)
                            {
                                var play = new GamePlayByPlay
                                {
                                    GameSimId = Id,
                                    Sequence = PlayByPlay.Count + 1,
                                    Key = "Type",
                                    Value = "Sub"
                                };

                                PlayByPlay.Add(play);
                            }

                            if (StartersRecorded)
                            {
                                RecordPlay("Sub", t, new string[] { Teams[t].Players[b].FullName, Teams[t].Players[p].FullName });
                            }
                            break;
                        }
                    }
                    i += 1;
                }
            }

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

        public void UpdateSynergy(Team[] Teams)
        {
            for (int t = 0; t < 2; t++)
            {
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

                    skillsCount["3"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameShootingThreePointer, 15, 0.7);
                    skillsCount["A"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameAthleticism, 15, 0.7);
                    skillsCount["B"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameDribbling, 15, 0.7);
                    skillsCount["Di"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameDefenseInterior, 15, 0.7);
                    skillsCount["Dp"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameDefensePerimeter, 15, 0.7);
                    skillsCount["Po"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameShootingLowPost, 15, 0.7);
                    skillsCount["Ps"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GamePassing, 15, 0.7);
                    skillsCount["R"] += RandomUtils.Sigmoid(Teams[t].Players[p].Ratings.Last().GameRebounding, 15, 0.7);
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

        public void UpdateTeamCompositeRatings()
        {
            string[] toUpdate = { "dribbling", "passing", "rebounding", "defense", "defensePerimeter", "blocking" };

            for (int t = 0; t < 2; t++)
            {
                foreach (string rating in toUpdate)
                {
                    team[t].compositeRating[rating] = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        int p = playersOnCourt[t][i];
                        team[t].compositeRating[rating] += team[t].player[p].compositeRating[rating] * Fatigue(team[t].player[p].stat.energy);
                    }

                    team[t].compositeRating[rating] /= 5;
                }

                team[t].compositeRating.dribbling += synergyFactor * team[t].synergy.off;
                team[t].compositeRating.passing += synergyFactor * team[t].synergy.off;
                team[t].compositeRating.rebounding += synergyFactor * team[t].synergy.reb;
                team[t].compositeRating.defense += synergyFactor * team[t].synergy.def;
                team[t].compositeRating.defensePerimeter += synergyFactor * team[t].synergy.def;
                team[t].compositeRating.blocking += synergyFactor * team[t].synergy.def;
            }
        }


    }
}