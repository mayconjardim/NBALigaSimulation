using NBALigaSimulation.Shared.Engine;
using System.ComponentModel.DataAnnotations.Schema;

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
            CompositeHelper.UpdatePace(Teams);

            double paceFactor = 106.1 / 100;
            paceFactor += 0.025 * Math.Clamp((paceFactor - 1) / 0.2, -1, 1);
            NumPossessions = Convert.ToInt32((((Teams[0].CompositeRating.Ratings["GamePace"]
                + Teams[1].CompositeRating.Ratings["GamePace"]) / 2) * 1.1 * paceFactor));

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
                //RecordPlay("Overtime");
                SimPossessions(Teams, PlayersOnCourt);
            }

        }

        public async void SimPossessions(Team[] Teams, int[][] PlayersOnCourt)
        {

            Offense = 0;
            Defense = 1;

            int i = 0;
            while (i < NumPossessions * 2)
            {
                if ((i * Dt > 12) || (i * Dt > 24) || (i * Dt > 36))
                {
                    T = 12;
                    //recordPlay("quarter");
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


                UpdateCompositeRatings(Teams, PlayersOnCourt);

                string outcome = SimPossession(Teams, PlayersOnCourt);

                // Troca o e d para que o receba outra posse quando eles forem trocados novamente no início do loop.
                if (outcome == "Orb")
                {
                    Offense = (Offense == 1) ? 0 : 1;
                    Defense = (Offense == 1) ? 0 : 1;
                }

                UpdatePlayingTime(Teams, PlayersOnCourt);

                //Injuries();

                if (i % SubsEveryN == 0)
                {
                    bool substitutions = UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                    if (substitutions)
                    {
                        UpdateSynergy(Teams, PlayersOnCourt);
                    }
                }

                i += 1;
            }
        }

        public bool UpdatePlayersOnCourt(Team[] teams, int[][] PlayersOnCourt)
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
                        ovrs[p] = teams[t].Players[p].Ratings.LastOrDefault().Ovr * Fatigue(teams[t].Players[p].Stats.Find(s => s.GameId == Id).Energy) *
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
                            // Require 2 Gs (or 1 PG) and 2 Fs (or 1 C)
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


        public void UpdateSynergy(Team[] Teams, int[][] PlayersOnCourt)
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
                        RecordStat(t, p, "Energy", Teams, 1, (-Dt * 0.06 * (1 - Teams[t].Players[p].Ratings.Last().GameEndurance)));
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

        public string SimPossession(Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios;
            int shooter;

            // Turnover?
            if (DefenseHelper.ProbTov(Teams) > new Random().NextDouble())
            {
                return DoTov(Teams, PlayersOnCourt);
            }

            // Shot if there is no turnover
            ratios = RatingArray(Teams, "GameUsage", Offense, PlayersOnCourt);
            shooter = ArrayHelper.PickPlayer(ratios);

            return DoShot(shooter, Teams, PlayersOnCourt); // fg, orb, or drb
        }

        public string DoTov(Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios;
            int p;

            ratios = RatingArray(Teams, "GameTurnovers", Offense, PlayersOnCourt, 0.5);
            p = PlayersOnCourt[Offense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Offense, p, "Tov", Teams);
            if (DefenseHelper.ProbStl(Teams) > new Random().NextDouble())
            {
                return DoStl(p, Teams, PlayersOnCourt); // "stl"
            }
            else
            {
                RecordPlay("Tov", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
            }

            return "Tov";
        }

        public string DoStl(int pStoleFrom, Team[] Teams, int[][] PlayersOnCourt)
        {
            double[] ratios;
            int p;

            ratios = RatingArray(Teams, "GameStealing", Defense, PlayersOnCourt);
            p = PlayersOnCourt[Defense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Defense, p, "Stl", Teams);
            RecordPlay("Stl", Defense, new string[] { Teams[Defense].Players[p].Name, Teams[Offense].Players[pStoleFrom].Name }, Teams);

            return "Stl";
        }

        public string DoShot(int shooter, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p = PlayersOnCourt[Offense][shooter];

            double currentFatigue = Fatigue(Teams[Offense].Players[p].Stats.Find(s => s.GameId == Id).Energy);

            int passer = -1;
            if (OffenseHelper.ProbAst(Teams, Offense, Defense) > new Random().NextDouble())
            {
                double[] ratios = RatingArray(Teams, "GamePassing", Offense, PlayersOnCourt, 2);
                passer = ArrayHelper.PickPlayer(ratios, shooter);
            }

            double probAndOne;
            double probMake;
            double probMissAndFoul;
            string type;

            double shootingThreePointerScaled = Teams[Offense].Players[p].Ratings.LastOrDefault().GameShootingThreePointer;
            if (shootingThreePointerScaled > 0.5 &&
               new Random().NextDouble() < 0.35 * Teams[Offense].Players[p].Ratings.LastOrDefault().GameShootingThreePointer)
            {
                // Three pointer
                type = "ThreePointer";
                probMissAndFoul = 0.02;
                probMake = shootingThreePointerScaled * 0.37 + 0.24;
                probAndOne = 0.01;
            }
            else
            {

                double r1 = new Random().NextDouble() * Teams[Offense].Players[p].Ratings.Last().GameShootingMidRange;
                double r2 = new Random().NextDouble() * (Teams[Offense].Players[p].Ratings.Last().GameShootingAtRim +
                    SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));
                double r3 = new Random().NextDouble() * (Teams[Offense].Players[p].Ratings.Last().GameShootingLowPost +
                    SynergyFactor * (Teams[Offense].Synergy.Off - Teams[Defense].Synergy.Def));

                if (r1 > r2 && r1 > r3)
                {
                    // Two point jumper
                    type = "MidRange";
                    probMissAndFoul = 0.07;
                    probMake = Teams[Offense].Players[p].Ratings.Last().GameShootingMidRange * 0.3 + 0.29;
                    probAndOne = 0.05;
                }
                else if (r2 > r3)
                {
                    // Dunk, fast break or half court
                    type = "AtRim";
                    probMissAndFoul = 0.37;
                    probMake = Teams[Offense].Players[p].Ratings.Last().GameShootingAtRim * 0.3 + 0.52;
                    probAndOne = 0.25;
                }
                else
                {
                    // Post up
                    type = "LowPost";
                    probMissAndFoul = 0.33;
                    probMake = Teams[Offense].Players[p].Ratings.Last().GameShootingLowPost * 0.3 + 0.37;
                    probAndOne = 0.15;
                }
            }

            probMake = (probMake - 0.25 * Teams[Defense].CompositeRating.Ratings["GameDefense"] + SynergyFactor * (Teams[Offense].Synergy.Off -
                Teams[Defense].Synergy.Def)) * currentFatigue;

            // Assisted shots are easier
            if (passer >= 0)
            {
                probMake += 0.025;
            }

            if (DefenseHelper.ProbBlk(Teams, Defense) > new Random().NextDouble())
            {
                return DoBlk(shooter, type, Teams, PlayersOnCourt); // orb or drb
            }

            // Make
            if (probMake > new Random().NextDouble())
            {
                // And 1
                if (probAndOne > new Random().NextDouble())
                {
                    return DoFg(shooter, passer, type, true, Teams, PlayersOnCourt); // fg, orb, or drb
                }
                else
                {
                    return DoFg(shooter, passer, type, false, Teams, PlayersOnCourt); // fg
                }
            }
            else
            {
                // Miss, but fouled
                if (probMissAndFoul > new Random().NextDouble())
                {
                    if (type == "ThreePointer")
                    {
                        return DoFt(shooter, 3, Teams, PlayersOnCourt);
                    }
                    else
                    {
                        return DoFt(shooter, 2, Teams, PlayersOnCourt);
                    }
                }
                else
                {
                    // Miss
                    p = PlayersOnCourt[Offense][shooter];
                    RecordStat(Offense, p, "Fga", Teams);
                    if (type == "AtRim")
                    {
                        RecordStat(Offense, p, "FgaAtRim", Teams);
                        RecordPlay("MissAtRim", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
                    }
                    else if (type == "LowPost")
                    {
                        RecordStat(Offense, p, "FgaLowPost", Teams);
                        RecordPlay("MissLowPost", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
                    }
                    else if (type == "MidRange")
                    {
                        RecordStat(Offense, p, "FgaMidRange", Teams);
                        RecordPlay("MissMidRange", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
                    }
                    else if (type == "ThreePointer")
                    {
                        RecordStat(Offense, p, "Tpa", Teams);
                        RecordPlay("MissTp", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
                    }

                    return DoReb(Teams, PlayersOnCourt);
                }
            }
        }

        private string DoBlk(int shooter, string type, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p, p2;
            string[] playArgs;
            p = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", Teams);
            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Tpa", Teams);
            }

            double[] ratios = RatingArray(Teams, "GameBlocking", Defense, PlayersOnCourt, 4);
            p2 = PlayersOnCourt[Defense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Defense, p2, "Blk", Teams);

            if (type == "AtRim")
            {
                playArgs = new string[] { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                RecordPlay("BlkAtRim", Defense, playArgs, Teams);
            }
            else if (type == "LowPost")
            {
                playArgs = new string[] { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                RecordPlay("BlkLowPost", Defense, playArgs, Teams);
            }
            else if (type == "MidRange")
            {
                playArgs = new string[] { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                RecordPlay("BlkMidRange", Defense, playArgs, Teams);
            }
            else if (type == "ThreePointer")
            {
                playArgs = new string[] { Teams[Defense].Players[p2].Name, Teams[Offense].Players[p].Name };
                RecordPlay("BlkTp", Defense, playArgs, Teams);
            }

            return DoReb(Teams, PlayersOnCourt); // orb or drb
        }

        private string DoFg(int shooter, int passer, string type, bool andOne, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p;
            p = PlayersOnCourt[Offense][shooter];
            RecordStat(Offense, p, "Fga", Teams);
            RecordStat(Offense, p, "Fg", Teams);
            RecordStat(Offense, p, "Pts", Teams, 2);  // 2 points for 2's

            if (type == "AtRim")
            {
                RecordStat(Offense, p, "FgaAtRim", Teams);
                RecordStat(Offense, p, "FgAtRim", Teams);
                RecordPlay("FgAtRim" + (andOne ? "AndOne" : ""), Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
            }
            else if (type == "LowPost")
            {
                RecordStat(Offense, p, "FgaLowPost", Teams);
                RecordStat(Offense, p, "FgLowPost", Teams);
                RecordPlay("FgLowPost" + (andOne ? "AndOne" : ""), Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
            }
            else if (type == "MidRange")
            {
                RecordStat(Offense, p, "FgaMidRange", Teams);
                RecordStat(Offense, p, "FgMidRange", Teams);
                RecordPlay("FgMidRange" + (andOne ? "AndOne" : ""), Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
            }
            else if (type == "ThreePointer")
            {
                RecordStat(Offense, p, "Pts", Teams);  // Extra point for 3's
                RecordStat(Offense, p, "Tpa", Teams);
                RecordStat(Offense, p, "Tp", Teams);
                RecordPlay("Tp" + (andOne ? "AndOne" : ""), Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
            }

            if (passer >= 0)
            {
                p = PlayersOnCourt[Offense][passer];
                RecordStat(Offense, p, "Ast", Teams);
                RecordPlay("Ast", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
            }

            if (andOne)
            {
                return DoFt(shooter, 1, Teams, PlayersOnCourt);  // fg, orb, or drb
            }
            return "Fg";
        }

        private string DoFt(int shooter, int amount, Team[] Teams, int[][] PlayersOnCourt)
        {
            Random random = new Random();

            int i, p;
            string outcome = null;

            DoPf(Defense, Teams, PlayersOnCourt);
            p = PlayersOnCourt[Offense][shooter];

            for (i = 0; i < amount; i++)
            {
                RecordStat(Offense, p, "Fta", Teams);
                if (random.NextDouble() < Teams[Offense].Players[p].Ratings.Last().GameShootingFT * 0.3 + 0.6)  // Between 60% and 90%
                {
                    RecordStat(Offense, p, "Ft", Teams);
                    RecordStat(Offense, p, "Pts", Teams);
                    RecordPlay("Ft", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
                    outcome = "Fg";
                }
                else
                {
                    RecordPlay("MissFt", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);
                }
            }

            if (outcome != "Fg")
            {
                outcome = DoReb(Teams, PlayersOnCourt);  // orb or drb
            }

            return outcome;
        }

        private void DoPf(int t, Team[] Teams, int[][] PlayersOnCourt)
        {
            int p;
            double[] ratios;

            ratios = RatingArray(Teams, "GameFouling", t, PlayersOnCourt);
            p = PlayersOnCourt[t][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Defense, p, "Pf", Teams);
            RecordPlay("Pf", Defense, new string[] { Teams[Defense].Players[p].Name }, Teams);

            // Foul out
            if (Teams[Defense].Players[p].Stats.Find(s => s.GameId == Id).Pf >= 6)
            {
                RecordPlay("FoulOut", Defense, new string[] { Teams[Defense].Players[p].Name }, Teams);
                // Force substitutions now
                UpdatePlayersOnCourt(Teams, PlayersOnCourt);
                UpdateSynergy(Teams, PlayersOnCourt);
            }
        }

        private string DoReb(Team[] Teams, int[][] PlayersOnCourt)
        {

            int p;
            double[] ratios;

            Random random = new Random();

            if (0.15 > random.NextDouble())
            {
                return null;
            }

            if (0.75 * (2 + Teams[Defense].CompositeRating.Ratings["GameRebounding"]) / (2 + Teams[Offense].CompositeRating.Ratings["GameRebounding"])
                > random.NextDouble())
            {
                ratios = RatingArray(Teams, "GameRebounding", Defense, PlayersOnCourt);
                p = PlayersOnCourt[Defense][ArrayHelper.PickPlayer(ratios)];
                RecordStat(Defense, p, "Drb", Teams);
                RecordStat(Defense, p, "Trb", Teams);
                RecordPlay("Drb", Defense, new string[] { Teams[Defense].Players[p].Name }, Teams);

                return "Drb";
            }

            ratios = RatingArray(Teams, "GameRebounding", Offense, PlayersOnCourt);
            p = PlayersOnCourt[Offense][ArrayHelper.PickPlayer(ratios)];
            RecordStat(Offense, p, "Orb", Teams);
            RecordStat(Offense, p, "Trb", Teams);
            RecordPlay("Orb", Offense, new string[] { Teams[Offense].Players[p].Name }, Teams);

            return "Orb";
        }

        public double[] RatingArray(Team[] Teams, string rating, int t, int[][] PlayersOnCourt, double power = 1)
        {
            double[] array = new double[5];

            for (int i = 0; i < 5; i++)
            {
                int p = PlayersOnCourt[t][i];
                var player = Teams[t].Players.Find(player => player.RosterOrder == p);

                double energy = (double)(player.Stats.Find(s => s.GameId == Id)?.Energy);

                if (player != null && player.Ratings?.LastOrDefault() != null)
                {
                    var playerRatings = player.Ratings.LastOrDefault();
                    array[i] = Math.Pow(CompositeHelper.GetRatingValue(rating, playerRatings) * energy, power);
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

        public void RecordPlay(string type, double t, string[] names, Team[] Teams)
        {
            int i;
            int qtr;
            string sect = "";
            string text = "";
            string[] texts = new string[] { };

            if (type == "Injury")
            {
                texts = new string[] { "{0} was injured!" };
            }
            else if (type == "Tov")
            {
                texts = new string[] { "{0} turned the ball over" };
            }
            else if (type == "Stl")
            {
                texts = new string[] { "{0} stole the ball from {1}" };
            }
            else if (type == "FgAtRim")
            {
                texts = new string[] { "{0} made a dunk/layup" };
            }
            else if (type == "FgAtRimAndOne")
            {
                texts = new string[] { "{0} made a dunk/layup and got fouled!" };
            }
            else if (type == "FgLowPost")
            {
                texts = new string[] { "{0} made a low post shot" };
            }
            else if (type == "FgLowPostAndOne")
            {
                texts = new string[] { "{0} made a low post shot and got fouled!" };
            }
            else if (type == "FgMidRange")
            {
                texts = new string[] { "{0} made a mid-range shot" };
            }
            else if (type == "FgMidRangeAndOne")
            {
                texts = new string[] { "{0} made a mid-range shot and got fouled!" };
            }
            else if (type == "Tp")
            {
                texts = new string[] { "{0} made a three pointer shot" };
            }
            else if (type == "TpAndOne")
            {
                texts = new string[] { "{0} made a three pointer and got fouled!" };
            }
            else if (type == "BlkAtRim")
            {
                texts = new string[] { "{0} blocked {1}'s dunk/layup" };
            }
            else if (type == "BlkLowPost")
            {
                texts = new string[] { "{0} blocked {1}'s low post shot" };
            }
            else if (type == "BlkMidRange")
            {
                texts = new string[] { "{0} blocked {1}'s mid-range shot" };
            }
            else if (type == "BlkTp")
            {
                texts = new string[] { "{0} blocked {1}'s three pointer" };
            }
            else if (type == "MissAtRim")
            {
                texts = new string[] { "{0} missed a dunk/layup" };
            }
            else if (type == "MissLowPost")
            {
                texts = new string[] { "{0} missed a low post shot" };
            }
            else if (type == "MissMidRange")
            {
                texts = new string[] { "{0} missed a mid-range shot" };
            }
            else if (type == "MissTp")
            {
                texts = new string[] { "{0} missed a three pointer" };
            }
            else if (type == "Orb")
            {
                texts = new string[] { "{0} grabbed the offensive rebound" };
            }
            else if (type == "Drb")
            {
                texts = new string[] { "{0} grabbed the defensive rebound" };
            }
            else if (type == "Ast")
            {
                texts = new string[] { "(assist: {0})" };
            }
            else if (type == "Quarter")
            {
                //texts = new string[] { "<b>Start of " + RandomUtils.Ordinal(Teams[0].Stats.Find(s => s.GameId == Id).PtsQtrs.Count) + " quarter</b>" };
            }
            else if (type == "Overtime")
            {
                //texts = new string[] { "<b>Start of " + RandomUtils.Ordinal(Teams[0].Stats.Find(s => s.GameId == Id).PtsQtrs.Count - 4) + " overtime period</b>" };
            }
            else if (type == "Ft")
            {
                texts = new string[] { "{0} made a free throw" };
            }
            else if (type == "MissFt")
            {
                texts = new string[] { "{0} missed a free throw" };
            }
            else if (type == "Pf")
            {
                texts = new string[] { "Foul on {0}" };
            }
            else if (type == "FoulOut")
            {
                texts = new string[] { "{0} fouled out" };
            }
            else if (type == "Sub")
            {
                texts = new string[] { "Substitution: {0} for {1}" };
            }


            /*
            if (texts != null && texts.Length > 0)
            {
                text = texts[0];
                if (names != null)
                {
                    for (int z = 0; z < names.Length; z++)
                    {
                        text = text.Replace("{" + z + "}", names[z]);
                    }
                }

                if (type == "Ast")
                {

                    for (int z = PlayByPlay.Count - 1; z >= 0; z--)
                    {
                        if (PlayByPlay[z].Type == "text")
                        {
                            PlayByPlay[z].Text += " " + text;
                            break;
                        }
                    }
                }
                else
                {
                    int sec = (int)(T % 1 * 60);
                    if (sec < 10)
                    {
                        sect = "0" + sec;
                    }
                    PlayByPlay.Add(new GamePlayByPlay
                    {
                        Type = "Text",
                        Text = text,
                        T = T,
                        Time = Math.Floor((double)T) + ":" + sect
                    });
                }
            }
            else
            {
                Console.WriteLine("No text for " + type);
            }

            */

        }

        public double Gauss(double mu = 0, double sigma = 1)
        {
            Random random = new Random();
            double result = (
                (random.NextDouble() * 2 - 1 +
                (random.NextDouble() * 2 - 1) +
                (random.NextDouble() * 2 - 1)) *
                sigma +
                mu
            );

            return result;
        }

        public void UpdateCompositeRatings(Team[] teams, int[][] playersOnCourt)
        {
            string[] toUpdate = { "GameDribbling", "GamePassing", "GameRebounding", "GameDefense", "GameDefensePerimeter", "GameBlocking", "GamePace" };

            for (int i = 0; i < 2; i++)
            {

                if (teams[i].CompositeRating == null)
                {
                    teams[i].CompositeRating = new TeamCompositeRating();
                }

                for (int j = 0; j < 5; j++)
                {

                    int playerRosterOrder = playersOnCourt[i][j];
                    var playerRatings = teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Ratings.LastOrDefault();
                    double ratingValue = 0;
                    string rats = string.Empty;

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

                        teams[i].CompositeRating.Ratings[rating] += ratingValue * Fatigue(teams[i].Players.Find(player => player.RosterOrder == playerRosterOrder).Stats.Find(s => s.GameId == Id).Energy);
                        rats = rating;
                    }

                    teams[i].CompositeRating.Ratings[rats] = teams[i].CompositeRating.Ratings[rats] / 5;

                }

                teams[i].CompositeRating.Ratings["GameDribbling"] += 0.1 * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GamePassing"] += 0.1 * teams[i].Synergy.Off;
                teams[i].CompositeRating.Ratings["GameRebounding"] += 0.1 * teams[i].Synergy.Reb;
                teams[i].CompositeRating.Ratings["GameDefense"] += 0.1 * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameDefensePerimeter"] += 0.1 * teams[i].Synergy.Def;
                teams[i].CompositeRating.Ratings["GameBlocking"] += 0.1 * teams[i].Synergy.Def;

            }
        }



    }
}